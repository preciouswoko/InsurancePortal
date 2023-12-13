using Oracle.ManagedDataAccess.Client;
using Microsoft.Extensions.Configuration;

public class DapperDbContext
{
    private readonly IConfiguration _configuration;
    private readonly string _connectionString;

    public DapperDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
        _connectionString = _configuration.GetConnectionString("OracleConnection");
    }

    public OracleConnection CreateConnection()
    {
        return new OracleConnection(_connectionString);
    }
}
