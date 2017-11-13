namespace AjaxControlToolkit
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.Caching;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;

    [ParseChildren(ChildrenAsProperties=true), PersistChildren(false), Designer(typeof(TwitterDesigner)), ClientCssResource("Twitter.Twitter_resource.css")]
    public class Twitter : CompositeControl
    {
        private ListView _listView;

        public Twitter()
        {
            this.Mode = TwitterMode.Profile;
            this.CacheDuration = 300;
            this.Count = 5;
            this.CssClass = "ajax__twitter";
        }

        public static string ActivateLinks(string text)
        {
            string pattern = @"(((http|https)+\:\/\/)[&#95;.a-z0-9-]+\.[a-z0-9\/&#95;:@=.+?,##%&~-]*[^.|\'|\# |!|\(|?|,| |>|<|;|\)])";
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
            return regex.Replace(text, "<a href=\"$1\">$1</a>");
        }

        public static string Ago(DateTime date)
        {
            TimeSpan span = (TimeSpan) (DateTime.Now - date);
            if (span.TotalMinutes < 1.0)
            {
                return "Less than a minute ago";
            }
            if (Math.Round(span.TotalHours) < 2.0)
            {
                return $"{Math.Round(span.TotalMinutes)} minutes ago";
            }
            if (Math.Round(span.TotalDays) < 2.0)
            {
                return $"{Math.Round(span.TotalHours)} hours ago";
            }
            return $"{Math.Round(span.TotalDays)} days ago";
        }

        private void ControlPropertiesValid()
        {
            switch (this.Mode)
            {
                case TwitterMode.Profile:
                    if (string.IsNullOrEmpty(this.ScreenName))
                    {
                        throw new HttpException("ScreenName must have a value");
                    }
                    break;

                case TwitterMode.Search:
                    if (string.IsNullOrEmpty(this.Search))
                    {
                        throw new HttpException("Search must have a value");
                    }
                    break;

                default:
                    return;
            }
        }

        protected override void CreateChildControls()
        {
            this.Controls.Clear();
            this._listView = new ListView();
            this.Controls.Add(this._listView);
            this.PrepareTemplates();
        }

        private IList<TwitterStatus> GetProfile()
        {
            string key = $"__TwitterProfile_{this.ScreenName}_{this.Count}_{this.IncludeRetweets}_{this.IncludeReplies}";
            IList<TwitterStatus> list = (IList<TwitterStatus>) this.Context.Cache[key];
            if (list == null)
            {
                TwitterAPI rapi = new TwitterAPI();
                try
                {
                    list = rapi.GetProfile(this.ScreenName, this.Count, this.IncludeRetweets, this.IncludeReplies);
                }
                catch
                {
                    return null;
                }
                this.Context.Cache.Insert(key, list, null, DateTime.UtcNow.AddSeconds((double) this.CacheDuration), Cache.NoSlidingExpiration);
            }
            return list;
        }

        private IList<TwitterStatus> GetSearch()
        {
            string key = $"__TwitterSearch_{this.Search}_{this.Count}";
            IList<TwitterStatus> search = (IList<TwitterStatus>) this.Context.Cache[key];
            if (search == null)
            {
                TwitterAPI rapi = new TwitterAPI();
                try
                {
                    search = rapi.GetSearch(this.Search, this.Count);
                }
                catch
                {
                    return null;
                }
                this.Context.Cache.Insert(key, search, null, DateTime.UtcNow.AddSeconds((double) this.CacheDuration), Cache.NoSlidingExpiration);
            }
            return search;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            ScriptObjectBuilder.RegisterCssReferences(this);
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            this.ControlPropertiesValid();
            IList<TwitterStatus> profile = null;
            switch (this.Mode)
            {
                case TwitterMode.Profile:
                    profile = this.GetProfile();
                    if ((profile != null) && (profile.Count > 0))
                    {
                        TwitterUser user = profile[0].User;
                        this.Title = this.Title ?? user.Name;
                        this.Caption = this.Caption ?? user.ScreenName;
                        this.ProfileImageUrl = this.ProfileImageUrl ?? user.ProfileImageUrl;
                    }
                    break;

                case TwitterMode.Search:
                    profile = this.GetSearch();
                    break;
            }
            this._listView.DataSource = profile;
            this._listView.DataBind();
        }

        private void PrepareTemplates()
        {
            switch (this.Mode)
            {
                case TwitterMode.Profile:
                    if (this.LayoutTemplate == null)
                    {
                        this.LayoutTemplate = new DefaultProfileLayoutTemplate(this);
                    }
                    if (this.StatusTemplate == null)
                    {
                        this.StatusTemplate = new DefaultProfileStatusTemplate(this);
                    }
                    break;

                case TwitterMode.Search:
                    if (this.LayoutTemplate == null)
                    {
                        this.LayoutTemplate = new DefaultSearchLayoutTemplate(this);
                    }
                    if (this.StatusTemplate == null)
                    {
                        this.StatusTemplate = new DefaultSearchStatusTemplate(this);
                    }
                    break;
            }
            if (this.EmptyDataTemplate == null)
            {
                this.EmptyDataTemplate = new DefaultEmptyDataTemplate();
            }
            this._listView.LayoutTemplate = this.LayoutTemplate;
            this._listView.ItemTemplate = this.StatusTemplate;
            this._listView.AlternatingItemTemplate = this.AlternatingStatusTemplate;
            this._listView.EmptyDataTemplate = this.EmptyDataTemplate;
        }

        [TemplateContainer(typeof(ListViewItem)), Browsable(false), PersistenceMode(PersistenceMode.InnerProperty)]
        public ITemplate AlternatingStatusTemplate { get; set; }

        public int CacheDuration { get; set; }

        [Category("Search"), Description("Twitter Caption")]
        public string Caption { get; set; }

        public int Count { get; set; }

        [TemplateContainer(typeof(ListView)), PersistenceMode(PersistenceMode.InnerProperty), Browsable(false)]
        public ITemplate EmptyDataTemplate { get; set; }

        public bool IncludeReplies { get; set; }

        public bool IncludeRetweets { get; set; }

        [Browsable(true), Description("Enable get live content from twitter server at design time")]
        public bool IsLiveContentOnDesignMode { get; set; }

        [TemplateContainer(typeof(Twitter)), Browsable(false), PersistenceMode(PersistenceMode.InnerProperty)]
        public ITemplate LayoutTemplate { get; set; }

        public TwitterMode Mode { get; set; }

        [Category("Search"), Description("Twitter Profile Image Url")]
        public string ProfileImageUrl { get; set; }

        [Category("Profile"), Description("Twitter Screen Name used when Mode=Profile")]
        public string ScreenName { get; set; }

        public string Search { get; set; }

        [PersistenceMode(PersistenceMode.InnerProperty), Browsable(false), TemplateContainer(typeof(ListViewItem))]
        public ITemplate StatusTemplate { get; set; }

        protected override HtmlTextWriterTag TagKey =>
            HtmlTextWriterTag.Div;

        [Description("Twitter Title"), Category("Search")]
        public string Title { get; set; }

        internal sealed class DefaultEmptyDataTemplate : ITemplate
        {
            void ITemplate.InstantiateIn(Control container)
            {
                container.Controls.Add(new LiteralControl("There are no matching tweets."));
            }
        }

        internal sealed class DefaultProfileLayoutTemplate : ITemplate
        {
            private Twitter _twitter;

            public DefaultProfileLayoutTemplate(Twitter twitter)
            {
                this._twitter = twitter;
            }

            void ITemplate.InstantiateIn(Control container)
            {
                HtmlGenericControl child = new HtmlGenericControl("div");
                child.Attributes.Add("class", "ajax__twitter_header");
                container.Controls.Add(child);
                Image image = new Image {
                    ImageUrl = this._twitter.ProfileImageUrl
                };
                child.Controls.Add(image);
                HtmlGenericControl control2 = new HtmlGenericControl("h3") {
                    Controls = { new LiteralControl(this._twitter.Title) }
                };
                child.Controls.Add(control2);
                HtmlGenericControl control3 = new HtmlGenericControl("h4") {
                    Controls = { new LiteralControl(this._twitter.Caption) }
                };
                child.Controls.Add(control3);
                HtmlGenericControl control4 = new HtmlGenericControl("ul");
                control4.Attributes.Add("class", "ajax__twitter_itemlist");
                control4.Style.Add("margin", "0px");
                container.Controls.Add(control4);
                PlaceHolder holder = new PlaceHolder {
                    ID = "ItemPlaceholder"
                };
                control4.Controls.Add(holder);
                HtmlGenericControl control5 = new HtmlGenericControl("div");
                string webResourceUrl = this._twitter.Page.ClientScript.GetWebResourceUrl(this._twitter.GetType(), "Twitter.Twitter24.png");
                control5.Attributes.Add("class", "ajax__twitter_footer");
                Image image3 = new Image {
                    ImageUrl = webResourceUrl
                };
                control5.Controls.Add(image3);
                container.Controls.Add(control5);
            }
        }

        internal sealed class DefaultProfileStatusTemplate : ITemplate
        {
            private Twitter _twitter;

            internal DefaultProfileStatusTemplate(Twitter twitter)
            {
                this._twitter = twitter;
            }

            private void ctlStatus_DataBind(object sender, EventArgs e)
            {
                LiteralControl control = (LiteralControl) sender;
                ListViewDataItem namingContainer = (ListViewDataItem) control.NamingContainer;
                TwitterStatus dataItem = (TwitterStatus) namingContainer.DataItem;
                control.Text = $"<li>{Twitter.ActivateLinks(dataItem.Text)}<br /><span class="ajax__twitter_createat">{Twitter.Ago(dataItem.CreatedAt)}</span></li>";
            }

            void ITemplate.InstantiateIn(Control container)
            {
                LiteralControl child = new LiteralControl();
                child.DataBinding += new EventHandler(this.ctlStatus_DataBind);
                container.Controls.Add(child);
            }
        }

        internal sealed class DefaultSearchLayoutTemplate : ITemplate
        {
            private Twitter _twitter;

            public DefaultSearchLayoutTemplate(Twitter twitter)
            {
                this._twitter = twitter;
            }

            void ITemplate.InstantiateIn(Control container)
            {
                HtmlGenericControl child = new HtmlGenericControl("div");
                child.Attributes.Add("class", "ajax__twitter_header");
                container.Controls.Add(child);
                HtmlGenericControl control2 = new HtmlGenericControl("h3") {
                    Controls = { new LiteralControl(this._twitter.Title) }
                };
                child.Controls.Add(control2);
                HtmlGenericControl control3 = new HtmlGenericControl("h4") {
                    Controls = { new LiteralControl(this._twitter.Caption) }
                };
                child.Controls.Add(control3);
                HtmlGenericControl control4 = new HtmlGenericControl("ul");
                control4.Style.Add("margin", "0px");
                control4.Attributes.Add("class", "ajax__twitter_itemlist");
                container.Controls.Add(control4);
                PlaceHolder holder = new PlaceHolder {
                    ID = "ItemPlaceholder"
                };
                control4.Controls.Add(holder);
                HtmlGenericControl control5 = new HtmlGenericControl("div");
                string webResourceUrl = this._twitter.Page.ClientScript.GetWebResourceUrl(this._twitter.GetType(), "Twitter.Twitter24.png");
                control5.Attributes.Add("class", "ajax__twitter_footer");
                Image image = new Image {
                    ImageUrl = webResourceUrl
                };
                control5.Controls.Add(image);
                container.Controls.Add(control5);
            }
        }

        internal sealed class DefaultSearchStatusTemplate : ITemplate
        {
            private Twitter _twitter;

            internal DefaultSearchStatusTemplate(Twitter twitter)
            {
                this._twitter = twitter;
            }

            private void ctlStatus_DataBind(object sender, EventArgs e)
            {
                LiteralControl control = (LiteralControl) sender;
                ListViewDataItem namingContainer = (ListViewDataItem) control.NamingContainer;
                TwitterStatus dataItem = (TwitterStatus) namingContainer.DataItem;
                control.Text = $"<li><img src="{dataItem.User.ProfileImageUrl}" /><div>{dataItem.Text}<br /><span class="ajax__twitter_createat">{Twitter.Ago(dataItem.CreatedAt)}</span></div></li>";
            }

            void ITemplate.InstantiateIn(Control container)
            {
                LiteralControl child = new LiteralControl();
                child.DataBinding += new EventHandler(this.ctlStatus_DataBind);
                container.Controls.Add(child);
            }
        }
    }
}

