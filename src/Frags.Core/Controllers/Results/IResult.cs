using System;

namespace Frags.Core.Controllers.Results
{
    public interface IResult
    {
        bool IsSuccess { get; }
        string Message { get; }
        Object ViewModel { get; }
        IResult WithViewModel(Object obj);
    }
}