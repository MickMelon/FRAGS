using System;

namespace Frags.Core.Controllers.Results
{
    /// <summary>
    /// Contains information for the result of the controller.
    /// </summary>
    public interface IResult
    {
        /// <summary>
        /// Indicates whether the operation was successful.
        /// </summary>
        bool IsSuccess { get; }

        /// <summary>
        /// The message of the result.
        /// </summary>
        string Message { get; }

        /// <summary>
        /// The optional ViewModel as part of the result.
        /// </summary>
        /// <remarks>
        /// Objects are identified as a ViewModel by the ViewModelAttribute class.
        /// </remarks>
        Object ViewModel { get; }

        /// <summary>
        /// Adds a ViewModel to the result.
        /// </summary>
        /// <param name="obj">Object with ViewModelAttribute</param>
        /// <returns>Itself (this)</returns>
        IResult WithViewModel(Object obj);
    }
}