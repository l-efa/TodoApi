namespace LoginApi.Models
{
    public class User
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public string? Password { get; set; }
        public bool IsLocked { get; set; }
        public bool IsLoggedIn { get; set; }
        public bool IsAdmin { get; set; }
        public List<string> friends { get; set; } = new List<string>();
    }
}