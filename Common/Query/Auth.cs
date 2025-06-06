﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Query
{
    public class Auth
    {
         public string DoLogin = @"
                SELECT c.""ContactId"", c.""UserName""
                FROM ""Contact"" c
                JOIN ""ContactRoleJn"" crj ON c.""ContactId"" = crj.""ContactId""
                JOIN ""Roles"" r ON crj.""RoleId"" = r.""RoleId""
                WHERE c.""UserName"" = @UserName AND c.""Password"" = @Password AND LOWER(r.""Name"") = @Role
                ";

        public string insertContactSql = @"
    INSERT INTO ""Contact"" (
        ""MobileNumber"",
        ""SendTransferBy"",
        ""Nickname"",
        ""Language"",
        ""Name""
    )
    VALUES (
        @MobileNumber,
        @SendTransferBy,
        @Nickname,
        @Language,
        @Name
    );
";

    }
    public class Account
    {
         public string AccountTypeList = @"SELECT * FROM public.""AccountCategory"" ORDER BY ""Name"" ASC";


        public string insertAccountSql = @"
    INSERT INTO ""Account"" (
        ""AccountNumber"",
        ""Balance"",
        ""Status"",
        ""AccountCategory""
    )
    VALUES (
        @AccountNumber,
        @Balance,
        @Status,
        @AccountCategory
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
    );
";

        public string getCustomerAccounts = @"
    SELECT 
        a.""AccountId"",
        a.""AccountNumber"",
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




    }
}
