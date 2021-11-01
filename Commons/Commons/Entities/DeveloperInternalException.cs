using Commons.Interfaces;
using System;

namespace Commons.Entities
{
    public class DeveloperInternalException : InternalException, IDeveloperInternalException
    {
        public DeveloperInternalException()
        {

        }

        public DeveloperInternalException(string message, Exception? exception, string stackTrace)
        {
            Message = message;
            InnerException = exception;
            StackTrace = stackTrace;
        }

        public Exception InnerException { get; set; }
        public string StackTrace { get; set; }
    }
}
