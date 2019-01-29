using System;
using Frags.Core.Common.Attributes;

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

        public IResult WithViewModel(Object viewModel)
        {
            if (Common.Attributes.ViewModel.IsViewModel(ViewModel))
                ViewModel = viewModel;

            return this;
        }
    }
}