namespace System.Web.UI.WebControls
{
    using System;
    using System.ComponentModel;
    using System.Drawing.Design;
    using System.Globalization;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.Resources;
    using System.Web.UI;
    using System.Web.UI.Design;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class NextPreviousPagerField : DataPagerField
    {
        private int _maximumRows;
        private int _startRowIndex;
        private int _totalRowCount;

        private void AddNonBreakingSpace(DataPagerFieldItem container)
        {
            if (this.RenderNonBreakingSpacesBetweenControls)
            {
                container.Controls.Add(new LiteralControl("&nbsp;"));
            }
        }

        protected override void CopyProperties(DataPagerField newField)
        {
            ((NextPreviousPagerField) newField).ButtonCssClass = this.ButtonCssClass;
            ((NextPreviousPagerField) newField).ButtonType = this.ButtonType;
            ((NextPreviousPagerField) newField).FirstPageImageUrl = this.FirstPageImageUrl;
            ((NextPreviousPagerField) newField).FirstPageText = this.FirstPageText;
            ((NextPreviousPagerField) newField).LastPageImageUrl = this.LastPageImageUrl;
            ((NextPreviousPagerField) newField).LastPageText = this.LastPageText;
            ((NextPreviousPagerField) newField).NextPageImageUrl = this.NextPageImageUrl;
            ((NextPreviousPagerField) newField).NextPageText = this.NextPageText;
            ((NextPreviousPagerField) newField).PreviousPageImageUrl = this.PreviousPageImageUrl;
            ((NextPreviousPagerField) newField).PreviousPageText = this.PreviousPageText;
            ((NextPreviousPagerField) newField).ShowFirstPageButton = this.ShowFirstPageButton;
            ((NextPreviousPagerField) newField).ShowLastPageButton = this.ShowLastPageButton;
            ((NextPreviousPagerField) newField).ShowNextPageButton = this.ShowNextPageButton;
            ((NextPreviousPagerField) newField).ShowPreviousPageButton = this.ShowPreviousPageButton;
            base.CopyProperties(newField);
        }

        private Control CreateControl(string commandName, string buttonText, int fieldIndex, string imageUrl, bool enabled)
        {
            IButtonControl control;
            if (!enabled && this.RenderDisabledButtonsAsLabels)
            {
                Label label = new Label {
                    Text = buttonText
                };
                if (!string.IsNullOrEmpty(this.ButtonCssClass))
                {
                    label.CssClass = this.ButtonCssClass;
                }
                return label;
            }
            switch (this.ButtonType)
            {
                case System.Web.UI.WebControls.ButtonType.Button:
                    control = new Button();
                    ((Button) control).Enabled = enabled;
                    break;

                case System.Web.UI.WebControls.ButtonType.Link:
                    control = new LinkButton();
                    ((LinkButton) control).Enabled = enabled;
                    break;

                default:
                    control = new ImageButton();
                    ((ImageButton) control).ImageUrl = imageUrl;
                    ((ImageButton) control).Enabled = enabled;
                    ((ImageButton) control).AlternateText = HttpUtility.HtmlDecode(buttonText);
                    break;
            }
            control.Text = buttonText;
            control.CausesValidation = false;
            control.CommandName = commandName;
            control.CommandArgument = fieldIndex.ToString(CultureInfo.InvariantCulture);
            WebControl control2 = control as WebControl;
            if ((control2 != null) && !string.IsNullOrEmpty(this.ButtonCssClass))
            {
                control2.CssClass = this.ButtonCssClass;
            }
            return (control as Control);
        }

        public override void CreateDataPagers(DataPagerFieldItem container, int startRowIndex, int maximumRows, int totalRowCount, int fieldIndex)
        {
            this._startRowIndex = startRowIndex;
            this._maximumRows = maximumRows;
            this._totalRowCount = totalRowCount;
            if (string.IsNullOrEmpty(base.DataPager.QueryStringField))
            {
                this.CreateDataPagersForCommand(container, fieldIndex);
            }
            else
            {
                this.CreateDataPagersForQueryString(container, fieldIndex);
            }
        }

        private void CreateDataPagersForCommand(DataPagerFieldItem container, int fieldIndex)
        {
            if (this.ShowFirstPageButton)
            {
                container.Controls.Add(this.CreateControl("First", this.FirstPageText, fieldIndex, this.FirstPageImageUrl, this.EnablePreviousPage));
                this.AddNonBreakingSpace(container);
            }
            if (this.ShowPreviousPageButton)
            {
                container.Controls.Add(this.CreateControl("Prev", this.PreviousPageText, fieldIndex, this.PreviousPageImageUrl, this.EnablePreviousPage));
                this.AddNonBreakingSpace(container);
            }
            if (this.ShowNextPageButton)
            {
                container.Controls.Add(this.CreateControl("Next", this.NextPageText, fieldIndex, this.NextPageImageUrl, this.EnableNextPage));
                this.AddNonBreakingSpace(container);
            }
            if (this.ShowLastPageButton)
            {
                container.Controls.Add(this.CreateControl("Last", this.LastPageText, fieldIndex, this.LastPageImageUrl, this.EnableNextPage));
                this.AddNonBreakingSpace(container);
            }
        }

        private void CreateDataPagersForQueryString(DataPagerFieldItem container, int fieldIndex)
        {
            bool flag = false;
            if (!base.QueryStringHandled)
            {
                int num;
                base.QueryStringHandled = true;
                if (int.TryParse(base.QueryStringValue, out num))
                {
                    num--;
                    int num1 = this._startRowIndex / this._maximumRows;
                    int num2 = (this._totalRowCount - 1) / this._maximumRows;
                    if ((num >= 0) && (num <= num2))
                    {
                        this._startRowIndex = num * this._maximumRows;
                        flag = true;
                    }
                }
            }
            if (this.ShowFirstPageButton)
            {
                container.Controls.Add(this.CreateLink(this.FirstPageText, 0, this.FirstPageImageUrl, this.EnablePreviousPage));
                this.AddNonBreakingSpace(container);
            }
            if (this.ShowPreviousPageButton)
            {
                int pageIndex = (this._startRowIndex / this._maximumRows) - 1;
                container.Controls.Add(this.CreateLink(this.PreviousPageText, pageIndex, this.PreviousPageImageUrl, this.EnablePreviousPage));
                this.AddNonBreakingSpace(container);
            }
            if (this.ShowNextPageButton)
            {
                int num4 = (this._startRowIndex + this._maximumRows) / this._maximumRows;
                container.Controls.Add(this.CreateLink(this.NextPageText, num4, this.NextPageImageUrl, this.EnableNextPage));
                this.AddNonBreakingSpace(container);
            }
            if (this.ShowLastPageButton)
            {
                int num5 = (this._totalRowCount / this._maximumRows) - (((this._totalRowCount % this._maximumRows) == 0) ? 1 : 0);
                container.Controls.Add(this.CreateLink(this.LastPageText, num5, this.LastPageImageUrl, this.EnableNextPage));
                this.AddNonBreakingSpace(container);
            }
            if (flag)
            {
                base.DataPager.SetPageProperties(this._startRowIndex, this._maximumRows, true);
            }
        }

        protected override DataPagerField CreateField() => 
            new NextPreviousPagerField();

        private HyperLink CreateLink(string buttonText, int pageIndex, string imageUrl, bool enabled)
        {
            int pageNumber = pageIndex + 1;
            HyperLink link = new HyperLink {
                Text = buttonText,
                NavigateUrl = base.GetQueryStringNavigateUrl(pageNumber),
                ImageUrl = imageUrl,
                Enabled = enabled
            };
            if (!string.IsNullOrEmpty(this.ButtonCssClass))
            {
                link.CssClass = this.ButtonCssClass;
            }
            return link;
        }

        public override bool Equals(object o)
        {
            NextPreviousPagerField field = o as NextPreviousPagerField;
            return (((((field != null) && string.Equals(field.ButtonCssClass, this.ButtonCssClass)) && ((field.ButtonType == this.ButtonType) && string.Equals(field.FirstPageImageUrl, this.FirstPageImageUrl))) && ((string.Equals(field.FirstPageText, this.FirstPageText) && string.Equals(field.LastPageImageUrl, this.LastPageImageUrl)) && (string.Equals(field.LastPageText, this.LastPageText) && string.Equals(field.NextPageImageUrl, this.NextPageImageUrl)))) && (((string.Equals(field.NextPageText, this.NextPageText) && string.Equals(field.PreviousPageImageUrl, this.PreviousPageImageUrl)) && (string.Equals(field.PreviousPageText, this.PreviousPageText) && (field.ShowFirstPageButton == this.ShowFirstPageButton))) && (((field.ShowLastPageButton == this.ShowLastPageButton) && (field.ShowNextPageButton == this.ShowNextPageButton)) && (field.ShowPreviousPageButton == this.ShowPreviousPageButton))));
        }

        public override int GetHashCode() => 
            (((((((((((((this.ButtonCssClass.GetHashCode() | this.ButtonType.GetHashCode()) | this.FirstPageImageUrl.GetHashCode()) | this.FirstPageText.GetHashCode()) | this.LastPageImageUrl.GetHashCode()) | this.LastPageText.GetHashCode()) | this.NextPageImageUrl.GetHashCode()) | this.NextPageText.GetHashCode()) | this.PreviousPageImageUrl.GetHashCode()) | this.PreviousPageText.GetHashCode()) | this.ShowFirstPageButton.GetHashCode()) | this.ShowLastPageButton.GetHashCode()) | this.ShowNextPageButton.GetHashCode()) | this.ShowPreviousPageButton.GetHashCode());

        public override void HandleEvent(CommandEventArgs e)
        {
            if (string.IsNullOrEmpty(base.DataPager.QueryStringField))
            {
                if (string.Equals(e.CommandName, "Prev"))
                {
                    int startRowIndex = this._startRowIndex - base.DataPager.PageSize;
                    if (startRowIndex < 0)
                    {
                        startRowIndex = 0;
                    }
                    base.DataPager.SetPageProperties(startRowIndex, base.DataPager.PageSize, true);
                }
                else if (string.Equals(e.CommandName, "Next"))
                {
                    int num2 = this._startRowIndex + base.DataPager.PageSize;
                    if (num2 > this._totalRowCount)
                    {
                        num2 = this._totalRowCount - base.DataPager.PageSize;
                    }
                    base.DataPager.SetPageProperties(num2, base.DataPager.PageSize, true);
                }
                else if (string.Equals(e.CommandName, "First"))
                {
                    base.DataPager.SetPageProperties(0, base.DataPager.PageSize, true);
                }
                else if (string.Equals(e.CommandName, "Last"))
                {
                    int num3;
                    int num4 = this._totalRowCount % base.DataPager.PageSize;
                    if (num4 == 0)
                    {
                        num3 = this._totalRowCount - base.DataPager.PageSize;
                    }
                    else
                    {
                        num3 = this._totalRowCount - num4;
                    }
                    base.DataPager.SetPageProperties(num3, base.DataPager.PageSize, true);
                }
            }
        }

        [ResourceDescription("NextPreviousPagerField_ButtonCssClass"), Category("Appearance"), DefaultValue(""), CssClassProperty]
        public string ButtonCssClass
        {
            get
            {
                object obj2 = base.ViewState["ButtonCssClass"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return string.Empty;
            }
            set
            {
                if (value != this.ButtonCssClass)
                {
                    base.ViewState["ButtonCssClass"] = value;
                    this.OnFieldChanged();
                }
            }
        }

        [Category("Appearance"), ResourceDescription("NextPreviousPagerField_ButtonType"), DefaultValue(2)]
        public System.Web.UI.WebControls.ButtonType ButtonType
        {
            get
            {
                object obj2 = base.ViewState["ButtonType"];
                if (obj2 != null)
                {
                    return (System.Web.UI.WebControls.ButtonType) obj2;
                }
                return System.Web.UI.WebControls.ButtonType.Link;
            }
            set
            {
                if ((value < System.Web.UI.WebControls.ButtonType.Button) || (value > System.Web.UI.WebControls.ButtonType.Link))
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                if (value != this.ButtonType)
                {
                    base.ViewState["ButtonType"] = value;
                    this.OnFieldChanged();
                }
            }
        }

        private bool EnableNextPage =>
            ((this._startRowIndex + this._maximumRows) < this._totalRowCount);

        private bool EnablePreviousPage =>
            (this._startRowIndex > 0);

        [Category("Appearance"), UrlProperty, DefaultValue(""), Editor(typeof(ImageUrlEditor), typeof(UITypeEditor)), ResourceDescription("NextPreviousPagerField_FirstPageImageUrl")]
        public string FirstPageImageUrl
        {
            get
            {
                object obj2 = base.ViewState["FirstPageImageUrl"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return string.Empty;
            }
            set
            {
                if (value != this.FirstPageImageUrl)
                {
                    base.ViewState["FirstPageImageUrl"] = value;
                    this.OnFieldChanged();
                }
            }
        }

        [ResourceDescription("NextPreviousPagerField_FirstPageText"), Category("Appearance"), Localizable(true), ResourceDefaultValue("NextPrevPagerField_DefaultFirstPageText")]
        public string FirstPageText
        {
            get
            {
                object obj2 = base.ViewState["FirstPageText"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return AtlasWeb.NextPrevPagerField_DefaultFirstPageText;
            }
            set
            {
                if (value != this.FirstPageText)
                {
                    base.ViewState["FirstPageText"] = value;
                    this.OnFieldChanged();
                }
            }
        }

        [DefaultValue(""), Editor(typeof(ImageUrlEditor), typeof(UITypeEditor)), ResourceDescription("NextPreviousPagerField_LastPageImageUrl"), UrlProperty, Category("Appearance")]
        public string LastPageImageUrl
        {
            get
            {
                object obj2 = base.ViewState["LastPageImageUrl"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return string.Empty;
            }
            set
            {
                if (value != this.LastPageImageUrl)
                {
                    base.ViewState["LastPageImageUrl"] = value;
                    this.OnFieldChanged();
                }
            }
        }

        [ResourceDescription("NextPreviousPagerField_LastPageText"), Localizable(true), ResourceDefaultValue("NextPrevPagerField_DefaultLastPageText"), Category("Appearance")]
        public string LastPageText
        {
            get
            {
                object obj2 = base.ViewState["LastPageText"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return AtlasWeb.NextPrevPagerField_DefaultLastPageText;
            }
            set
            {
                if (value != this.LastPageText)
                {
                    base.ViewState["LastPageText"] = value;
                    this.OnFieldChanged();
                }
            }
        }

        [UrlProperty, ResourceDescription("NextPreviousPagerField_NextPageImageUrl"), Category("Appearance"), DefaultValue(""), Editor("System.Web.UI.Design.ImageUrlEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        public string NextPageImageUrl
        {
            get
            {
                object obj2 = base.ViewState["NextPageImageUrl"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return string.Empty;
            }
            set
            {
                if (value != this.NextPageImageUrl)
                {
                    base.ViewState["NextPageImageUrl"] = value;
                    this.OnFieldChanged();
                }
            }
        }

        [ResourceDescription("NextPreviousPagerField_NextPageText"), Category("Appearance"), Localizable(true), ResourceDefaultValue("NextPrevPagerField_DefaultNextPageText")]
        public string NextPageText
        {
            get
            {
                object obj2 = base.ViewState["NextPageText"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return AtlasWeb.NextPrevPagerField_DefaultNextPageText;
            }
            set
            {
                if (value != this.NextPageText)
                {
                    base.ViewState["NextPageText"] = value;
                    this.OnFieldChanged();
                }
            }
        }

        [Editor(typeof(ImageUrlEditor), typeof(UITypeEditor)), UrlProperty, ResourceDescription("NextPreviousPagerField_PreviousPageImageUrl"), Category("Appearance"), DefaultValue("")]
        public string PreviousPageImageUrl
        {
            get
            {
                object obj2 = base.ViewState["PreviousPageImageUrl"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return string.Empty;
            }
            set
            {
                if (value != this.PreviousPageImageUrl)
                {
                    base.ViewState["PreviousPageImageUrl"] = value;
                    this.OnFieldChanged();
                }
            }
        }

        [Category("Appearance"), ResourceDefaultValue("NextPrevPagerField_DefaultPreviousPageText"), ResourceDescription("NextPreviousPagerField_PreviousPageText"), Localizable(true)]
        public string PreviousPageText
        {
            get
            {
                object obj2 = base.ViewState["PreviousPageText"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return AtlasWeb.NextPrevPagerField_DefaultPreviousPageText;
            }
            set
            {
                if (value != this.PreviousPageText)
                {
                    base.ViewState["PreviousPageText"] = value;
                    this.OnFieldChanged();
                }
            }
        }

        [Category("Behavior"), ResourceDescription("NextPreviousPagerField_RenderDisabledButtonsAsLabels"), DefaultValue(false)]
        public bool RenderDisabledButtonsAsLabels
        {
            get
            {
                object obj2 = base.ViewState["RenderDisabledButtonsAsLabels"];
                return ((obj2 != null) && ((bool) obj2));
            }
            set
            {
                if (value != this.RenderDisabledButtonsAsLabels)
                {
                    base.ViewState["RenderDisabledButtonsAsLabels"] = value;
                    this.OnFieldChanged();
                }
            }
        }

        [ResourceDescription("NextPreviousPagerField_RenderNonBreakingSpacesBetweenControls"), DefaultValue(true), Category("Behavior")]
        public bool RenderNonBreakingSpacesBetweenControls
        {
            get
            {
                object obj2 = base.ViewState["RenderNonBreakingSpacesBetweenControls"];
                if (obj2 != null)
                {
                    return (bool) obj2;
                }
                return true;
            }
            set
            {
                if (value != this.RenderNonBreakingSpacesBetweenControls)
                {
                    base.ViewState["RenderNonBreakingSpacesBetweenControls"] = value;
                    this.OnFieldChanged();
                }
            }
        }

        [Category("Behavior"), ResourceDescription("NextPreviousPagerField_ShowFirstPageButton"), DefaultValue(false)]
        public bool ShowFirstPageButton
        {
            get
            {
                object obj2 = base.ViewState["ShowFirstPageButton"];
                return ((obj2 != null) && ((bool) obj2));
            }
            set
            {
                if (value != this.ShowFirstPageButton)
                {
                    base.ViewState["ShowFirstPageButton"] = value;
                    this.OnFieldChanged();
                }
            }
        }

        [DefaultValue(false), Category("Behavior"), ResourceDescription("NextPreviousPagerField_ShowLastPageButton")]
        public bool ShowLastPageButton
        {
            get
            {
                object obj2 = base.ViewState["ShowLastPageButton"];
                return ((obj2 != null) && ((bool) obj2));
            }
            set
            {
                if (value != this.ShowLastPageButton)
                {
                    base.ViewState["ShowLastPageButton"] = value;
                    this.OnFieldChanged();
                }
            }
        }

        [DefaultValue(true), ResourceDescription("NextPreviousPagerField_ShowNextPageButton"), Category("Behavior")]
        public bool ShowNextPageButton
        {
            get
            {
                object obj2 = base.ViewState["ShowNextPageButton"];
                if (obj2 != null)
                {
                    return (bool) obj2;
                }
                return true;
            }
            set
            {
                if (value != this.ShowNextPageButton)
                {
                    base.ViewState["ShowNextPageButton"] = value;
                    this.OnFieldChanged();
                }
            }
        }

        [Category("Behavior"), DefaultValue(true), ResourceDescription("NextPreviousPagerField_ShowPreviousPageButton")]
        public bool ShowPreviousPageButton
        {
            get
            {
                object obj2 = base.ViewState["ShowPreviousPageButton"];
                if (obj2 != null)
                {
                    return (bool) obj2;
                }
                return true;
            }
            set
            {
                if (value != this.ShowPreviousPageButton)
                {
                    base.ViewState["ShowPreviousPageButton"] = value;
                    this.OnFieldChanged();
                }
            }
        }
    }
}

