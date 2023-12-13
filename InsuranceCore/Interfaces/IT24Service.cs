using InsuranceCore.DTO;
using InsuranceCore.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InsuranceCore.Interfaces
{
    public interface IT24Service
    {
        Task<HttpResponseMessage> Postofs(string ofs, string transactionRef, string action, CancellationToken cancellation);

        Task<AccountEnquiry> NameEnquiry(string AccountNo, CancellationToken cancellation);

        Task<ApiResponse<AccountEnquiryResponse>> AccountEnquiry(string AccountNo, CancellationToken cancellation);

        Task<bool> ReverseFundTransfer(string ftref);
        Task<FundstransferResponse> FundTransfer(FundsTransferRequestDto request, Credential credential);
        Task<ApiResponse<TransferResponse>> FundTransfer1(TransactionLog transaction, CancellationToken cancellation);
        Task<ApiResponse<string>> LockFunds(string AccountNo, string description, decimal amount, Credential credential);

    }
}
