using System;
using System.Collections.Generic;
using System.Text;

namespace Interfaces
{
    public interface IOrder
    {
        string OrderId { get; }
        DateTime ApplicationDate { get; }
        IInstrument Instrument { get; }
        double Sum1 { get; set; }
        double Sum2 { get; }
        double StraitCrossrate { get; }
        double ReveseCrossrate { get; }
        bool IsOwn { get; }
    }
}
