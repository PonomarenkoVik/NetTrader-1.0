using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using WebMoneyVendor.Cache;

namespace WebMoneyVendor
{
    public static class WebParser
    {
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


        public static List<BestRate> GreateBaseRatesByXML(string doc)
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
            catch (Exception){}
            return bestRates;
        }

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
                    int exchtype = int.Parse(node.Attributes.GetNamedItem(EXCHTYPE).Value);
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
            }
            return null;
        }
    }
}
