namespace webApplication.Tests.Utils
{
    public class ServiceDescriptorNotFoundException : Exception
    {
        public ServiceDescriptorNotFoundException()
        {
        }

        public ServiceDescriptorNotFoundException(string message)
            : base(message)
        {
        }

        public ServiceDescriptorNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}