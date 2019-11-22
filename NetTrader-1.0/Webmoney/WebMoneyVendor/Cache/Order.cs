using Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebMoneyVendor.Cache
{
    class Order : IOrder
    {
        OrderParams _params;
        public Order(OrderParams param)
        {
            _params = param;
        }

        public string OrderId => _params.OrderId;

        public DateTime LastUpdateDate => _params.ApplicationDate;

        public IInstrument Instrument => _params.Instrument;

        public double AmountIn => _params.AmountIn;

        public double AmountOut => _params.AmountOut;

        public double StraitCrossrate => _params.StraitCrossrate;

        public double ReverseCrossrate => _params.ReverseCrossrate;

        public double AllAmount => _params.AllAmountOut;

        public double ProcentBankRate => _params.ProcentBankRate;

        public bool IsOwn
        {
            get => _params.IsOwn;
            set => _params.IsOwn = value;
        } 
    }
}
