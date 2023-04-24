namespace AppDiv.CRVS.Application.Exceptions
{
    public class AuthenticationException : Exception
    {
        public AuthenticationException(string errors) : base(errors)
        {

        }

    }
}
