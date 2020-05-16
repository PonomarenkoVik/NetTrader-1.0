using Interfaces.AdditionalFunctions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WebMoneyVendor
{
    internal class WebConnection : IDisposable
    {
        private class ProxyURL : IComparable
        { 
            public static ProxyURL Empty => new ProxyURL(string.Empty, -1);
            public ProxyURL(string ip, int port)
            {
                IP = ip;
                Port = port;
                Exception = 0;
            }

            public int Exception { get; set; }

            public string IP { get; }
            public int Port { get; }

            public static bool operator ==(ProxyURL prUrl1, ProxyURL prUrl2)
            {
                if (prUrl1 is null && prUrl2 is null)
                    return true;

                if (prUrl1 is null || prUrl2 is null)
                    return false;

                return prUrl1.IP == prUrl2.IP && prUrl1.Port == prUrl2.Port;
            }
            
            public static bool operator !=(ProxyURL prUrl1, ProxyURL prUrl2) => !(prUrl1 == prUrl2);
           
            public override int GetHashCode() => IP.GetHashCode() + Port;

            public override bool Equals(object obj) => obj != null && obj is ProxyURL pr && this == pr;


            public int CompareTo(object obj)
            {
                var t2 = (ProxyURL)obj;
                if (Exception > t2.Exception)
                    return 1;
                if (Exception < t2.Exception)
                    return -1;
                return 0;
            }
        }

        #region Properties
        const string HOSTS_DIRECTORY_NAME = "ProxyHosts";
        const int MAX_PORT = 65535;
        const int MIN_PORT = 1023;
        const string HOSTS_FILE_NAME = "hosts.txt";
        const string SELECTOR = ":";
        private static string HOSTS_FILE_PATH => Path.Combine(HOSTS_DIRECTORY_NAME, HOSTS_FILE_NAME);
        private object _syncHosts = new object();
        private static List<ProxyURL> _proxyURLs = new List<ProxyURL>();

        public bool UseProxy { get; set; } = false;

        public event Action OnProxiesLoaded;
        #endregion

        internal WebConnection()
        {
            
        }

        public void InitializeAsync() => Task.Factory.StartNew(InitializeProxyHosts);

        #region Public

        public async Task<string> ReadUrlAsync(string url)
        {
            return UseProxy ? await ReadUrlWithProxyAsync(url) : await ReadUrlWithOutPxoxyAsync(url);
        }

        private async Task<string> ReadUrlWithOutPxoxyAsync(string url)
        {
            try
            {
                using (var client = new WebClient())
                {
                    return await client.DownloadStringTaskAsync(url);
                }               
            }
            catch (Exception)
            {
                return null;
            }
        }

        private async Task<string> ReadUrlWithProxyAsync(string url)
        {

            WebClient cl = null;
            try
            {
                var proxy = new WebProxy(_currentProxy.IP, _currentProxy.Port);
                cl = new WebClient() { Proxy = proxy  };
                var res = await cl.DownloadStringTaskAsync(url);
                return res;
            }
            catch (Exception)
            {
                _currentProxy.Exception += 1;
                UpdateProxy();
                return await ReadUrlWithProxyAsync(url);
            }
            finally
            {
                cl?.Dispose();
            }
        }

        public void Dispose()
        {
            
        }
        #endregion

        #region Private
        private void InitializeProxyHosts()
        {

            var addresses = GetAdresses();
            if (addresses.Count == 0)
            {
                throw new Exception($"Set proxies \"{HOSTS_FILE_PATH}\"");
            }

            foreach (var adr in addresses)
            {

                if (!string.IsNullOrEmpty(adr) && TryParseAddress(adr, out ProxyURL prUrl))
                {
                    _proxyURLs.Add(prUrl);
                }
            }
            _currentProxy = _proxyURLs.First();
            OnProxiesLoaded?.Invoke();
        }

        private static List<string> GetAdresses()
        {
            List<string> urls = new List<string>();
            if (FilesHelper.CheckDirectory(HOSTS_DIRECTORY_NAME) && FilesHelper.CheckFile(HOSTS_FILE_PATH))
            {
                var lines = FilesHelper.ReadAllLines(HOSTS_FILE_PATH);
                if (lines.Count == 0)
                    return urls;

                foreach (var line in lines)
                {
                    if (!urls.Contains(line))
                        urls.Add(line);
                }
            }
            return urls;
        }

        private ProxyURL _currentProxy;

        private void UpdateProxy()
        {
            if (_proxyURLs.Count > 0)
            {
                lock (_syncHosts)
                {
                    _proxyURLs.Sort();
                    _currentProxy = _proxyURLs.First();
                }
            }
        }

      

        internal void AddException()
        {
            _currentProxy.Exception++;
            UpdateProxy();
        }

        private static bool TryParseAddress(string adr, out ProxyURL prUrl)
        {
            if (!string.IsNullOrEmpty(adr) && adr.Contains(SELECTOR))
            {
                var list = adr.Split(SELECTOR[0]);
                int.TryParse(list[1], out int port);
                if (port > MIN_PORT && port < MAX_PORT)
                {
                    prUrl = new ProxyURL(list[0], port);
                    return true;
                }
                prUrl = ProxyURL.Empty;
                return false;
            }
            prUrl = ProxyURL.Empty;
            return false;
        }

        #endregion
    }
}
