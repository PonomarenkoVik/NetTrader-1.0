﻿using Interfaces;
using Interfaces.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebMoneyVendor.Cache;

namespace WebMoneyVendor
{
    [Serializable]
    public class WebmoneyVendor : IVendor, IDisposable
    {
        #region Properties
        const string TRADE_URL = "https://wm.exchanger.ru/asp/wmlist.asp?exchtype=";
        const string TRADE_XML_URL = "https://wm.exchanger.ru/asp/XMLwmlist.asp?exchtype=";
        const string BEST_RATES = "https://wm.exchanger.ru/asp/XMLbestRates.asp";
        [NonSerialized]
        private ICache _cache;
        [NonSerialized]
        private QuoteProcessor _quoteProcessor;
        [NonSerialized]
        private WebConnection _connection;

        public event Action<Quote3Message> OnNewQuoteEvent;
        public event Action LoadedEvent;
        public string VendorName => "Wm.Exchanger.ru";

        public string PropertyId => "wmvendor";

        public bool CacheIsLoaded => _instrumentsIsLoaded;

        public Properties Properties
        {
            get
            {
                var props = new Properties(PropertyId);
                // Instruments
                if (_cache.Instruments.Count > 0)
                {
                    var instrProps = new Properties(WebmoneyInstrument.PROPERTY_INSTRUMENTS);
                    new List<IInstrument>(_cache.Instruments.Values).ForEach((i) => instrProps.InsideProperties.Add(i.InstrumentId, i.Properties));
                    props.InsideProperties.Add(WebmoneyInstrument.PROPERTY_INSTRUMENTS, instrProps);
                }

                // Instruments
                
                return props;
            }
            set
            {
                // Instruments
                if (value.Id != PropertyId)
                    return;

                if (value.InsideProperties.ContainsKey(WebmoneyInstrument.PROPERTY_INSTRUMENTS))
                {
                    SetInstrumentsByProperties(value.InsideProperties[WebmoneyInstrument.PROPERTY_INSTRUMENTS]);
                    if (_cache.Instruments.Count > 0)
                        _instrumentsIsLoaded = true;

                }
            }
        }

        private void SetInstrumentsByProperties(Properties instrsProp)
        {
            if (instrsProp == null || instrsProp.InsideProperties.Count == 0)
                return;


            foreach (var prop in instrsProp.InsideProperties)
            {
                try
                {
                    var i = new WebmoneyInstrument(this);
                    i.Properties = prop.Value;
                    _cache.AddInstrument(i);
                }
                catch (Exception ex)
                {

                }                
            }
        }

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
            _connection = new WebConnection();
            _connection.OnProxiesLoaded += PopulateAsync;
            _connection.InitializeAsync();
            UseProxy = true;
            _cache = cache;
            _quoteProcessor = new QuoteProcessor(this) { DataType = QuoteSource.Web};
            _quoteProcessor.OnQuoteEvent += _cache.AddQuote;
            _cache.OnNewQuoteEvent += OnNewQuote;
        }

        private void OnNewQuote(Quote3Message mess) => OnNewQuoteEvent?.Invoke(mess);


        private bool _instrumentsIsLoaded = false;
        private async void PopulateAsync()
        {
            var instruments = await GetInstrumentsAsync();
            lock (_cache)
            {
                _cache.Instruments.Clear();
                foreach (var instr in instruments)
                    _cache.AddInstrument(instr);
            }

            if (!_instrumentsIsLoaded)
                LoadedEvent?.Invoke();

            _instrumentsIsLoaded = true;

        }


        private async Task<List<IInstrument>> GetInstrumentsAsync()
        {
            string content;

            content = await _connection.ReadUrlAsync(BEST_RATES);

            var bestRates = XmlParser.GreateBestRatesByXML(content);
            var instruments = WebmoneyHelper.CreateInstruments(bestRates, this);
            return instruments;
        }

        private List<IInstrument> GetInstruments()
        {
            string content;

            content = _connection.ReadUrl(BEST_RATES);

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
            string u = ((QuoteSource)source) == QuoteSource.XML ? url : $"{url}";
            var content = await _connection.ReadUrlAsync($"{u}{instrument.InstrumentId}");
            if (((QuoteSource)source) == QuoteSource.XML)
            {
                var q = XmlParser.CreateQuote3MessageByXML(content, instrument);
                if (q == null)
                {
                    _connection.AddException();
                    return new List<Quote3Message>();
                }
                    
                return new List<Quote3Message>() { q };
            }
            var quotes =  WebParser.CreateQuote3MessageByWeb(content, instrument);
            if (quotes.Count == 0)
                _connection.AddException();
            
            return quotes;
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
            _connection.OnProxiesLoaded -= PopulateAsync;
            _connection.Dispose();
        }
    }
}
