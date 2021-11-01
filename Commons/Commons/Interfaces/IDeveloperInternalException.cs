using System;

namespace Commons.Interfaces
{
    public interface IDeveloperInternalException : IInternalException
    {
        Exception? InnerException { get; set; }
        string StackTrace { get; set; }
    }
}
