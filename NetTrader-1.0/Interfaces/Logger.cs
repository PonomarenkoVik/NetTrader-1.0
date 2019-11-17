using System;
using System.Collections.Generic;
using System.Text;

namespace Interfaces
{
    public class Logger
    {
        public enum LogType {Exception, Trading, Info };

        public static Logger Instance = new Logger();

        private Logger() {}

        public bool Save(string str, LogType type)
        {
            throw new NotImplementedException();
        }

        public bool Save(Exception ex, string commet = null)
        {
            throw new NotImplementedException();
        }
    }
}
