using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using WebMoneyVendor;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            ProxyConnection proxy = ProxyConnection.Instance;
            Thread.Sleep(1000);
            Thread.Sleep(1000);
            Thread.Sleep(1000);
            Thread.Sleep(1000);
            Thread.Sleep(1000);
            Thread.Sleep(1000);

            var res = proxy.ReadUrlAsync("https://wm.exchanger.ru/asp/XMLwmlist.asp?exchtype=1");
            var u = res.Result;

            var mess = XmlParser.CreateQuote3MessageByXML(u);

            Console.ReadKey();
        }
    }
}
