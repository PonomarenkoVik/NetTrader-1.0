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
    public static class WebParser
    {
        private static readonly byte OrderSymbolNumber = 8;
        private static readonly byte OrderPointNumber = 8;

        private static readonly string[] OrderSeparator = new string[] { "title=\"#" };
        private static readonly string[] OrderPointSeparator = new string[] { "align='right'>", "</td>", "<span>", "</span>" };
        private static readonly string[] OrderPointAllAmountOutsideSeparator = new string[] { "(*)" };
        private static readonly string[] OrderPointAllAmountInsideSeparator = new string[] { ";", ":" };
        private static readonly char[] FirstStringSeparator = new[] { ' ', ';', ':' };
        private static readonly string[] OrderExceptString = new string[] { "<td", "<tr", "</tr", "&", "%", "div", "class" };
        private static readonly int[] OrderIndexes = new int[] { 0, 1, 3, 7 };

        internal static List<Quote3Message> CreateQuote3MessageByWeb(string content, IInstrument instr)
        {
            List<IOrder> ordersStraight = new List<IOrder>();
            List<IOrder> ordersReverse = new List<IOrder>();
            string[] orderlines = content.Split(OrderSeparator, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in orderlines)
            {
                IOrder order = null;
                try
                {
                    order = CreateOrderByWebLine(line, instr);
                    
                }
                catch (Exception){}

                if (order == null)
                    continue;

                if (order.Instrument.InstrumentName == instr.InstrumentName)
                    ordersStraight.Add(order);

                if (order.Instrument.InstrumentName == instr.OppositeInstrumentName)
                    ordersReverse.Add(order);
            }

            var quotes = new List<Quote3Message>();

            if (ordersStraight.Count == 0 || ordersReverse.Count == 0)
                return quotes;

            Quote3Message messStr = new Quote3Message(instr.InstrumentName, BankRate.Empty, ordersStraight) { QuoteSource = QuoteSource.Web.ToString() };
            Quote3Message messRev = new Quote3Message(instr.OppositeInstrumentName, BankRate.Empty, ordersReverse) { QuoteSource = QuoteSource.Web.ToString() };
            if (messStr != null)
                quotes.Add(messStr);
            if (messRev != null)
                quotes.Add(messRev);

            return quotes;
        }

        private static Order CreateOrderByWebLine(string orderLine, IInstrument instr)
        {
            string orderId = String.Empty;
            string instrumentName = String.Empty;
            double straightCrossRate = double.NaN;
            double reverseCrossRate = double.NaN;
            DateTime applicationDate = DateTime.MinValue;
            double sum1 = double.NaN;
            double sum2 = double.NaN;

            List<string> orderPointlines = GetWebPageOrderPoints(orderLine);
            if (orderPointlines == null || orderPointlines.Count < OrderPointNumber)
                return null;
            orderId = orderPointlines[0];

            instrumentName = $"{orderPointlines[3]}/{orderPointlines[4]}";

            if (!double.TryParse(orderPointlines[2], out reverseCrossRate))
                return null;

            bool normalFormat = orderPointlines[5].Length == 19;
            string date = orderPointlines[normalFormat ? 5 : 8].Substring(0, 19);
            if (!DateTime.TryParseExact(date, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture,
                DateTimeStyles.AdjustToUniversal, out applicationDate))
                return null;
            if (!double.TryParse(orderPointlines[6], out sum1))
                return null;
            if (!double.TryParse(orderPointlines[7], out sum2))
                return null;
            if (!double.TryParse(orderPointlines[normalFormat ? 8 : 5], out straightCrossRate))
                return null;

            string[] allAmountStrings = orderLine.Split(OrderPointAllAmountOutsideSeparator, StringSplitOptions.RemoveEmptyEntries);
            string inAllAmountString = allAmountStrings[1].Split(OrderPointAllAmountInsideSeparator, StringSplitOptions.RemoveEmptyEntries)[0];
            string outAllAmountString = allAmountStrings[2].Split(OrderPointAllAmountInsideSeparator, StringSplitOptions.RemoveEmptyEntries)[0];
            double inAllAmount = double.Parse(inAllAmountString);
            double outAmount = double.Parse(outAllAmountString);

            IInstrument instrForQuote = null;
            if (instrumentName == instr.InstrumentName)
                instrForQuote = instr;

            if (instrumentName == instr.OppositeInstrumentName)
                instrForQuote = instr.Vendor.GetInstrumentByName(instr.OppositeInstrumentName);

            if (instrForQuote == null)
                return null;

            var param = new OrderParams()
            {
                OrderId = orderId,
                Instrument = instrForQuote,
                AmountIn = instrumentName == instr.InstrumentName ? sum1 : sum2,
                AmountOut = instrumentName == instr.InstrumentName ? sum2 : sum1,
                StraitCrossrate = instrumentName == instr.InstrumentName ? straightCrossRate : reverseCrossRate,
                ReverseCrossrate = instrumentName == instr.InstrumentName ? reverseCrossRate : straightCrossRate,
                ProcentBankRate = double.NaN,
                AllAmountOut = instrumentName == instr.InstrumentName ? outAmount : inAllAmount,
                AllAmountIn = instrumentName == instr.InstrumentName ? inAllAmount: outAmount,
                ApplicationDate = applicationDate
           };

            return new Order(param);
        }


        private static List<string> GetWebPageOrderPoints(string line)
        {
            List<string> orderPointlines = new List<string>();
            if (line.Length < OrderSymbolNumber)
                return null;

            var num = line.Substring(0, OrderSymbolNumber);
            if (!int.TryParse(num, out int number))
                return null;

            orderPointlines.Add(num);

            string orderline = line.Substring(OrderSymbolNumber, line.Length - OrderSymbolNumber);
            var tempStrings = orderline.Split(OrderPointSeparator, StringSplitOptions.RemoveEmptyEntries);

            string[] firstStringSplit =
                tempStrings[0].Split(FirstStringSeparator, StringSplitOptions.RemoveEmptyEntries);
            foreach (var orderIndex in OrderIndexes)
            {
                if (orderIndex < firstStringSplit.Length)
                    orderPointlines.Add(firstStringSplit[orderIndex]);
            }

            foreach (var tempString in tempStrings)
            {
                bool isPointOrder = true;
                foreach (var exceptString in OrderExceptString)
                {
                    if (tempString.Contains(exceptString))
                    {
                        isPointOrder = false;
                        break;
                    }
                }

                if (isPointOrder)
                    orderPointlines.Add(tempString);
            }

            return orderPointlines;
        }
    }
}
