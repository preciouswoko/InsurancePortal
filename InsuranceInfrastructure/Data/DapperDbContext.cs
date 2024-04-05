using Oracle.ManagedDataAccess.Client;
using Microsoft.Extensions.Configuration;
using InsuranceCore.Interfaces;
using System.Threading;
using System.Threading.Tasks;

public class DapperDbContext
{
    private readonly IConfiguration _configuration;
    private readonly string _connectionString;
    private readonly IUtilityService _utilityService;
    public DapperDbContext(IConfiguration configuration, IUtilityService utilityService)
    {
        _configuration = configuration;
        _utilityService = utilityService;
        _connectionString = _configuration.GetConnectionString("OracleConnection");
    }

    public async Task<OracleConnection> CreateConnection()
    {
        var connectionString = _connectionString;
        string[] splitValues = connectionString.Split(';');
        string password = null;
        string id = null;
        foreach (string item in splitValues)
        {
            if (item.Contains("User Id="))
            {
                id = item.Substring(item.IndexOf('=') + 1);
                continue;

            }
            else if (item.Contains("Password="))
            {
                password = item.Substring(item.IndexOf('=') + 1);
                continue;
            }
            //string[] keyValue = item.Split('=');
            //if (keyValue.Length == 2 && keyValue[0].Trim() == "Password")
            //{
            //    password = keyValue[1].Trim();
            //    continue;
            //}
            //if (keyValue.Length == 2 && keyValue[0].Trim() == "User Id")
            //{
            //    id = keyValue[1].Trim();
            //    continue;
            //}
        }


        //var DecryptedUserId = "User Id=" + await _utilityService.AESDecryptString(password, default(CancellationToken));
        //var Decryptedpassword = "Password=" + await _utilityService.AESDecryptString(id, default(CancellationToken));
        var DecryptedUserId = "User Id=" + _utilityService.Decrypt(password, default(CancellationToken));
        var Decryptedpassword = "Password=" + _utilityService.Decrypt(id, default(CancellationToken));
        var DecryptedconnectionString = splitValues[0] + ";"  + DecryptedUserId + ";" + Decryptedpassword + ";";
        return new OracleConnection(DecryptedconnectionString);
        // return new OracleConnection(_utilityService.Decrypt(_connectionString, default(CancellationToken)));
    }
}
