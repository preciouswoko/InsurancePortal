using InsuranceCore.DTO;
using InsuranceCore.Enums;
using InsuranceCore.Interfaces;
using InsuranceCore.Models;
using InsuranceInfrastructure.Util;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InsuranceInfrastructure.Services
{
    public class RenewalService : IHostedService /*BackgroundService*/
    {

        private volatile bool _stopRequested;
        private AppSettings _appsettings;

        private readonly IServiceScopeFactory _serviceScopeFactory;

        public RenewalService(IServiceScopeFactory serviceScopeFactory,
            IOptions<AppSettings> ioptions
         )
        {
            _serviceScopeFactory = serviceScopeFactory;

            _appsettings = ioptions.Value;

        }


        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (_stopRequested)
                return;

            while (!cancellationToken.IsCancellationRequested)
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedServiceProvider = scope.ServiceProvider;
                    var requestRepository = scopedServiceProvider.GetRequiredService<IGenericRepository<Request>>();
                    var InsuranceRepository = scopedServiceProvider.GetRequiredService<IGenericRepository<InsuranceTable>>();
                    var brokerRepo = scopedServiceProvider.GetRequiredService<IGenericRepository<Broker>>();
                    var requestservice = scopedServiceProvider.GetRequiredService<IRequestService>();
                    var service = scopedServiceProvider.GetRequiredService<IInsuranceService>();

                    //await SendReminder();
                    var requests = await requestservice.GetAllInsuranceRequestsByStages(InsuranceStage.End.ToString(), InsuranceStage.Due.ToString());



                    foreach (var request in requests)
                    {
                        await RenewRequest(request);
                    }
                    await Task.Delay(TimeSpan.FromDays(1), cancellationToken);
                    //await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
                }
            }
        }


        public Task StopAsync(CancellationToken cancellationToken)
        {
            // Set cancellation flag
            _stopRequested = true;

            return Task.CompletedTask;
        }

        private async Task RenewRequest(InsuranceTable request)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var scopedServiceProvider = scope.ServiceProvider;

                var requestRepository = scopedServiceProvider.GetRequiredService<IGenericRepository<Request>>();
                var service = scopedServiceProvider.GetRequiredService<IInsuranceService>();
                var InsuranceRepository = scopedServiceProvider.GetRequiredService<IGenericRepository<InsuranceTable>>();
                var requestService = scopedServiceProvider.GetRequiredService<IRequestService>();
                var _emailService = scopedServiceProvider.GetRequiredService<IEmailService>();
                var _t24 = scopedServiceProvider.GetRequiredService<IT24Service>();
                var _logger = scopedServiceProvider.GetRequiredService<ILoggingService>();
                var _oracle = scopedServiceProvider.GetRequiredService<IOracleDataService>();
                var _utility = scopedServiceProvider.GetRequiredService<IUtilityService>();
                try {
                    var getrequest = await requestRepository.GetWithIncludeAsync(
                   x => (x.RequestID == request.RequestID && x.Status != CommentStatus.Closed.ToString()),
                   x => x.Broker,
                   x => x.InsuranceType,
                   x => x.InsuranceSubType,
                   x => x.Broker
                   );
                    Random random = new Random();
                    var insuranceTypeperecentage = await requestService.GetPercentageAsync(getrequest.BrokerID, getrequest.InsuranceTypeId);
                  
                  decimal  insuranceTypeId = Convert.ToDecimal(insuranceTypeperecentage);
                    var estimatedPremium = getrequest.UpdatedPremium;

                    //var commission = (estimatedPremium * insuranceTypeId) / 100;
                    var commission = (Convert.ToDecimal((estimatedPremium * insuranceTypeId) / 100)) / Convert.ToDecimal(_appsettings.VAT);

                    var formattedCommission = Decimal.Round(commission, 2, MidpointRounding.AwayFromZero);
                    if (request.Serial == 0)
                    {
                        request.Serial = 1;
                    }
                    else
                    {
                        request.Serial++;
                    }
                    request.RequestType = "Renewal";
                    string Serial = requestService.FormatSerial(request.Serial);
                    var transfer = new FundsTransferRequestDto()
                    {
                        TransactionType = TransactionType.InsuranceRequest.ToString(),
                        Amount = Convert.ToDecimal(getrequest.UpdatedPremium),
                        DebitAccount = getrequest.AccountNo,
                        Channel = _appsettings.Channel,
                        Narration = $"PREMIUM FOR INSURANCE RENEWAL FOR COLLATERAL {getrequest.CollateralValue} / {getrequest.ContractID} ",
                        CreditAccount = getrequest.Broker.AccountNumber,
                        BranchCode = getrequest.Branchcode,
                        CreditCurrency = "NGN",
                        DebitCurrency = "NGN",
                        TransactionCode = _appsettings.Channel,
                        TransactionId = $"{getrequest.UpdatedPremium}-{getrequest.ID}-{requestService.GenerateRandomString(3)}",
                        Id = Convert.ToInt64(request.Serial),
                        TransactionRef = $"F{getrequest.RequestID}",
                        UnquieId = $"FEES-{request.RequestID}-{Serial}",
                        RequestID = request.RequestID,
                        InsuranceTableId = request.ID

                    };
                    request.FEESFTReference = transfer.UnquieId;
                    var credential1 = new Credential
                    {
                        T24password = await _utility.AESDecryptString(_appsettings.T24password2, default(CancellationToken)),
                        T24Username = await _utility.AESDecryptString(_appsettings.T24Username2, default(CancellationToken))
                    };
                    var fundtrans = await _t24.FundTransfer(transfer, credential1);
                    var credential = new Credential
                    {
                        T24password = await _utility.AESDecryptString(_appsettings.T24password, default(CancellationToken)),
                        T24Username = await _utility.AESDecryptString(_appsettings.T24Username, default(CancellationToken))
                    };
                    _logger.LogInformation(fundtrans.ToString(), $"FundTransfer to Broker Account ={getrequest.Broker.AccountNumber}");
                    var requery = await _oracle.RequeryFTUniqueId(transfer.UnquieId);
                    InsuranceRepository.Update(request);

                    if (fundtrans.Status == true)
                    {

                        var paycommission = new FundsTransferRequestDto()
                        {
                            TransactionType = TransactionType.Commission.ToString(),
                            Amount = formattedCommission,
                            DebitAccount = getrequest.Broker.AccountNumber,
                            Channel = _appsettings.Channel,
                            Narration = $"COMMISSION ON INSURANCE RENEWAL   {request.RequestID} {request.Serial} {getrequest.CustomerName} {getrequest.ContractID} ".Replace("-", "/"),
                            CreditAccount = _appsettings.GLAccountNumber,
                            BranchCode = null,
                            CreditCurrency = "NGN",
                            DebitCurrency = "NGN",
                            TransactionCode = _appsettings.Channel,
                            TransactionId = $"{formattedCommission}-{getrequest.ID}-{requestService.GenerateRandomString(3)}",
                            Id = Convert.ToInt64(request.Serial),
                            TransactionRef = $"C{request.RequestID}",
                            UnquieId = $"COMM-{request.RequestID}-{Serial}",
                            RequestID = request.RequestID,
                            InsuranceTableId = request.ID

                        };
                        request.COMMFTReference = paycommission.UnquieId;

                        var fundtranstoglaccount = await _t24.FundTransfer(paycommission, credential);
                        _logger.LogInformation(fundtrans.ToString(), $"FundTransfer to GL account ={_appsettings.GLAccountNumber}");
                        var requeryComm = await _oracle.RequeryFTUniqueId(paycommission.UnquieId);
                        InsuranceRepository.Update(request);
                        if (fundtranstoglaccount.Status == false)
                        {
                            var lockfund = await _t24.LockFunds(paycommission.DebitAccount, "Lock " + paycommission.UnquieId, paycommission.Amount, credential1);
                            _logger.LogInformation($"{ paycommission.Amount} lock with unquieid {paycommission.UnquieId},", $"lockfund in broker account ={paycommission.DebitAccount}");

                            //request.Stage = InsuranceRenewal.Due.ToString();
                            request.Stage = InsuranceStage.UnderwriterAssigned.ToString();
                            request.ErrorMessage = fundtranstoglaccount.ResponseMessage;
                            request.Status = TransferStatus.Failed.ToString();
                            InsuranceRepository.Update(request);

                            string sanitizedResponseMessage = fundtranstoglaccount.ResponseMessage
                                .Replace("\r", "")
                                .Replace("\n", "<br>");

                            string mailTemplate = "Dear User,<br><br>" +
                                "We are writing to inform you that your transaction has failed due to the following reason:<br><br>" +
                                "Reason:<br>" + $"{sanitizedResponseMessage}<br><br>" +
                                "Please contact our support team for further assistance.<br>" +
                                "via phone {+234 700 2000 3000} or e-mail {contactcentre@keystonebankng.com}<br><br>" +
                                "Sincerely,<br>" +
                                "Keystone Bank";



                        }
                        if (fundtranstoglaccount.Status == true)
                        {
                            request.Stage = InsuranceStage.UnderwriterAssigned.ToString();
                            //InsuranceRepository.Update(request);

                            string emailSubjectRequest = $"Payment Request for  Insurance was successful for {getrequest.CustomerName}";

                            string emailBodyRequest = $"Dear {getrequest.Broker.AccountName},<br><br>" +
                               $"We are pleased to inform you that the request for Payment on Insurance for {getrequest.CustomerName} has been successfully processed.<br><br>" +
                               "Details:<br>" +
                               "Transaction Type: PAYMENT<br>" +
                               $"Customer Name: {getrequest.CustomerName}<br>" +
                               $"Amount: #{transfer.Amount}<br>" +
                               $"Unique Number: {transfer.UnquieId}<br><br>" +
                               "Thank you for using our services.<br>" +
                               "Sincerely,<br>" +
                               "Keystone Bank";


                            _emailService.SmtpSendMail(getrequest.Broker.EmailAddress, emailBodyRequest, emailSubjectRequest);

                        }
                        request.RequestDate = DateTime.Now;
                        var updateRequest = InsuranceRepository.Update(request);

                    }

                    if (fundtrans.Status == false)
                    {
                        request.Stage = InsuranceStage.Due.ToString();
                        request.ErrorMessage = fundtrans.ResponseMessage;
                        request.Status = TransferStatus.Failed.ToString();
                        var updateRequest = InsuranceRepository.Update(request);

                        string sanitizedResponseMessage = fundtrans.ResponseMessage
                            .Replace("\r", "")
                            .Replace("\n", "<br>");

                        string mailTemplate = "Dear User,<br><br>" +
                            "We are writing to inform you that your transaction has failed due to the following reason:<br><br>" +
                            "Reason:<br>" + $"{sanitizedResponseMessage}<br><br>" +
                            "Please contact our support team for further assistance.<br>" +
                            "via phone {+234 700 2000 3000} or e-mail {contactcentre@keystonebankng.com}<br><br>" +
                            "Sincerely,<br>" +
                            "Keystone Bank";


                        //  _emailService.SmtpSendMail(request.CustomerEmail, mailTemplate", "Debit Notification For Insurance  Process");
                    }
                }catch(Exception ex)
                {
                    _logger.LogError(ex.Message.ToString(), "RenewRequest");
                }
                
            }


        }
        public void ScheduleReport(DataExportFormat type, string frequency, DateTime hourToSend)
        {
            // Configure scheduled job to generate & email
        }
        private async Task SendReminder()
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var scopedServiceProvider = scope.ServiceProvider;

                var requestRepository = scopedServiceProvider.GetRequiredService<IGenericRepository<Request>>();
                var InsuranceRepository = scopedServiceProvider.GetRequiredService<IGenericRepository<InsuranceTable>>();
                var _emailService = scopedServiceProvider.GetRequiredService<IEmailService>();
                var _logger = scopedServiceProvider.GetRequiredService<ILoggingService>();
                var _utility = scopedServiceProvider.GetRequiredService<IUtilityService>();
                try
                {


                    var currentDate = DateTime.Now.Date;
                    var date30DaysBefore = currentDate.AddDays(-30).Date;
                    var date15DaysBefore = currentDate.AddDays(-15).Date;

                    var getInsurance = await InsuranceRepository.GetAllWithPredicate(
                        x => x.CertificateRequestDate.Date == currentDate ||
                             x.CertificateRequestDate.Date == date30DaysBefore ||
                             x.CertificateRequestDate.Date == date15DaysBefore
                    );
                    foreach (var item in getInsurance)
                    {
                        var getRequest = await requestRepository.GetWithPredicate(
                            x => x.RequestID == item.RequestID && x.Status != CommentStatus.Closed.ToString()
                            );
                        if (getRequest == null)
                        {
                            continue;
                        }
                        string mailTemplate = "Dear Relationship Manager,<br><br>" +
                               $"The insurance policy of your customer {getRequest.AccountName} and {getRequest.AccountNo} will expire on {_utility.ConvertDateToString(item.PolicyExpiryDate.ToString())}.<br><br>" +
                               "Kindly inform the customer and ensure the account is adequately funded for the insurance premium on or before the due date<br><br>";



                        string bodytemp = _utility.BuildEmailTemplate(item.CertificateRequestByName, "Reminder on Insurance", mailTemplate);


                        _emailService.SmtpSendMail(item.CertificateRequestByemail, bodytemp, "Reminder on Insurance",""," ");
                    }
                }catch(Exception ex)
                {
                    _logger.LogError(ex.Message.ToString(), "SendReminderJob");
                }
            }
        }
    }
}
