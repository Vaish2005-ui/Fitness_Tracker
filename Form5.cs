using System;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace DDOOCP_Assignment
{
    public partial class ViewProgressForm : Form
    {
        private string username;
        string connString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\owner\Desktop\ddoocp\Fitness\Fitness.mdb;";
        private double? calorieGoal = null;
        private string goalDuration = "";

        public ViewProgressForm(string user = "Guest")
        {
            InitializeComponent();
            username = string.IsNullOrEmpty(user) ? "Guest" : user;
            LoadUserProgress();
           
        }

        private void ViewProgressForm_Load(object sender, EventArgs e)
        {
            if (!VerifyDatabaseStructure())
            {
                MessageBox.Show("Database structure verification failed!\nPlease check your tables and columns.",
                              "Configuration Error",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);
                this.Close();
                return;
            }
            LoadUserProgress();
        }

        private bool VerifyDatabaseStructure()
        {
            try
            {
                using (OleDbConnection conn = new OleDbConnection(connString))
                {
                    conn.Open();

                    using (var cmd = new OleDbCommand(
                        "SELECT TOP 1 ActivityID, Username, Activity, CaloriesBurned, DateRecorded FROM ActivityRecords", conn))
                    {
                        cmd.ExecuteReader().Close();
                    }

                    using (var cmd = new OleDbCommand(
                        "SELECT TOP 1 GoalID, Username, Goal, Duration FROM Goals", conn))
                    {
                        cmd.ExecuteReader().Close();
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database verification failed: {ex.Message}");
                return false;
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

        private void UpdateSummaryLabels(double totalCaloriesBurned)
        {
            lblBurned.Text = $"Burned: {totalCaloriesBurned:0} kcal";

            if (calorieGoal.HasValue)
            {
                string durationText = string.IsNullOrEmpty(goalDuration) ? "" : $" ({goalDuration})";
                lblGoal.Text = $"Goal: {calorieGoal.Value:0} kcal{durationText}";

                double progressPercentage = (totalCaloriesBurned / calorieGoal.Value) * 100;
                lblStatus.Text = $"Progress: {progressPercentage:0}%";
            }
            else
            {
                lblGoal.Text = "Goal: Not Set";
                lblStatus.Text = "Progress: N/A";
                
            }
        }


        private void LoadUserProgress()
        {
            if (string.IsNullOrEmpty(username))
            {
                MessageBox.Show("Username not found. Please log in again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            double totalCaloriesBurned = 0;
            DataTable activityTable = new DataTable();

            try
            {
                using (OleDbConnection conn = new OleDbConnection(connString))
                {
                    conn.Open();

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

                    string goalQuery = "SELECT Goal, Duration FROM Goals WHERE Username = ?";
                    using (OleDbCommand cmd = new OleDbCommand(goalQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("?", username);
                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                if (reader["Goal"] != DBNull.Value)
                                {
                                    calorieGoal = Convert.ToDouble(reader["Goal"]);
                                }
                                if (reader["Duration"] != DBNull.Value)
                                {
                                    goalDuration = reader["Duration"].ToString();
                                }
                            }
                        }
                    }

                    foreach (DataRow row in activityTable.Rows)
                    {
                        if (row["CaloriesBurned"] != DBNull.Value)
                        {
                            totalCaloriesBurned += Convert.ToDouble(row["CaloriesBurned"]);
                        }
                    }

                    UpdateDataGridView(activityTable);
                    UpdateSummaryLabels(totalCaloriesBurned);
                    UpdateProgressChart(totalCaloriesBurned);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading progress: {ex.Message}",
                               "Database Error",
                               MessageBoxButtons.OK,
                               MessageBoxIcon.Error);
            }
        }

        private void UpdateProgressChart(double totalCaloriesBurned)
        {
            chart1.Series.Clear();
            chart1.Titles.Clear();

            if (calorieGoal.HasValue || totalCaloriesBurned > 0)
            {
                Series barSeries = new Series("Calories")
                {
                    ChartType = SeriesChartType.Column,
                    IsValueShownAsLabel = true,
                    Color = Color.SteelBlue
                };

                barSeries.Points.AddXY("Burned", totalCaloriesBurned);

                if (calorieGoal.HasValue)
                {
                    barSeries.Points.AddXY("Goal", calorieGoal.Value);

                    Series lineSeries = new Series("Target")
                    {
                        ChartType = SeriesChartType.Line,
                        BorderWidth = 3,
                        Color = Color.Red,
                        BorderDashStyle = ChartDashStyle.Dash
                    };
                    lineSeries.Points.AddY(calorieGoal.Value);
                    chart1.Series.Add(lineSeries);
                }

                chart1.Series.Add(barSeries);
                chart1.ChartAreas[0].AxisY.Minimum = 0;
                chart1.Titles.Add("Calorie Progress");

                if (!string.IsNullOrEmpty(goalDuration))
                {
                    chart1.Titles.Add($"Goal Duration: {goalDuration}");
                }
            }
        }

        private void RefreshBtn_Click(object sender, EventArgs e)
        {
            LoadUserProgress();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DasboardForm dasboardForm = new DasboardForm(username);
            dasboardForm.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
            SetGoalForm setGoalForm = new SetGoalForm(username);
            setGoalForm.Show();
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
           
            RecordActivityForm recordActivityForm = new RecordActivityForm(username);
            recordActivityForm.Show();
            this.Hide();
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

      
    }
}