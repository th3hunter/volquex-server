namespace Volquex.Models.Firebase
{
    public class AuthResponse
    {
        public User[] users { get; set; }
    }

    public class User
    {
        public string localId { get; set; }
        public string email { get; set; }
        public string displayName { get; set; }
        public string photoUrl { get; set; }
    }
}