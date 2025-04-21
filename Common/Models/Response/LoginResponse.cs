

namespace Common.Models.Response
{
    public class LoginResponse : ResponseModel
    {
        public string Token { get; set; }
        public string Name { get; set; }
        public string TokenType { get; set; } = "Bearer";
        public int ExpiresIn { get; set; }
        public string RefreshToken { get; set; }
        public string ContactId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
    
}
