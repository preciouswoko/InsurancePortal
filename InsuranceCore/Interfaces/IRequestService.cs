using InsuranceCore.DTO;
using InsuranceCore.Enums;
using InsuranceCore.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceCore.Interfaces
{
    public interface IRequestService
    {
        Task<string> UploadCertificate(CertificateRequest certificateRequest);
        Task<string> CreateRequest(Request request);
        Task<IEnumerable<Request>> GetAllNeeded(string stage);
        Task<IEnumerable<InsuranceTable>> GetInsuranceRequestsByStageAsync1(string stage);
        string GetPermissionName(string permissionConstant);
        Task<BrokerInsuranceType> GetBrokerInsuranceTypebyId(int requestid);
        Task<string> AssignUnderwriter(InsuranceTable Insurance, Request request);
        Task<List<Request>> GetAllNeededforAuth();
        Task<decimal> GetPercentageAsync(Request request);
        string FormatSerial(int serial);
        string GenerateRandomString(int length);
        Task<int> GetPercentage(int id);
        Task<Underwriter> GetUnderwritersbybroker(int brokerid);
        Task<IEnumerable<Request>> GetAllNeeded1(string stage, string branchCode);
        Task<DataTablesResponse> FetchInsurancesForDataTableAsync(DataTablesRequest request);
        Task<Request> GetRequestDetailsForInsuranceRequestsAsync1(string requestId);
        Task<IEnumerable<InsuranceTable>> GetInsuranceByRequester(string email);

        Task<InsuranceType> GetInsuranceTypebyId(int requestid);
        List<PermissionInfo> MapPermissions(List<string> permissionsList);
        CommentStatus GetEnumValueByIndex1(int index);

        string UpdateRequest(Request request, InsuranceTable Insurance, string email, string name, string comment);
        Task<Underwriter> GetUnderwritebyId(int requestid);
        Task<bool> SetContractId(Request request);
        Task<string> AuthorizeInsurance(InsuranceTable request);
        BrokerStatus GetEnumValueByIndex(int index);
        Task<Request> GetRequestDetailsForInsuranceRequestsAsync(string requestId);
        Task<IEnumerable<InsuranceTable>> GetInsuranceRequestsByStageAsync(string stage);
        string UpdateInsuranceReq(InsuranceTable request, string comment,Request model);
        Task<IEnumerable<InsuranceTable>> GetAllInsuranceRequestsByStages(string stage1, string stage2);
        Task<List<RecordReport>> ReportReport();
        Task<List<RecordReport>> FilterReport(string ContractId, string CustomerID, string CustomerName, string AccountNo, string InsuranceFlag, DateTime? PolicyExpiryStartDate, DateTime? PolicyExpiryEndDate, string Broker, string Underwriter);
    }
}
