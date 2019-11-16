using Interfaces;
using Interfaces.Messages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using WebMoneyVendor.Cache;

namespace WebMoneyVendor
{
    public static class XmlParser
    {
        #region Properties
        const string BANKRATE = "BankRate";
        const string DIRECTION = "direction";
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
        #endregion

        public static IOrder CreateOrderByXml(XmlNode node)
        {
            if (node == null && node.Attributes.Count != ORDER_ATTRIBUTE_NUMBER)
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
                par.AllAmount = double.Parse(node.Attributes.GetNamedItem(ALLAMOUNT).Value);
                par.ApplicationDate = DateTime.Parse(node.Attributes.GetNamedItem(QUERYDATE).Value);
                return new Order(par);
            }
            catch (Exception){}
            return null;
        }

        public static BankRate CreateBankRateByXml(XmlNode node)
        {
            if (node == null && node.Attributes.Count != BANKRATE_ATTRIBUTE_NUMBER)
                return BankRate.Empty;
            try
            {
                double bankRate = double.Parse(node.ChildNodes[0].Value);
                var direction = node.Attributes.GetNamedItem(DIRECTION).Value;
                var rateType = node.Attributes.GetNamedItem(RATETYPE).Value;
                return new BankRate(direction, rateType, bankRate);
            }
            catch (Exception) { }
            return BankRate.Empty;
        }



        public static List<IOrder> CreateOrdersByXML(XmlNodeList nodeList)
        {
            if (nodeList == null || nodeList.Count == 0)
                return new List<IOrder>();

            List<IOrder> orders = new List<IOrder>();
            foreach (XmlNode node in nodeList)
                orders.Add(CreateOrderByXml(node));

            return orders;
        }

      

        public static Quote3Message CreateQuote3MessageByXML(string xml)
        {
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.LoadXml(xml);
                var bankRate = CreateBankRateByXml(doc.GetElementsByTagName(BANKRATE)[0]);
                var orders = CreateOrdersByXML(doc.GetElementsByTagName(WMEXCHANGERQUERYS)[0].ChildNodes);
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
    }
}
