using Interfaces.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace Interfaces.MainClasses
{
    public class DataCache : ILocalCache
    {
        public List<IInstrument> Instruments => throw new NotImplementedException();

        public List<IInstrument> Accounts => throw new NotImplementedException();

        public bool AddAccount(IInstrument instrument)
        {
            throw new NotImplementedException();
        }

        public bool AddInstrument(IInstrument instrument)
        {
            throw new NotImplementedException();
        }

        public void AddQuote(Quote3Message quote)
        {
            throw new NotImplementedException();
        }

        public Dictionary<DateTime, List<IOrder>> GetHistory()
        {
            throw new NotImplementedException();
        }

        public IOrder GetOrderById(string id)
        {
            throw new NotImplementedException();
        }

        public List<IOrder> GetOrders(IInstrument instr, IAccount account = null)
        {
            throw new NotImplementedException();
        }

        public bool RemoveAccount(IInstrument instrument)
        {
            throw new NotImplementedException();
        }

        public bool RemoveInstrument(IInstrument instrument)
        {
            throw new NotImplementedException();
        }
    }
}
