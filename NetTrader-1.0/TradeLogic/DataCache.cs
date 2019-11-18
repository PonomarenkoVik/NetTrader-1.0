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
        #region Properties
        private Dictionary<string, Dictionary<DateTime, Quote3Message>> _quotes;
        private ConcurrentDictionary<string, IInstrument> _instruments;
        private object _quoteSyncObj = new object();
        public event Action<Quote3Message> OnNewQuoteEvent;
        #endregion



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


        const int QUOTE_CACHE_DEFAULT_SIZE = 1000;
        private Dictionary<string, int> _quoteSizes = new Dictionary<string, int>();
        public void SetSizeQuoteCache(string instrName, int newSize)
        {
            if (_quoteSizes.ContainsKey(instrName))
            {
                _quoteSizes[instrName] = newSize;
                return;
            }
            _quoteSizes.Add(instrName, newSize);
        }

        private int GetSizeQuoteCache(string instrName)
        {
            if (_quoteSizes.TryGetValue(instrName, out int size))
                return size;

            return QUOTE_CACHE_DEFAULT_SIZE;
        }

        public void AddQuote(Quote3Message quote)
        {           
            if (_quotes.TryGetValue(quote.InstrumentName, out Dictionary<DateTime, Quote3Message> qs))
            {
                var lastQuote = qs.Values.LastOrDefault();
                if (quote.IsEqualQuotes(lastQuote))
                    return;

                lock (_quoteSyncObj)
                {
                    if (!qs.ContainsKey(quote.LastUpdateDate))
                    {
                        qs.Add(quote.LastUpdateDate, quote);
                    }
                    if (qs.Count > GetSizeQuoteCache(quote.InstrumentName))
                        qs.Remove(qs.First().Key);
                }
                OnNewQuoteEvent?.Invoke(quote);
                return;
            }

            lock (_quoteSyncObj)
            {
                if (_quotes.ContainsKey(quote.InstrumentName))
                    return;

                var quotDict = new Dictionary<DateTime, Quote3Message>() { { quote.LastUpdateDate, quote } };
                _quotes.Add(quote.InstrumentName, quotDict);
            }                                          
        }

        public Dictionary<DateTime, Quote3Message> GetHistory(IInstrument instr)
        {
            if (_quotes.TryGetValue(instr.InstrumentName, out Dictionary<DateTime, Quote3Message> qs))
                return new Dictionary<DateTime, Quote3Message>(qs);
            
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

        public bool RemoveAccount(string id) => Accounts.TryRemove(id, out IAccount acc);
      
        public bool RemoveInstrument(IInstrument instrument) => _instruments.TryRemove(instrument.InstrumentName, out IInstrument instr);
      
    }
}
