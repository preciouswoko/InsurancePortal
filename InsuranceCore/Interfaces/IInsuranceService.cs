using InsuranceCore.DTO;
using InsuranceCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceCore.Interfaces
{
    public interface IInsuranceService
    {
        
        string UpdateBrokerInsuranceSubType(BrokerSubInsuranceType request);
        Task<IEnumerable<BrokerSubInsuranceType>> GetAllBrokerInsuranceSubType();
        Task<BrokerSubInsuranceType> GetBrokerInsuranceSubTypebyId(int requestid);
        Task<string> CreateBrokerInsuranceSubType(BrokerSubInsuranceType request);
        string UpdateBrokerInsuranceType(BrokerInsuranceType request);
        Task<BrokerInsuranceType> GetBrokerInsuranceType(int id);
        string CreateBrokerInsuranceType(BrokerInsuranceType request);
        Task<InsuranceTable> GetInsurancereq(string requestid);
        Task<IEnumerable<Underwriter>> GetUnderwriters(int brokerid);
        string UpdateComment(Comments request);
        string InsertComment(Comments request);
        //string UpdateInsuranceReq(InsuranceTable request);
        Task<Request> Getrequest(string requestid);
        
        Task<IEnumerable<InsuranceTable>> BuildInsuranceReport(InsuranceReportViewModel filters);
        bool DeleteBroker(int id);
        bool DeleteUnderwriter(int id);
        bool DeleteInsuranceSub(int id);
        bool DeleteInsurance(int id);
        Task<Broker> GetBroker(int id);
        Task<Underwriter> GetUnderwrite(int id);
        Task<InsuranceType> GetInsuranceType(int id);
        Task<InsuranceSubType> GetInsuranceSubType(int id);
        //bool UserHasPermission(string requiredPermission, string userPermissions);
        Task<IEnumerable<InsuranceType>> GetAllInsuranceType();
        Task<IEnumerable<InsuranceSubType>> GetAllSubInsuranceType();
        Task<InsuranceSubType> GetInsuranceSubTypebyId(int requestid);
        Task<AccountDetail> FetchDetail(string Nubam);
        Task<Comments> GetLastComment(string requestId);
        Task<IEnumerable<Broker>> GetAllBroker();
        Task<IEnumerable<Underwriter>> GetAllUnderwriter();
        Task<IEnumerable<BrokerSubInsuranceType>> GetAllBrokerInsuranceSubTypebyId(int requestid);
        Task<IEnumerable<BrokerInsuranceType>> GetAllbrokerInsuranceTypebyId(int requestid);
        Task<string> AssignUnderwriter(InsuranceTable Insurance, Request request);
       
        Task<IEnumerable<BrokerInsuranceType>> GetAllBrokerInsuranceType();
        Task<IEnumerable<BrokerSubInsuranceType>> GetAllbrokerSubInsuranceType();
        string CreateBroker(Broker request);
        string UpdateBroker(Broker request);
        string CreateUnderwriter(Underwriter request);
        string UpdateUnderwriter(Underwriter request);
        string CreateInsuranceType(InsuranceType request);
        string UpdateInsuranceType(InsuranceType request);
        Task<string> CreateInsuranceSubType(InsuranceSubType request);
        string UpdateInsuranceSubType(InsuranceSubType request);
        Task<RecordReport> GetInsuranceInformationAsync(InsuranceTable request, int id);
        Task<IEnumerable<InsuranceTable>> BuildInsuranceReportQuery(InsuranceReportQuery filters);
        Task<IEnumerable<RecordReport>> MapInsuranceRequestsToInfo(IEnumerable<InsuranceTable> requests);

        Task<string> GetRelativePath(string base64String, string contentType, string file_name);
        Task<string> ReviewCertificateUploaded(InsuranceTable request,Request model);
        Task<IEnumerable<InsuranceSubType>> GetInsuranceSubTypesByInsuranceType(int insuranceTypeId);
        Task<IEnumerable<InsuranceType>> GetAllInsuranceTypebyId(int requestid);
        Task<IEnumerable<InsuranceSubType>> GetAllInsuranceSubTypebyId(int requestid);
        Task<IEnumerable<InsuranceType>> InsuranceTypebyId(int requestid);
        Task<IEnumerable<InsuranceSubType>> InsuranceSubTypebyId(int requestid);
    }

}
