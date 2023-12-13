using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace InsuranceInfrastructure.Logging
{
    public class FileLogger : ILogger
    {
        private readonly string _categoryName;
        private readonly string _logDirectory;
        private readonly long _maxFileSizeBytes;
        private readonly int _maxFilesToKeep;
        private readonly object _lock = new object();
        private StreamWriter _logWriter;

        public FileLogger(string categoryName, string logDirectory, long maxFileSizeBytes, int maxFilesToKeep)
        {
            _categoryName = categoryName;
            _logDirectory = logDirectory;
            _maxFileSizeBytes = maxFileSizeBytes;
            _maxFilesToKeep = maxFilesToKeep;

            InitializeLogFileWriter();
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true; // Adjust log level filtering as needed
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;

            var logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{logLevel}] {_categoryName}: {formatter(state, exception)}{Environment.NewLine}";

            lock (_lock)
            {
                if (_logWriter == null)
                    InitializeLogFileWriter();

                // Check if the log file size exceeds the limit
                //if (new FileInfo(_logWriter.BaseStream as FileStream).Length > _maxFileSizeBytes)
                //{
                //    // Rotate log files
                //    RotateLogFiles();
                //}
                if (_logWriter.BaseStream.Length > _maxFileSizeBytes)
                {
                    // Rotate log files
                    RotateLogFiles();
                }


                _logWriter.Write(logMessage);
                _logWriter.Flush();
            }
        }

        // Implement log file rotation logic
        // Implement log file rotation logic
        private void RotateLogFiles()
        {
            _logWriter.Dispose();

            for (int i = _maxFilesToKeep - 1; i >= 1; i--)
            {
                string sourceFileName = GetLogFilePath(i - 1);
                string targetFileName = GetLogFilePath(i);

                if (File.Exists(sourceFileName))
                {
                    File.Move(sourceFileName, targetFileName);
                }
            }

            InitializeLogFileWriter();
        }

        //private void RotateLogFiles()
        //{
        //    _logWriter.Dispose();

        //    for (int i = _maxFilesToKeep - 1; i >= 1; i--)
        //    {
        //        string sourceFileName = GetLogFilePath(i - 1);
        //        string targetFileName = GetLogFilePath(i);

        //        if (File.Exists(sourceFileName))
        //        {
        //            File.Move(sourceFileName, targetFileName);
        //        }
        //    }

        //    InitializeLogFileWriter();
        //}

        // Get the log file path based on the current date
        private string GetLogFilePath(int fileNumber = 0)
        {
            string fileName = $"{DateTime.Now:yyyyMMdd}{(fileNumber == 0 ? "" : $"_{fileNumber}")}.log";
            return Path.Combine(_logDirectory, fileName);
        }

        private void InitializeLogFileWriter()
        {
            // Close the previous writer if it exists
            if (_logWriter != null)
            {
                _logWriter.Flush();
                _logWriter.Dispose();
            }

            // Get the log file path based on the current date
            string logFilePath = GetLogFilePath();
            _logWriter = new StreamWriter(logFilePath, append: true);
        }
    }

}
