using System;
using System.Collections.Generic;
using System.Text;

namespace Interfaces
{
    [Serializable]
    public class Properties
    {
        public Properties(string name, object value = null)
        {
            Id = name;
            Value = value;
        }

        public Properties()
        {

        }

        public string Id { get; set; }
        public object Value { get; set; }
        public Dictionary<string, Properties> InsideProperties { get; set; } = new Dictionary<string, Properties>();
    }
}
