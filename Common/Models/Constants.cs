using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class Constants
    {

        public const string DB_CONNECTIONSTRING = "ConnectionStrings:PostgreSqlConnection";
   
        public static readonly string SUCCESS_STATUS = "Success";
        public static readonly string FAILED_STATUS = "Failed";
        public static readonly string ERROR_STATUS = "An Error Occured";

        public static readonly string LOGIN_SUCCESSFULLY = "Login Successfull";
        public static readonly string LOGIN_FAILED = "Login Failed";
        public const string LOGIN_TOKEN_USERNAME_EMPTY = "Username is missing.";
        public const string LOGIN_PASSWORD_MISSING = "Password is missing.";
        public const string LOGIN_TYPE_MISSING = "Password is missing.";

        public static readonly string MISSING_FIELDS = "Missing required fields.";

        #region Token
        public static readonly string TOKEN_GENERATED_SUCCESSFULLY = "Token Generated Successfully.";
        public static readonly string TOKEN_GENERATED_FAILED = "Token Generated Failed.";
        public static readonly string TOKEN_TOKEN_KEY_EMPTY = "Key is missing.";
        #endregion

        #region Account
        public const string ACCOUNT_CONTACTID_REQUIRED = "ContactId is required.";
        public const string ACCOUNT_AccountName_REQUIRED = "Account Name is required.";
        public const string ACCOUNT_AccountType_REQUIRED = "Account Type is required.";
        public const string ACCOUNT_Amount_REQUIRED = "Amount is required.";
        public const string ACCOUNT_VALID_AMOUNT_ERROR = "Amount must be greater than zero.";
        #endregion
    }
}
