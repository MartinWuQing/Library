﻿using IdentityModel.Client;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Library.UI.Utilities
{
    public class ApiRequestWithFormUrlEncodedContent
    {
        private static readonly HttpClient _httpClient;
        private static readonly string _identityServer = string.Empty;
        private static readonly string _clientId = string.Empty;
        private static readonly string _clientSecret = string.Empty;

        static ApiRequestWithFormUrlEncodedContent()
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = new TimeSpan(0, 0, 10);
            _httpClient.DefaultRequestHeaders.Connection.Add("keep-alive");
            _identityServer = ConfigurationManager.AppSettings["identityServer"];
            _clientId = ConfigurationManager.AppSettings["identityClientID"];
            _clientSecret = ConfigurationManager.AppSettings["identityClientSecret"];
        }

        public static string GetToken(string serviceName)
        {
            var tokenClient = new TokenClient($"{_identityServer}/connect/token", _clientId, _clientSecret);
            return tokenClient.RequestClientCredentialsAsync(serviceName).Result.AccessToken;
        }

        public static T Get<T>(string serviceName, string url)
        {
            _httpClient.SetBearerToken(GetToken(serviceName));

            if (url.StartsWith("https"))
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
            }

            HttpResponseMessage response = _httpClient.GetAsync(url).Result;

            T result = default(T);

            if (response.IsSuccessStatusCode)
            {
                Task<string> t = response.Content.ReadAsStringAsync();
                string s = t.Result;

                result = JsonConvert.DeserializeObject<T>(s);
            }

            return result;
        }

        public static T Post<T>(string serviceName, string url, NameValueCollection data)
        {
            _httpClient.SetBearerToken(GetToken(serviceName));

            if (url.StartsWith("https"))
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
            }

            HttpContent httpContent = new FormUrlEncodedContent(Correct(data));
            HttpResponseMessage response = _httpClient.PostAsync(url, httpContent).Result;

            T result = default(T);

            if (response.IsSuccessStatusCode)
            {
                Task<string> t = response.Content.ReadAsStringAsync();
                string s = t.Result;

                result = JsonConvert.DeserializeObject<T>(s);
            }

            return result;
        }

        public static T Put<T>(string serviceName, string url, NameValueCollection data)
        {
            _httpClient.SetBearerToken(GetToken(serviceName));

            if (url.StartsWith("https"))
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
            }

            HttpContent httpContent = new FormUrlEncodedContent(Correct(data));
            HttpResponseMessage response = _httpClient.PutAsync(url, httpContent).Result;

            T result = default(T);

            if (response.IsSuccessStatusCode)
            {
                Task<string> t = response.Content.ReadAsStringAsync();
                string s = t.Result;

                result = JsonConvert.DeserializeObject<T>(s);
            }

            return result;
        }

        public static void Delete(string serviceName, string url, object data)
        {
            _httpClient.SetBearerToken(GetToken(serviceName));

            if (url.StartsWith("https"))
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
            }

            HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(data));
            HttpResponseMessage response = _httpClient.DeleteAsync(url).Result;
        }

        private static IEnumerable<KeyValuePair<string, string>> Correct(NameValueCollection formData)
        {
            return formData.Keys.Cast<string>().Select(key => new KeyValuePair<string, string>(key, formData[key])).ToList();
        }
    }
}