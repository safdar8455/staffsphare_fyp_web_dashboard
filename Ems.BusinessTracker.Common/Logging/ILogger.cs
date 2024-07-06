using System;

namespace Ems.BusinessTracker.Common
{
   public interface ILogger
    {
        void LogMessage(string message);
        void Log(string message);
        void LogException(Exception exception);
        void Log(Exception exception);
        void LogException(Exception exception, string additionalMessage);
    }
}
