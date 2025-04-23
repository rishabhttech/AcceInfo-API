using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.Response
{
    public class TransferResponse
    {
        public string AccountNumberFrom { get; set; }
        public string AccountNumberTo { get; set; }
        public decimal Amount { get; set; }
    }


}
