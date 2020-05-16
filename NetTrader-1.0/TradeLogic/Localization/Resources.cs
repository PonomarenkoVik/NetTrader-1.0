using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TradeLogic.Localization
{
    public static class Resources
    {
        private const string DIRECTORY = "Localization";
        private const string EXTENSION = ".txt";
        private const string DEFAULT_LOCALIZATION = "RU";
        private const string SELECTOR = "=";
        static Resources()
        { 
            if (!Directory.Exists(DIRECTORY))
                return;
            _localizations = new Dictionary<string, Dictionary<string, string>>();

            var files = Directory.GetFiles(DIRECTORY).Where((f) => f.EndsWith(EXTENSION));
            foreach (var file in files)
            {
                AddLocalization(file);
            }
        }


        public static string GetResources(string key, string loc = null)
        {
            loc = loc ?? DEFAULT_LOCALIZATION;

            if (_localizations.TryGetValue(loc, out Dictionary<string, string> localization))
                if (localization.TryGetValue(key, out string res))
                    return res;

            return key;            
        }


        private static void AddLocalization(string file)
        {
            Dictionary<string, string> loc = new Dictionary<string, string>();
            var lines = File.ReadAllLines(file, Encoding.UTF8);
            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line) || !line.Contains(SELECTOR))
                    continue;

                int index = line.IndexOf(SELECTOR);
                string key = line.Substring(0, index);
                string val = line.Substring(index + 1, line.Length - (index + 1));
                loc.Add(key, val);
            }
            _localizations.Add(Path.GetFileName(file).Replace(EXTENSION, string.Empty), loc);
        }

        private static Dictionary<string, Dictionary<string, string>> _localizations;

    }
}
