using System;
using System.Collections.Generic;
using System.Text;

namespace Interfaces
{
    public enum LogType { Trading, Exception, System, Info};

    public interface ILog
    {
        bool Save(string str, LogType type);
    }
}
