using System;

namespace Instagram
{
    public class Botting
    {
        private bool debug = false;
        private Helpers helpers = new Helpers();

        public Botting(bool debug)
        {
            this.debug = debug;
        }

        private string[] Names()
        {
            return new string[]
            {
                "Fighter", "Majestic", "Unicorn", "Dizzy", "Playful", "Swimmer", "Lolipop", "Hole", "Dancer", "Fighter", "Salty", "Butterfly",
                "Snake", "Rabbit", "Dog", "Cat", "Kangaroo", "Giraffe", "Hippopotamus", "Doodler"
            };
        }

        private enum InfoType
        {
            Username, Password, Email, Fullname
        }

        private string GetInfo(InfoType info)
        {
            string myinfo = string.Empty;
            string[] names = Names();

            Random r = new Random();
            switch (info)
            {
                case InfoType.Email:
                    myinfo = names[r.Next(0, names.Length)] + names[r.Next(0, names.Length)] + r.Next(0, 1000000) + "@gmail.com";
                    break;
                case InfoType.Fullname:
                    myinfo = "full name";
                    break;
                case InfoType.Password:
                    myinfo = names[r.Next(0, names.Length)] + r.Next(0, 1000000);
                    break;
                case InfoType.Username:
                    myinfo = names[r.Next(0, names.Length)] + names[r.Next(0, names.Length)] + r.Next(0, 1000000);
                    break;
            }

            helpers.Log(debug, "Type: " + info.ToString() + " // Value: " + myinfo);
            return myinfo;
        }

        public bool Like(string photo, string proxy = "")
        {
            Account acc = new Account(debug);

            string username = GetInfo(InfoType.Username);
            string password = GetInfo(InfoType.Password);
            string email = GetInfo(InfoType.Email);
            string fullname = GetInfo(InfoType.Fullname);

            if (acc.Register(username, email, password, fullname, proxy))
            {
                if (acc.Login(username, password, proxy))
                {
                    Posts posts = new Posts(username, acc.SessionID, debug);
                    if (posts.Like(photo, proxy))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public bool Comment(string photo, string comment, string proxy = "")
        {
            Account acc = new Account(debug);

            string username = GetInfo(InfoType.Username);
            string password = GetInfo(InfoType.Password);
            string email = GetInfo(InfoType.Email);
            string fullname = GetInfo(InfoType.Fullname);

            if (acc.Register(username, email, password, fullname, proxy))
            {
                if (acc.Login(username, password, proxy))
                {
                    Posts posts = new Posts(username, acc.SessionID, debug);
                    if (posts.Comment(photo, comment, proxy))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public bool Follow(string user, string proxy = "")
        {
            Account acc = new Account(debug);

            string username = GetInfo(InfoType.Username);
            string password = GetInfo(InfoType.Password);
            string email = GetInfo(InfoType.Email);
            string fullname = GetInfo(InfoType.Fullname);

            if (acc.Register(username, email, password, fullname, proxy))
            {
                if (acc.Login(username, password, proxy))
                {
                    Users users = new Users(username, acc.SessionID, debug);
                    if (users.Follow(user, proxy))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}

