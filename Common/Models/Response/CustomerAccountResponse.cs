using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.Response
{
    public class CustomerAccountResponse
    {
        public Dictionary<string, List<AccountDetail>> Accounts { get; set; }
    }
    public class AccountDetail
    {
        public int AccountId { get; set; }
        public string Nickname { get; set; }
        public string AccountType { get; set; }
        public decimal Balance { get; set; }
        public string AccountNumber { get; set; }
        public string Currency { get; set; }
    }
}
