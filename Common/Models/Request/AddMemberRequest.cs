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

        public string NickName { get; set; }
        [Required(ErrorMessage = Constants.NAME_REQUIRED_MESSAGE)]
        public string Name { get; set; }

        [Required(ErrorMessage = Constants.EMAIL_REQUIRED_MESSAGE)]
        [EmailAddress(ErrorMessage = Constants.EMAIL_FORMAT_REQUIRED)]
        public string Email { get; set; }

        [Required(ErrorMessage = Constants.CONTACTNUMBER_FORMAT_REQUIRED)]
        [Phone(ErrorMessage = Constants.INVALID_PHONE)]
        public string ContactNumber { get; set; }

        [Required(ErrorMessage = Constants.TRANSFER_METHOD_PHONE)]
        public string TransferMethod { get; set; }

        [Required(ErrorMessage = Constants.PREFERRED_METHOD_LANGUAGE)]
        public string PrefLanguage { get; set; }

        [Required(ErrorMessage = Constants.ACCOUNT_NUMBER_REQUIRED)]
        public string AccountNumber { get; set; }

        [Required(ErrorMessage =Constants.ACCOUNT_TYPE_REQURED)]
        public string Accounttype { get; set; }

        
    }
}
