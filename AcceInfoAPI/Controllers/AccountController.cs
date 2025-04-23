using Common.Helper;
using Common.Models;
using Common.Models.Request;
using Common.Models.Response;
using Common.Query;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Microsoft.AspNetCore.Authorization;


namespace AcceInfoAPI.Controllers
{
    [Route("api/accounts")]
    public class AccountController : Controller
    {
        private readonly IConfiguration _configuration;
        private Common.Query.Account _account;
        private Common.Query.Account _masterList;


        public AccountController(IConfiguration configuration)
        {
            _configuration = configuration;
            _masterList = new Common.Query.Account();     
            _account = new Common.Query.Account();
        }

        [HttpPost("add")]
        public async Task<IActionResult> CreateAccount([FromBody] CreateAccountRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                          .SelectMany(v => v.Errors)
                          .Select(e => e.ErrorMessage)
                          .ToList();
                return BadRequest(new Common.Models.ResponseModel
                {
                    Status = Common.Models.Constants.ERROR_STATUS,
                    Message = Common.Models.Constants.MISSING_FIELDS,
                    statusCode = HttpStatusCode.BadRequest,
                    ErrorMessages = errors
                });
            }
            var db = new Common.Helper.DBConnectionHelper(_configuration, _configuration[Common.Models.Constants.DB_CONNECTIONSTRING]);
            using var conn = await db.GetOpenConnectionAsync();
            using var trx = conn.BeginTransaction();
            try
            {
                string accountNumber = Common.Helper.AuthHelper.Generate13DigitAccountNumber();
                string CustomerAccountJnId = Guid.NewGuid().ToString();
                

                var accountId = await conn.ExecuteScalarAsync<string>(_account.insertAccountSql, new
                {
                    AccountNumber = accountNumber,
                    Balance = request.Amount,
                    Status = true,
                    AccountCategory = request.AccountType,
                    Name = request.AccountName
                }, trx);
                var contactAccountJnId = await conn.ExecuteScalarAsync<Guid>(_account.insertCustomerAccountSql, new
                {
                    AccountId = accountId,
                    CustomerId = request.ContactId,
                    Status = true,
                }, trx);

                await trx.CommitAsync();
                //return Ok(new Common.Models.Response.AccountResponse
                //{
                //    statusCode = System.Net.HttpStatusCode.OK,
                //    Status = Constants.SUCCESS_STATUS,
                //    Message = Constants.LOGIN_SUCCESSFULLY
                //});
            }
            catch (Exception ex)
            {
                if (trx != null)
                {
                    await trx.RollbackAsync();
                }
                return Unauthorized(new Common.Models.Response.LoginResponse
                {
                    Status = Constants.FAILED_STATUS,
                    statusCode = System.Net.HttpStatusCode.Unauthorized,
                    Message = Constants.LOGIN_FAILED
                });
            }
            var newAccountId = Guid.NewGuid().ToString();

            return Ok(new
            {
                status = "success",
                message = "Account created successfully.",
                accountId = newAccountId
            });
        }

        [HttpGet("cust-account")]
        public async Task<IActionResult> GetCustomerAccounts([FromQuery] string ContactId)
        {
            if(!string.IsNullOrEmpty(ContactId))
            {
                var db = new Common.Helper.DBConnectionHelper(_configuration, _configuration[Common.Models.Constants.DB_CONNECTIONSTRING]);

                var AccountTypeListQuery = (await db.QueryAsync<dynamic>(_account.getCustomerAccounts, new { CustomerId = ContactId })).ToList();
                var AccountTypeListQueryList = AccountTypeListQuery.Select(x => new
                {
                    AccountId = (string)x.AccountId,
                    AccountNumber = (string)x.AccountNumber,
                    AccountCategoryName = (string)x.AccountCategoryName,
                    AccountCategoryId = (string)x.AccountCategoryId,
                    Balance = (long)x.Balance,
                    AccountName = (string)x.Name
                });


                return Ok(new Common.Models.ResponseModel
                {
                    Status = Constants.SUCCESS_STATUS,
                    Data = AccountTypeListQueryList,
                    statusCode = HttpStatusCode.OK
                });
            }
            else
            {
                return BadRequest(new Common.Models.Response.LoginResponse
                {
                    Status = Constants.FAILED_STATUS,
                    statusCode = System.Net.HttpStatusCode.BadRequest,
                    Message = Constants.LOGIN_FAILED
                });
            }
                
        }

