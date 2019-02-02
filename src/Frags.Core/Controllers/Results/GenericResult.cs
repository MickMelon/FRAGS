namespace Frags.Core.Controllers.Results
{
    /// <summary>
    /// Represents a result type for general purposes.
    /// </summary>
    public class GenericResult : BaseResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericResult" /> class
        /// </summary>
        public GenericResult(string message, bool success = true, object viewModel = null) : base(message, success, viewModel)
        {
        }

        /// <summary>
        /// Returns a new <see cref="GenericResult" />.
        /// </summary>
        /// <param name="message">The result message.</param>
        public static GenericResult Generic(string message) =>
            new GenericResult(message);
    }
}