using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace InsuranceCore.DTO
{
    public class ReusableVariables
    {
        [Serializable]
        public class TemporaryVariables
        {
            public string string_var0 { get; set; }
            public string string_var1 { get; set; }
            public string string_var2 { get; set; }
            public string string_var3 { get; set; }
            public string string_var4 { get; set; }
            public string string_var5 { get; set; }
            public string string_var6 { get; set; }
            public string string_var7 { get; set; }
            public string string_var8 { get; set; }
            public string string_var9 { get; set; }
            public string string_var10 { get; set; }
            public int int_var0 { get; set; }
            public int int_var1 { get; set; }
            public int int_var2 { get; set; }

            public Int64 Int64_var0 { get; set; }
            public Int64 Int64_var1 { get; set; }
            public DateTime date_var0 { get; set; }
            public DateTime date_var1 { get; set; }
            public DateTime date_var2 { get; set; }
            public DateTime date_var4 { get; set; }
            public bool bool_var0 { get; set; }

            public decimal decimal_var0 { get; set; }
            public decimal decimal_var1 { get; set; }
            public decimal decimal_var2 { get; set; }
            public decimal decimal_var3 { get; set; }
            public decimal decimal_var4 { get; set; }
            public decimal decimal_var5 { get; set; }
            public decimal[] date_array_temp0 { get; set; }
            public decimal[] date_array_temp1 { get; set; }
            public decimal[] date_array_temp2 { get; set; }
            public decimal[] date_array_temp3 { get; set; }
            public string[] string_array_temp0 { get; set; }
            public string[] string_array_temp1 { get; set; }
            public string[] string_array_temp2 { get; set; }
            public string[] string_array_temp3 { get; set; }
            public string[] string_array_temp4 { get; set; }

            public string ptitle { get; set; }
            public string idrep { get; set; }

            public string ApprovalLevelStatus { get; set; }
        }

        public class GlobalVariables
        {
            public int saltValue { get; set; }
            public string userid { get; set; }
            public string name { get; set; }
            public string userName { get; set; }
            public int TenantId { get; set; }
            public string[] sarrayt1 { get; set; }
            public string ptitle { get; set; }
            public string ReportPeriod { get; set; }
            public string viewflag { get; set; }
            public string filep { get; set; }
            public string tempvar { get; set; }
            public string branchCode { get; set; }
            public string region { get; set; }
            public string MenuHtml { get; set; }
            public string ReportHtml { get; set; }
            public string Email { get; set; }
            public List<string> Permissions { get; set; }
            public int RoleId { get; set; }
            public int ApprovalLevel { get; set; }

            public static implicit operator Guid(GlobalVariables v)
            {
                throw new NotImplementedException();
            }
        }

        public class SpectaViewModel
        {
            public string string_sp0 { get; set; }
            public string string_sp1 { get; set; }
            public string string_sp2 { get; set; }
            public string string_sp3 { get; set; }
            public string string_sp4 { get; set; }
            public string string_sp5 { get; set; }
            public string string_sp6 { get; set; }
            public string string_sp7 { get; set; }
            public string string_sp8 { get; set; }
            public string string_sp9 { get; set; }
            public string string_sp10 { get; set; }
            public string string_sp11 { get; set; }
            public int int_sp0 { get; set; }
            public int int_sp1 { get; set; }
            public int int_sp2 { get; set; }
            public int int_sp3 { get; set; }
            public int int_sp4 { get; set; }
            public int int_sp5 { get; set; }

            public decimal decimal_sp0 { get; set; }
            public decimal decimal_sp1 { get; set; }
            public decimal decimal_sp2 { get; set; }
            public decimal[] decimal_array_temp0 { get; set; }
            public Int64 Int64_sp0 { get; set; }
            public Int64 Int64_sp1 { get; set; }
            public decimal date_sp0 { get; set; }
            public decimal date_sp1 { get; set; }
            public decimal date_sp2 { get; set; }
            public decimal date_sp4 { get; set; }
            public bool bool_sp0 { get; set; }

            public bool bool_sp1 { get; set; }
            public bool bool_sp2 { get; set; }
            public bool bool_sp3 { get; set; }
            public bool bool_sp4 { get; set; }
            public bool bool_sp5 { get; set; }
            public decimal[] date_array_temp0 { get; set; }
            public decimal[] date_array_temp1 { get; set; }
            public decimal[] date_array_temp2 { get; set; }
            public decimal[] date_array_temp3 { get; set; }
            public string[] string_array_temp0 { get; set; }
            public string[] string_array_temp1 { get; set; }
            public string[] string_array_temp2 { get; set; }
            public string[] string_array_temp3 { get; set; }
            public string[] string_array_temp4 { get; set; }
            public string ptitle { get; set; }
            public string idrep { get; set; }
        }

        public class ReadOnlyVariables
        {
            public string c1 { get; set; }
            public string c2 { get; set; }
            public string c3 { get; set; }
            public string c4 { get; set; }
            public string c5 { get; set; }
            public string c6 { get; set; }
            public string c7 { get; set; }
            public string c8 { get; set; }
            public string c9 { get; set; }
            public string c10 { get; set; }
            public string c11 { get; set; }
            public string c12 { get; set; }
            public string c13 { get; set; }
            public string c14 { get; set; }

            public string c15 { get; set; }
            public string c16 { get; set; }
            public string c17 { get; set; }
            public string c18 { get; set; }
            public string c19 { get; set; }
            public string c20 { get; set; }
            public string c21 { get; set; }
            public string c22 { get; set; }
            public string c23 { get; set; }
            public string c24 { get; set; }
            public string c25 { get; set; }
            public string c26 { get; set; }
            public int? intc0 { get; set; }
            public int? intc1 { get; set; }
            public int? intc2 { get; set; }
            public int? intc3 { get; set; }
            public int? intc4 { get; set; }
            public int? intc5 { get; set; }
            public int int_var0 { get; set; }
            public int int_var1 { get; set; }
            public int int_var2 { get; set; }
            public int int_var3 { get; set; }
            public int int_var4 { get; set; }
            public double dclf0 { get; set; }
            public double dclf1 { get; set; }
            public double dclf2 { get; set; }
            public double dclf3 { get; set; }
            public double dclf4 { get; set; }
            public double dclf5 { get; set; }
            public double dclf6 { get; set; }
            public double dclf7 { get; set; }
            public Int64? lintc0 { get; set; }
            public Int64? lintc1 { get; set; }
            public Int64? lintc2 { get; set; }
            public Int64? lintc3 { get; set; }
            public Int64? lintc4 { get; set; }
            public DateTime? dtc0 { get; set; }

            public DateTime? dtc1 { get; set; }
            public DateTime? dtc2 { get; set; }
            public DateTime? dtc3 { get; set; }
            public DateTime? dtc4 { get; set; }
            public DateTime? dtc5 { get; set; }
            public bool? blc0 { get; set; }
            public bool? blc1 { get; set; }
            public bool? blc2 { get; set; }
            public bool? blc3 { get; set; }
            public bool? blc4 { get; set; }
            public bool? blc5 { get; set; }
            public bool? blc6 { get; set; }
            public bool? blc7 { get; set; }
            public decimal? dclc0 { get; set; }
            public decimal? dclc1 { get; set; }
            public decimal? dclc2 { get; set; }
            public decimal? dclc3 { get; set; }
            public decimal? dclc4 { get; set; }
            public decimal? dclc5 { get; set; }
            public decimal dclc_var0 { get; set; }
            public decimal dclc_var1 { get; set; }


        }

    
     
        public class AdminUsersVariable
        {
            [Key]
            public int Id { get; set; }
            public DateTime CreationTime { get; set; }
            public int CreatorUserId { get; set; }
            public DateTime LastModificationTime { get; set; }
            public int LastModifierUserId { get; set; }
            public int ApprovalId { get; set; }
            public bool IsDeleted { get; set; }
            public int DeleterUserId { get; set; }
            public DateTime DeletionTime { get; set; }
            public string UserName { get; set; }
            public string Name { get; set; }
            public string Password { get; set; }
            public string Surname { get; set; }
            public int TenantId { get; set; }

            public string BranchCode { get; set; }
            public string EmailAddress { get; set; }
            public DateTime LockoutEndDateUtc { get; set; }
            public int AccessFailedCount { get; set; }
            public bool IsLockoutEnabled { get; set; }
            public string PhoneNumber { get; set; }
            public bool IsActive { get; set; }
            public DateTime LastLoginTime { get; set; }
            public int ApprovalLevel { get; set; }
            public DateTime ApprovalTime { get; set; }
            public int ApprovedBy { get; set; }
            public string UserRole { get; set; }
            public string Region { get; set; }
        }

        public class TempUservariable
        {
            public string Edit { get; set; }
            public long Id { get; set; }
            public string Name { get; set; }
            public string Username { get; set; }
            public string PhoneNumber { get; set; }
            public string BVN { get; set; }
            public string LoginTime { get; set; }
            public string EmailAddress { get; set; }
            public string Referrer { get; set; }
            public string LoanLimit { get; set; }
            public string DOB { get; set; }
            public string Block { get; set; }
        }
        public class TempBranchVariable
        {
            public string Check { get; set; }
            public string Action { get; set; }
            public string BranchId { get; set; }
            public string Reason { get; set; }
            public string BranchName { get; set; }
            public string CreationTime { get; set; }
        }

        public class UnlockDto
        {
            public string Check { get; set; }
            public string Initiate { get; set; }
            public string LockId { get; set; }
            public string CustomerName { get; set; }
            public string LoanAmount { get; set; }
            public string AccountNumber { get; set; }
            public string ApprovedDate { get; set; }
            public string ArrangementId { get; set; }
        }
        public class ApproveDto
        {
            public string Check { get; set; }
            public string Approve { get; set; }
            public string Reject { get; set; }
            public string MarkAsApproved { get; set; }
            public string LoanLockId { get; set; }
            public string CustomerName { get; set; }
            public string LoanAmount { get; set; }
            public string AccountNumber { get; set; }
            public string Initiator { get; set; }
            public string ApprovedDate { get; set; }
            public long? AdminId { get; set; }
        }
        public class TransactionDto
        {
            public string More { get; set; }
            public string Name { get; set; }
            public string BVN { get; set; }
            public string Email { get; set; }
            public string PhoneNumber { get; set; }
            public string Status { get; set; }
            public string AccountNumber { get; set; }
            public string LoanAmount { get; set; }
            public string Branch { get; set; }
            public string Date { get; set; }
        }
        public class AccountDoc
        {
            public string Edit { get; set; }
            public long Id { get; set; }
            public string Name { get; set; }
            public string Username { get; set; }
            public string PhoneNumber { get; set; }
            public string Bvn { get; set; }
            public string AccountNumber { get; set; }
            public string BranchName { get; set; }
            public string KYC { get; set; }
            public string update { get; set; }
        }

       
        public class ApprovedDto
        {
            public string LockId { get; set; }
            public string CustomerName { get; set; }
            public string AccountNumber { get; set; }
            public string LoanAmount { get; set; }
            public string InitiatedDate { get; set; }
            public string Initiator { get; set; }

            public string ApprovedBy { get; set; }
            public string ApprovedDate { get; set; }
            public long? InitiatorId { get; internal set; }
            public long? ApprovalId { get; internal set; }
        }

       

        public class DatatableVariable
        {
            public int draw { get; set; }
            public int recordsTotal { get; set; }
            public int recordsFiltered { get; set; }
            public List<TempUservariable> data { get; set; }

        }
        public class DataBranchVariable
        {
            public int draw { get; set; }
            public int recordsTotal { get; set; }
            public int recordsFiltered { get; set; }
            public List<TempBranchVariable> data { get; set; }

        }
        public class UnlockDatatable
        {
            public int draw { get; set; }
            public int recordsTotal { get; set; }
            public int recordsFiltered { get; set; }
            public List<UnlockDto> data { get; set; }

        }
        public class ApproveDatatable
        {
            public int draw { get; set; }
            public int recordsTotal { get; set; }
            public int recordsFiltered { get; set; }
            public List<ApproveDto> data { get; set; }

        }
        public class TransactionsDatatable
        {
            public int draw { get; set; }
            public int recordsTotal { get; set; }
            public int recordsFiltered { get; set; }
            public List<TransactionDto> data { get; set; }

        }
        public class AccountsDatatable
        {
            public int draw { get; set; }
            public int recordsTotal { get; set; }
            public int recordsFiltered { get; set; }
            public List<AccountDoc> data { get; set; }

        }

        public class ApprovedDatatable
        {
            public int draw { get; set; }
            public int recordsTotal { get; set; }
            public int recordsFiltered { get; set; }
            public List<ApprovedDto> data { get; set; }

        }
        public class DataTableArray
        {
            public int draw { get; set; }
            public int start { get; set; }
            public int length { get; set; }
            public Dictionary<string, string> search { get; set; }

        }

        public class AuthenticateResponse
        {
            public Result result { get; set; }
        }

        public class Result
        {
            public string accessToken { get; set; }
            public string encryptedAccessToken { get; set; }
            public int expireInSeconds { get; set; }
            public Int64 userId { get; set; }

        }

        public class Error
        {
            public string code { get; set; }
            public string details { get; set; }
        }
      
      

      

       

    }
}
