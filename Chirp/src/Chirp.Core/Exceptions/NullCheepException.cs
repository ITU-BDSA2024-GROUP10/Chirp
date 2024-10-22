using System;

namespace SimpleDB.Exceptions
{
    public class NullCheepException : Exception
    {
        public NullCheepException()
        {
        }

        public NullCheepException(string message)
            : base(message)
        {
        }

        public NullCheepException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}