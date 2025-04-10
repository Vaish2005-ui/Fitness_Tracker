using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DDOOCP_Assignment
{
    public partial class LoginForm : Form
    {
        string connString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\owner\Desktop\ddoocp\Fitness\Fitness.mdb;";
        private const int maxAttempts = 3;
        private Dictionary<string, int> failedLoginAttempts = new Dictionary<string, int>();
        private string _testUser = "";

        public LoginForm(string user)
        {
            InitializeComponent();
            _testUser = user.ToLower();
        }

        public bool AttemptLogin(string username, string password)
        {
            username = username.Trim().ToLower();
            password = password.Trim();

            if (failedLoginAttempts.ContainsKey(username) && failedLoginAttempts[username] >= maxAttempts)
            {
                return false;
            }

            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                conn.Open();

                string query = "SELECT COUNT(*) FROM Users WHERE Username = @Username AND Password = @Password";
                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Password", password);

                    int userFound = (int)cmd.ExecuteScalar();

                    if (userFound > 0)
                    {
                        return true;
                    }
                    else
                    {
                        if (!failedLoginAttempts.ContainsKey(username))
                            failedLoginAttempts[username] = 0;

                        failedLoginAttempts[username]++;
                        return false;
                    }
                }
            }
        }

        public bool IsAccountLocked
        {
            get
            {
                return failedLoginAttempts.ContainsKey(_testUser) && failedLoginAttempts[_testUser] >= maxAttempts;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim().ToLower();
            string password = txtPassword.Text.Trim();

            if (IsAccountLocked)
            {
                MessageBox.Show("Too many failed attempts for this account! Try again later.", "Account Locked", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool success = AttemptLogin(username, password);

            if (success)
            {
                lblMessage.Text = $" Welcome, {username}!";
                MessageBox.Show("Login Successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                DasboardForm dasboardForm = new DasboardForm(username);
                dasboardForm.Show();
                this.Hide();
            }
            else
            {
                int remainingAttempts = maxAttempts - failedLoginAttempts[username];
                lblMessage.Text = $" Incorrect username or password! {remainingAttempts} attempts left.";

                if (remainingAttempts <= 0)
                {
                    MessageBox.Show("Too many failed attempts for this account! Try again later.", "Account Locked", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            RegistrationForm registrationForm = new RegistrationForm();
            registrationForm.Show();
            this.Hide();
        }

        private void LoginForm_Load(object sender, EventArgs e) { }
}







        
    }


