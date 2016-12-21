using System;
using System.Net;

namespace Instagram
{
    public class Users
    {
        private Helpers helpers = new Helpers();
        private HttpRequests MyRequests = new HttpRequests();

        private string SessionID { get; set; }
        private string DSUserID { get; set; }
        private bool debug = false;

        public Users(string CurrentUser, string SessionID, bool debug)
        {
            this.SessionID = SessionID;
            this.debug = debug;

            this.DSUserID = helpers.GetUserID(CurrentUser);
            helpers.Log(debug, "DSUserID: " + DSUserID);
        }

        private string FollowCSRF { get; set; }

        public bool Follow(string user, string proxy = "")
        {
            FollowCSRF = helpers.GetCSRF("https://www.instagram.com/" + user, proxy);
            helpers.Log(debug, "Follow CSRF: " + FollowCSRF);

            string userid = helpers.GetUserID(user, proxy);
            helpers.Log(debug, user + "'s id: " + userid);

            string url = "https://www.instagram.com/web/friendships/" + userid + "/follow/";
            string[] headers = new string[]
            {
                "Host: www.instagram.com",
                "User-Agent: " + helpers.GetUserAgent(proxy),
                "Accept: */*",
                "Accept-Language: en-US,en;q=0.5",
                "Referer: https://www.instagram.com/" + user + "/",
                "X-CSRFToken: " + FollowCSRF,
                "X-Instagram-AJAX: 1",
                "Content-Type: application/x-www-form-urlencoded",
                "X-Requested-With: XMLHttpRequest",
                "Cookie: mid=WFotuQAEAAEj_k7xPdGLSnl7c6r8; csrftoken=" + FollowCSRF + "; s_network=; sessionid=" + SessionID + "; ds_user_id=" + DSUserID + "; ig_pr=1; ig_vw=1600",
                "Connection: keep-alive",
                "Content-Length: 0"
            };

            HttpWebRequest request = MyRequests.Create(url, HttpRequests.Method.POST, headers);

            if (proxy != string.Empty && proxy.Contains(":"))
            {
                string[] splitter = proxy.Split(':');
                WebProxy prox = new WebProxy(splitter[0], Int32.Parse(splitter[1]));
                prox.BypassProxyOnLocal = true;
                request.Proxy = prox;
                helpers.Log(debug, "Proxy Set: " + proxy);
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            string html = MyRequests.GetSource(response);
            helpers.Log(debug, "Follow HTML: " + html);
            
            if (html.Contains("\"result\": \"following\""))
            {
                helpers.Log(debug, "Followed user: " + user);
                return true;
            }
            else
            {
                helpers.Log(debug, "Failed to follow user");
                return false;
            }
        }
	}
}

