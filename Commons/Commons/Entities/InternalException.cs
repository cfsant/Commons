using Commons.Interfaces;

namespace Commons.Entities
{
    public class InternalException : IInternalException
    {
        public InternalException()
        {

        }

        public InternalException(string message)
        {
            Message = message;
        }

        public string Message { get; set; }
    }
}
