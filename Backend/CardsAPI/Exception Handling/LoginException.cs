using System;
namespace CardsAPI.ExceptionHandling
{
    public class LoginException : ApplicationException
    {
        public LoginException()
        {
        }

        public LoginException(string message) : base(message)
        {
        }

        public LoginException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public LoginException(Exception innerException) : base(string.Empty, innerException)
        {
        }
    }
}
