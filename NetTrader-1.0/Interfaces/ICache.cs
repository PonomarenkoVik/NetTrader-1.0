using Interfaces.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace Interfaces
{
    public interface ILocalCache
    {
        bool AddInstrument(IInstrument instrument);
        bool RemoveInstrument(IInstrument instrument);
        List<IInstrument> Instruments { get; }
        bool AddAccount(IInstrument instrument);
        bool RemoveAccount(IInstrument instrument);
        List<IInstrument> Accounts { get; }
        void AddQuote(Quote3Message quote);
        List<IOrder> GetOrders(IInstrument instr, IAccount account = null);
        IOrder GetOrderById(string id);
        Dictionary<DateTime, List<IOrder>> GetHistory();
    }
}
