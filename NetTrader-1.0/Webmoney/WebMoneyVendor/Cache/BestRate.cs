using System;
using System.Collections.Generic;
using System.Text;

namespace WebMoneyVendor.Cache
{
    public class BestRate
    {
        public BestRate(string instrumentName, int exchangeType, double baseRate, Dictionary<int, double> percentages, Dictionary<int, double> volumes)
        {
            InstrumentName = instrumentName;
            BaseRate = baseRate;
            Percentages = percentages;
            Volumes = volumes;
            ExchangeType = exchangeType;
        }

        public string InstrumentName { get; }
        public int ExchangeType { get; }
        public string Currency1 => InstrumentName?.Split(WebmoneyInstrument.INSTRUMENT_NAME_SELECTOR)[0];
        public string Currency2 => InstrumentName?.Split(WebmoneyInstrument.INSTRUMENT_NAME_SELECTOR)[1];
        public double BaseRate { get; }
        public Dictionary<int, double> Percentages { get; }
        public Dictionary<int, double> Volumes { get; }
    }
}
