using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace TradeTerminal
{
    internal class PropertyHelper
    {
        const string FILE_NAME_PROPGRAMM_INI = "program.ini";
        public static bool SaveProperties(Interfaces.Properties properties)
        {
            try
            {
                if (File.Exists(FILE_NAME_PROPGRAMM_INI))
                    File.Delete(FILE_NAME_PROPGRAMM_INI);
                string toSave = PropertyHelper.SerializeToJson(properties);
                if (string.IsNullOrEmpty(toSave))
                    return false;
                File.WriteAllText(FILE_NAME_PROPGRAMM_INI, toSave);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            

        }

        public static Interfaces.Properties ReadProperties()
        {
            try
            {
                if (!File.Exists(FILE_NAME_PROPGRAMM_INI))
                    return null;
                string text = File.ReadAllText(FILE_NAME_PROPGRAMM_INI);
                var prop = PropertyHelper.DeserializeFromJson<Interfaces.Properties>(text);
                return prop;
            }
            catch (Exception ex)
            {
                return null;
            }          
        }

        public static string SerializeToJson(object obj)
        {
            var serializer = new JavaScriptSerializer();
            try
            {
                return serializer.Serialize(obj);
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
        public static T DeserializeFromJson<T>(string str)
        {
            var serializer = new JavaScriptSerializer();
            try
            {
                return serializer.Deserialize<T>(str);
            }
            catch (Exception)
            {
                return default;
            }
        }
    }
}
