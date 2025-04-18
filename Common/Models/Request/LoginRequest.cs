using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.Request
{
    public class LoginRequest
    {
        [Required(ErrorMessage = Constants.LOGIN_TOKEN_USERNAME_EMPTY)]
        public string Username { get; set; }
        [Required(ErrorMessage = Constants.LOGIN_PASSWORD_MISSING)]
        public string Password { get; set; }
        [Required(ErrorMessage = Constants.LOGIN_TYPE_MISSING)]
        public string Type { get; set; }
        public string RefreshToken { get; set; }
    }
    public class OtpRequest
    {
        public string otp { get; set; }
    }
}
