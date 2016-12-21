using System;
using System.Net;

namespace Instagram
{
	public class Posts
	{
        private Helpers helpers = new Helpers();
        private HttpRequests MyRequests = new HttpRequests();

        private string SessionID { get; set; }
		private string DSUserID { get; set; }
		private bool debug = false;

		public Posts (string CurrentUser, string SessionID, bool debug)
		{
			this.SessionID = SessionID;
			this.debug = debug;

            this.DSUserID = helpers.GetUserID(CurrentUser);
            helpers.Log(debug, "DSUserID: " + DSUserID);
		}

		private string LikeCSRF { get; set; }
		private string CommentCSRF { get; set; }

		public bool Like(string photoUrl, string proxy = "")
		{
            LikeCSRF = helpers.GetCSRF(photoUrl, proxy);
            helpers.Log(debug, "Like CSRF " + LikeCSRF);

            string postID = helpers.GetPostID(photoUrl, proxy);
            helpers.Log(debug, "Post ID: " + postID);

            string url = "https://www.instagram.com/web/likes/" + postID + "/like/";
            string[] headers = new string[]
            {
                "Host: www.instagram.com",
                "User-Agent: " + helpers.GetUserAgent(proxy),
                "Accept: */*",
                "Accept-Language: en-US,en;q=0.5",
                "Referer: " + photoUrl,
                "X-CSRFToken: " + LikeCSRF,
                "X-Instagram-AJAX: 1",
                "Content-Type: application/x-www-form-urlencoded",
                "X-Requested-With: XMLHttpRequest",
                "Cookie: mid=WFotuQAEAAEj_k7xPdGLSnl7c6r8; csrftoken=" + LikeCSRF + "; s_network=; ig_pr=1; ig_vw=1600; sessionid=" + SessionID + "; ds_user_id=" + DSUserID,
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

            helpers.Log(debug, "Like HTML: " + html);
            if (html.Contains("ok"))
            {
                helpers.Log(debug, photoUrl + " was liked by user.");
                return true;
            }
            else
            {
                helpers.Log(debug, "Failed to like photo");
                return false;
            }
		}

		public bool Comment(string photoUrl, string comment, string proxy = "")
		{
            string post = "comment_text=" + comment.Replace(" ", "+");

            CommentCSRF = helpers.GetCSRF(photoUrl, proxy);
            helpers.Log(debug, "Comment CSRF " + CommentCSRF);

            string postID = helpers.GetPostID(photoUrl, proxy);
            helpers.Log(debug, "Post ID: " + postID);

            string url = "https://www.instagram.com/web/comments/" + postID + "/add/";
            string[] headers = new string[]
            {
                "Host: www.instagram.com",
                "User-Agent: " + helpers.GetUserAgent(proxy),
                "Accept: */*",
                "Accept-Language: en-US,en;q=0.5",
                "Referer: " + photoUrl,
                "X-CSRFToken: " + CommentCSRF,
                "X-Instagram-AJAX: 1",
                "Content-Type: application/x-www-form-urlencoded",
                "X-Requested-With: XMLHttpRequest",
                "Cookie: mid=WFotuQAEAAEj_k7xPdGLSnl7c6r8; csrftoken=" + CommentCSRF + "; s_network=; ig_pr=1; ig_vw=1600; sessionid=" + SessionID + "; ds_user_id=" + DSUserID,
                "Connection: keep-alive"
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

            MyRequests.Write(request, post);

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string html = MyRequests.GetSource(response);

            helpers.Log(debug, "Comment HTML: " + html);
            if (html.Contains("ok"))
            {
                helpers.Log(debug, "Comment was added to photo");
                return true;
            }
            else
            {
                helpers.Log(debug, "Comment not added.");
                return false;
            }
        }
	}
}

