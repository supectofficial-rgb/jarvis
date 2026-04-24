namespace Insurance.WebApp.Panel.Models
{
    public class ApiResponse<T>
    {
        public bool IsSuccess { get; set; }
        public T? Data { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public DateTime TokenExpiration { get; set; }
        public UserInfo? User { get; set; }
        public List<object>? Memberships { get; set; }
        public List<string>? Permissions { get; set; }
    }

    public class UserInfo
    {
        public long Id { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
    }
}

