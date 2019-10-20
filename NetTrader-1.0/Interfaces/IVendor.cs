using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface IVendor
    {
        Task<IResult<IAccount>> CreateAccount(string id, string pass);
        Task<IResult<List<IInstrument>>> GetAllInstrument();
        Task<IResult<IInstrument>> GetInstrumentById(string id);
        Task<IResult<IOrder>> CreateOrder(IAccount acc);
        Task<IResult<IEnumerable<IOrder>>> GetLevel2Async(IInstrument instrument, int sourceType);
        Task<IResult<List<IAsset>>> GetAssetsAsync(IAccount account);
        Task<IResult<List<IOrder>>> GetOrders(IAccount account);
        Task<IResult<IOrder>> GetOrderById(string id, IAccount account);
    }
}
