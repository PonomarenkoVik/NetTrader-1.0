using Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WebMoneyVendor.Cache
{
    public class WebmoneyInstrument : IInstrument
    {
        internal const char INSTRUMENT_NAME_SELECTOR = '/';

        public string InstrumentId => throw new NotImplementedException();

        public double BankRate { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public string InstrumentName => throw new NotImplementedException();

        public string Currency1 => throw new NotImplementedException();

        public string Currency2 => throw new NotImplementedException();

        public IVendor Vendor => throw new NotImplementedException();

        public Task<IResult<IEnumerable<IOrder>>> GetLevel2Async(int sourceType)
        {
            throw new NotImplementedException();
        }

        private WebmoneyInstrument()
        {

        }

        public List<IInstrument> CreateInstruments(List<BestRate> bestRates)
        {
            List<string> currencies = new List<string>();
            foreach (var bRate in bestRates)
            {
                if (!currencies.Contains(bRate.Currency1))
                    currencies.Add(bRate.Currency1);
                if (!currencies.Contains(bRate.Currency2))
                    currencies.Add(bRate.Currency2);

                
            }
        }
    }
}
