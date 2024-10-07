using Azure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CasinoRegistro.DataAccess.Helpers
{
    public class Plesk
    {
        public string Hostname { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }

        public Plesk(string hostname = "", string username = "", string password = "")
        {
            Hostname = hostname;
            Username = username;
            Password = password;
        }

       
        public async Task<Dictionary<string, string>> AddEmail(string domainId, string username, string password, int quota = 500)
        {
            quota = quota * 1024 * 1024;
            string packet = $"<packet version='1.4.2.0'>" +
                            $"<mail>" +
                            $"<create>" +
                            $"<filter>" +
                            $"<domain_id>{domainId}</domain_id>" +
                            $"<mailname>" +
                            $"<name>{username}</name>" +
                            $"<mailbox>" +
                            $"<enabled>true</enabled>" +
                            $"<quota>{quota}</quota>" +
                            $"</mailbox>" +
                            $"<password>{password}</password>" +
                            $"<password_type>plain</password_type>" +
                            $"<permissions>" +
                            $"<cp_access>true</cp_access>" +
                            $"</permissions>" +
                            $"</mailname>" +
                            $"</filter>" +
                            $"</create>" +
                            $"</mail>" +
                            $"</packet>";

            var result = await Send(packet);
            var result1 = new Dictionary<string, string>();// result[0]["elements"][0]["elements"];

            if (result1["text"] == "error")
            {
                return new Dictionary<string, string>
            {
                { "status", "error" },
                { "error_code", result1["text"] },
                { "message", result1["message"] }
            };
            }
            //else if (result[0]["elements"][0]["elements"][0]["elements"][0]["elements"][0]["text"] == "error")
            //{
            //    return new Dictionary<string, string>
            //{
            //    { "status", "error" },
            //    { "error_code", result[0]["elements"][0]["elements"][0]["elements"][0]["elements"][1]["text"] },
            //    { "message", result[0]["elements"][0]["elements"][0]["elements"][0]["elements"][2]["text"] }
            //};
            //}
            else
            {
                return new Dictionary<string, string>
            {
                { "status", "ok" },
                { "message", "created" }
            };
            }
        }

        public async Task<List<Dictionary<string, object>>> Send(string packet = "")
        {
            var headers = new Dictionary<string, string>
            {
                { "HTTP_AUTH_LOGIN", Username },
                { "HTTP_AUTH_PASSWD", Password },
                { "Content-Type", "text/xml" }
            };

            using (var client = new HttpClient())
            {
                foreach (var header in headers)
                {
                    client.DefaultRequestHeaders.Add(header.Key, header.Value);
                }

                var content = new StringContent(packet, Encoding.UTF8, "text/xml");
                var response = await client.PostAsync(Hostname, content);
                var retval = await response.Content.ReadAsStringAsync();
                return XmlToArray(retval);
            }
        }

        public List<Dictionary<string, object>> XmlToArray(string xml)
        {
            var xmlary = new List<Dictionary<string, object>>();
            var reels = new Regex(@"<(\w+)\s*([^\/>]*)\s*(?:\/>|>(.*)<\/\s*\\1\s*>)(?s)");
            var reattrs = new Regex(@"(\w+)=(?:""|')([^""']*)(?:""|')");
            var elements = reels.Matches(xml);

            foreach (Match match in elements)
            {
                var element = new Dictionary<string, object>
            {
                { "name", match.Groups[1].Value }
            };

                if (!string.IsNullOrWhiteSpace(match.Groups[2].Value))
                {
                    var attributes = reattrs.Matches(match.Groups[2].Value);
                    var attrDict = new Dictionary<string, string>();
                    foreach (Match attr in attributes)
                    {
                        attrDict[attr.Groups[1].Value] = attr.Groups[2].Value;
                    }
                    element["attributes"] = attrDict;
                }

                var cdend = match.Groups[3].Value.IndexOf("<");
                if (cdend > 0)
                {
                    element["text"] = match.Groups[3].Value.Substring(0, cdend - 1);
                }

                if (reels.IsMatch(match.Groups[3].Value))
                {
                    element["elements"] = XmlToArray(match.Groups[3].Value);
                }
                else if (!string.IsNullOrWhiteSpace(match.Groups[3].Value))
                {
                    element["text"] = match.Groups[3].Value;
                }

                xmlary.Add(element);
            }
            return xmlary;
        }
    }
}
