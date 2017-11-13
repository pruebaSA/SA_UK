namespace AjaxControlToolkit
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web.UI;
    using System.Web.UI.Design;
    using System.Web.UI.Design.WebControls;
    using System.Web.UI.WebControls;

    public class TwitterDesigner : CompositeControlDesigner
    {
        private Twitter _twitter;
        private int _valCounter;
        private List<string> _values = new List<string>();

        private string FillStatusValue(Match match)
        {
            this._valCounter++;
            return this._values[this._valCounter - 1];
        }

        private IList<TwitterStatus> GenerateData()
        {
            if (!this._twitter.IsLiveContentOnDesignMode)
            {
                return this.GenerateFakeData();
            }
            IList<TwitterStatus> list = null;
            TwitterAPI rapi = new TwitterAPI();
            if (this._twitter.Mode == TwitterMode.Profile)
            {
                list = rapi.GetProfile(this._twitter.ScreenName, this._twitter.Count, this._twitter.IncludeRetweets, this._twitter.IncludeReplies);
                if ((list != null) && (list.Count > 0))
                {
                    TwitterUser user = list[0].User;
                    this._twitter.Title = this._twitter.Title ?? user.Name;
                    this._twitter.Caption = this._twitter.Caption ?? user.ScreenName;
                    this._twitter.ProfileImageUrl = this._twitter.ProfileImageUrl ?? user.ProfileImageUrl;
                }
                return list;
            }
            return rapi.GetSearch(this._twitter.Search, this._twitter.Count);
        }

        private IList<TwitterStatus> GenerateFakeData()
        {
            List<TwitterStatus> list = new List<TwitterStatus>();
            string webResourceUrl = base.ViewControl.Page.ClientScript.GetWebResourceUrl(base.GetType(), "Twitter.Twitter32.png");
            TwitterUser user = new TwitterUser {
                ScreenName = "ajaxcontroltoolkit",
                Description = "Ajax Control Toolkit",
                Id = "ajaxcontroltoolkit",
                Name = "Ajax Control Toolkit",
                Location = "US",
                ProfileImageUrl = webResourceUrl
            };
            string str2 = "";
            if (this._twitter.Mode == TwitterMode.Profile)
            {
                TwitterStatus item = new TwitterStatus {
                    CreatedAt = DateTime.Now,
                    Text = "Ajax Control Toolkit",
                    User = user
                };
                list.Add(item);
                this._twitter.Title = this._twitter.Title ?? user.Name;
                this._twitter.Caption = this._twitter.Caption ?? user.ScreenName;
                this._twitter.ProfileImageUrl = this._twitter.ProfileImageUrl ?? user.ProfileImageUrl;
            }
            else
            {
                foreach (string str3 in this._twitter.Search.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries))
                {
                    str2 = str2 + "<em>" + str3 + "</em> ";
                }
                str2 = " " + str2;
            }
            string[] strArray2 = new string[] { "Lorem <a href='http://www.sample_ipsum_link.com'>ipsum</a> dolor sit amet, " + str2 + "consectetur adipisicing elit, sed do eiusmod tempor incididunt ut", "labore et dolore magna aliqua. Ut enim ad minim veniam, quis " + str2 + "nostrud exercitation", "ullamco laboris " + str2 + "nisi ut aliquip ex ea <a href='http://comodo_sample_link'>commodo</a> consequat", str2 + "Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla", "Excepteur sint " + str2 + "occaecat cupidatat non proident, sunt in culpa qui officia deserunt" };
            int num = 0;
            Random random = new Random();
            foreach (string str4 in strArray2)
            {
                TwitterStatus status2 = new TwitterStatus {
                    CreatedAt = DateTime.Now.AddMinutes((double) (random.Next(1, 0x3e8) * -1)),
                    Text = str4,
                    User = user
                };
                list.Add(status2);
                num++;
                if (num > this._twitter.Count)
                {
                    break;
                }
            }
            if ((this._twitter.Mode == TwitterMode.Profile) && (list.Count > 1))
            {
                list.RemoveAt(list.Count - 1);
            }
            return list;
        }

        public override string GetDesignTimeHtml()
        {
            string designTimeHtml = base.GetDesignTimeHtml();
            designTimeHtml = (designTimeHtml.IndexOf("<div", 1) > 0) ? designTimeHtml.Substring(0, designTimeHtml.IndexOf("<div", 1)) : designTimeHtml.Remove(designTimeHtml.Length - 6, 6);
            designTimeHtml = designTimeHtml.Replace("\r", "").Replace("\n", "").Replace("\t", "");
            string str2 = null;
            try
            {
                if (this._twitter.Mode == TwitterMode.Profile)
                {
                    if (string.IsNullOrEmpty(this._twitter.ScreenName))
                    {
                        throw new Exception("Please specify a screen name");
                    }
                }
                else if (string.IsNullOrEmpty(this._twitter.Search))
                {
                    throw new Exception("Please specify a search keyword");
                }
                new TwitterAPI();
                IList<TwitterStatus> statuses = this.GenerateData();
                if (statuses.Count > 0)
                {
                    str2 = this.RenderLayout(statuses);
                }
                else
                {
                    str2 = this.RenderEmptyData();
                }
            }
            catch (Exception exception)
            {
                if (str2 == null)
                {
                    str2 = "<div>" + exception.Message + "</div>";
                }
            }
            string webResourceUrl = base.ViewControl.Page.ClientScript.GetWebResourceUrl(base.GetType(), "Twitter.Twitter_resource.css");
            string str4 = $"<link href="{webResourceUrl}" rel="stylesheet" type="text/css"/>";
            return (designTimeHtml + str4 + str2 + "</div>");
        }

        public override void Initialize(IComponent component)
        {
            this._twitter = component as Twitter;
            if (this._twitter == null)
            {
                throw new ArgumentException("Component must be a Twitter control", "component");
            }
            base.Initialize(component);
        }

        private string PersistTemplate(ITemplate template)
        {
            IDesignerHost service = (IDesignerHost) this.GetService(typeof(IDesignerHost));
            return ControlPersister.PersistTemplate(template, service);
        }

        private static string RenderControl(Control control)
        {
            StringBuilder sb = new StringBuilder();
            using (StringWriter writer = new StringWriter(sb))
            {
                using (HtmlTextWriter writer2 = new HtmlTextWriter(writer))
                {
                    control.RenderControl(writer2);
                }
            }
            return sb.ToString();
        }

        private string RenderEmptyData() => 
            this.PersistTemplate(this._twitter.EmptyDataTemplate);

        private string RenderEvalScripts(ITemplate template, TwitterStatus status)
        {
            string input = this.PersistTemplate(template);
            Regex regex = new Regex("(<%#) ?.*eval?.*%>", RegexOptions.IgnoreCase);
            MatchCollection matchs = regex.Matches(input);
            this._valCounter = 0;
            this._values = new List<string>();
            if (matchs.Count <= 0)
            {
                return null;
            }
            Regex regex2 = new Regex("\"(.*?)\"");
            foreach (object obj2 in matchs)
            {
                string name = regex2.Match(obj2.ToString()).ToString();
                name = name.Substring(1, name.Length - 2);
                object obj3 = null;
                if (name.Contains("."))
                {
                    string[] strArray = name.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                    object obj4 = typeof(TwitterStatus).GetProperty(strArray[0]).GetValue(status, null);
                    if (obj4 != null)
                    {
                        obj3 = obj4.GetType().GetProperty(strArray[1]).GetValue(obj4, null);
                    }
                }
                else
                {
                    obj3 = typeof(TwitterStatus).GetProperty(name).GetValue(status, null);
                }
                if (obj3 == null)
                {
                    obj3 = "[" + name + "]";
                }
                this._values.Add(obj3.ToString());
            }
            MatchEvaluator evaluator = new MatchEvaluator(this.FillStatusValue);
            return regex.Replace(input, evaluator);
        }

        private string RenderLayout(IList<TwitterStatus> statuses)
        {
            string str = this.RenderEvalScripts(this._twitter.LayoutTemplate, statuses[0]);
            if (string.IsNullOrEmpty(str))
            {
                str = this.PersistTemplate(this._twitter.LayoutTemplate);
            }
            string pattern = @"<(asp:)\b([^>]*?)(ITEMPLACEHOLDER)([^>]*?)(>([^>]*?)</asp:PlaceHolder>|/>)(.*?)";
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
            string replacement = "";
            foreach (TwitterStatus status in statuses)
            {
                replacement = replacement + this.RenderStatus(status);
            }
            return regex.Replace(str, replacement);
        }

        private string RenderStatus(TwitterStatus status)
        {
            string str = this.RenderEvalScripts(this._twitter.StatusTemplate, status);
            if (string.IsNullOrEmpty(str))
            {
                ListViewDataItem container = new ListViewDataItem(0, 0) {
                    DataItem = status
                };
                this._twitter.StatusTemplate.InstantiateIn(container);
                container.DataBind();
                str = RenderControl(container);
            }
            return str;
        }

        public override bool AllowResize =>
            true;

        protected override bool Visible =>
            true;
    }
}

