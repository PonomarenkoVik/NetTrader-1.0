using System;
using System.Collections.Generic;
using System.Text;

namespace Interfaces
{
    public class Subscription
    {
        public Subscription(IInstrument instr, SubscriptionType subscriptionType)
        {
            Instrument = instr;
            SubscriptionType = subscriptionType;
        }

        public IInstrument Instrument { get; }
        public SubscriptionType SubscriptionType { get; }
    }
}
