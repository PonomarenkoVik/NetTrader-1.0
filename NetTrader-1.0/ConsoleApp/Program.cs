using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using WebMoneyVendor;
using WebMoneyVendor.Cache;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            WebConnection proxy = WebConnection.Instance;
            Thread.Sleep(1000);
            Thread.Sleep(1000);
            Thread.Sleep(1000);
            Thread.Sleep(1000);
            Thread.Sleep(1000);
            Thread.Sleep(1000);

            var res = proxy.ReadUrlAsync("https://wm.exchanger.ru/asp/XMLbestRates.asp");
            var u = res.Result;

            var resp = WebParser.GreateBaseRatesByXML(u);

            var instrs = WebmoneyInstrument.CreateInstruments(resp, null);
           

            Console.ReadKey();
        }
    }
}
