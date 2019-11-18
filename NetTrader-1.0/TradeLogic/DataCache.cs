using Interfaces.Messages;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Interfaces.MainClasses
{
    public class DataCache : ICache
    {
        private Dictionary<string, Dictionary<DateTime, Quote3Message>> _quotes;
        private ConcurrentDictionary<string, IInstrument> _instruments;
        private object _quoteSyncObj = new object();
        public DataCache()
        {
            _quotes = new Dictionary<string, Dictionary<DateTime, Quote3Message>>();
            Instruments = new Dictionary<string, IInstrument>();
            Accounts = new ConcurrentDictionary<string, IAccount>();
        }

        public Dictionary<string, IInstrument> Instruments { get; }

        public ConcurrentDictionary<string, IAccount> Accounts { get; }

        public bool AddAccount(IAccount account)
        {
            if (!Accounts.ContainsKey(account.Id))
            {
               return Accounts.TryAdd(account.Id, account);
            }
            return false;
        }

        public void AddInstrument(IInstrument instrument)
        {
            if (!Instruments.ContainsKey(instrument.InstrumentName))
                Instruments.Add(instrument.InstrumentName, instrument);
        }

        public void AddQuote(Quote3Message quote)
        {           
            if (_quotes.TryGetValue(quote.Direction, out Dictionary<DateTime, Quote3Message> qs))
            {
                var lastQuote = qs.Values.LastOrDefault();
                if (!quote.IsEqualQuotes(lastQuote))
                {
                    lock (_quoteSyncObj)
                    {
                        if (!qs.ContainsKey(quote.LastUpdateDate))
                        {
                            qs.Add(quote.LastUpdateDate, quote);
                        }                      
                    }
                }               
            }
            else
            {               
                lock (_quoteSyncObj)
                {
                    if (_quotes.ContainsKey(quote.Direction))
                        return;

                    var d = new Dictionary<DateTime, Quote3Message>();
                    d.Add(quote.LastUpdateDate, quote);
                    _quotes.Add(quote.Direction, d);
                }               
            }                   
        }


        public Dictionary<DateTime, Quote3Message> GetHistory(IInstrument instr)
        {
            if (_quotes.TryGetValue(instr.InstrumentName, out Dictionary<DateTime, Quote3Message> qs))
            {
                return new Dictionary<DateTime, Quote3Message>(qs);
            }
            return new Dictionary<DateTime, Quote3Message>();
        }

        public Quote3Message GetLevel2(IInstrument instr)
        {
            if (_quotes.TryGetValue(instr.InstrumentName, out Dictionary<DateTime, Quote3Message> qs) && qs.Count > 0)
            {
                return qs.Last().Value;
            }
            return null;
        }

        public IOrder GetOrderById(IInstrument instr, string id)
        {
            var level2 = GetLevel2(instr);
            return level2.Orders.Where(o => o.OrderId == id).FirstOrDefault();          
        }

        public List<IOrder> GetOrders(IInstrument instr, IAccount account = null)
        {
            throw new NotImplementedException();
        }

        public bool RemoveAccount(IAccount account) => Accounts.TryRemove(account.Id, out IAccount acc);
      
        public bool RemoveInstrument(IInstrument instrument) => _instruments.TryRemove(instrument.InstrumentName, out IInstrument instr);
      
    }
}
