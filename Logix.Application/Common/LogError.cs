using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logix.Application.Interfaces.IRepositories;

namespace Logix.Application.Common
{
    public interface ILogError
    {
        bool Add(string errorMessage, string functionName, string className);
    }

    public class LogError : ILogError
    {
        public bool Add(string errorMessage, string functionName, string className)
        {
            // this function is async Task,, to be able if we want to save log into database
            try
            {
                // Define the directory and file paths with the current year
                string currentYear = DateTime.Now.Year.ToString();
                string logsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files\\Logs", currentYear);
                string logFileName = $"log_{DateTime.Now:yy_MM}.txt";
                string logFilePath = Path.Combine(logsDirectory, logFileName);

                // Check if the Logs directory exists, if not, create it
                if (!Directory.Exists(logsDirectory))
                {
                    Directory.CreateDirectory(logsDirectory);
                }

                // Prepare the log entry
                string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - Error in class '{className}', function '{functionName}': {errorMessage}";

                // Write the log entry to the file
                using (StreamWriter writer = new StreamWriter(logFilePath, true))
                {
                    writer.WriteLine(logEntry);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static void AddToFile(string errorMessage, string functionName, string className)
        {
            try
            {
                // Define the directory and file paths with the current year
                string currentYear = DateTime.Now.Year.ToString();
                string logsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files\\Logs", currentYear);
                string logFileName = $"log_{DateTime.Now:yy_MM}.txt";
                string logFilePath = Path.Combine(logsDirectory, logFileName);

                // Check if the Logs directory exists, if not, create it
                if (!Directory.Exists(logsDirectory))
                {
                    Directory.CreateDirectory(logsDirectory);
                }

                // Prepare the log entry
                string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - Error in class '{className}', function '{functionName}': {errorMessage}";

                // Write the log entry to the file
                using (StreamWriter writer = new StreamWriter(logFilePath, true))
                {
                    writer.WriteLine(logEntry);
                }
            }
            catch
            {
            }
        }
    }
}
