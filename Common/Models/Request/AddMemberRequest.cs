using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.Request
{
    public class AddMemberRequest
    {
        [Required(ErrorMessage = Constants.NAME_REQUIRED_MESSAGE)]
        public string Name { get; set; }

        [Required(ErrorMessage = Constants.EMAIL_REQUIRED_MESSAGE)]
        [EmailAddress(ErrorMessage = Constants.EMAIL_FORMAT_REQUIRED)]
        public string Email { get; set; }

        [Required(ErrorMessage = Constants.CONTACTNUMBER_FORMAT_REQUIRED)]
        [Phone(ErrorMessage = Constants.INVALID_PHONE)]
        public string ContactNumber { get; set; }

        public bool IstransferByEmail { get; set; }
        public bool IstransferByMobile { get; set; }
        public string NickName { get; set; }

        [Required(ErrorMessage = Constants.PREFERRED_METHOD_LANGUAGE)]
        public string PrefLanguage { get; set; }
    }
}
