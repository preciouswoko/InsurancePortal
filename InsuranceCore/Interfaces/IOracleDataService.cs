using InsuranceCore.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceCore.Interfaces
{
    public interface IOracleDataService
    {
        Task<AccountDetailsInfo> ExecuteQuery(string accountNumber);
        Task<ContractDetails> ExecuteQuerywithContractId(string ContractId);
        Task<ReqeryFt> RequeryFTUniqueId(string UniqueId);


    }
}
