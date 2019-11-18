using Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WebMoneyVendor
{
    internal class WebmoneyAccount : IAccount
    {
        public string Login { get; }

        public string Id { get;}

        public string Password { get; }

        public IVendor Vendor { get; }

        public WebmoneyAccount(string login, string id, string pass, IVendor vend)
        {
            Login = login;
            Id = id;
            Password = pass;
            Vendor = vend;
        }

        public async Task<IResult<List<IAsset>>> GetAssetsAsync() => await Vendor.GetAssetsAsync(this);


        public Task<IResult<IOrder>> GetOrderById(string id) => throw new NotImplementedException();
        

        public async Task<IResult<List<IOrder>>> GetOrders() => await Vendor.GetOrdersByAccount(this);
       
    }
}
