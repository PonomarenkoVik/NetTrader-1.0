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
        List<IOrder> GetLevel2(IInstrument instrument);
        Task<IResult<List<IAsset>>> GetAssetsAsync(IAccount account);
        Task<IResult<List<IOrder>>> GetOrdersByAccount(IAccount account);
        Task<IResult<IOrder>> GetOrderById(string id);
    }
}
