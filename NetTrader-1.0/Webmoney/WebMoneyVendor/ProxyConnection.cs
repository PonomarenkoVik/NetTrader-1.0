﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WebMoneyVendor
{
    internal class ProxyConnection : IDisposable
    {
        private struct ProxyURL
        {
            public static ProxyURL Empty => new ProxyURL(string.Empty, -1);
            public ProxyURL(string ip, int port)
            {
                IP = ip;
                Port = port;
            }

            public string IP { get; }
            public int Port { get; }

            public static bool operator ==(ProxyURL prUrl1, ProxyURL prUrl2) => prUrl1.IP == prUrl2.IP && prUrl1.Port == prUrl2.Port;
            
            public static bool operator !=(ProxyURL prUrl1, ProxyURL prUrl2) => !(prUrl1 == prUrl2);
           
            public override int GetHashCode() => IP.GetHashCode() + Port;
            
        }


        #region Properties
        const string HOSTS_DIRECTORY_NAME = "ProxyHosts";
        const int MAX_PORT = 65535;
        const int MIN_PORT = 65535;
        const int maxProxyErrorNumber = 5;
        const string CHECK_ADDRESS = "google.com.ua";
        const string HOSTS_FILE_NAME = "hosts";
        const string SELECTOR = ":";
        private static string HOSTS_FILE_PATH => Path.Combine(HOSTS_DIRECTORY_NAME, HOSTS_FILE_NAME);
        private object _syncHosts = new object();
        private static List<ProxyURL> _proxyURLs = new List<ProxyURL>();
        public static ProxyConnection Instance = new ProxyConnection();
        private WebClient _webClient;
        private int _currentProxyIndex = 0;
        private int _errorCounter = 0;
        #endregion




        private ProxyConnection()
        {
            _webClient = new WebClient();
            InitializeProxyHostsAsync();
        }

        private async void InitializeProxyHostsAsync()
        {
            var addresses = GetAdresses();
            List<Task<ProxyURL>> tasks = new List<Task<ProxyURL>>();
            foreach (var adr in addresses)
            {
                if (!string.IsNullOrEmpty(adr) && TryParseAddress(adr, out ProxyURL prUrl))
                    tasks.Add(CheckProxy(adr, prUrl));
            }

            foreach (var task in tasks)
            {
                var proxy = await task;
                if (proxy != ProxyURL.Empty)
                {
                    lock (_syncHosts)
                    {
                        _proxyURLs.Add(proxy);
                    }                    
                }
            }
        }

        private async Task<ProxyURL> CheckProxy(string url, ProxyURL prUrl)
        {
            try
            {
                var content = await GetContentWithApartClientAsync(url, prUrl);
                return !string.IsNullOrEmpty(content) ? prUrl : ProxyURL.Empty;
            }
            catch (Exception)
            {
                return ProxyURL.Empty;
            }
        }

        private async Task<string> GetContentWithApartClientAsync(string url, ProxyURL prUrl)
        {
            using (var webClient = new WebClient())
            {
                if (prUrl != ProxyURL.Empty)
                {
                    webClient.Proxy = new WebProxy(prUrl.IP, prUrl.Port);
                }
                return await webClient.DownloadStringTaskAsync(url);
            }
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

        public async Task<string> ReadUrlAsync(string url)
        {
            if (_webClient.Proxy != null)
            {
                try
                {
                    return await _webClient.DownloadStringTaskAsync(url);
                }
                catch (Exception)
                {
                    NextProxy();
                }
            }

            _webClient.Proxy = new WebProxy(_currentProxy.IP, _currentProxy.Port);

            return await ReadUrlAsync(url);      
        }

        private ProxyURL _currentProxy = ProxyURL.Empty;
        
        private void NextProxy()
        {
            if (_currentProxyIndex < _proxyURLs.Count)
            {
                lock(_syncHosts)
                {
                    _currentProxy = _proxyURLs[_currentProxyIndex++];
                }
                return;
            }

            if (_errorCounter > maxProxyErrorNumber)
            {
                throw new Exception("Proxies don't work");
            }
            _errorCounter++;
            _currentProxyIndex = 0;
            lock (_syncHosts)
            {
                _currentProxy = _proxyURLs.Count > 0 ? _proxyURLs[_currentProxyIndex] : ProxyURL.Empty;
            }           
        }

        private static List<string> GetAdresses()
        {
            if (FilesHelper.CheckDirectory(HOSTS_DIRECTORY_NAME) && FilesHelper.CheckFile(HOSTS_FILE_PATH))
            {
                var lines = FilesHelper.ReadAllLines(HOSTS_FILE_PATH);
                if (lines.Count == 0)
                    return null;
                List<string> urls = new List<string>();
                foreach (var line in lines)
                {
                    if (!urls.Contains(line))
                        urls.Add(line);
                }
                return urls;
            }
            return null;
        }

        

        public void Dispose()
        {
            _webClient.Dispose();
        }       
    }
}
