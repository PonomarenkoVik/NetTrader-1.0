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
        private Dictionary<string, IInstrument> _subscribedInstruments = new Dictionary<string, IInstrument>();
        public Action<Quote3Message> OnQuote;

        public QuoteProcessor(IVendor vendor, int interval = 100)
        {
            _vendor = vendor;
            _timer.Interval = interval;
            _timer.Elapsed += Tick;
            _timer.Start();
        }

        private void Tick(object sender, ElapsedEventArgs e)
        {
            foreach (var subInstr in _subscribedInstruments.Values)
                Task.Factory.StartNew(() => GetQuote(subInstr));
        }

        private async void GetQuote(IInstrument subInstr)
        {
            if (OnQuote != null)
            {
                var quote = await _vendor.GetLevel2FromServer(subInstr);
                if (quote != null)
                    OnQuote.Invoke(quote);
            }
        }

        public void Subscribe(IInstrument instr)
        {
            _subscribedInstruments.Add(instr.InstrumentName, instr);
        }

        public void UnSubscribe(IInstrument instr)
        {
            _subscribedInstruments.Remove(instr.InstrumentName);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
