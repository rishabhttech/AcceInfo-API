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
    SELECT 
        c.""ContactId"", 
        c.""UserName"", 
        c.""First Name"" AS ""FirstName"", 
        c.""Last Name"" AS ""LastName""
    FROM ""Contact"" c
    JOIN ""ContactRoleJn"" crj ON c.""ContactId"" = crj.""ContactId""
    JOIN ""Roles"" r ON crj.""RoleId"" = r.""RoleId""
    WHERE 
        (c.""UserName"" = @UserName OR c.""CardNumber"" = @UserName)
        AND c.""Password"" = @Password 
        AND LOWER(r.""Name"") = @Role;
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
        public string GetTransactionHistoryByAccountIdWithDate = @"
SELECT 
    t.""TransactionId"",
    t.""TransactionFrom"",
    t.""TransactionTo"",
    af.""AccountNumber"" AS ""TransactionFromAccountNumber"",
    cf.""First Name"" || ' ' || cf.""Last Name"" AS ""TransactionFromCustomerName"",
    acf.""Name"" AS ""TransactionFromAccountCategoryName"",
    
    CASE 
        WHEN t.""TransactionType"" = 'Bill Transfer' THEN pt.""Payee Name""
        ELSE at.""AccountNumber""
    END AS ""TransactionToAccountNumber"",
    
    CASE 
        WHEN t.""TransactionType"" = 'Bill Transfer' THEN pt.""Payee Name""
        ELSE ct.""First Name"" || ' ' || ct.""Last Name""
    END AS ""TransactionToCustomerName"",
    
    CASE 
        WHEN t.""TransactionType"" = 'Bill Transfer' THEN ptt.""Name""
        ELSE act.""Name""
    END AS ""TransactionToAccountCategoryName"",
    
    pt.""Payee Name"" AS ""PayeeName"",
    ptt.""Name"" AS ""PayeeTypeName"",
    pt.""Payee Number"" AS ""PayeeNumber"",
    
    t.""CreatedOn"",
    t.""Amount"",
    t.""Note"",
    t.""TransactionType"",
    t.""IsSelfTransfer"",
    t.""TransactionNumber""

FROM 
    ""Transactions"" t

LEFT JOIN ""Account"" af ON t.""TransactionFrom"" = af.""AccountId""
LEFT JOIN ""CustomerAccountJn"" caf ON af.""AccountId"" = caf.""Account""
LEFT JOIN ""Contact"" cf ON caf.""Customer"" = cf.""ContactId""
LEFT JOIN ""AccountCategory"" acf ON af.""AccountCategory"" = acf.""AccountCategoryId""

LEFT JOIN ""Account"" at ON t.""TransactionTo"" = at.""AccountId""
LEFT JOIN ""CustomerAccountJn"" cat ON at.""AccountId"" = cat.""Account""
LEFT JOIN ""Contact"" ct ON cat.""Customer"" = ct.""ContactId""
LEFT JOIN ""AccountCategory"" act ON at.""AccountCategory"" = act.""AccountCategoryId""

LEFT JOIN ""Payee"" pt ON t.""TransactionTo"" = pt.""PayeeId""
LEFT JOIN ""PayeeType"" ptt ON pt.""PayeeType"" = ptt.""PayeeTypeId""

WHERE 
    (t.""TransactionFrom"" = @AccountId OR t.""TransactionTo"" = @AccountId)
    AND t.""CreatedOn"" BETWEEN @StartDate AND @EndDate

ORDER BY 
    t.""CreatedOn"" DESC

LIMIT 20;
";

        public string GetLast20TransactionHistoryByAccountId = @"
SELECT 
    t.""TransactionId"",
    t.""TransactionFrom"",
    t.""TransactionTo"",
    af.""AccountNumber"" AS ""TransactionFromAccountNumber"",
    cf.""First Name"" || ' ' || cf.""Last Name"" AS ""TransactionFromCustomerName"",
    acf.""Name"" AS ""TransactionFromAccountCategoryName"",
    
    CASE 
        WHEN t.""TransactionType"" = 'Bill Transfer' THEN pt.""Payee Name""
        ELSE at.""AccountNumber""
    END AS ""TransactionToAccountNumber"",
    
    CASE 
        WHEN t.""TransactionType"" = 'Bill Transfer' THEN pt.""Payee Name""
        ELSE ct.""First Name"" || ' ' || ct.""Last Name""
    END AS ""TransactionToCustomerName"",
    
    CASE 
        WHEN t.""TransactionType"" = 'Bill Transfer' THEN ptt.""Name""
        ELSE act.""Name""
    END AS ""TransactionToAccountCategoryName"",
    
    pt.""Payee Name"" AS ""PayeeName"",
    ptt.""Name"" AS ""PayeeTypeName"",
    pt.""Payee Number"" AS ""PayeeNumber"",
    
    t.""CreatedOn"",
    t.""Amount"",
    t.""Note"",
    t.""TransactionType"",
    t.""IsSelfTransfer"",
    t.""TransactionNumber""

FROM 
    ""Transactions"" t

LEFT JOIN ""Account"" af ON t.""TransactionFrom"" = af.""AccountId""
LEFT JOIN ""CustomerAccountJn"" caf ON af.""AccountId"" = caf.""Account""
LEFT JOIN ""Contact"" cf ON caf.""Customer"" = cf.""ContactId""
LEFT JOIN ""AccountCategory"" acf ON af.""AccountCategory"" = acf.""AccountCategoryId""

