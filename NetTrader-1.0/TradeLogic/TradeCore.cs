using Interfaces;
using Interfaces.MainClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebMoneyVendor;

namespace TradeLogic
{
    public class TradeCore : ITradeLogic
    {
        #region Properties
        private Dictionary<string, IVendor> _vendors;
        const string PROPERTY_VENDORS = "vendors";
        public string PropertyId => "tradelogic";

        public Properties Properties
        {
            get
            {
                var properties = new Properties(PropertyId);
                // Vendors
                if (_vendors.Count > 0)
                {
                    var vprops = new Properties(PROPERTY_VENDORS);
                    foreach (var v in _vendors.Values)
                    {
                        vprops.InsideProperties.Add(v.PropertyId, v.Properties);
                    }
                }
                // Vendors
                return properties;
            }
            set
            {
                if (value.InsideProperties.ContainsKey(PROPERTY_VENDORS) && _vendors.Count > 0)
                {
                    var vprops = value.InsideProperties[PROPERTY_VENDORS];
                    foreach (var p in vprops.InsideProperties)
                    {
                        var v = _vendors.Values.Where(x => x.PropertyId == p.Value.Id).ToList().FirstOrDefault();
                        if (v != null)
                            v.Properties = p.Value;
                    }
                }
            }
        }
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
