using System;
using System.Collections.Generic;
using System.Text;

namespace Interfaces
{
    public interface IAsset
    {
        string Id { get; }
        double Balance { get; }
        string Currency { get; }
        IAccount Account { get; }
    }
}
