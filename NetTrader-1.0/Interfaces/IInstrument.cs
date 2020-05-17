using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface IInstrument : IProperties
    {
        string InstrumentId { get; }
        BankRate BankRate { get; }
        string InstrumentName { get; }
        string OppositeInstrumentName { get; }
        string Currency1 { get; }
        string Currency2 { get; }
        IVendor Vendor { get; }
        Task<IResult<IEnumerable<IOrder>>> GetLevel2Async(int sourceType);
    }
}
