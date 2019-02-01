namespace Frags.Core.Controllers.Results
{
    public class GenericResult : BaseResult
    {
        public GenericResult(string message, bool success = true) : base(message, success)
        {
        }

        public static GenericResult Generic(string message) =>
            new GenericResult(message);
    }
}