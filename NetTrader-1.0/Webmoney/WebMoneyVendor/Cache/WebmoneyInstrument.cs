using Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WebMoneyVendor.Cache
{
    internal class WebmoneyInstrument : IInstrument
    {
        internal const char INSTRUMENT_NAME_SELECTOR = '/';
        internal const string PROPERTY_INSTRUMENTS = "instruments";
        public string InstrumentId { get; private set; }

        public BankRate BankRate { get; private set; }

        public string InstrumentName { get; private set; }

        public string Currency1 { get; private set; }

        public string Currency2 { get; private set; }

        public IVendor Vendor { get; }

        public string OppositeInstrumentName => $"{Currency2}{INSTRUMENT_NAME_SELECTOR}{Currency1}";

        public string PropertyId => "instrument";



        private const string INSTRUMENT_DESERIALIZATION_ERROR = "Serialization error";
        public Properties Properties
        {
            get
            {
                Properties prop = new Properties(PropertyId, null);
                prop.InsideProperties.Add(nameof(InstrumentId), new Properties(nameof(InstrumentId), InstrumentId));


                //var bankRateProp = new Properties(nameof(BankRate));
                //bankRateProp.InsideProperties.Add(nameof(BankRate.Empty.Direction), new Properties(nameof(BankRate.Empty.Direction), BankRate.Direction));
                //bankRateProp.InsideProperties.Add(nameof(BankRate.Empty.RateType), new Properties(nameof(BankRate.Empty.RateType), BankRate.RateType));
                //bankRateProp.InsideProperties.Add(nameof(BankRate.Empty.Rate), new Properties(nameof(BankRate.Empty.Rate), BankRate.Rate));
                //prop.InsideProperties.Add(nameof(BankRate), bankRateProp);
                prop.InsideProperties.Add(nameof(InstrumentName), new Properties(nameof(InstrumentName), InstrumentName));
                prop.InsideProperties.Add(nameof(Currency1), new Properties(nameof(Currency1), Currency1));
                prop.InsideProperties.Add(nameof(Currency2), new Properties(nameof(Currency2), Currency2));
                return prop;
            }
            set 
            {
                if (PropertyId != value.Id || value.InsideProperties.Count == 0)
                    return;


                if (value.InsideProperties.ContainsKey(nameof(InstrumentId)) && value.InsideProperties[nameof(InstrumentId)]?.Value is string instrumentId)
                    InstrumentId = instrumentId;
                else
                    throw new Exception($"{INSTRUMENT_DESERIALIZATION_ERROR} {nameof(InstrumentId)}");

                //if (value.InsideProperties.ContainsKey(nameof(BankRate)) && value.InsideProperties[nameof(BankRate)]?.InsideProperties.Count > 0)
                //{
                //    var props = value.InsideProperties[nameof(BankRate)].InsideProperties;
                //    if (props.ContainsKey(nameof(BankRate.Empty.Direction)) && props.ContainsKey(nameof(BankRate.Empty.Rate)) && props.ContainsKey(nameof(BankRate.Empty.RateType)))
                //    {
                //        BankRate = new BankRate(props[nameof(BankRate.Empty.Direction)].Value as string, props[nameof(BankRate.Empty.RateType)].Value as string, (double)props[nameof(BankRate.Empty.Rate)].Value);
                //    }
                //}

                //else
                //    throw new Exception($"{INSTRUMENT_DESERIALIZATION_ERROR} {nameof(BankRate)}");

                if (value.InsideProperties.ContainsKey(nameof(InstrumentName)) && value.InsideProperties[nameof(InstrumentName)]?.Value is string instrumentName)
                    InstrumentName = instrumentName;
                else
                    throw new Exception($"{INSTRUMENT_DESERIALIZATION_ERROR} {nameof(InstrumentName)}");

                if (value.InsideProperties.ContainsKey(nameof(Currency1)) && value.InsideProperties[nameof(Currency1)]?.Value is string currency1)
                    Currency1 = currency1;
                else
                    throw new Exception($"{INSTRUMENT_DESERIALIZATION_ERROR} {nameof(Currency1)}");

                if (value.InsideProperties.ContainsKey(nameof(Currency2)) && value.InsideProperties[nameof(Currency2)]?.Value is string currency2)
                    Currency2 = currency2;
                else
                    throw new Exception($"{INSTRUMENT_DESERIALIZATION_ERROR} {nameof(Currency2)}");
            }
        }

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

        public WebmoneyInstrument(WebmoneyVendor webmoneyVendor)
        {
            this.Vendor = webmoneyVendor;
        }

        public static bool operator ==(WebmoneyInstrument instr1, WebmoneyInstrument instr2)
        {
            if (instr1 is null && instr2 is null)
                return true;

            if (instr1 is null || instr2 is null)
                return false;

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
