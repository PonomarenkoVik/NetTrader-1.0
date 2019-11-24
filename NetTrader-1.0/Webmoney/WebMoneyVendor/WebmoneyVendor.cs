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
    public class WebmoneyVendor : IVendor, IDisposable
    {
        #region Properties
        const string TRADE_URL = "https://wm.exchanger.ru/asp/wmlist.asp?exchtype=";
        const string TRADE_XML_URL = "https://wm.exchanger.ru/asp/XMLwmlist.asp?exchtype=";
        const string BEST_RATES = "https://wm.exchanger.ru/asp/XMLbestRates.asp";
        private ICache _cache;
        private QuoteProcessor _quoteProcessor;
        private WebConnection _connection = WebConnection.Instance;

        public event Action<Quote3Message> OnNewQuoteEvent;
        public event Action LoadedEvent;
        public string VendorName => "Wm.Exchanger.ru";

        public bool UseProxy
        {
            get => _connection.UseProxy;
            set => _connection.UseProxy = value;
        }
        public string DataType
        {
            get => _quoteProcessor.DataType.ToString();
            set
            {
                if (Enum.TryParse<QuoteSource>(value, out QuoteSource qSource))
                {
                    _quoteProcessor.DataType = qSource;
                }
                else
                    throw new Exception("Wrong data type");               
            }
        }
        #endregion



        public WebmoneyVendor(ICache cache, ILog logCache)
        {
            UseProxy = true;
            _cache = cache;
            _quoteProcessor = new QuoteProcessor(this) { DataType = QuoteSource.WebXML};
            _quoteProcessor.OnQuoteEvent += _cache.AddQuote;
            _cache.OnNewQuoteEvent += OnNewQuote;
            WebConnection.Instance.OnProxiesLoaded += Populate;
        }

        private void OnNewQuote(Quote3Message mess) => OnNewQuoteEvent?.Invoke(mess);
       
        private async void Populate()
        {
            List<IInstrument> instruments = await GetInstruments();
            foreach (var instr in instruments)
                _cache.AddInstrument(instr);
            LoadedEvent?.Invoke();
        }

        private async Task<List<IInstrument>> GetInstruments()
        {
            var content = await WebConnection.Instance.ReadUrlAsync(BEST_RATES);
            var bestRates = XmlParser.GreateBestRatesByXML(content);
            var instruments = WebmoneyHelper.CreateInstruments(bestRates, this);
            return instruments;
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


        public async Task<List<Quote3Message>> GetLevel2FromServer(IInstrument instrument, int source)
        {
            string url = ((QuoteSource)source) == QuoteSource.XML ? TRADE_XML_URL : TRADE_URL;
            string u = ((QuoteSource)source) == QuoteSource.XML ? url : $"{url}&lang=en-US";
            var content = await _connection.ReadUrlAsync($"{url}{instrument.InstrumentId}");
            if (((QuoteSource)source) == QuoteSource.XML)
            {
                var q = XmlParser.CreateQuote3MessageByXML(content, instrument);
                if (q == null)
                    _connection.AddException();
                return new List<Quote3Message>() { q };
            }
            var quote =  WebParser.CreateQuote3MessageByWeb(content, instrument);
            if (quote.Count == 0)
                _connection.AddException();
            
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

        public List<string> GetDataTypes() => Enum.GetNames(typeof(QuoteSource)).ToList();

        public void Dispose()
        {
            WebConnection.Instance.OnProxiesLoaded -= Populate;
        }
    }
}
