using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface ITradeLogic
    {
        bool AddAccount(string vendorName, string id, string pass);
        bool RemoveAccount(string vendorName, string id);       
        List<IVendor> GetVendors();
    }
}
