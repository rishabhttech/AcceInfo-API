using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.Response
{
    public class AccountCategoriesResponse
    {
        public string Name { get; set; }
        public string AccountCategoryId { get; set; }
    }
    public class AccountResponse : ResponseModel
    {
        public string AccountId { get; set; }
    }

    public class TransactionHistoryResponse
    {
        public string TransactionId { get; set; }
        public string? TransactionFrom { get; set; }
        public string? TransactionTo { get; set; }
        public DateTime CreatedOn { get; set; }
        public int Amount { get; set; }
        public string? Note { get; set; }
        public string? TransactionType { get; set; }
        public bool IsSelfTransfer { get; set; }
        public string FromAccountNumber { get; set; }
        public string ToAccountNumber { get; set; }
        public string TransactionFromCustomerName { get; set; }
        public string TransactionToCustomerName { get; set; }
        public bool isCredit { get; set; }
        public string FromAccountType { get; set; }
        public string TransactionNumber { get; set; }
        public string ToAccountType { get; set; }
    }


}
