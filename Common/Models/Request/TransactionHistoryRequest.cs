using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.Request
{
    public class TransactionHistoryRequest
    {
        public string AccountId { get; set; }
        public DateTime? StartDate { get; set; } 
        public DateTime? EndDate { get; set; }   
    }
}
