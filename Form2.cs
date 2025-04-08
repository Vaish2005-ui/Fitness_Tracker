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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace DDOOCP_Assignment
{
    public partial class DasboardForm : Form
    {
        private string username;
        string connString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\owner\Desktop\ddoocp\Fitness\Fitness.mdb;";
        OleDbConnection connection;
        public DasboardForm(string user)
        {
            InitializeComponent();
            username = user;
            lblWelcome.Text = $"Welcome, {username}!";
            LoadSummaryFromDatabase();

        }

        private void LoadSummaryFromDatabase()
        {
            int goal = 0;
            string duration = "N/A";
            int totalBurned = 0;
            DataTable activityTable = new DataTable();


            try
            {
                using (OleDbConnection conn = new OleDbConnection(connString))
                {
                    conn.Open();

                    string goalQuery = "SELECT Goal, Duration FROM Goals WHERE Username = ?";
                    using (OleDbCommand goalCmd = new OleDbCommand(goalQuery, conn))
                    {
                        goalCmd.Parameters.AddWithValue("?", username);
                        using (OleDbDataReader reader = goalCmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                if (reader["Goal"] != DBNull.Value)
                                {
                                    goal = Convert.ToInt32(reader["Goal"]);
                                }
                                if (reader["Duration"] != DBNull.Value)
                                {
                                    duration = reader["Duration"].ToString();
                                }
                            }
                        }
                    }

                    string burnedQuery = "SELECT SUM(CaloriesBurned) AS TotalBurned FROM ActivityRecords WHERE Username = ?";
                    using (OleDbCommand burnedCmd = new OleDbCommand(burnedQuery, conn))
                    {
                        burnedCmd.Parameters.AddWithValue("?", username);
                        object result = burnedCmd.ExecuteScalar();
                        if (result != DBNull.Value && result != null)
                        {
                            totalBurned = Convert.ToInt32(result);
                        }
                    }
                 
                    string query = @"SELECT Activity, CaloriesBurned, DateRecorded 
                                   FROM ActivityRecords 
                                   WHERE Username = ?
                                   ORDER BY DateRecorded DESC";

                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("?", username);
                        OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);
                        adapter.Fill(activityTable);
                    }

                    lblGoal.Text = $"{goal} kcal ({duration})";
                    lblBurned.Text = $"{totalBurned} kcal";
                    UpdateDataGridView(activityTable);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading summary from database: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblGoal.Text = "N/A";
                lblBurned.Text = "N/A";
            }
        }
        private void UpdateDataGridView(DataTable activityTable)
        {
            dataGridActivities.DataSource = activityTable;

            if (dataGridActivities.Columns.Contains("DateRecorded"))
            {
                dataGridActivities.Columns["DateRecorded"].DefaultCellStyle.Format = "dd-MM-yyyy";
                dataGridActivities.Columns["DateRecorded"].HeaderText = "Date Recorded";
            }

            if (dataGridActivities.Columns.Contains("CaloriesBurned"))
            {
                dataGridActivities.Columns["CaloriesBurned"].DefaultCellStyle.Format = "0";
                dataGridActivities.Columns["CaloriesBurned"].HeaderText = "Calories Burned";
            }

            dataGridActivities.AutoResizeColumns();
        }


        private void button5_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to log out?",
                                                 "Log Out",
                                                 MessageBoxButtons.YesNo,
                                                 MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                LoginForm LoginForm = new LoginForm(username);
                LoginForm.Show();
                this.Hide();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            String User = username;
            SetGoalForm setGoalForm = new SetGoalForm(username);
           setGoalForm.Show();
           LoadSummaryFromDatabase();
           this.Hide();

        }
       
        

        private void button4_Click(object sender, EventArgs e)
        {
            string User = username;
            ViewProgressForm ViewProgressForm = new ViewProgressForm(User);
            ViewProgressForm.Show();
            LoadSummaryFromDatabase();
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            String User = username;
            RecordActivityForm recordActivityForm = new RecordActivityForm(username);
            recordActivityForm.Show();
            LoadSummaryFromDatabase();
            this.Hide();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
