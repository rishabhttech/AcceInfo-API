using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Query
{
    public class Auth
    {
        public string DoLogin = @"
                SELECT c.""ContactId"", c.""UserName"", c.""First Name"" AS ""FirstName"", c.""Last Name"" AS ""LastName""
                FROM ""Contact"" c
                JOIN ""ContactRoleJn"" crj ON c.""ContactId"" = crj.""ContactId""
                JOIN ""Roles"" r ON crj.""RoleId"" = r.""RoleId""
                WHERE c.""UserName"" = @UserName AND c.""Password"" = @Password AND LOWER(r.""Name"") = @Role
                ";

        public string insertContactSql = @"
    INSERT INTO ""Contact"" (
        ""MobileNumber"",
        ""SendTransferBy"",
        ""Email"",
        ""NickName"",
        ""Language"",
        ""First Name""
    )
    VALUES (
        @MobileNumber,
        @SendTransferBy,
        @Email,
        @Nickname,
        @Language,
        @Name
    )
    RETURNING ""ContactId"";
";

        public string ContactDetails = @"
SELECT 
    c.*, 
    c.""First Name"" AS ""FirstName"", 
    c.""Last Name"" AS ""LastName"", 
    c.""Date of Birth"" AS ""DOB""
FROM public.""Contact"" c 
WHERE c.""ContactId"" = @CustomerId";
    }
    public class Account
    {
         public string AccountTypeList = @"SELECT * FROM public.""AccountCategory"" ORDER BY ""Name"" ASC";


        public string insertAccountSql = @"
    INSERT INTO ""Account"" (
        ""AccountNumber"",
        ""Balance"",
        ""Status"",
        ""AccountCategory"",
        ""Name""
    )
    VALUES (
        @AccountNumber,
        @Balance,
        @Status,
        @AccountCategory,
        @Name
    )
    RETURNING ""AccountId"";
";

        public string insertCustomerAccountSql = @"
    INSERT INTO ""CustomerAccountJn"" (
        ""Account"",
        ""Customer"",
        ""Status""
    )
    VALUES (
        @AccountId,
        @CustomerId,
        @Status
    )
    RETURNING ""CustomerAccountJnId"";
";

        public string getCustomerAccounts = @"
    SELECT 
        a.""AccountId"",
        a.""AccountNumber"",
        a.""Name"",
        ac.""Name"" AS ""AccountCategoryName"",
        ac.""AccountCategoryId"",
        a.""Balance""
    FROM ""Account"" a
    JOIN ""AccountCategory"" ac ON a.""AccountCategory"" = ac.""AccountCategoryId""
    JOIN ""CustomerAccountJn"" caj ON caj.""Account"" = a.""AccountId""
    WHERE caj.""Customer"" = @CustomerId;
";

        public string getAccountById = @"
    SELECT 
        a.""AccountId"",
        a.""AccountNumber"",
        ac.""Name"" AS ""AccountCategoryName"",
        ac.""AccountCategoryId"",
        a.""Balance"",
        a.""Status""
    FROM ""Account"" a
    JOIN ""AccountCategory"" ac ON a.""AccountCategory"" = ac.""AccountCategoryId""
    WHERE a.""AccountId"" = @AccountId;
";

        public string GetTransactionHistoryByAccountId = @"
    SELECT 
        t.""TransactionId"",
        t.""TransactionFrom"",
        t.""TransactionTo"",
        t.""CreatedOn"",
        t.""Amount"",
        t.""Note"",
        t.""TransactionType"",
        t.""IsSelfTransfer""
    FROM ""Transactions"" t
    ORDER BY t.""CreatedOn"" DESC;
";

        public string checkAccountSql = @"
    SELECT ""AccountId""
    FROM ""Account""
    WHERE ""AccountNumber"" = @AccountNumber;
";

        public string insertContactRoleJnSql = @"
    INSERT INTO ""ContactRoleJn"" (
        ""ContactId"",
        ""RoleId"",
        ""MemberId""
    )
    VALUES (
        @ContactId,
        @RoleId,
        @MemberId
    )
    RETURNING ""ContactRoleJnId""
";

        public string insertCustomerAccountJnSql = @"
    INSERT INTO ""CustomerAccountJn"" (
        ""ContactId"",
        ""AccountId""
    )
    VALUES (   
        @ContactId,
        @AccountId
    );
";

        public string GetMemberQuery= @"SELECT * FROM public.""ContactRoleJn"" WHERE ""MemberId"" IS NOT NULL AND ""ContactId"" = @ContactId";

        public string GetPayeeCategoriesQuery = @"SELECT * FROM public.""PayeeType"" ORDER BY ""Name"" ASC;";
        public string GetPayeeByContact = @"SELECT  ""PayeeId"",
    ""Payee Name"" AS ""PayeeName"",
    ""Payee Number"" AS ""PayeeNumber"",
    ""PayeeType"",
    ""PayeeId""
    ""ContactId""
FROM public.""Payee""
WHERE ""ContactId"" = @ContactId;";

        public string GetMemberListOfContact = @"
SELECT 
    r.""RecipientId"",
    r.""IstransferByEmail"",
    r.""IstransferByMobile"",
    r.""Name"",
    r.""Email"",
    r.""ContactNumber"",
    r.""NickName"",
    r.""PrefLanguage"",
    r.""CreatedOn"",
    a.""AccountId"",
    a.""AccountNumber"",
    a.""Balance""
FROM public.""ContactRoleJn"" crj
JOIN public.""Recipient"" r ON r.""RecipientId"" = crj.""MemberId""
JOIN public.""CustomerAccountJn"" caj ON caj.""Customer"" = r.""Contact""
JOIN public.""Account"" a ON a.""AccountId"" = caj.""Account""
JOIN public.""AccountCategory"" ac ON ac.""AccountCategoryId"" = a.""AccountCategory""
WHERE crj.""ContactId"" = @ContactId
  AND crj.""MemberId"" IS NOT NULL;
";


        public string TransferAccountInfo = @"
    SELECT ""Balance""
    FROM ""Account""
    WHERE ""AccountId"" = @AccountNumberFrom;
";

            public string TransferbyAccount = @"
        BEGIN;

            INSERT INTO ""Transactions"" (
                ""TransactionFrom"",
                ""TransactionTo"",
                ""Amount"",
                ""Note""
            ) 
            VALUES (
                @AccountNumberFrom,
                @AccountNumberTo,
                @Amount,
                @Note
            );
        
            UPDATE ""Account""
            SET ""Balance"" = ""Balance"" - @Amount
            WHERE ""AccountId"" = @AccountNumberFrom;

            UPDATE ""Account""
            SET ""Balance"" = ""Balance"" + @Amount
            WHERE ""AccountId"" = @AccountNumberTo;



        COMMIT;
";

        public string CheckIfMemberExistbyEmail = @"SELECT ""ContactId"" FROM public.""Contact"" WHERE ""Email"" = @Email";
        public string insertRecipientSql = @"
INSERT INTO public.""Recipient"" (
    ""Name"",
    ""Email"",
    ""ContactNumber"",
    ""IstransferByEmail"",
    ""IstransferByMobile"",
    ""PrefLanguage"",
    ""NickName"",
    ""Contact""
)
VALUES (
    @Name,
    @Email,
    @ContactNumber,
    @IstransferByEmail,
    @IstransferByMobile,
    @PrefLanguage,
    @NickName,
    @Contact
)
RETURNING ""RecipientId"";
";

        public string AddPayeeQuery = @"
INSERT INTO public.""Payee"" (
    ""Payee Name"",
    ""Payee Number"",
    ""PayeeType"",
    ""ContactId""
)
VALUES (
    @PayeeName,
    @PayeeNumber,
    @PayeeType,
    @ContactId
)
RETURNING ""PayeeId"";";
    }
}
