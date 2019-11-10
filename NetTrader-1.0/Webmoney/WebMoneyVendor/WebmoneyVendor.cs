using Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WebMoneyVendor
{
    public class WebmoneyVendor : IVendor
    {
        public Task<IResult<IAccount>> CreateAccount(string id, string pass)
        {
            throw new NotImplementedException();
        }

        public Task<IResult<IOrder>> CreateOrder(IAccount acc)
        {
            throw new NotImplementedException();
        }

        public Task<IResult<List<IInstrument>>> GetAllInstruments()
        {
            throw new NotImplementedException();
        }

        public Task<IResult<List<IAsset>>> GetAssetsAsync(IAccount account)
        {
            throw new NotImplementedException();
        }

        public Task<IResult<IInstrument>> GetInstrumentById(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IResult<IEnumerable<IOrder>>> GetLevel2Async(IInstrument instrument, int sourceType)
        {
            throw new NotImplementedException();
        }

        public Task<IResult<IOrder>> GetOrderById(string id, IAccount account)
        {
            throw new NotImplementedException();
        }

        public Task<IResult<List<IOrder>>> GetOrders(IAccount account)
        {
            throw new NotImplementedException();
        }
    }
}
