namespace IFRAME.Controllers.Controls
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;

    public class IFRMPager : Control, INamingContainer
    {
        public event PageCreatedHandler PageCreated;

        protected override void CreateChildControls()
        {
            this.Controls.Clear();
            this.CreatePrevious();
            this.CreatePages();
            this.CreateNext();
            if (this.TotalRowCount < this.PageSize)
            {
                this.Visible = false;
            }
        }

        private void CreateNext()
        {
            if (this.PageIndex < this.TotalPages)
            {
                this.Controls.Add(new LiteralControl("&nbsp;&nbsp;"));
                HtmlGenericControl child = new HtmlGenericControl("span");
                HtmlAnchor anchor = new HtmlAnchor {
                    InnerHtml = this.NextButtonText
                };
                anchor.Attributes.Add("class", "next");
                if (this.PageCreated != null)
                {
                    PageCreatedEventArgs e = new PageCreatedEventArgs {
                        Control = anchor,
                        NewPageIndex = this.PageIndex + 1
                    };
                    this.PageCreated(this, e);
                }
                child.Controls.Add(anchor);
                this.Controls.Add(child);
            }
        }

        private void CreatePages()
        {
            for (int i = 1; i < (this.TotalPages + 1); i++)
            {
                HtmlGenericControl child = new HtmlGenericControl("span");
                HtmlAnchor anchor = new HtmlAnchor {
                    InnerHtml = i.ToString()
                };
                if (i == this.PageIndex)
                {
                    anchor.Attributes.Add("class", "selected");
                    anchor.Attributes.Add("href", "javascript:void(0)");
                }
                else if (this.PageCreated != null)
                {
                    PageCreatedEventArgs e = new PageCreatedEventArgs {
                        Control = anchor,
                        NewPageIndex = i
                    };
                    this.PageCreated(this, e);
                }
                child.Controls.Add(anchor);
                this.Controls.Add(child);
            }
        }

        private void CreatePrevious()
        {
            if (this.PageIndex > 1)
            {
                HtmlGenericControl child = new HtmlGenericControl("span");
                HtmlAnchor anchor = new HtmlAnchor {
                    InnerHtml = this.PreviousButtonText
                };
                anchor.Attributes.Add("class", "previous");
                if (this.PageCreated != null)
                {
                    PageCreatedEventArgs e = new PageCreatedEventArgs {
                        Control = anchor,
                        NewPageIndex = this.PageIndex - 1
                    };
                    this.PageCreated(this, e);
                }
                child.Controls.Add(anchor);
                this.Controls.Add(child);
                this.Controls.Add(new LiteralControl("&nbsp;&nbsp;"));
            }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            string str = "div";
            writer.Write("<{0}", str);
            if (!string.IsNullOrEmpty(this.CssClass))
            {
                writer.Write(" class=\"{0}\"", this.CssClass);
            }
            writer.Write(">");
            base.Render(writer);
            writer.Write("</{0}>", str);
        }

        public virtual string CssClass
        {
            get => 
                (((string) this.ViewState["CssClass"]) ?? string.Empty);
            set
            {
                this.ViewState["CssClass"] = value;
            }
        }

        [Localizable(true)]
        public string NextButtonText
        {
            get => 
                (((string) this.ViewState["NextButtonText"]) ?? "Next");
            set
            {
                this.ViewState["NextButtonText"] = value;
            }
        }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        [Localizable(true)]
        public string PreviousButtonText
        {
            get => 
                (((string) this.ViewState["PreviousButtonText"]) ?? "Previous");
            set
            {
                this.ViewState["PreviousButtonText"] = value;
            }
        }

        public virtual int TotalPages
        {
            get
            {
                if ((this.TotalRowCount == 0) || (this.PageSize == 0))
                {
                    return 0;
                }
                int num = this.TotalRowCount / this.PageSize;
                if ((this.TotalRowCount % this.PageSize) > 0)
                {
                    num++;
                }
                return num;
            }
        }

        public virtual int TotalRowCount
        {
            get
            {
                if (this.ViewState["TotalCount"] == null)
                {
                    return 0;
                }
                return (int) this.ViewState["TotalCount"];
            }
            set
            {
                this.ViewState["TotalCount"] = value;
            }
        }

        public delegate void PageCreatedHandler(object sender, PageCreatedEventArgs e);
    }
}

