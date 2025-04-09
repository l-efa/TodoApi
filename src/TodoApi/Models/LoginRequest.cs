namespace LoginApi.Models
{
    public class LoginRequest
    {
        public string Name { get; set; }
        public string Password { get; set; }
    }

    public class AdminLogin
    {
        public string AdminID { get; set; }
        public string Password { get; set; }
    }
}