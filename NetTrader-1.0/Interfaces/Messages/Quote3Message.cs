using System;
using System.Collections.Generic;
using System.Text;

namespace Interfaces.Messages
{
    public class Quote3Message : Message
    {

        public override MessageType Type => MessageType.QUOTE3;
        
        public string Direction { get; }

        public BankRate BankRate { get; }

        public List<IOrder> Orders { get; }

        public Quote3Message(string dir, BankRate bankRate, List<IOrder> orders)
        {
            Direction = dir;
            BankRate = bankRate;
            Orders = orders;
        }
    }
}
