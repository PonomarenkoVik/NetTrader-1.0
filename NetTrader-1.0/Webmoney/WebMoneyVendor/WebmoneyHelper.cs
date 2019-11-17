using Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using WebMoneyVendor.Cache;

namespace WebMoneyVendor
{
    internal static class WebmoneyHelper
    {
        public static List<IInstrument> CreateInstruments(List<BestRate> bestRates, IVendor vendor)
        {
            List<IInstrument> instruments = new List<IInstrument>();
            foreach (var bRate in bestRates)
            {
                var instrument = CreateInstrument(bRate, vendor);
                instruments.Add(instrument);
            }
            return instruments;
        }

        private static IInstrument CreateInstrument(BestRate bRate, IVendor vendor)
        {
            return new WebmoneyInstrument(bRate.ExchangeType, bRate.InstrumentName, bRate.Currency1, bRate.Currency2, BankRate.Empty, vendor);
        }
    }
}
