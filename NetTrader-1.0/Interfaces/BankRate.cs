using System;
using System.Collections.Generic;
using System.Text;

namespace Interfaces
{
    public struct BankRate
    {
        public BankRate(string direction, string rateType, double rate)
        {
            Direction = direction;
            RateType = rateType;
            Rate = rate;
        }

        public static BankRate Empty => new BankRate(string.Empty, string.Empty, -1);

        public string Direction { get; }
        public string RateType { get; }
        public double Rate { get; }

        public static bool operator ==(BankRate br1, BankRate br2) => br1.Direction == br2.Direction && br1.RateType == br2.RateType && br1.Rate == br2.Rate;

        public static bool operator !=(BankRate prUrl1, BankRate prUrl2) => !(prUrl1 == prUrl2);

        public override int GetHashCode() => Direction.GetHashCode() + RateType.GetHashCode() + Rate.GetHashCode();

        public override bool Equals(object obj) => obj != null && obj is BankRate pr && this == pr;
    }
}
