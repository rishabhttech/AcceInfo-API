

namespace Common.Models.Response
{
    public class LoginResponse : ResponseModel
    {
        public string Token { get; set; }
        public string Name { get; set; }
        public string TokenType { get; set; } = "Bearer";
        public int ExpiresIn { get; set; }
        public string RefreshToken { get; set; }
    }
    
}
