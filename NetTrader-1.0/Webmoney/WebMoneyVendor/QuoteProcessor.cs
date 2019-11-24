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

        private object _subscriberSync = new object();
        private object _subscriptionrSync = new object();
        private List<Subscription> _subscriptions = new List<Subscription>();
        private Dictionary<IInstrument, SubscriptionType> _subscribedInstruments = new Dictionary<IInstrument, SubscriptionType>();
        public Action<Quote3Message> OnQuoteEvent;
        private long counter = 0;

        public QuoteSource DataType { get; internal set; }

        public QuoteProcessor(IVendor vendor)
        {
            _vendor = vendor;
            _timer.Elapsed += Tick;
            _timer.Start();
        }

        private void Tick(object sender, ElapsedEventArgs e)
        {
            counter++;
            Dictionary<IInstrument, SubscriptionType> subscrInstruments = null;
            lock (_subscriberSync)
            {
                subscrInstruments = new Dictionary<IInstrument, SubscriptionType>(_subscribedInstruments);
            }
           
            foreach (var subInstr in subscrInstruments)
            {
                if (counter % (int)subInstr.Value == 0)
                {
                    if (DataType == QuoteSource.WebXML || DataType == QuoteSource.XML)
                        Task.Factory.StartNew(() => GetQuote(subInstr.Key, QuoteSource.XML));

                    if (DataType == QuoteSource.WebXML || DataType == QuoteSource.Web)
                        Task.Factory.StartNew(() => GetQuote(subInstr.Key, QuoteSource.Web));
                }
            }
        }

        private async void GetQuote(IInstrument subInstr, QuoteSource source)
        {
            if (OnQuoteEvent != null)
            {
                var quotes = await _vendor.GetLevel2FromServer(subInstr, (int)source);
                foreach (var q in quotes)
                    OnQuoteEvent.Invoke(q);
            }
        }

        public void Subscribe(Subscription subscr)
        {
            lock (_subscriberSync)
            {
                lock (_subscriptionrSync)
                {
                    if (!_subscriptions.Contains(subscr))
                        _subscriptions.Add(subscr);
                }
                UpdateSubscribers();
            }          
        }

        private void UpdateSubscribers()
        {
            lock (_subscriptionrSync)
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
        }

        public void UnSubscribe(Subscription subscr)
        {
            lock (_subscriberSync)
            {
                lock (_subscriptionrSync)
                {
                    if (_subscriptions.Contains(subscr))
                        _subscriptions.Remove(subscr);
                }
                UpdateSubscribers();
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
