using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Windows.Forms;

namespace DDOOCP_Assignment
{
    public partial class RecordActivityForm : Form
    {
        private string username;
        string connString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\owner\Desktop\ddoocp\Fitness\Fitness.mdb;";

        public RecordActivityForm(string user = "Guest")
        {
            InitializeComponent();
            LoadActivities();
            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            username = user;
           
        }
        private void LoadActivities()
        {
            comboBox1.Items.Clear();
            comboBox1.Items.Add("Walking");
            comboBox1.Items.Add("Swimming");
            comboBox1.Items.Add("Running");
            comboBox1.Items.Add("Cycling");
            comboBox1.Items.Add("Jump Rope");
            comboBox1.Items.Add("Yoga");
            comboBox1.Items.Insert(0, "Choose Activity");
            comboBox1.SelectedIndex = 0;
        }

        private void LoadActivityHistory()
        {
            try
            {
                using (OleDbConnection conn = new OleDbConnection(connString))
                {
                    conn.Open();
                    string query = "SELECT Activity, DateRecorded FROM ActivityRecords WHERE Username = @Username ORDER BY DateRecorded DESC";

                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);

                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            List<string> historyItems = new List<string>();

                            while (reader.Read())
                            {
                                string activity = reader["Activity"] != DBNull.Value
                                    ? reader["Activity"].ToString()
                                    : "Unknown";

                                DateTime date = reader["DateRecorded"] != DBNull.Value
                                    ? Convert.ToDateTime(reader["DateRecorded"])
                                    : DateTime.MinValue;

                                historyItems.Add($"{date:yyyy-MM-dd} - {activity}");
                            }

                            // Return history items without adding to ComboBox
                            // You could store these in a separate List or property
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading history: {ex.Message}");
            }
        }
        
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null) return;

            string selectedItem = comboBox1.SelectedItem.ToString();
            string selectedActivity = selectedItem.Contains(" - ")
                ? selectedItem.Split(new[] { " - " }, StringSplitOptions.None)[1].Trim()
                : selectedItem;

            lblMetric1.Text = "";
            lblMetric2.Text = "";
            lblMetric3.Text = "";

            switch (selectedActivity)
            {
                case "Walking":
                    lblMetric1.Text = "Steps:";
                    lblMetric2.Text = "Distance (km):";
                    lblMetric3.Text = "Time (min):";
                    break;
                case "Swimming":
                    lblMetric1.Text = "Laps:";
                    lblMetric2.Text = "Time (min):";
                    lblMetric3.Text = "Heart Rate (bpm):";
                    break;
                case "Running":
                    lblMetric1.Text = "Distance (km):";
                    lblMetric2.Text = "Time (min):";
                    lblMetric3.Text = "Speed (km/h):";
                    break;
                case "Cycling":
                    lblMetric1.Text = "Distance (km):";
                    lblMetric2.Text = "Time (min):";
                    lblMetric3.Text = "Average Speed (km/h):";
                    break;
                case "Jump Rope":
                    lblMetric1.Text = "Jumps:";
                    lblMetric2.Text = "Time (min):";
                    lblMetric3.Text = "Heart Rate (bpm):";
                    break;
                case "Yoga":
                    lblMetric1.Text = "Time (min):";
                    lblMetric2.Text = "Calories per min:";
                    lblMetric3.Text = "Intensity (1-10):";
                    break;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null)
            {
                MessageBox.Show("Please select an activity!", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!double.TryParse(txtMetric1.Text, out double metric1) ||
                !double.TryParse(txtMetric2.Text, out double metric2) ||
                !double.TryParse(txtMetric3.Text, out double metric3))
            {
                MessageBox.Show("Please enter valid numeric values!", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string selectedItem = comboBox1.SelectedItem.ToString();
            string selectedActivity = selectedItem.Contains(" - ")
                ? selectedItem.Split(new[] { " - " }, StringSplitOptions.None)[1].Trim()
                : selectedItem;

            double caloriesBurned = CalculateCalories(selectedActivity, metric1, metric2, metric3);
            lblCalories.Text = $"Calories Burned: {caloriesBurned:0.00} kcal";

            SaveActivityToDatabase(selectedActivity, metric1, metric2, metric3, caloriesBurned);
        }

        private double CalculateCalories(string activity, double metric1, double metric2, double metric3)
        {
            switch (activity)
            {
                case "Walking":
                    return (metric1 * 0.04) + (metric2 * 50) + (metric3 * 4);
                case "Swimming":
                    return (metric1 * 8) + (metric2 * 6) + (metric3 * 0.1);
                case "Running":
                    return (metric1 * 60) + (metric2 * 10) + (metric3 * 5);
                case "Cycling":
                    return (metric1 * 40) + (metric2 * 6) + (metric3 * 3);
                case "Jump Rope":
                    return (metric1 * 0.2) + (metric2 * 12) + (metric3 * 0.2);
                case "Yoga":
                    return (metric1 * metric2) + (metric3 * 2);
                default:
                    return 0;
            }
        }

        private void SaveActivityToDatabase(string activity, double metric1, double metric2, double metric3, double caloriesBurned)
        {
            try
            {
                using (OleDbConnection conn = new OleDbConnection(connString))
                {
                    conn.Open();
                    string insertQuery = @"INSERT INTO ActivityRecords 
                                (Username, Activity, Metric1, Metric2, Metric3, CaloriesBurned, DateRecorded) 
                                VALUES (@Username, @Activity, @Metric1, @Metric2, @Metric3, @CaloriesBurned, @Date)";

                    using (OleDbCommand cmd = new OleDbCommand(insertQuery, conn))
                    {
                        // Explicitly specify parameter types
                        cmd.Parameters.Add("@Username", OleDbType.VarWChar).Value = username;
                        cmd.Parameters.Add("@Activity", OleDbType.VarWChar).Value = activity;
                        cmd.Parameters.Add("@Metric1", OleDbType.Double).Value = metric1;
                        cmd.Parameters.Add("@Metric2", OleDbType.Double).Value = metric2;
                        cmd.Parameters.Add("@Metric3", OleDbType.Double).Value = metric3;
                        cmd.Parameters.Add("@CaloriesBurned", OleDbType.Double).Value = caloriesBurned;
                        cmd.Parameters.Add("@Date", OleDbType.Date).Value = DateTime.Now;

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Activity saved successfully!", "Success",
                                          MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Refresh the activities list
                            comboBox1.Items.Clear();
                            LoadActivities();
                            
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving activity: {ex.Message}\n\n" +
                               $"Activity: {activity}\n" +
                               $"Metrics: {metric1}, {metric2}, {metric3}\n" +
                               $"Calories: {caloriesBurned}",
                               "Database Error",
                               MessageBoxButtons.OK,
                               MessageBoxIcon.Error);
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            String User = username;
            DasboardForm dasboardForm = new DasboardForm(username);
            dasboardForm.Show();
            this.Hide();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            String User = username;
            SetGoalForm setGoalForm = new SetGoalForm(username);
            setGoalForm.Show();
            this.Hide();
        }


        private void button4_Click(object sender, EventArgs e)
        {
            string User = username;
            ViewProgressForm ViewProgressForm = new ViewProgressForm(User);
            ViewProgressForm.Show();
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
