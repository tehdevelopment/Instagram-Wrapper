using System;
using System.Net;
using System.Text.RegularExpressions;

namespace Instagram
{
	public class Helpers
	{
        public string GetCSRF(string page, string proxy = "")
        {
            using (WebClient client = new WebClient())
            {
                if (proxy != string.Empty && proxy.Contains(":"))
                {
                    string[] splitter = proxy.Split(':');
                    WebProxy prox = new WebProxy(splitter[0], Int32.Parse(splitter[1]));
                    prox.BypassProxyOnLocal = true;
                    client.Proxy = prox;
                }

                string source = client.DownloadString(page);
                string pattern = "\"csrf_token\": \"(.*?)\"";

                return Regex.Matches(source, pattern)[0].Groups[1].Value.Split('"')[0];
            }
		}

        public string GetUserID(string user, string proxy = "")
        {
            using (WebClient client = new WebClient())
            {
                if (proxy != string.Empty && proxy.Contains(":"))
                {
                    string[] splitter = proxy.Split(':');
                    WebProxy prox = new WebProxy(splitter[0], Int32.Parse(splitter[1]));
                    prox.BypassProxyOnLocal = true;
                    client.Proxy = prox;
                }

                string page = "https://www.instagram.com/" + user;
                string source = client.DownloadString(page);
                string pattern = "\"id\": \"(.*?)\"";

                return Regex.Matches(source, pattern)[0].Groups[1].Value.Split('"')[0];
            }
        }

		public void Log(bool debug, string text)
		{
			if (debug)
			{
				Console.WriteLine("[" + DateTime.Now.ToLongDateString () + "] [" + DateTime.Now.ToLongTimeString () + "] " + text);
			}
		}

		public string GetUserAgent(string proxy = "")
		{
            using (WebClient client = new WebClient())
            {
                if (proxy != string.Empty && proxy.Contains(":"))
                {
                    string[] splitter = proxy.Split(':');
                    WebProxy prox = new WebProxy(splitter[0], Int32.Parse(splitter[1]));
                    prox.BypassProxyOnLocal = true;
                    client.Proxy = prox;
                }

                string[] useragents = client.DownloadString("http://pastebin.com/mcfXxS5v").Split('\n');

                return useragents[new Random().Next(0, useragents.Length)];
            }
		}

        public string GetPostID(string url, string proxy = "")
        {
            using (WebClient client = new WebClient())
            {
                if (proxy != string.Empty && proxy.Contains(":"))
                {
                    string[] splitter = proxy.Split(':');
                    WebProxy prox = new WebProxy(splitter[0], Int32.Parse(splitter[1]));
                    prox.BypassProxyOnLocal = true;
                    client.Proxy = prox;
                }

                string source = client.DownloadString(url);
                string pattern = "\"id\": \"(.*?)\"";

                string postid = string.Empty;
                foreach (Match item in (new Regex(pattern).Matches(source)))
                {
                    string currentid = item.Groups[1].Value.Split('"')[0];
                    if (currentid.Length == 19)
                    {
                        postid = currentid;
                        break;
                    }
                }

                return postid;
            }
        }
	}
}

