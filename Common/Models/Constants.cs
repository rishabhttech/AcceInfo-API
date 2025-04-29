using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Principal;
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
        public static readonly string DATA_FOUND_SUCCESSFULLY = "Data found Successfully";
        public static readonly string DATA_NOT_FOUND = "Data not found";

        public static readonly string LOGIN_SUCCESSFULLY = "Login Successfull";
        public static readonly string LOGIN_FAILED = "Incorrect username or password. Please try again.";
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
        public const string ACCOUNT_NOT_FOUND = "Account not found.";
        public const string ACCOUNT_CHEQUING_ID = "1c130c28-9074-4e4e-94ed-a8aa6019cb43";
        #endregion

        #region Member
        public const string MEMBER_NOT_CREATED = "Member is required.";
        public const string EMAIL_REQUIRED_MESSAGE = "Email is required.";
        public const string NAME_REQUIRED_MESSAGE = "Name is required.";
        public const string EMAIL_FORMAT_REQUIRED= "Invalid email format.";
        public const string CONTACTNUMBER_FORMAT_REQUIRED = "Contact number is required.";
        public const string INVALID_PHONE = "Invalid phone number format.";
        public const string TRANSFER_METHOD_PHONE = "Transfer method is required.";
        public const string PREFERRED_METHOD_LANGUAGE = "Preferred language is required.";
        public const string ACCOUNT_NUMBER_REQUIRED = "Account number is required.";
        public const string ACCOUNT_TYPE_REQURED = "Account type is required.";
        public const string ROLE_MEMBER_VALUE = "ca603712-4ffc-4e49-8e29-1b1f58bdb713";
        public const string MEMBER_ADDED_SUCCESSFULLY = "Member Added Successfully";
        public const string ENTER_VALID_MEMBERID = "Please enter valid MemberId";
        #endregion

        #region Payee
        public const string PAYEE_NAME_REQUIRE = "Payee Name is required.";
        public const string PAYEE_NUMBER_REQUIRE = "Payee Number is required.";
        public const string PAYEE_TYPE_REQUIRE = "Payee Type is required.";
        public const string CUSTOMER_TYPE_ID = "31443fe6-1588-4b45-bc00-3717595a1cff";
        public const string CUSTOMER_NOT_CREATED = "Customer not created.";
        public const string First_NAME_REQUIRED = "First Name Required.";
        public const string Last_NAME_REQUIRED = "Last Name Required.";
        public const string PASSWORD_REQUIRED = "Password Required.";
        public const string USERNAME_REQUIRED = "UserName Required.";



        #endregion
    }
}
