using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace KanbanizeCardTest
{
    public class Client
    {
        readonly string API_KEY;
        readonly string BASE_URL;
        const string METHOD = "POST";
        const string END_URL = "/format/json";

        public Client(string apiKey, string baseUrl)
        {
            API_KEY = apiKey;
            BASE_URL = baseUrl;
        }

        public string SendRequest(string url, string method = METHOD, string body = "", Dictionary<string, string> headers = null)
        {
            string html = string.Empty;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BASE_URL+url+END_URL);
            request.Method = method;
            request.Headers.Add("apikey", API_KEY);

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            request.AutomaticDecompression = DecompressionMethods.GZip;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            html = reader.ReadToEnd();
                        }
                    }
                }
            }
            return html;
        }

        public dynamic Get(string url)
        {
            string response = SendRequest(url);

            dynamic result = JObject.Parse(response);

            return result;
        }
    }
}