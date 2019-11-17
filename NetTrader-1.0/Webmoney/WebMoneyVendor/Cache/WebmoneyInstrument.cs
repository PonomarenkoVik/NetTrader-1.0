using Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WebMoneyVendor.Cache
{
    internal class WebmoneyInstrument : IInstrument
    {
        internal const char INSTRUMENT_NAME_SELECTOR = '/';

        public string InstrumentId { get; }

        public BankRate BankRate { get; private set; }

        public string InstrumentName { get; }

        public string Currency1 { get; }

        public string Currency2 { get; }

        public IVendor Vendor { get; }

        public string OppositeInstrumentName => $"{Currency2}{INSTRUMENT_NAME_SELECTOR}{Currency1}";

        public Task<IResult<IEnumerable<IOrder>>> GetLevel2Async(int sourceType)
        {
            throw new NotImplementedException();
        }

        public WebmoneyInstrument(string instrId, string instrName, string curr1, string curr2, BankRate bRate, IVendor vend )
        {
            InstrumentId = instrId;
            InstrumentName = instrName;
            Currency1 = curr1;
            Currency2 = curr2;
            BankRate = bRate;
            Vendor = vend;
        }

        public static bool operator ==(WebmoneyInstrument instr1, WebmoneyInstrument instr2)
        {
            if (instr1 == null || instr2 == null)
            {
                throw new Exception("instrument is null");
            }

            return instr1.InstrumentId == instr2.InstrumentId && instr1.InstrumentName == instr2.InstrumentName;
        }

        public static bool operator !=(WebmoneyInstrument instr1, WebmoneyInstrument instr2) => !(instr1 == instr2);

        public override int GetHashCode() => (InstrumentId + InstrumentName).GetHashCode();

        public override bool Equals(object obj)
        {
            return obj is WebmoneyInstrument instr && instr == this; ;
        }
    }
}
