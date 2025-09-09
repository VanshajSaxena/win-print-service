namespace PrintService.Exceptions
{
    public class JobNotFoundException : Exception
    {
        public JobNotFoundException()
        {
        }

        public JobNotFoundException(string? message) : base(message)
        {
        }

        public JobNotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
    public class PrintQueueNotFoundException : Exception
    {
        public PrintQueueNotFoundException()
        {
        }

        public PrintQueueNotFoundException(string? message) : base(message)
        {
        }

        public PrintQueueNotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
    public class ConversionFailedException : Exception
    {
        public ConversionFailedException()
        {
        }

        public ConversionFailedException(string? message) : base(message)
        {
        }

        public ConversionFailedException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
