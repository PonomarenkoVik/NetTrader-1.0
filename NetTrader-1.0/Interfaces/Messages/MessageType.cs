using System;
using System.Collections.Generic;
using System.Text;

namespace Interfaces.Messages
{
    public enum MessageType
    {
        CreateMarketOrder,
        CreateLimitOrder,
        CreateMarketLimit,
        QUOTE3
    }
}
