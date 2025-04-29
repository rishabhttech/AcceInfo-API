using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.Request
{
    public class CustomerRequestModel
    {
        [Required(ErrorMessage = Constants.First_NAME_REQUIRED)]
        public string FirstName { get; set; }
        [Required(ErrorMessage = Constants.Last_NAME_REQUIRED)]
        public string LastName { get; set; }
        [Required(ErrorMessage = Constants.PASSWORD_REQUIRED)]
        public string Password { get; set; }
        [Required(ErrorMessage = Constants.USERNAME_REQUIRED)]
        public string UserName { get; set; }
        [Required(ErrorMessage = Constants.EMAIL_REQUIRED_MESSAGE)]
        [EmailAddress(ErrorMessage = Constants.EMAIL_FORMAT_REQUIRED)]
        public string Email { get; set; }
        [Required(ErrorMessage = Constants.CONTACTNUMBER_FORMAT_REQUIRED)]
        [Phone(ErrorMessage = Constants.INVALID_PHONE)]
        public string MobileNumber { get; set; }
    }
}
