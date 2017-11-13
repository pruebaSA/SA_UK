namespace AjaxControlToolkit
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Web;
    using System.Xml.Linq;

    public class TwitterAPI
    {
        public IList<TwitterStatus> GetProfile(string screenName, int count, bool includeRetweets, bool includeReplies)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("http://api.twitter.com/1/statuses/user_timeline.xml?screen_name={0}&count={1}", HttpUtility.UrlEncode(screenName), count);
            if (includeRetweets)
            {
                builder.AppendFormat("&include_rts=1", new object[0]);
            }
            if (!includeReplies)
            {
                builder.AppendFormat("&exclude_replies=1", new object[0]);
            }
            string url = builder.ToString();
            return (from s in this.Query(url).Descendants("status") select new TwitterStatus { 
                CreatedAt = DateTime.ParseExact(s.Element("created_at").Value, "ddd MMM dd HH:mm:ss zzz yyyy", CultureInfo.InvariantCulture),
                Text = s.Element("text").Value,
                User = (from u in s.Descendants("user") select new TwitterUser { 
                    Id = u.Element("id").Value,
                    ScreenName = u.Element("screen_name").Value,
                    Name = u.Element("name").Value,
                    Description = u.Element("description").Value,
                    ProfileImageUrl = u.Element("profile_image_url").Value
                }).FirstOrDefault<TwitterUser>()
            }).ToList<TwitterStatus>();
        }

        public IList<TwitterStatus> GetSearch(string search, int count)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("http://search.twitter.com/search.atom?q={0}&rpp={1}", HttpUtility.UrlEncode(search), count);
            string url = builder.ToString();
            XDocument document = this.Query(url);
            string str2 = "{http://www.w3.org/2005/Atom}";
            List<TwitterStatus> source = new List<TwitterStatus>();
            foreach (XElement element in from e in document.Descendants(str2 + "entry") select e)
            {
                TwitterUser user = new TwitterUser {
                    Name = element.Descendants(str2 + "name").FirstOrDefault<XElement>().Value,
                    ProfileImageUrl = (from link in element.Elements(str2 + "link")
                        where ((string) link.Attribute("rel")) == "image"
                        select (string) link.Attribute("href")).First<string>()
                };
                TwitterStatus item = new TwitterStatus {
                    CreatedAt = DateTime.Parse(element.Element(str2 + "published").Value),
                    Text = element.Element(str2 + "content").Value,
                    User = user
                };
                source.Add(item);
            }
            return source.ToList<TwitterStatus>();
        }

        private XDocument Query(string url) => 
            XDocument.Load(url);
    }
}

