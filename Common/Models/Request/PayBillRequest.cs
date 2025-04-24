using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.Request
{
    public class PayBillRequest
    {
        [Required(ErrorMessage = Constants.ACCOUNT_AccountName_REQUIRED)]
        public string AccountNumberFrom { get; set; }

        public List<FromParty> ToAccountNumbers { get; set; }

    }
    public class FromParty
    {
        [Required(ErrorMessage = Constants.ACCOUNT_AccountName_REQUIRED)]
        public string AccountNumberTo { get; set; }

        [Required(ErrorMessage = Constants.ACCOUNT_AccountName_REQUIRED)]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = Constants.ACCOUNT_AccountName_REQUIRED)]
        public string Currency { get; set; }

        [Required(ErrorMessage = Constants.ACCOUNT_AccountName_REQUIRED)]
        public string Frequency { get; set; }

        [Required(ErrorMessage = Constants.ACCOUNT_AccountName_REQUIRED)]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = Constants.ACCOUNT_AccountName_REQUIRED)]
        public DateTime EndDate { get; set; }
        public string Memo { get; set; }

    }
}
