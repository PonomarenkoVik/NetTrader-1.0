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
            WebmoneyVendor proxy = new WebmoneyVendor(true);
            Thread.Sleep(1000);
            Thread.Sleep(1000);
            Thread.Sleep(1000);
            Thread.Sleep(1000);
            Thread.Sleep(1000);
            Thread.Sleep(1000);
            var instrs = proxy.GetAllInstruments();
            proxy.Subscribe(instrs.Values.First());

            Console.ReadKey();
        }
    }
}
