using System;
using System.Collections.Generic;
using System.Text;

namespace Interfaces.Messages
{
    public abstract class Message
    {
        public abstract MessageType Type { get; }

        public abstract IAccount Account { get; }
    }
}
