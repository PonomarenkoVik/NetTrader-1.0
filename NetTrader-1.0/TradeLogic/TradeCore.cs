using Interfaces;
using Interfaces.MainClasses;
using System;
using System.Collections.Generic;
using System.Text;
using WebMoneyVendor;

namespace TradeLogic
{
    internal class TradeCore : ITradeLogic
    {
        #region Properties
        private Dictionary<string, IVendor> _vendors;
        #endregion

        public TradeCore()
        {
            PopulateVendors();
        }

        private void PopulateVendors()
        {
            var webmoney = new WebmoneyVendor(new DataCache(), new Logger());
            _vendors = new Dictionary<string, IVendor>
            {
                { webmoney.VendorName, webmoney}
            };
        }

        public IVendor GetVendorById(string id)
        {
            if (_vendors.TryGetValue(id, out IVendor vendor))
                return vendor;

            return null;
        }

        public Dictionary<string, IVendor> GetVendors() => _vendors;      
    }
}
