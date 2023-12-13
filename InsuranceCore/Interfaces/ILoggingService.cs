using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace InsuranceCore.Interfaces
{
    public interface ILoggingService
    {
        void LogInformation(string message, [CallerMemberName] string methodName = null);
        void LogWarning(string message, [CallerMemberName] string methodName = null);
        void LogError(string message, [CallerMemberName] string methodName = null);
        void LogFatal(string message, [CallerMemberName] string methodName = null);
    }

}
