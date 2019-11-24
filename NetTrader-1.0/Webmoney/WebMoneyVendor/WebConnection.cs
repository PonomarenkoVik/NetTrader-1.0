﻿using Interfaces.AdditionalFunctions;
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
        const string CHECK_ADDRESS = "https://www.google.com";
        const string HOSTS_FILE_NAME = "hosts.txt";
        const string SELECTOR = ":";
        const int THREAD_MAX_NUMBER = 10;
        private static string HOSTS_FILE_PATH => Path.Combine(HOSTS_DIRECTORY_NAME, HOSTS_FILE_NAME);
        private object _syncHosts = new object();
        private static List<ProxyURL> _proxyURLs = new List<ProxyURL>();
        public static WebConnection Instance = new WebConnection();

        public bool UseProxy { get; set; } = false;

        public event Action OnProxiesLoaded;
        #endregion

        private WebConnection()
        {
            Task.Factory.StartNew(InitializeProxyHostsAsync);
        }

        #region Public
        public bool WriteProxies()
        {
            if (_proxyURLs.Count == 0)
                return false;
            try
            {
                File.Delete(HOSTS_FILE_PATH);
                return FilesHelper.WriteAllLines(HOSTS_FILE_PATH, _proxyURLs.Select(p => $"{p.IP}:{p.Port}").ToList(), _syncHosts);
            }
            catch (Exception)
            {
                return false;
            }
        }

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
                cl = new WebClient() { Proxy = proxy , Encoding = Encoding.UTF8};
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
        private void InitializeProxyHostsAsync()
        {

            var addresses = GetAdresses();
            if (addresses.Count == 0)
            {
                throw new Exception($"Set proxies \"{HOSTS_FILE_PATH}\"");
            }

            List<Task> tasks = new List<Task>();
            foreach (var adr in addresses)
            {

                if (!string.IsNullOrEmpty(adr) && TryParseAddress(adr, out ProxyURL prUrl))
                {
                    Task task = new Task(() => CheckProxy(CHECK_ADDRESS, prUrl, AddProxy));
                    tasks.Add(task);
                }
            }

            int counter = 0;
            List<Task> taskList = new List<Task>();
            foreach (var task in tasks)
            {
                taskList.Add(task);
                task.Start();
                counter++;
                if (counter > THREAD_MAX_NUMBER)
                {
                    Task.WaitAll(taskList.ToArray());
                    taskList.Clear();
                    counter = 0;
                }
            }
            WriteProxies();
        }

        private static List<string> GetAdresses()
        {
            List<string> urls = new List<string>();
            if (FilesHelper.CheckDirectory(HOSTS_DIRECTORY_NAME) & FilesHelper.CheckFile(HOSTS_FILE_PATH))
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

        private string GetContentWithApartClient(string url, ProxyURL prUrl)
        {
            using (var webClient = new WebClient())
            {
                if (prUrl != ProxyURL.Empty)
                {
                    webClient.Proxy = new WebProxy(prUrl.IP, prUrl.Port);
                }
                return webClient.DownloadString(url);
            }
        }

        private void CheckProxy(string url, ProxyURL prUrl, Action<ProxyURL> callBack)
        {
            try
            {
                var content = GetContentWithApartClient(url, prUrl);
                if (!string.IsNullOrEmpty(content))
                {
                    callBack(prUrl);
                }
            }
            catch (Exception)
            {
            }
        }

        private void AddProxy(ProxyURL prUrl)
        {
            if (prUrl != ProxyURL.Empty)
            {
                if (_currentProxy == null)
                    _currentProxy = prUrl;

                lock (_syncHosts)
                {
                    _proxyURLs.Add(prUrl);
                }
                if (_proxyURLs.Count == 1)
                    OnProxiesLoaded?.Invoke();
            }
        }
        #endregion
    }
}
