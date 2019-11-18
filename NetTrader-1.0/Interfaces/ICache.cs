using Interfaces.Messages;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Interfaces
{
    public interface ICache
    {
        void AddInstrument(IInstrument instrument);
        bool RemoveInstrument(IInstrument instrument);
        Dictionary<string, IInstrument> Instruments { get; }
        bool AddAccount(IAccount account);
        bool RemoveAccount(string id);
        ConcurrentDictionary<string, IAccount> Accounts { get; }
        void AddQuote(Quote3Message quote);
        Quote3Message GetLevel2(IInstrument instr);
        IOrder GetOrderById(IInstrument instr, string id);
        Dictionary<DateTime, Quote3Message> GetHistory(IInstrument instr);
        event Action<Quote3Message> OnNewQuoteEvent;
    }
}
