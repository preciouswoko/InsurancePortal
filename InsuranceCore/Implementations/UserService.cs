using InsuranceCore.DTO;
using InsuranceCore.Enums;
using InsuranceCore.Interfaces;
using InsuranceCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static InsuranceCore.DTO.ReusableVariables;

namespace InsuranceCore.Implementations
{
    //public class UserService : IUserService
    //{
    //    private readonly IEmailService _emailService;
    //    private readonly IGenericRepository<Customer> _customerRepository;
    //    public readonly IUtilityService _utilityService;
    //    private readonly ILoggingService _logging;
    //    TemporaryVariables temporaryVariables;
    //    GlobalVariables globalVariables;
    //    private readonly GlobalVariables _globalVariables;
    //    private readonly TemporaryVariables _temporaryVariables;
    //    private readonly ISessionService _service;



    //    public UserService(IEmailService emailService, IGenericRepository<Customer> customerRepository,
    //        IUtilityService utilityService,
    //        ILoggingService logging, ISessionService service
    //        )
    //    {
    //        _emailService = emailService;
    //        _customerRepository = customerRepository;
    //        _utilityService = utilityService;
    //        _logging = logging;
    //        _service = service;
    //        _globalVariables = _service.Get<GlobalVariables>("GlobalVariables");
    //        _temporaryVariables = _service.Get<TemporaryVariables>("TemporaryVariables");
    //    }
    //    public async Task<Customer> CreateExternalUser(CreateCustomer model)
    //    {
    //        var exist = _customerRepository.GetWithPredicate(x => x.Username == model.EmailAddress && x.Surname == model.Surname ||
    //        x.Username == model.EmailAddress && x.FirstName == model.Firstname);

    //        if (exist != null) return null;
    //        // Map model to customer entity
            
    //        var customer = new Customer();
    //        customer.Username = model.EmailAddress;
    //        customer.Surname = model.Surname;
    //        customer.FirstName = model.Firstname;
    //        customer.Role = Role.ExternalUser.ToString();
    //        customer.Status = UserStatus.ResetPassword.ToString();

    //        // etc

    //        // Generate temp password
    //        customer.TempPassword = GenerateTempPassword();

    //        // Save customer
    //        _customerRepository.Insert(customer);

    //        // Send email with temp password
    //        var email = new EmailMessage
    //        {
    //            To = model.EmailAddress,
    //            Subject = "New insurance request assigned",
    //            Body = $"your one time password{customer.TempPassword}"
    //        };
    //        _emailService.SendEmail(email);

    //        return customer;
    //    }

    //    private string GenerateTempPassword()
    //    {
    //        // Define the character set for the password
    //        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

    //        // Set the desired password length
    //        int passwordLength = 12; // You can adjust this as needed

    //        // Create a random number generator
    //        Random random = new Random();

    //        // Generate a password by selecting random characters from the character set
    //        string password = new string(Enumerable.Repeat(chars, passwordLength)
    //            .Select(s => s[random.Next(s.Length)]).ToArray());

    //        return password;
    //    }
    //    public async Task ManageExternalUser(string username, UserStatus action)
    //    {
    //        var customer = await _customerRepository.GetWithPredicate(x => x.Username == username);

    //        if (action == UserStatus.ResetPassword)
    //        {
    //            // Generate and save new temp password
    //            customer.TempPassword = GenerateTempPassword();

    //            // Send reset password email
    //        }
    //        else if (action == UserStatus.Disable)
    //        {
    //            customer.Status = "Disabled";
    //        }
    //        else if (action == UserStatus.Enable)
    //        {
    //            customer.Status = "Active";
    //        }

    //         _customerRepository.Update(customer);
    //    }
    //    public  Task<IEnumerable<Customer>> GetAllCustomer()
    //    {
    //        return  _customerRepository.GetAll();
    //    }
    //    public string UpdateCustomer(Customer request)
    //    {
    //        try
    //        {
    //            var update = _customerRepository.Update(request);
    //            if (update == true) return "Successful";
    //            return "UnSuccessful";

    //        }
    //        catch (Exception ex)
    //        {
    //            _logging.LogFatal(ex.ToString(), "UpdateBroker");
    //            return "Error";
    //        }

    //    }
    //    public async Task<Customer> GetCustomerAsync(int id)
    //    {
    //        return await _customerRepository.GetWithPredicate(x=> x.Id == id);
    //    }
    //    public async Task<Customer> GetByUsername(string username)
    //    {
    //        var customer= await _customerRepository.GetWithPredicate(x => x.Username == username);
    //        var globalVariables = new GlobalVariables();
    //        var temporaryVariables = new TemporaryVariables();
    //        globalVariables.userName = username;
    //        globalVariables.userid = customer.Id.ToString();
    //        globalVariables.ApprovalLevel = 1;
    //        //globalVariables.MenuHtml = await _utilityService.GeneratedMenuHtml(5);
    //        return customer;
    //    }
    //    public string hash(string password )
    //    {
    //        return  _utilityService.ComputeSHA256Hash(password);
    //    }
    //}
}
