public class User
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }

    public bool IsLocked { get; set; }
    public int FailedAttempts { get; set; }
    public int DailyCalorieGoal { get; set; }

    public User(string username, string email, string password)
    {
        Username = username;
        Email = email;
        Password = password;
        IsLocked = false;
        FailedAttempts = 0;
        DailyCalorieGoal = 0;
    }
}
