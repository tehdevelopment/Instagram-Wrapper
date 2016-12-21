using System.Net;
using System.Text.RegularExpressions;

namespace Instagram
{
    public class Profile
    {
        private Helpers helpers = new Helpers();

        public enum InfoType
        {
            FollowerCount, FollowingCount, PostCount, Bio, ProfilePicture, UserID
        }

        public string GetInfo(string user, InfoType type)
        {
            using (WebClient client = new WebClient())
            {
                string source = client.DownloadString("https://www.instagram.com/" + user);

                string pattern = string.Empty;
                string info = string.Empty;

                switch (type)
                {
                    case InfoType.FollowerCount:
                        pattern = "\"followed_by\": {\"count\": (.*?)}";
                        info = Regex.Matches(source, pattern)[0].Groups[1].Value.Split('}')[0];
                        break;
                    case InfoType.FollowingCount:
                        pattern = "\"follows\": {\"count\": (.*?)}";
                        info = Regex.Matches(source, pattern)[0].Groups[1].Value.Split('}')[0];
                        break;
                    case InfoType.PostCount:
                        pattern = "\"media\": {\"count\": (.*?),";
                        info = Regex.Matches(source, pattern)[0].Groups[1].Value.Split(',')[0];
                        break;
                    case InfoType.Bio:
                        pattern = "\"biography\": \"(.*?)\"";
                        info = Regex.Matches(source, pattern)[0].Groups[1].Value.Split('"')[0];
                        break;
                    case InfoType.ProfilePicture:
                        pattern = "\"profile_pic_url\": \"(.*?)\"";
                        info = Regex.Matches(source, pattern)[0].Groups[1].Value.Split('"')[0];
                        break;
                    case InfoType.UserID:
                        info = helpers.GetUserID(user);
                        break;
                }

                return info;
            }
        }
    }
}
