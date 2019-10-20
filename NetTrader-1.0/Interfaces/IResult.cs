using System;
using System.Collections.Generic;
using System.Text;

namespace Interfaces
{
    public interface IResult<T>
    {
        bool Success { get; }
        string Message { get; }
        T Result { get; }
    }
}
