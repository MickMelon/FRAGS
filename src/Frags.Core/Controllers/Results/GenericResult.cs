namespace Frags.Core.Controllers.Results
{
    public class GenericResult : BaseResult
    {
        public GenericResult(string message) : base(message)
        {
        }

        public static GenericResult Result(string message) =>
            new GenericResult(message);
    }
}