        [HttpGet("acc-detail")]
        public async Task<IActionResult> GetAccountDetail([FromQuery] string AccountId)
        {
            if (!string.IsNullOrEmpty(AccountId))
            {
                var db = new Common.Helper.DBConnectionHelper(_configuration, _configuration[Common.Models.Constants.DB_CONNECTIONSTRING]);

                var AccountTypeListQuery = (await db.QueryAsync<dynamic>(_account.getAccountById, new { AccountId = AccountId })).ToList();
                var AccountTypeListQueryList = AccountTypeListQuery.Select(x => new
                {
                    AccountId = (string)x.AccountId,
                    AccountNumber = (string)x.AccountNumber,
                    AccountCategoryName = (string)x.AccountCategoryName,
                    AccountCategoryId = (string)x.AccountCategoryId,
                    Balance = (long)x.Balance
                });


                return Ok(new Common.Models.ResponseModel
                {
                    Status = Constants.SUCCESS_STATUS,
                    Data = AccountTypeListQueryList,
                    statusCode = HttpStatusCode.OK
                });
            }
            else
            {
                return BadRequest(new Common.Models.Response.LoginResponse
                {
                    Status = Constants.FAILED_STATUS,
                    statusCode = System.Net.HttpStatusCode.BadRequest,
                    Message = Constants.LOGIN_FAILED
                });
            }

        }

        [Authorize]
        [HttpPost("transaction-history")]
        public async Task<IActionResult> GetTransactionHistoryByAccountId([FromBody] TransactionHistoryRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.AccountId))
                {
                    return BadRequest(new
                    {
                        Status = Constants.ERROR_STATUS,
                        Message = "AccountId is required."
                    });
                }

                var db = new DBConnectionHelper(_configuration, _configuration[Constants.DB_CONNECTIONSTRING]);

                var transactions = (await db.QueryAsync<dynamic>(_masterList.GetTransactionHistoryByAccountId, new
                {
                    AccountId = request.AccountId
                })).ToList();

                var result = transactions.Select(t => new TransactionHistoryResponse
                {
                    TransactionId = (string)t.TransactionId,
                    TransactionFrom = (int)t.TransactionFrom,
                    TransactionTo = (int)t.TransactionTo,
                    CreatedOn = (DateTime)t.CreatedOn,
                    Amount = (int)t.Amount,
                    Note = (string)t.Note,
                    TransactionType = (string)t.TransactionType,
                    IsSelfTransfer = (bool)t.IsSelfTransfer
                });

                return Ok(new
                {
                    Status = Constants.SUCCESS_STATUS,
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = Constants.FAILED_STATUS,
                    Message = "An error occurred while retrieving transaction history"
                });
            }
        }

        [Authorize]
        [HttpPost("transfer-money")]
        public async Task<IActionResult> GetTransferMoney([FromBody] TransferRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.AccountNumberTo))
                {
                    return BadRequest(new
                    {
                        Status = Constants.ERROR_STATUS,
                        Message = "AccountNumber To is required."
                    });
                }



                if (string.IsNullOrEmpty(request.AccountNumberFrom))
                {
                    return BadRequest(new
                    {
                        Status = Constants.ERROR_STATUS,
                        Message = "AccountNumber From is required."
                    });
                }



                if (request.Amount <= 0)
                {
                    return BadRequest(new
                    {
                        Status = Constants.ERROR_STATUS,
                        Message = "Amount should be greater than zero."
                    });
                }



                var db = new DBConnectionHelper(_configuration, _configuration[Constants.DB_CONNECTIONSTRING]);



                int rowsAffected = await db.ExecuteAsync(_masterList.TransferbyAccount, new
                {
                    AccountNumberFrom = request.AccountNumberFrom,
                    AccountNumberTo = request.AccountNumberTo,
                    Amount = request.Amount,
                });



                if (rowsAffected > 0)
                {
                    var transferResult = new TransferResponse
                    {
                        AccountNumberFrom = request.AccountNumberFrom,
                        AccountNumberTo = request.AccountNumberTo,
                        Amount = request.Amount,
                    };



                    return Ok(new
                    {
                        Status = Constants.SUCCESS_STATUS,
                        Data = transferResult
                    });
                }
                else
                {
                    return StatusCode(500, new
                    {
                        Status = Constants.FAILED_STATUS,
                        Message = "Transfer failed. No rows affected."
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = Constants.FAILED_STATUS,
                    Message = "An error occurred while transferring money.",
                    Error = ex.Message
                });
            }
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
