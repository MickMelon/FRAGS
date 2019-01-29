using System;

namespace Frags.Core.Controllers.Results
{
    public interface IResult
    {
        string Message { get; }
        Object ViewModel { get; }
        IResult WithViewModel(Object obj);
    }
}