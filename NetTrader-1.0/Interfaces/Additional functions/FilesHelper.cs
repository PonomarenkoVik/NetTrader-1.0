using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Interfaces.AdditionalFunctions
{
    public static class FilesHelper
    {
        #region Public
        public static bool CheckDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                return false;
            }
            return true;
        }

        public static bool WriteAllText(string path, string content, object sync)
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

        public static List<string> ReadAllLines(string path, object sync = null)
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

        public static bool WriteAllLines(string path, List<string> content, object sync)
        {
            if (sync != null)
            {
                lock (sync)
                {
                    return WriteAllLines(path, content);
                }
            }
            return WriteAllLines(path, content);
        }

        public static bool CheckFile(string path)
        {
            if (!File.Exists(path))
            {
                WriteAllText(path, string.Empty);
                return false;
            }
            return true;
        }
        #endregion


        #region Private
        private static bool WriteAllLines(string path, List<string> content)
        {
            try
            {
                File.WriteAllLines(path, content);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
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

        #endregion
    }
}
