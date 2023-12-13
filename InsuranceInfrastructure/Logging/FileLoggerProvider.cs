using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace InsuranceInfrastructure.Logging
{
    public class FileLoggerProvider : ILoggerProvider
    {
        private readonly string _logDirectory;
        private readonly long _maxFileSizeBytes;
        private readonly int _maxFilesToKeep;

        public FileLoggerProvider(string logDirectory, long maxFileSizeBytes, int maxFilesToKeep)
        {
            _logDirectory = logDirectory;
            _maxFileSizeBytes = maxFileSizeBytes;
            _maxFilesToKeep = maxFilesToKeep;

            // Ensure the log directory exists
            Directory.CreateDirectory(logDirectory);
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new FileLogger(categoryName, _logDirectory, _maxFileSizeBytes, _maxFilesToKeep);
        }

        public void Dispose()
        {
            // Cleanup if needed
        }
    }

}
