using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Interfaces.Messages
{
    public class Quote3Message : Message
    {

        public override MessageType Type => MessageType.QUOTE3;
        
        public string Direction { get; }

        public DateTime LastUpdateDate { get; }
        public BankRate BankRate { get; }

        public List<IOrder> Orders { get; }

        public Quote3Message(string dir, BankRate bankRate, List<IOrder> orders)
        {
            Direction = dir;
            BankRate = bankRate;
            Orders = orders;
            var dates = orders.Select(o => o.LastUpdateDate).ToList();
            dates.Sort();
            LastUpdateDate = dates.Last();
        }


        internal bool IsEqualQuotes(Quote3Message quote)
        {
            if (quote == null || Orders.Count != quote.Orders.Count || LastUpdateDate != quote.LastUpdateDate)
                return false;

            return true;
        }
    }
}
