using System;
using System.Collections.Generic;
using System.Text;

namespace Interfaces
{
    public enum LogType { Exception, Trading, Info };

    public interface ILog
    {
        bool Save(string str, LogType type);

        bool Save(Exception ex, string comment = null);
    }
}
