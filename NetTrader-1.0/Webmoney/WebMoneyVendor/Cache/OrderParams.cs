using Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebMoneyVendor.Cache
{
    class OrderParams
    {
        public string OrderId { get; set; }

        public DateTime ApplicationDate { get; set; }

        public IInstrument Instrument { get; set; }

        public double AmountIn { get; set; }

        public double AmountOut { get; set; }

        public double StraitCrossrate { get; set; }

        public double ReverseCrossrate { get; set; }

        public double AllAmount { get; set; }

        public double ProcentBankRate { get; set; }

        public bool IsOwn { get; set; }
    }
}
