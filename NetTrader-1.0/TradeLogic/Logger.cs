using System;
using System.Collections.Generic;
using System.Text;

namespace Interfaces
{
    internal class Logger : ILog
    {
        public bool Save(string str, LogType type)
        {
            throw new NotImplementedException();
        }

        public bool Save(Exception ex, string comment = null)
        {
            throw new NotImplementedException();
        }
    }
}
