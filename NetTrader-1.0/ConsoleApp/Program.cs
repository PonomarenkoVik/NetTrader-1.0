﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Interfaces;
using Interfaces.Messages;
using TradeLogic;
using WebMoneyVendor;
using WebMoneyVendor.Cache;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            TradeCore core = new TradeCore();
            var v = core.GetVendors().Values.FirstOrDefault();
         
            v.OnNewQuoteEvent += OnNewQuote;
           
            var instrs = v.GetAllInstruments().Values;
            int i = 0;
            Console.ReadKey();
            foreach (var inst in instrs)
            {
                i++;
                v.Subscribe(new Subscription( inst, SubscriptionType.TradingSlow));
                if (i == 1)
                {
                    break;
                }
            }
           

            Console.ReadKey();
        }

        private static void OnNewQuote(Quote3Message mess)
        {
           
        }
    }
}
