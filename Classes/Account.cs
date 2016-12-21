using System;
using System.Net;
using System.Linq;

namespace Instagram
{
	public class Account
	{
		private bool debug = false;

		public Account(bool debug)
		{
			this.debug = debug;
		}

		private Helpers helpers = new Helpers();
		private HttpRequests MyRequests = new HttpRequests ();

		public string SessionID { get; set; }
		private string LoginCSRF { get; set; }
		private string RegisterCSRF { get; set; }
		private string CheckCSRF { get; set; }

		public bool Login(string user, string pass, string proxy = "")
		{
			LoginCSRF = helpers.GetCSRF ("https://www.instagram.com/accounts/login/", proxy);
			helpers.Log (debug, "Grabbed Login CSRF: " + LoginCSRF);

			string post = "username=" + user + "&password=" + pass;
			string url = "https://www.instagram.com/accounts/login/ajax/";

			string[] headers = new string[]
			{
				"Host: www.instagram.com",
				"User-Agent: " + helpers.GetUserAgent(proxy),
				"Accept: */*",
				"Accept-Language: en-US,en;q=0.5",
				"X-CSRFToken: " + LoginCSRF,
				"X-Instagram-AJAX: 1",
				"Content-Type: application/x-www-form-urlencoded",
				"X-Requested-With: XMLHttpRequest",
				"Referer: https://www.instagram.com/accounts/login/",
				"Cookie: csrftoken=" + LoginCSRF + "; mid=WFOHewAEAAEl7tki8YuogH4lc9rS; ig_pr=1; ig_vw=1366",
				"Connection: keep-alive",
			};

			HttpWebRequest request = MyRequests.Create (url, HttpRequests.Method.POST, headers);

      if (proxy != string.Empty && proxy.Contains(":"))
      {
    	  string[] splitter = proxy.Split(':');
        WebProxy prox = new WebProxy(splitter[0], Int32.Parse(splitter[1]));
        prox.BypassProxyOnLocal = true;
        request.Proxy = prox;
        helpers.Log(debug, "Proxy Set: " + proxy);
      }

      MyRequests.Write (request, post);

			HttpWebResponse response = (HttpWebResponse)request.GetResponse ();

			string html = MyRequests.GetSource (response);
			helpers.Log (debug, "Login HTML: " + html);

			if (html.Contains ("\"authenticated\": true"))
			{
				string cookieName = "sessionid";
				string cookie = response.Headers.GetValues ("Set-Cookie").First (x => x.StartsWith (cookieName));
				SessionID = cookie.Replace (cookieName + "=", string.Empty);
				helpers.Log (debug, "Session ID: " + SessionID);
				helpers.Log (debug, "Authenticated User: " + user);
				return true;
			}
			else
			{
				helpers.Log (debug, "Authentication failed");
				return false;
			}
		}


		public bool Register(string username, string email, string password, string fullname, string proxy = "")
		{
			fullname = fullname.Replace (" ", "+");
			email = email.Replace ("@", "%40");

			RegisterCSRF = helpers.GetCSRF ("https://www.instagram.com/", proxy);
			helpers.Log (debug, "Register CSRF: " + RegisterCSRF);

			string url = "https://www.instagram.com/accounts/web_create_ajax/";
			string post = "email=" + email + "&password=" + password + "&username=" + username + "&first_name=" + fullname;

			string[] headers = new string[]
			{
				"Host: www.instagram.com",
				"User-Agent: " + helpers.GetUserAgent(proxy),
				"Accept: */*",
				"Accept-Language: en-US,en;q=0.5",
				"X-CSRFToken: " + RegisterCSRF,
				"X-Instagram-AJAX: 1",
				"Content-Type: application/x-www-form-urlencoded",
				"X-Requested-With: XMLHttpRequest",
				"Connection: keep-alive",
				"Cookie: mid=WFTOrAAEAAGeFe3by_c5bY_ODw_U; ig_pr=1; ig_vw=1366; csrftoken=" + RegisterCSRF,
				"Referer: https://www.instagram.com/"
			};

			HttpWebRequest request = MyRequests.Create (url, HttpRequests.Method.POST, headers);

      if (proxy != string.Empty && proxy.Contains(":"))
      {
        string[] splitter = proxy.Split(':');
        WebProxy prox = new WebProxy(splitter[0], Int32.Parse(splitter[1]));
        prox.BypassProxyOnLocal = true;
        request.Proxy = prox;
        helpers.Log(debug, "Proxy Set: " + proxy);
      }

      MyRequests.Write (request, post);
			HttpWebResponse response = (HttpWebResponse)request.GetResponse ();

			string html = MyRequests.GetSource(response);
			helpers.Log (debug, "Register HTML: " + html);

			if (html.Contains("\"account_created\": true"))
			{
				helpers.Log (debug, "An account has been created with the username: " + username);
				return true;
			}
			else
			{
				helpers.Log(debug, "Account Creation Failed");
				return false;
			}
		}

		public bool IsUsernameAvailable(string username)
		{
			CheckCSRF = helpers.GetCSRF ("https://www.instagram.com/");
			helpers.Log (debug, "Check Username CSRF: " + CheckCSRF);

			string url = "https://www.instagram.com/accounts/web_create_ajax/attempt/";
			string post = "email=email%40provider.com&password=password&username=" + username + "&first_name=chris";

			string[] headers = new string[]
			{
				"Host: www.instagram.com",
				"User-Agent: " + helpers.GetUserAgent(),
				"Accept: */*",
				"Accept-Language: en-US,en;q=0.5",
				"X-CSRFToken: " + CheckCSRF,
				"X-Instagram-AJAX: 1",
				"Content-Type: application/x-www-form-urlencoded",
				"X-Requested-With: XMLHttpRequest",
				"Connection: keep-alive",
				"Cookie: mid=WFTOrAAEAAGeFe3by_c5bY_ODw_U; ig_pr=1; ig_vw=1366; csrftoken=" + CheckCSRF,
				"Referer: https://www.instagram.com/"
			};

			HttpWebRequest request = MyRequests.Create (url, HttpRequests.Method.POST, headers);

			MyRequests.Write (request, post);

			HttpWebResponse response = (HttpWebResponse)request.GetResponse ();

			string html = MyRequests.GetSource(response);

			if (!html.Contains ("Sorry, that username is taken"))
			{
				helpers.Log(debug, username + " is available");
				return true;
			}
			else
			{
				helpers.Log(debug, username + " is taken");
				return false;
			}
		}
	}
}
