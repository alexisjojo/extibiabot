using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;

namespace exTibia.Web
{
    class Player
    {
        public delegate void LookupReceived(CharInfo info);

        public delegate void LookupTimeReceived(LastOnlineTimes info);

        public static void LookupPlayer(string playername, LookupReceived callback)
        {
            playername = playername.Replace(' ', '+');
            playername = playername.Replace((char)0xA0, '+'); // Non-breaking space

            string url = "http://www.tibia.com/community/?subtopic=characters&name=" + playername;

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);

            request.BeginGetResponse(delegate(IAsyncResult ar)
            {
                string html = Tools.GetHTML(ar);

                callback(CharInfo.Parse(html));
            }, request);
        }

        public static void LookupTime(string playername, LookupTimeReceived callback)
        {
            playername = playername.Replace(' ', '+');
            playername = playername.Replace((char)0xA0, '+'); // Non-breaking space

            string url = "http://www.pskonejott.com/otc_display.php?character=" + playername;

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);

            request.BeginGetResponse(delegate(IAsyncResult ar)
            {
                string html = Tools.GetHTML(ar);

                callback(LastOnlineTimes.Parse(html));
            }, request);
        }

        public class LastOnlineTimes
        {
            public List<String> LastTimes = new List<string>();
            private string _total;

            public string Total
            {
                get { return _total; }
                set { _total = value; }
            }

            public static LastOnlineTimes Parse(string html)
            {
                if (html == null)
                    throw new ArgumentNullException("html");

                LastOnlineTimes times = new LastOnlineTimes();

                string text = html.Substring(html.IndexOf("Daily History"),
                   2500);

                try
                {
                    MatchCollection onlineTimes = Regex.Matches(text, @">([^<]*)<BR>([^<]*)</TD>", RegexOptions.Singleline);

                    foreach (Match match in onlineTimes)
                    {
                        times.LastTimes.Add(match.Groups[1].Value + "   " +match.Groups[2].Value);
                    }
                    
                }
                catch(ArgumentNullException ex)
                {
                    Helpers.Debug.Report(ex);
                }
                catch (InvalidOperationException ex)
                {
                    Helpers.Debug.Report(ex);
                }
                return times;
            }


        }

        public class CharInfo
        {
            private string _Name;

            public string Name
            {
                get { return _Name; }
                set { _Name = value; }
            }
            private string _Sex;

            public string Sex
            {
                get { return _Sex; }
                set { _Sex = value; }
            }
            private string _Profession;

            public string Profession
            {
                get { return _Profession; }
                set { _Profession = value; }
            }
            private int _Level;

            public int Level
            {
                get { return _Level; }
                set { _Level = value; }
            }
            private string _World;

            public string World
            {
                get { return _World; }
                set { _World = value; }
            }
            private string _Residence;

            public string Residence
            {
                get { return _Residence; }
                set { _Residence = value; }
            }
            private string _GuildName;

            public string GuildName
            {
                get { return _GuildName; }
                set { _GuildName = value; }
            }
            private string _GuildTitle;

            public string GuildTitle
            {
                get { return _GuildTitle; }
                set { _GuildTitle = value; }
            }
            private string _Comment;

            public string Comment
            {
                get { return _Comment; }
                set { _Comment = value; }
            }
            private string _AccountStatus;

            public string AccountStatus
            {
                get { return _AccountStatus; }
                set { _AccountStatus = value; }
            }
            private string _LastLogin;

            public string LastLogin
            {
                get { return _LastLogin; }
                set { _LastLogin = value; }
            }
            private string _Achievements;

            public string Achievements
            {
                get { return _Achievements; }
                set { _Achievements = value; }
            }
            private CharDeath[] _Deaths;

            public CharDeath[] Deaths
            {
                get { return _Deaths; }
                set { _Deaths = value; }
            }



            private string _Status;

            public string Status
            {
                get { return _Status; }
                set { _Status = value; }
            }

            //public DateTime GuildJoin;
            private string _GuildNickName;

            public string GuildNickName
            {
                get { return _GuildNickName; }
                set { _GuildNickName = value; }
            }


            public static CharInfo Parse(string html)
            {
                CharInfo i = new CharInfo();

                if (html == null)
                    throw new ArgumentNullException("html");

                try
                {
                    i.Name = Tools.Match(html, @"Name:</td><td>([^<]*)\s");
                    i.Sex = Tools.Match(html, @"Sex:</td><td>([^<]*)</td>");
                    i.Profession = Tools.Match(html, @"Vocation:</td><td>([^<]*)</td>");
                    i.Level = int.Parse(Tools.Match(html, @"Level:</td><td>([^<]*)</td>"));
                    i.World = Tools.Match(html, @"World:</td><td>([^<]*)<\/td>");
                    i.Achievements = Tools.Match(html, @"Achievement Points:</nobr></td><td>([^<]*)</td>");
                    i.Residence = Tools.Match(html, @"Residence:</td><td>([^<]*)</td>");
                    string guildDetails = Tools.Match(html, @"membership:</td><td>(.*?)</td>");
                    i.GuildTitle = Tools.Match(guildDetails, @"(.*) of the <a href");
                    i.GuildName = Tools.Match(guildDetails, @">([^<]*)</a>");
                    i.LastLogin = HttpUtility.HtmlDecode(Regex.Match(html, @"Last login:</td><td>([^<]*)</td>").Groups[1].Value);
                    i.Comment = Tools.Match(html, @"Comment:</td><td>(.*?)</td>").Replace("<br />", string.Empty);
                    i.AccountStatus = Tools.Match(html, @"Account&#160;Status:</td><td>([^<]*)</td>");
                    //MatchCollection deaths = Regex.Matches(html, @"<tr bgcolor=(?:#D4C0A1|#F1E0C6)><td width=25%>(.*?)?</td><td>((?:Died|Killed) at Level ([^ ]*)|and) by (?:<[^>]*>)?([^<]*)", RegexOptions.Singleline);
                    //CharDeath deads = new CharDeath();
                    
                
                }
                catch(ArgumentNullException)
                {
                    return i;
                }
                return i;
            }
        }

        public class CharDeath
        {
            private string _CharName;

            public string CharName
            {
                get { return _CharName; }
                set { _CharName = value; }
            }
            private DateTime _Time;

            public DateTime Time
            {
                get { return _Time; }
                set { _Time = value; }
            }
            private int _AtLevel;

            public int AtLevel
            {
                get { return _AtLevel; }
                set { _AtLevel = value; }
            }
            private string[] _KilledBy;

            public string[] KilledBy
            {
                get { return _KilledBy; }
                set { _KilledBy = value; }
            }
        }
    }
}