LEFT JOIN ""Account"" at ON t.""TransactionTo"" = at.""AccountId""
LEFT JOIN ""CustomerAccountJn"" cat ON at.""AccountId"" = cat.""Account""
LEFT JOIN ""Contact"" ct ON cat.""Customer"" = ct.""ContactId""
LEFT JOIN ""AccountCategory"" act ON at.""AccountCategory"" = act.""AccountCategoryId""

LEFT JOIN ""Payee"" pt ON t.""TransactionTo"" = pt.""PayeeId""
LEFT JOIN ""PayeeType"" ptt ON pt.""PayeeType"" = ptt.""PayeeTypeId""

WHERE 
    t.""TransactionFrom"" = @AccountId
    OR t.""TransactionTo"" = @AccountId

ORDER BY 
    t.""CreatedOn"" DESC

LIMIT 20;
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

        public string insertCustomerRoleJnSql = @"
    INSERT INTO ""ContactRoleJn"" (
        ""ContactId"",
        ""RoleId""
        
    )
    VALUES (
        @ContactId,
        @RoleId
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
        public string GetPayeeByContact = @"
SELECT  
    p.""PayeeId"",
    p.""Payee Name"" AS ""PayeeName"",
    p.""Payee Number"" AS ""PayeeNumber"",
    pt.""Name"" AS ""PayeeTypeName"",
    p.""ContactId""
FROM public.""Payee"" p
JOIN public.""PayeeType"" pt ON pt.""PayeeTypeId"" = p.""PayeeType""
WHERE p.""ContactId"" = @ContactId;
";

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
    c.""ContactId"" AS ""MemberContactId"",
    a.""AccountId"",
    a.""AccountNumber"",
    a.""Balance"",
    a.""Name""
FROM public.""ContactRoleJn"" crj
JOIN public.""Recipient"" r ON r.""RecipientId"" = crj.""MemberId""
JOIN public.""Contact"" c ON c.""Email"" = r.""Email""
JOIN public.""CustomerAccountJn"" caj ON caj.""Customer"" = c.""ContactId""
JOIN public.""Account"" a ON a.""AccountId"" = caj.""Account""
JOIN public.""AccountCategory"" ac ON ac.""AccountCategoryId"" = a.""AccountCategory""
WHERE crj.""ContactId"" = @ContactId
  AND crj.""MemberId"" IS NOT NULL
  AND ac.""Name"" = 'Spending (Chequing)';
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
        ""Currency"",
        ""IsSelfTransfer"",
        ""Frequency"",
        ""Note"",
        ""StartDate"",
        ""EndDate"",
        ""TransactionNumber"",
        ""TransactionType""
    ) 
    VALUES (
        @AccountNumberFrom,
        @AccountNumberTo,
        @Amount,
        @Currency,
        @IsSelfTransfer,
        @Frequency,
        @Note,
        @StartDate,
        @EndDate,
        @TransactionNumber,
        @TransactionType
        
    )
    RETURNING 
        ""TransactionId"", 
        ""CreatedOn"",
        ""TransactionNumber""
        ""TransactionFrom"",
        ""TransactionTo"",
        ""Amount"",
        ""Currency"",
        ""IsSelfTransfer"",
        ""Frequency"",
        ""Note"",
        ""StartDate"",
        ""EndDate"",
        ""TransactionNumber"",
        ""TransactionType"";

    UPDATE ""Account""
    SET ""Balance"" = ""Balance"" - @Amount
    WHERE ""AccountId"" = @AccountNumberFrom;

    UPDATE ""Account""
    SET ""Balance"" = ""Balance"" + @Amount
    WHERE ""AccountId"" = @AccountNumberTo;

COMMIT;
";

        public string PayBill = @"
BEGIN;
 
INSERT INTO ""Transactions"" (
    ""TransactionFrom"",
    ""TransactionTo"",
    ""Amount"",
    ""Currency"",
    ""Frequency"",
    ""Note"",
    ""StartDate"",
    ""EndDate"",
    ""TransactionNumber"",
    ""TransactionType""
) 
VALUES (
    @AccountNumberFrom,
    @AccountNumberTo,
    @Amount,
    @Currency,
    @Frequency,
    @Note,
    @StartDate,
    @EndDate,
    @TransationNumber, 
    @TransactionType
)
RETURNING 
    ""TransactionId"", 
    ""CreatedOn"",
    ""TransactionFrom"",
    ""TransactionTo"",
    ""Amount"",
    ""Currency"",
    ""Frequency"",
    ""Note"",
    ""StartDate"",
    ""EndDate"",
    ""TransactionNumber"",
    ""TransactionType"";
 
UPDATE ""Account""
SET ""Balance"" = ""Balance"" - @Amount
WHERE ""AccountId"" = @AccountNumberFrom;
 
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

    public class Customer
    {
        public string insertCustomerQuery = @"
        INSERT INTO public.""Contact"" (
            ""First Name"",
            ""Last Name"",
            ""Password"",
            ""UserName"",
            ""Email"",
            ""MobileNumber"",
            ""CreatedBy"",
            ""CardNumber""
        )
        VALUES (
            @FirstName,
            @LastName,
            @Password,
            @UserName,
            @Email,
            @MobileNumber,
            @CreatedBy,
            @CardNumber
        )
        RETURNING ""ContactId""
;";

        public string GetCustomer = @"
        SELECT 
            c.""ContactId"",
            c.""First Name"" AS ""FirstName"",
            c.""Last Name"" AS ""LastName"",
            c.""UserName"",
            c.""Email"",
            c.""MobileNumber"",
            c.""CardNumber""
        FROM 
            ""Contact"" c
        INNER JOIN 
            ""ContactRoleJn"" crj ON c.""ContactId"" = crj.""ContactId""
        WHERE 
            crj.""RoleId"" = @RoleId;
    ";
    }
}
