using Interfaces.Messages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface IVendor
    {
        bool CreateAccount(string id, string pass);
        List<IInstrument> GetAllInstruments();
        IInstrument GetInstrumentById(string id);
        Task<IResult<T>> SendMessageToVendor<T>(Message mess);
        Task<IResult<IEnumerable<IOrder>>> GetLevel2Async(IInstrument instrument, int sourceType = -1);
        Task<IResult<List<IAsset>>> GetAssetsAsync(IAccount account);
        Task<IResult<List<IOrder>>> GetOrders(IAccount account);
        Task<IResult<IOrder>> GetOrderById(string id, IAccount account);
    }
}
