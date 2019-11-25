using System;
using System.Collections.Generic;
using System.Text;

namespace Interfaces
{
    public interface IProperties
    {
        string PropertyId { get; }
        Properties Properties{ get; set; }
    }
}
