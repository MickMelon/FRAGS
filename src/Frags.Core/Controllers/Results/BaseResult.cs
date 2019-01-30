using System;
using Frags.Core.Common.Attributes;
using Frags.Core.Controllers.ViewModels;

namespace Frags.Core.Controllers.Results
{
    public abstract class BaseResult : IResult
    {
        public string Message { get; private set; }
        public Object ViewModel { get; private set; }

        protected BaseResult(string message)
        {
            Message = message;
        }

        public IResult WithViewModel(Object obj)
        {
            if (Common.Attributes.ViewModel.IsViewModel(obj))
                ViewModel = obj;
                
            return this;
        }
    }
}