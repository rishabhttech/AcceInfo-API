using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.Request
{
    public class AddPayeeRequest
    {
        [Required(ErrorMessage = "Payee Name is required.")]
        public string PayeeName { get; set; }
        [Required(ErrorMessage = "Payee Number is required.")]
        public string PayeeNumber { get; set; }
        [Required(ErrorMessage = "Payee Type is required.")]
        public string PayeeType { get; set; }
    }
}
