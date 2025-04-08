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
        string connString = @"Provider=Microsoft.JET.OLEDB.4.0;Data Source=C:\Users\owner\Desktop\ddoocp\Fitness\Fitness.mdb;";
        OleDbConnection conn;
        private int failedAttempts = 0;
        private const int maxAttempts = 3;

        public LoginForm(string user)
        {
            InitializeComponent();
            
        }
        private void button1_Click(object sender, EventArgs e)
        {
            RegistrationForm registrationForm = new RegistrationForm();
            registrationForm.Show();
            this.Hide();
        }
        private bool ValidateUser(string enteredUsername, string enteredPassword)
        {
            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                conn.Open();
                string query = "SELECT Username, Password FROM Users WHERE LCase(Username) = LCase(@Username)";

                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Username", enteredUsername.ToLower()); 

                    using (OleDbDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string storedPassword = reader["Password"].ToString();

                            if (storedPassword == enteredPassword) 
                            {
                                return true; 
                            }
                        }
                    }
                }
            }

            return false; 
        }


        private void button2_Click(object sender, EventArgs e)
        {
            if (failedAttempts >= maxAttempts)
            {
                MessageBox.Show("Too many failed attempts! Try again later.", "Account Locked", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string username = txtUsername.Text.Trim().ToLower();
            string password = txtPassword.Text.Trim();

            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                conn.Open();

                string loginQuery = "SELECT COUNT(*) FROM Users WHERE Username = @Username AND Password = @";
                using (OleDbCommand cmd = new OleDbCommand(loginQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Password", password); 

                    int userFound = (int)cmd.ExecuteScalar();

                    if (userFound > 0)
                    {
                        lblMessage.Text = $"✅ Welcome, {username}!";
                        MessageBox.Show("Login Successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        DasboardForm dasboardForm = new DasboardForm(username);
                        dasboardForm.Show();
                        this.Hide();
                    }
                    else
                    {
                        failedAttempts++;
                        lblMessage.Text = $"❌ Incorrect username or password! {maxAttempts - failedAttempts} attempts left.";

                        if (failedAttempts >= maxAttempts)
                        {
                            MessageBox.Show("Too many failed attempts! Try again later.", "Account Locked", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
                }
            }

        

        private void LoginForm_Load(object sender, EventArgs e)
        {

        }

        private void txtUsername_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }

}
