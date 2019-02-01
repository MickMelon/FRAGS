using System;
using Frags.Core.Common.Attributes;
using Frags.Core.Controllers.ViewModels;

namespace Frags.Core.Controllers.Results
{
    public abstract class BaseResult : IResult
    {
        public bool IsSuccess { get; private set; }
        public string Message { get; private set; }
        public Object ViewModel { get; private set; }

        protected BaseResult(string message, bool success = true)
        {
            Message = message;
            IsSuccess = success;
        }

        public IResult WithViewModel(Object obj)
        {
            if (Common.Attributes.ViewModelAttribute.IsViewModel(obj))
                ViewModel = obj;
                
            return this;
        }
    }
}