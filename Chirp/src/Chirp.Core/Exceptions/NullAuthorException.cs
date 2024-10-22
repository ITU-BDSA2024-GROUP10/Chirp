using System;

namespace SimpleDB.Exceptions
{
    public class NullAuthorException : Exception
    {
        public NullAuthorException()
        {
        }

        public NullAuthorException(string message)
            : base(message)
        {
        }

        public NullAuthorException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}