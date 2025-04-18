using Common.Models;
using Common.Models.Response;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Common.Helper
{
    public class AuthHelper
    {
        private static Dictionary<string, string> _refreshTokenStore = new Dictionary<string, string>();
        private readonly IConfiguration _configuration;
        private readonly LoginResponse _responseModel;
        public AuthHelper(IConfiguration configuration)
        {
            _configuration = configuration;
            _responseModel = new LoginResponse();
        }

        public LoginResponse GenerateJwtToken(string username, string JwtKey, string issuer, string audience)
        {
            if (!string.IsNullOrEmpty(JwtKey) && !string.IsNullOrEmpty(username))
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtKey));

                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var claims = new[]
                {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(ClaimTypes.Role, "User"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };
               
                var token = new JwtSecurityToken(
                    issuer: issuer,
                    audience: audience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(30),
                    signingCredentials: credentials
                );
                var refreshToken = Guid.NewGuid().ToString();
                _refreshTokenStore[username] = refreshToken;

                _responseModel.Status = Constants.SUCCESS_STATUS;
                _responseModel.Message = Constants.TOKEN_GENERATED_SUCCESSFULLY;
                _responseModel.Token = new JwtSecurityTokenHandler().WriteToken(token);
                _responseModel.RefreshToken = refreshToken;
                _responseModel.ExpiresIn = 1800;
            }
            else
            {
                _responseModel.Status = Constants.FAILED_STATUS;
                _responseModel.Message = Constants.TOKEN_GENERATED_FAILED;

                if (string.IsNullOrEmpty(username))
                    _responseModel.ErrorMessages.Add(Constants.LOGIN_TOKEN_USERNAME_EMPTY);
                if (string.IsNullOrEmpty(username))
                    _responseModel.ErrorMessages.Add(Constants.TOKEN_TOKEN_KEY_EMPTY);
            }
            return _responseModel;
        }

        public LoginResponse RefreshJwtToken(string refreshToken, string username, string JwtKey, string issuer, string audience)
        {
            if (_refreshTokenStore.TryGetValue(username, out var storedRefreshToken) && storedRefreshToken == refreshToken)
            {
                return GenerateJwtToken(username, JwtKey, issuer, audience); // generate new token
            }

            return new LoginResponse
            {
                Status = Constants.FAILED_STATUS,
                Message = "Invalid or expired refresh token.",
                ErrorMessages = new List<string> { "The provided refresh token is invalid or does not match." }
            };
        }

        public static string Generate13DigitAccountNumber()
        {
            var random = new Random();
            return random.Next(1000000, 9999999).ToString() + random.Next(1000000, 9999999).ToString("D6").Substring(0, 6);
        }
    }
}
