using InsuranceCore.Interfaces;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using Serilog;

namespace InsuranceInfrastructure.Logging
{
    //public class LoggingService : ILoggingService
    //{
    //    private readonly ILogger<LoggingService> _logger;

    //    public LoggingService(ILogger<LoggingService> logger/*IConfiguration configuration*/)
    //    {
    //        _logger = logger;
    //        //_logger = new LoggerConfiguration()
    //        //    .ReadFrom.Configuration(configuration)
    //        //    .MinimumLevel.Override("InsuranceInfrastructure.Logging.LoggingService", LogEventLevel.Fatal)
    //        //    .CreateLogger();
    //    }

    //    public void LogInformation(string message, [CallerMemberName] string methodName = null)
    //    {

    //        _logger.LogInformation(message, methodName);

    //    }

    //    public void LogWarning(string message, [CallerMemberName] string methodName = null)
    //    {

    //        _logger.LogWarning(message, methodName);
    //    }

    //    public void LogError(string message, [CallerMemberName] string methodName = null)
    //    {

    //        _logger.LogError(message, methodName);
    //    }

    //    public void LogFatal(string message, [CallerMemberName] string methodName = null)
    //    {

    //        _logger.LogCritical(message, methodName);
    //    }
    //}


    public class LoggingService : ILoggingService
    {
        private readonly ILogger<LoggingService> _logger;

        public LoggingService(ILogger<LoggingService> logger)
        {
            _logger = logger;


        }

        public void LogInformation(string message, [CallerMemberName] string methodName = null)
        {
            _logger.LogInformation(message, methodName);
            Log.Information(message);
        }

        public void LogWarning(string message, [CallerMemberName] string methodName = null)
        {
            _logger.LogWarning(message, methodName);
            Log.Warning(message);
        }

        public void LogError(string message, [CallerMemberName] string methodName = null)
        {
            _logger.LogError(message, methodName);
            Log.Error(message);
        }

        public void LogFatal(string message, [CallerMemberName] string methodName = null)
        {
            _logger.LogCritical(message, methodName);
            Log.Fatal(message);
        }
    }

}
