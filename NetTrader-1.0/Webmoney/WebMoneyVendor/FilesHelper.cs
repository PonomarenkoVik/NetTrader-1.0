using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace WebMoneyVendor
{
    internal static class FilesHelper
    {
        internal static bool CheckDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                return false;
            }
            return true;
        }


        internal static bool WriteAllText(string path, string content, object sync)
        {
            if (sync != null)
            {
                lock (sync)
                {
                    return WriteAllText(path, content);
                }
            }
            return WriteAllText(path, content);
        }

        internal static List<string> ReadAllLines(string path, object sync = null)
        {
            if (sync != null)
            {
                lock (sync)
                {
                    return File.ReadAllLines(path).ToList();
                }
            }
            return File.ReadAllLines(path).ToList();
        }

        private static bool WriteAllText(string path, string content)
        {
            try
            {
                File.WriteAllText(path, content);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        internal static bool CheckFile(string path)
        {
            if (!File.Exists(path))
            {
                WriteAllText(path, string.Empty);
                return false;
            }
            return true;
        }
    }
}
