using System;
using System.Text;

namespace ProjectTemplate.Desktop
{
    internal static class ExceptionExtensions
    {
        public static string GetDisplayMessage(this Exception exception)
        {
            string message;
            if (exception is AggregateException)
            {
                StringBuilder messageBuilder = new StringBuilder();
                AggregateException aggregateException = (AggregateException)exception;
                foreach (Exception innerException in aggregateException.InnerExceptions)
                {
                    string innerExceptionMessage = innerException.GetDisplayMessage();
                    messageBuilder.AppendLine(innerExceptionMessage);
                }
                message = messageBuilder.ToString();
            }
            else
            {
                message = exception.Message;
            }
            return message;
        }
    }
}
