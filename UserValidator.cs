using System;
using System.Data.SqlClient;

namespace DDOOCP_Assignment
{
    public class UserValidator
    {
        private string connectionString;

        public UserValidator(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public bool ValidateUsername(string username)
        {
            return !string.IsNullOrWhiteSpace(username) && username.Length >= 4;
        }

        public bool ValidatePassword(string password)
        {
            return !string.IsNullOrWhiteSpace(password) && password.Length >= 6;
        }

        public bool ValidateEmail(string email)
        {
            return email.Contains("@") && email.Contains(".");
        }

        public bool ValidateUser(string username, string password)
        {
            if (username == null || password == null)
                throw new ArgumentNullException();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM Users WHERE Username=@Username AND Password=@Password";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@Password", password);

                int count = (int)command.ExecuteScalar();
                return count > 0;
            }
        }
    }
}
