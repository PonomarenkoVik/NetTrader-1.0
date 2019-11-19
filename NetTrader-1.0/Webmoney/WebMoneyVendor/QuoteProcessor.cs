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
        private Timer _timer = new Timer() { Interval = 50 };

        private object _subscrSync = new object();
        private List<Subscription> _subscriptions = new List<Subscription>();
        private Dictionary<IInstrument, SubscriptionType> _subscribedInstruments = new Dictionary<IInstrument, SubscriptionType>();
        public Action<Quote3Message> OnQuoteEvent;
        private long counter = 0;

        public QuoteProcessor(IVendor vendor)
        {
            _vendor = vendor;
            _timer.Elapsed += Tick;
            _timer.Start();
        }

        private void Tick(object sender, ElapsedEventArgs e)
        {
            counter++;
            foreach (var subInstr in _subscribedInstruments)
            {
                if (counter % (int)subInstr.Value == 0)
                    Task.Factory.StartNew(() => GetQuote(subInstr.Key));
            }
        }

        private async void GetQuote(IInstrument subInstr)
        {
            if (OnQuoteEvent != null)
            {
                var quote = await _vendor.GetLevel2FromServer(subInstr);
                if (quote != null)
                    OnQuoteEvent.Invoke(quote);
            }
        }

        public void Subscribe(Subscription subscr)
        {
            lock (_subscrSync)
            {
                if (!_subscriptions.Contains(subscr))
                    _subscriptions.Add(subscr);

                UpdateSubscribers();
            }          
        }

        private void UpdateSubscribers()
        {
            _subscribedInstruments.Clear();
            foreach (var subscr in _subscriptions)
            {
                if (_subscribedInstruments.ContainsKey(subscr.Instrument) && (int)_subscribedInstruments[subscr.Instrument] < (int)subscr.SubscriptionType)
                {
                    _subscribedInstruments[subscr.Instrument] = subscr.SubscriptionType;
                }
                else
                    _subscribedInstruments.Add(subscr.Instrument, subscr.SubscriptionType);
            }
        }

        public void UnSubscribe(Subscription subscr)
        {
            lock (_subscrSync)
            {
                if (_subscriptions.Contains(subscr))
                    _subscriptions.Remove(subscr);

                UpdateSubscribers();
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
