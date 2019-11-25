using System;
using System.Collections.Generic;
using System.Text;

namespace Interfaces
{
    public class Properties
    {
        public Properties(string name, object value = null)
        {
            Id = name;
            Value = value;
        }
        public string Id { get; }
        public object Value { get; }
        public Dictionary<string, Properties> InsideProperties { get; } = new Dictionary<string, Properties>();
    }
}
