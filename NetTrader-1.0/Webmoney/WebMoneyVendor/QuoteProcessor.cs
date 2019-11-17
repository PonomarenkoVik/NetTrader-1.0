using Interfaces;
using Interfaces.Messages;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace WebMoneyVendor
{
    internal class QuoteProcessor : IDisposable
    {
        private IVendor _vendor;
        private Timer _timer = new Timer();
        private BlockingCollection<IInstrument> _subscribedInstruments = new BlockingCollection<IInstrument>();

        public Action<Quote3Message> OnQuote;

        public QuoteProcessor(IVendor vendor, int interval = 50)
        {
            _vendor = vendor;
            _timer.Interval = interval;
            _timer.Elapsed += Tick;
            Task.Factory.StartNew(Start);
        }

        private void Tick(object sender, ElapsedEventArgs e)
        {
            foreach (var subInstr in _subscribedInstruments)
                Task.Factory.StartNew(() => GetQuote(subInstr));


        }

        private void GetQuote(IInstrument subInstr)
        {
            if (OnQuote != null)
            {
               var quote = _vendor.GetLevel2
            }
        }

        public void Subscribe(IInstrument instr)
        {
            throw new NotImplementedException();
        }

        public void UnSubscribe(IInstrument instr)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
