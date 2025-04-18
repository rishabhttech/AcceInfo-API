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
}
