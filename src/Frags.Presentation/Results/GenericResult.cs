using Frags.Core.Common;

namespace Frags.Presentation.Results
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

        /// <summary>
        /// Used when a command fails to complete.
        /// Usually raised with an exception message.
        /// </summary>
        public static GenericResult Failure(string error) =>
            new GenericResult(error, false);

        /// <summary>
        /// Used when a command's input is in an invalid format.
        /// </summary>
        public static GenericResult InvalidInput() =>
            new GenericResult(Messages.INVALID_INPUT, false);

        /// <summary>
        /// Used when a command's input was too low.
        /// </summary>
        public static GenericResult NotEnoughPoints() =>
            new GenericResult(Messages.NOT_ENOUGH_POINTS, false);

        /// <summary>
        /// Used when a command's input was too low.
        /// </summary>
        public static GenericResult ValueTooLow() =>
            new GenericResult(Messages.TOO_LOW, false);

        /// <summary>
        /// Used when a command's input was too high.
        /// </summary>
        public static GenericResult ValueTooHigh() =>
            new GenericResult(Messages.TOO_HIGH, false);
    }
}