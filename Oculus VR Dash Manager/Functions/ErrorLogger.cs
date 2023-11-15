using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace OVR_Dash_Manager.Functions
{
    public static class ErrorLogger
    {
        private static readonly string LogFilePath;

        static ErrorLogger()
        {
            // Get the path of the executable and create a log file in the same directory.
            var exePath = Assembly.GetExecutingAssembly().Location;
            var exeDirectory = Path.GetDirectoryName(exePath);
            LogFilePath = Path.Combine(exeDirectory, "ErrorLog.txt");
        }

        public static void LogError(Exception ex, string additionalInfo = "")
        {
            try
            {
                // Create a string builder to hold the error details.
                var errorDetails = new StringBuilder();

                errorDetails.AppendLine("Timestamp: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                errorDetails.AppendLine("Additional Info: " + additionalInfo);
                errorDetails.AppendLine("Exception Type: " + ex.GetType().Name);
                errorDetails.AppendLine("Message: " + ex.Message);
                errorDetails.AppendLine("StackTrace: " + ex.StackTrace);

                // If there are inner exceptions, log them recursively.
                if (ex.InnerException != null)
                {
                    errorDetails.AppendLine("Inner Exception: ");
                    errorDetails.AppendLine(LogInnerException(ex.InnerException, "  "));
                }

                errorDetails.AppendLine(new string('-', 80)); // Separator for readability

                // Write the error details to the log file.
                File.AppendAllText(LogFilePath, errorDetails.ToString());
            }
            catch
            {
                // If logging fails (for example, due to file access issues),
                // your application needs to decide on the appropriate action to take.
            }
        }

        private static string LogInnerException(Exception ex, string indent)
        {
            var innerExceptionDetails = new StringBuilder();

            innerExceptionDetails.AppendLine(indent + "Exception Type: " + ex.GetType().Name);
            innerExceptionDetails.AppendLine(indent + "Message: " + ex.Message);
            innerExceptionDetails.AppendLine(indent + "StackTrace: " + ex.StackTrace);

            // Recursively log any further inner exceptions.
            if (ex.InnerException != null)
            {
                innerExceptionDetails.AppendLine(indent + "Inner Exception: ");
                innerExceptionDetails.AppendLine(LogInnerException(ex.InnerException, indent + "  "));
            }

            return innerExceptionDetails.ToString();
        }
    }
}

/*
 * Usage Example:
 * Here's how you might use this ErrorLogger in your code:
 *
 * try
 * {
 *     // Your code that might throw exceptions...
 * }
 * catch (Exception ex)
 * {
 *     ErrorLogger.LogError(ex, "An error occurred while processing the data.");
 * }
 */