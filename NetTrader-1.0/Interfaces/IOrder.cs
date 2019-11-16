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
        double AmountIn { get; }
        double AmountOut { get; }
        double StraitCrossrate { get; }
        double ReverseCrossrate { get; }
        double AllAmount { get; }
        double ProcentBankRate { get; }
        bool IsOwn { get; set; }
    }
}
