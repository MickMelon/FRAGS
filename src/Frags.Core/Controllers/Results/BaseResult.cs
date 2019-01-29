namespace Frags.Core.Controllers.Results
{
    public abstract class BaseResult : IResult
    {
        public string Message { get; }

        protected BaseResult(string message)
        {
            Message = message;
        }
    }
}