using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DDOOCP_Assignment
{
    public partial class SetGoalForm : Form
    {
        private string username;
        string connString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\owner\Desktop\ddoocp\Fitness\Fitness.mdb;";
        OleDbConnection connection;


        public SetGoalForm(string user)
        {
            InitializeComponent();
            username = user;
            comboBox1.SelectedIndex = 0; 
            LoadUserGoal();
        }
       

        private void lblGoal_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (int.TryParse(textBox1.Text, out int goal) && goal > 0)
            {
                string selectedDuration = comboBox1.SelectedItem.ToString();
                SaveUserGoal(goal, selectedDuration);

                MessageBox.Show($"{username}, your goal is set:\n" +
                                $"{goal} kcal ({selectedDuration})",
                                "Goal Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Please enter a valid calorie goal!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(comboBox1.Text))
            {
                MessageBox.Show("Username and Duration cannot be empty!", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void LoadUserGoal()
        {
            try
            {
                using (OleDbConnection conn = new OleDbConnection(connString))
                {
                    conn.Open();
                   
                    string query = "SELECT Goal, Duration FROM Goals WHERE Username = @Username";
                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                textBox1.Text = reader["Goal"].ToString();
                                comboBox1.SelectedItem = reader["Duration"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading goal: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void SaveUserGoal(int goal, string duration)
        {
            try
            {
                using (OleDbConnection conn = new OleDbConnection(connString))
                {
                    conn.Open();
                    string checkQuery = "SELECT COUNT(*) FROM Goals WHERE Username = @Username";
                    using (OleDbCommand checkCmd = new OleDbCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@username", username);
                        int count = (int)checkCmd.ExecuteScalar();

                        if (count > 0) 
                        {
                            string updateQuery = "UPDATE Goals SET Goal = @Goal, Duration = @Duration WHERE Username = @Username";
                            using (OleDbCommand updateCmd = new OleDbCommand(updateQuery, conn))
                            {
                                updateCmd.Parameters.AddWithValue("@goal", Convert.ToInt32(goal));
                                updateCmd.Parameters.AddWithValue("@duration", duration);
                                updateCmd.Parameters.AddWithValue("@username", username);
                                updateCmd.ExecuteNonQuery();
                            }
                        }
                        else 
                        {
                            string insertQuery = "INSERT INTO Goals (Username, Goal, Duration) VALUES (?, @Goal, @Duration)";
                            using (OleDbCommand insertCmd = new OleDbCommand(insertQuery, conn))
                            {
                                insertCmd.Parameters.AddWithValue("@username", (username));
                                insertCmd.Parameters.AddWithValue("@goal", Convert.ToInt32(goal));
                                insertCmd.Parameters.AddWithValue("@duration", duration);
                                insertCmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving goal: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            comboBox1.SelectedIndex = 0;

            try
            {
                using (OleDbConnection conn = new OleDbConnection(connString))
                {
                    conn.Open();
                    string deleteQuery = "DELETE FROM Goals WHERE Username = @Username";
                    using (OleDbCommand cmd = new OleDbCommand(deleteQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Goal cleared successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error clearing goal: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        
        private void button6_Click(object sender, EventArgs e)
        {
            String User = username;
            RecordActivityForm recordActivityForm = new RecordActivityForm(User);
            recordActivityForm.Show();
            this.Hide();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to log out?","Log Out",MessageBoxButtons.YesNo,MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                LoginForm LoginForm = new LoginForm(username);
                LoginForm.Show();
                this.Hide();
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            string User = username;
            ViewProgressForm ViewProgressForm = new ViewProgressForm(User);
            ViewProgressForm.Show();
            this.Hide();
        }


        private void button7_Click(object sender, EventArgs e)
        {
            String User = username;
            DasboardForm dasboardForm = new DasboardForm(username);
            dasboardForm.Show();
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string User = username;
            ViewProgressForm ViewProgressForm = new ViewProgressForm(User);
            ViewProgressForm.Show();
            this.Hide();
        }

        private void SetGoalForm_Load(object sender, EventArgs e)
        {

        }
    }
}
