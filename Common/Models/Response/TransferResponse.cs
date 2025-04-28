using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.Response
{
    public class TransferResponse
    {
        public Guid TransactionId { get; set; }
        public DateTime CreatedOn { get; set; }

        public string AccountNumberFrom { get; set; }
        public string AccountNumberTo { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public Boolean IsSelfTransfer { get; set; }
        public string Frequency { get; set; }
        public string Note { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string TransactionType { get; set; }


    }


}
