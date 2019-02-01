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
        /// <inheritdoc/>
        public bool IsSuccess { get; private set; }

        /// <inheritdoc/>
        public string Message { get; private set; }

        /// <inheritdoc/>
        public Object ViewModel { get; private set; }

        /// <summary>
        /// Initializes a new <see cref="BaseResult" /> class.
        /// </summary>
        /// <param name="message">The result message.</param>
        /// <param name="success">Whether the operation was successful.</param>
        protected BaseResult(string message, bool success = true)
        {
            Message = message;
            IsSuccess = success;
        }

        /// <inheritdoc/>
        public IResult WithViewModel(Object obj)
        {
            if (Common.Attributes.ViewModelAttribute.IsViewModel(obj))
                ViewModel = obj;
                
            return this;
        }
    }
}