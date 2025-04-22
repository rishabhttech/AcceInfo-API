using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.Request
{
    public class CreateAccountRequest
    {
        [Required(ErrorMessage = Constants.ACCOUNT_CONTACTID_REQUIRED)]
        public string ContactId { get; set; }
        [Required(ErrorMessage = Constants.ACCOUNT_AccountName_REQUIRED)]
        public string AccountName { get; set; }
        [Required(ErrorMessage = Constants.ACCOUNT_AccountType_REQUIRED)]
        public string AccountType { get; set; }
        [Required(ErrorMessage = Constants.ACCOUNT_Amount_REQUIRED)]
        [Range(0.01, double.MaxValue, ErrorMessage = Constants.ACCOUNT_VALID_AMOUNT_ERROR)]
        public decimal Amount { get; set; }
    }
    public class TransactionHistoryRequest
    {
        public string AccountId { get; set; }
    }
}
