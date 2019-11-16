using Interfaces;
using Interfaces.MainClasses;
using Interfaces.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebMoneyVendor
{
    public class WebmoneyVendor : IVendor
    {
        #region Properties
        private static readonly string _tradeUrl = "https://wm.exchanger.ru/asp/wmlist.asp?exchtype=";
        private static readonly string _tradeXMLUrl = "https://wm.exchanger.ru/asp/XMLwmlist.asp?exchtype=";
        private static readonly string _bestRates = "https://wm.exchanger.ru/asp/XMLbestRates.asp";
        private ILocalCache _cache;
        #endregion



        public WebmoneyVendor()
        {
            _cache = new DataCache();
        }

        public bool CreateAccount(string id, string pass)
        {
            throw new NotImplementedException();
        }

        public List<IInstrument> GetAllInstruments() => _cache.Instruments;


        public Task<IResult<List<IAsset>>> GetAssetsAsync(IAccount account)
        {
            throw new NotImplementedException();
        }

        public IInstrument GetInstrumentById(string id) => _cache.Instruments.Where(i => i.InstrumentId == id).FirstOrDefault();


        public List<IOrder> GetLevel2(IInstrument instrument) => _cache.GetOrders(instrument);
       

        public Task<IResult<IOrder>> GetOrderById(string id)
        {
            throw new NotImplementedException();
        }

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
