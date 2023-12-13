using Dapper;
using InsuranceCore.Interfaces;
using InsuranceCore.Models;
using InsuranceInfrastructure.Data;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace InsuranceInfrastructure.Services
{
    public class OracleDataService : IOracleDataService
    {
        private readonly ILoggingService _logger;
        private readonly DapperDbContext _context;
        private readonly IConfiguration _configuration;

        public OracleDataService(IConfiguration configuration, ILoggingService logger, DapperDbContext context)
        {
            _logger = logger;
            _context = context;
            _configuration = configuration;
        }

        public async  Task<AccountDetailsInfo> ExecuteQuery(string accountNumber)
        {
            try
            {


                using (var connection = _context.CreateConnection())
                {
                    connection.Open();

                   
                    string query = @"select a.currency_code currency ,a.branch_code BranchCode,  b.customer_email1 CustomerEmail, a.account_no T24AccountNo, a.nuban_account_no AccountNo,a.customer_id CustomerID
                                            ,b.customer_dob dob,b.bvn,CONCAT(CONCAT(b.customer_phone1 ,']'), b.customer_sms) CustomerPhone,b.customer_sms
                                            ,a.type_code,b.customer_name AccountName,b.customer_gender  Gender            
                                            from infobis.accounts_trans a, infobis.customers b where a.account_no=infobis.fn_gett24account(:accountNumber)
                                            and a.customer_id=b.customer_id";
                    // Use Dapper to execute the query and return a list of dynamic objects
                    var result = await connection.QueryFirstOrDefaultAsync<AccountDetailsInfo>(query, new { accountNumber });

                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString(), "ExecuteQuery");
                return null;
            }
        }
        public async Task<ContractDetails> ExecuteQuerywithContractId(string ContractId)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    connection.Open();

                    string query = "select * from  table(infobis.fn_get_loans_mg(:ContractId))";
                    var result = await connection.QueryFirstOrDefaultAsync<ContractDetails>(query, new { ContractId });


                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString(), "ExecuteQuerywithContractId");
                return null;
            }
        }
        public async Task<ReqeryFt> RequeryFTUniqueId(string UniqueId)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    connection.Open();

                    string query = "select * from  table(Infobis.fn_get_api_ft_unique(:UniqueId))";

                    var result = await connection.QueryFirstOrDefaultAsync<ReqeryFt>(query, new { UniqueId });

                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString(), "RequeryFTUniqueId");
                return null;
            }
        }

    }
}
