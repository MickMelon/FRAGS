using System;
using Frags.Core.Common.Attributes;
using Frags.Core.Controllers.ViewModels;

namespace Frags.Core.Controllers.Results
{
    /// <summary>
    /// The parent Result class that all Results inherit from.
    /// </summary>
    public abstract class BaseResult : IResult
    {
        /// <summary>
        /// Indicates whether the operation was successful.
        /// </summary>
        public bool IsSuccess { get; private set; }

        /// <summary>
        /// The message of the result.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// The optional ViewModel as part of the result.
        /// </summary>
        /// <remarks>
        /// Objects are identified as a ViewModel by the ViewModelAttribute class.
        /// </remarks>
        public Object ViewModel { get; private set; }

        /// <summary>
        /// Initializes a new <see cref="BaseResult" /> class.
        /// </summary>
        /// <param name="message">The result message.</param>
        /// <param name="success">Whether the operation was successful.</param>
        protected BaseResult(string message, bool success = true, Object viewModel = null)
        {
            Message = message;
            IsSuccess = success;

            if (viewModel != null && ViewModelAttribute.IsViewModel(viewModel))
                ViewModel = viewModel;
        }
    }
}