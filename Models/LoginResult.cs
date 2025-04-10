namespace DDOOCP_Assignment.Models
{
    public class LoginResult
    {
        public bool IsSuccess { get; }
        public string Message { get; }
        public bool IsLocked { get; }

        public LoginResult(bool isSuccess, string message, bool isLocked = false)
        {
            IsSuccess = isSuccess;
            Message = message;
            IsLocked = isLocked;
        }
    }

}
