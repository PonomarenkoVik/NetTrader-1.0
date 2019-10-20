using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface IAccount
    {
        string Login { get; }
        string Id { get; }
        string Password { get; }
        Task<IResult<List<IAsset>>> GetAssetsAsync();
        Task<IResult<List<IOrder>>> GetOrders();
        Task<IResult<IOrder>> GetOrderById(string id);
        IVendor Vendor { get; }
    }
}
