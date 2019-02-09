using System;
using Frags.Core.Common.Extensions;
using Frags.Presentation.Attributes;

namespace Frags.Presentation.Results
{
    /// <summary>
    /// The parent Result class that all Results inherit from.
    /// </summary>
    public abstract class BaseResult : IResult, IEquatable<BaseResult>
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

        /// <summary>
        /// Checks whether the values of an object is equal to this object.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>Whether the objects values are equals.</returns>
        public override bool Equals(object obj)
        {
            if (obj != null && obj is BaseResult result)
                return IsSuccess == result.IsSuccess &&
                       Message.EqualsIgnoreCase(result.Message);

            return false;
        }

        /// <summary>
        /// Checks whether the values of the BaseResults are equal.
        /// </summary>
        /// <remarks>
        /// This is an overload of the overriden Equals because it will
        /// save performance because there's no need to check if parameter
        /// is BaseResult type.
        /// </remarks>
        /// <param name="result">The result to check.</param>
        /// <returns>Whether the results match.</returns>
        public bool Equals(BaseResult result) =>
            result != null && 
            IsSuccess == result.IsSuccess &&
            Message.EqualsIgnoreCase(result.Message) &&
            ViewModel.GetType() == result.ViewModel.GetType();

        /// <summary>
        /// Gets the object's hash code.
        /// </summary>
        /// <remarks>
        /// Josh Bloch's algorithm.
        /// </remarks>
        /// <returns>The object's hash code.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 41;
                hash = hash * 53 + base.GetHashCode();
                hash = hash * 53 + Message.GetHashCode();
                return hash;
            }
        }
    }
}