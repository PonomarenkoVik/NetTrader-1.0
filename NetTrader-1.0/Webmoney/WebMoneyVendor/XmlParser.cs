using Interfaces;
using Interfaces.Messages;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml;
using WebMoneyVendor.Cache;

namespace WebMoneyVendor
{
    public static class XmlParser
    {
        #region Properties
        //Quote
        const string BANKRATE = "BankRate";
        const string RATETYPE = "ratetype";
        const string WMEXCHANGERQUERYS = "WMExchnagerQuerys";
        const string ID = "id";
        const string AMOUNTIN = "amountin";
        const string AMOUNTOUT = "amountout";
        const string INOUTRATE = "inoutrate";
        const string OUTINRATE = "outinrate";
        const string PROCENTBANKRATE = "procentbankrate";
        const string ALLAMOUNT = "allamountin";
        const string QUERYDATE = "querydate";
        const int ORDER_ATTRIBUTE_NUMBER = 8;
        const int BANKRATE_ATTRIBUTE_NUMBER = 2;
        //Quote

        //Best rates
        const string RESPONSE = "response";
        const char INSTRUMENT_NAME_SELECTOR = '-';
        const string BASERATE = "BaseRate";
        const string DIRECT = "Direct";
        const string EXCHTYPE = "exchtype";
        const string PERSENTAGE_IDENT = "Plus0";
        const string VOLUME_IDENT = "Plus";
        const int MAX_INDEX_PERCENTRATE = 9;
        const int ATTRIBUTE_NUMBER = 17;
        readonly static int[] MAX_INDEX_VOLUME = { 1, 2, 3, 5, 10 };
        //Best rates
        #endregion

        public static IOrder CreateOrderByXml(XmlNode node, IInstrument instr)
        {
            if (node == null && node.Attributes.Count != ORDER_ATTRIBUTE_NUMBER )
                return null;
            try
            {
                OrderParams par = new OrderParams();
                par.OrderId = node.Attributes.GetNamedItem(ID).Value;
                par.AmountIn = double.Parse(node.Attributes.GetNamedItem(AMOUNTIN).Value);
                par.AmountOut = double.Parse(node.Attributes.GetNamedItem(AMOUNTOUT).Value);
                par.StraitCrossrate = double.Parse(node.Attributes.GetNamedItem(INOUTRATE).Value);
                par.ReverseCrossrate = double.Parse(node.Attributes.GetNamedItem(OUTINRATE).Value);
                par.ProcentBankRate = double.Parse(node.Attributes.GetNamedItem(PROCENTBANKRATE).Value);
                par.AllAmountOut = double.Parse(node.Attributes.GetNamedItem(ALLAMOUNT).Value);
                par.ApplicationDate = DateTime.ParseExact(node.Attributes.GetNamedItem(QUERYDATE).Value, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                par.Instrument = instr;
                return new Order(par);
            }
            catch (Exception){}
            return null;
        }

        public static BankRate CreateBankRateByXml(XmlDocument doc)
        {
            try
            {
                var node = doc.GetElementsByTagName(BANKRATE)[0];
                var exch = doc.GetElementsByTagName(WMEXCHANGERQUERYS)[0];
                if (node.Attributes.Count != BANKRATE_ATTRIBUTE_NUMBER)
                    return BankRate.Empty;
           
                double bankRate = double.Parse(node.ChildNodes[0].Value);
                var amountIn = exch.Attributes.GetNamedItem(AMOUNTIN).Value;
                var amountOut = exch.Attributes.GetNamedItem(AMOUNTOUT).Value;
                var rateType = node.Attributes.GetNamedItem(RATETYPE).Value;
                return new BankRate($"{amountIn}/{amountOut}", rateType, bankRate);
            }
            catch (Exception) { }
            return BankRate.Empty;
        }

        public static List<IOrder> CreateOrdersByXML(XmlNodeList nodeList, IInstrument instr)
        {
            if (nodeList == null || nodeList.Count == 0)
                return new List<IOrder>();

            List<IOrder> orders = new List<IOrder>();
            foreach (XmlNode node in nodeList)
                orders.Add(CreateOrderByXml(node, instr));

            return orders;
        }
    
        public static Quote3Message CreateQuote3MessageByXML(string xml, IInstrument instr)
        {
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.LoadXml(xml);
                var bankRate = CreateBankRateByXml(doc);
                var orders = CreateOrdersByXML(doc.GetElementsByTagName(WMEXCHANGERQUERYS)[0].ChildNodes, instr);
                if (bankRate == BankRate.Empty || orders.Count == 0)
                    return null;

                Quote3Message mess = new Quote3Message(bankRate.Direction, bankRate, orders);
                return mess;

            }
            catch (Exception)
            {
                return null;
            }
        }

        public static List<BestRate> GreateBestRatesByXML(string doc)
        {
            List<BestRate> bestRates = new List<BestRate>();
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(doc);
                var resp = xmlDoc.GetElementsByTagName("response")[0];
                foreach (XmlNode node in resp.ChildNodes)
                {
                    var bestRate = GreateBaseRateByXML(node);
                    if (bestRate != null)
                        bestRates.Add(bestRate);

                }
            }
            catch (Exception) { }
            return bestRates;
        }

        #region Private

        private static BestRate GreateBaseRateByXML(XmlNode node)
        {
            try
            {
                if (node != null && node.Attributes.Count == ATTRIBUTE_NUMBER)
                {
                    double bestRate = double.Parse(node.Attributes.GetNamedItem(BASERATE).Value.Replace(".", ","));
                    string direct = node.Attributes.GetNamedItem(DIRECT).Value;
                    var splits = direct.Split(INSTRUMENT_NAME_SELECTOR);
                    string instrName = $"{splits[0].Trim()}{WebmoneyInstrument.INSTRUMENT_NAME_SELECTOR}{splits[1].Trim()}";
                    string exchtype = node.Attributes.GetNamedItem(EXCHTYPE).Value;
                    Dictionary<int, double> persentages = new Dictionary<int, double>();
                    for (int i = 1; i <= MAX_INDEX_PERCENTRATE; i++)
                    {
                        persentages.Add(i, double.Parse(node.Attributes.GetNamedItem($"{PERSENTAGE_IDENT}{i}").Value.Replace(".", ",")));
                    }
                    Dictionary<int, double> volumes = new Dictionary<int, double>();
                    foreach (var index in MAX_INDEX_VOLUME)
                    {
                        volumes.Add(index, double.Parse(node.Attributes.GetNamedItem($"{VOLUME_IDENT}{index}").Value.Replace(".", ",")));
                    }
                    return new BestRate(instrName, exchtype, bestRate, persentages, volumes);
                }
            }
            catch (Exception ex)
            {
                //Logger.Instance.Save(ex);
            }
            return null;
        }
        #endregion
    }
}
