using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface ITradeLogic
    {
        Dictionary<string, IVendor> GetVendors();
        IVendor GetVendorById(string id);
    }
}
