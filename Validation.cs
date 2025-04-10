using System;
using System.Linq;

namespace DDOOCP_Assignment
{
    public static class Validation
    {
        public static bool ValidateUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return false;

            return username.All(char.IsLetterOrDigit);
        }
    }
}
