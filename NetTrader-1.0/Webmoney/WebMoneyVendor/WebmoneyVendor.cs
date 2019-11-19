using Interfaces;
using Interfaces.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebMoneyVendor
{
    public class WebmoneyVendor : IVendor
    {
        #region Properties
        private static readonly string _tradeUrl = "https://wm.exchanger.ru/asp/wmlist.asp?exchtype=";
        private static readonly string _tradeXMLUrl = "https://wm.exchanger.ru/asp/XMLwmlist.asp?exchtype=";
        private static readonly string _bestRates = "https://wm.exchanger.ru/asp/XMLbestRates.asp";
        private ICache _cache;
        private QuoteProcessor _quoteProcessor;
        private WebConnection _connection = WebConnection.Instance;

        public event Action<Quote3Message> OnNewQuoteEvent;

        public string VendorName => "Wm.Exchanger.ru";

        public bool UseProxy
        {
            get => _connection.UseProxy;
            set => _connection.UseProxy = value;
        }
        #endregion



        public WebmoneyVendor(ICache cache, ILog logCache)
        {
            UseProxy = true;
            _cache = cache;
            _quoteProcessor = new QuoteProcessor(this);
            _quoteProcessor.OnQuoteEvent += _cache.AddQuote;
            _cache.OnNewQuoteEvent += OnNewQuote;
            Populate();
        }

        private void OnNewQuote(Quote3Message mess) => OnNewQuoteEvent?.Invoke(mess);
       
        private async void Populate()
        {
            Thread.Sleep(3000);
            var content = await WebConnection.Instance.ReadUrlAsync(_bestRates);
            var bestRates = XmlParser.GreateBestRatesByXML(content);
            var instruments = WebmoneyHelper.CreateInstruments(bestRates, this);
            foreach (var instr in instruments)
            {
                _cache.AddInstrument(instr);
            }
        }

        public void Subscribe(Subscription subscr) => _quoteProcessor.Subscribe(subscr);
     
        public void UnSubscribe(Subscription subscr) => _quoteProcessor.UnSubscribe(subscr);
       

        public bool CreateAccount(string login, string id, string pass)
        {
            var acc = new WebmoneyAccount(login, id, pass, this);
           return  _cache.AddAccount(acc);
        }

        public bool RemoveAccount(string id) => _cache.RemoveAccount(id);
       

        public Dictionary<string, IInstrument> GetAllInstruments() => _cache.Instruments;


        public Task<IResult<List<IAsset>>> GetAssetsAsync(IAccount account)
        {
            throw new NotImplementedException();
        }

        public IInstrument GetInstrumentByName(string name)
        {
            if (_cache.Instruments.TryGetValue(name, out IInstrument instr))
                return instr;

            return null;
        }


        public async Task<Quote3Message> GetLevel2FromServer(IInstrument instrument)
        {
            var content = await _connection.ReadUrlAsync($"{_tradeXMLUrl}{instrument.InstrumentId}");
            var quote = XmlParser.CreateQuote3MessageByXML(content);
            return quote;
        }

        public Quote3Message GetLevel2(IInstrument instrument) => _cache.GetLevel2(instrument);


        public IOrder GetOrderById(IInstrument instr, string id) => _cache.GetOrderById(instr, id);


        public Task<IResult<List<IOrder>>> GetOrdersByAccount(IAccount account)
        {
            throw new NotImplementedException();
        }

        public Task<IResult<T>> SendMessageToVendor<T>(Message mess)
        {
            throw new NotImplementedException();
        }
    }
}
