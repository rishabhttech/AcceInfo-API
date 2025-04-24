using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.Request
{
    public class TransferRequest
    {
        [Required(ErrorMessage = Constants.ACCOUNT_AccountName_REQUIRED)]
        public string AccountNumberTo { get; set; }

        [Required(ErrorMessage = Constants.ACCOUNT_AccountName_REQUIRED)]
        public string AccountNumberFrom { get; set; }

        [Required(ErrorMessage = Constants.ACCOUNT_Amount_REQUIRED)]
        [Range(0.01, double.MaxValue, ErrorMessage = Constants.ACCOUNT_VALID_AMOUNT_ERROR)]
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public bool IsSelfTransfer { get; set; }
        public string Frequency { get; set; }
        public string Note { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

    }

}
