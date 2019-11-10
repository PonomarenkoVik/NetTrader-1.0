using System;
using System.Collections.Generic;
using System.Text;

namespace Interfaces
{
    public class Logger : ILog
    {
        public Logger Instance = new Logger();

        private Logger() {}

        public bool Save(string str, LogType type)
        {
            throw new NotImplementedException();
        }
    }
}
