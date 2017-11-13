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

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class NumericPagerField : DataPagerField
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
            ((NumericPagerField) newField).ButtonCount = this.ButtonCount;
            ((NumericPagerField) newField).ButtonType = this.ButtonType;
            ((NumericPagerField) newField).CurrentPageLabelCssClass = this.CurrentPageLabelCssClass;
            ((NumericPagerField) newField).NextPageImageUrl = this.NextPageImageUrl;
            ((NumericPagerField) newField).NextPageText = this.NextPageText;
            ((NumericPagerField) newField).NextPreviousButtonCssClass = this.NextPreviousButtonCssClass;
            ((NumericPagerField) newField).NumericButtonCssClass = this.NumericButtonCssClass;
            ((NumericPagerField) newField).PreviousPageImageUrl = this.PreviousPageImageUrl;
            ((NumericPagerField) newField).PreviousPageText = this.PreviousPageText;
            base.CopyProperties(newField);
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
            int num = this._startRowIndex / this._maximumRows;
            int num2 = (this._startRowIndex / (this.ButtonCount * this._maximumRows)) * this.ButtonCount;
            int num3 = (num2 + this.ButtonCount) - 1;
            int num4 = ((num3 + 1) * this._maximumRows) - 1;
            if (num2 != 0)
            {
                container.Controls.Add(this.CreateNextPrevButton(this.PreviousPageText, "Prev", fieldIndex.ToString(CultureInfo.InvariantCulture), this.PreviousPageImageUrl));
                this.AddNonBreakingSpace(container);
            }
            for (int i = 0; (i < this.ButtonCount) && (this._totalRowCount > ((i + num2) * this._maximumRows)); i++)
            {
                if ((i + num2) == num)
                {
                    Label child = new Label();
                    child.Text = ((i + num2) + 1).ToString(CultureInfo.InvariantCulture);
                    if (!string.IsNullOrEmpty(this.CurrentPageLabelCssClass))
                    {
                        child.CssClass = this.CurrentPageLabelCssClass;
                    }
                    container.Controls.Add(child);
                }
                else
                {
                    int num7 = (i + num2) + 1;
                    int num8 = i + num2;
                    container.Controls.Add(this.CreateNumericButton(num7.ToString(CultureInfo.InvariantCulture), fieldIndex.ToString(CultureInfo.InvariantCulture), num8.ToString(CultureInfo.InvariantCulture)));
                }
                this.AddNonBreakingSpace(container);
            }
            if (num4 < (this._totalRowCount - 1))
            {
                this.AddNonBreakingSpace(container);
                container.Controls.Add(this.CreateNextPrevButton(this.NextPageText, "Next", fieldIndex.ToString(CultureInfo.InvariantCulture), this.NextPageImageUrl));
                this.AddNonBreakingSpace(container);
            }
        }

        private void CreateDataPagersForQueryString(DataPagerFieldItem container, int fieldIndex)
        {
            int num = this._startRowIndex / this._maximumRows;
            bool flag = false;
            if (!base.QueryStringHandled)
            {
                int num2;
                base.QueryStringHandled = true;
                if (int.TryParse(base.QueryStringValue, out num2))
                {
                    num2--;
                    int num3 = (this._totalRowCount - 1) / this._maximumRows;
                    if ((num2 >= 0) && (num2 <= num3))
                    {
                        num = num2;
                        this._startRowIndex = num * this._maximumRows;
                        flag = true;
                    }
                }
            }
            int num4 = (this._startRowIndex / (this.ButtonCount * this._maximumRows)) * this.ButtonCount;
            int num5 = (num4 + this.ButtonCount) - 1;
            int num6 = ((num5 + 1) * this._maximumRows) - 1;
            if (num4 != 0)
            {
                container.Controls.Add(this.CreateNextPrevLink(this.PreviousPageText, num4 - 1, this.PreviousPageImageUrl));
                this.AddNonBreakingSpace(container);
            }
            for (int i = 0; (i < this.ButtonCount) && (this._totalRowCount > ((i + num4) * this._maximumRows)); i++)
            {
                if ((i + num4) == num)
                {
                    Label child = new Label();
                    child.Text = ((i + num4) + 1).ToString(CultureInfo.InvariantCulture);
                    if (!string.IsNullOrEmpty(this.CurrentPageLabelCssClass))
                    {
                        child.CssClass = this.CurrentPageLabelCssClass;
                    }
                    container.Controls.Add(child);
                }
                else
                {
                    container.Controls.Add(this.CreateNumericLink(i + num4));
                }
                this.AddNonBreakingSpace(container);
            }
            if (num6 < (this._totalRowCount - 1))
            {
                this.AddNonBreakingSpace(container);
                container.Controls.Add(this.CreateNextPrevLink(this.NextPageText, num4 + this.ButtonCount, this.NextPageImageUrl));
                this.AddNonBreakingSpace(container);
            }
            if (flag)
            {
                base.DataPager.SetPageProperties(this._startRowIndex, this._maximumRows, true);
            }
        }

        protected override DataPagerField CreateField() => 
            new NumericPagerField();

        private Control CreateNextPrevButton(string buttonText, string commandName, string commandArgument, string imageUrl)
        {
            IButtonControl control;
            switch (this.ButtonType)
            {
                case System.Web.UI.WebControls.ButtonType.Button:
                    control = new Button();
                    break;

                case System.Web.UI.WebControls.ButtonType.Link:
                    control = new LinkButton();
                    break;

                default:
                    control = new ImageButton();
                    ((ImageButton) control).ImageUrl = imageUrl;
                    ((ImageButton) control).AlternateText = HttpUtility.HtmlDecode(buttonText);
                    break;
            }
            control.Text = buttonText;
            control.CausesValidation = false;
            control.CommandName = commandName;
            control.CommandArgument = commandArgument;
            WebControl control2 = control as WebControl;
            if ((control2 != null) && !string.IsNullOrEmpty(this.NextPreviousButtonCssClass))
            {
                control2.CssClass = this.NextPreviousButtonCssClass;
            }
            return (control as Control);
        }

        private HyperLink CreateNextPrevLink(string buttonText, int pageIndex, string imageUrl)
        {
            int pageNumber = pageIndex + 1;
            HyperLink link = new HyperLink {
                Text = buttonText,
                NavigateUrl = base.GetQueryStringNavigateUrl(pageNumber),
                ImageUrl = imageUrl
            };
            if (!string.IsNullOrEmpty(this.NextPreviousButtonCssClass))
            {
                link.CssClass = this.NextPreviousButtonCssClass;
            }
            return link;
        }

        private Control CreateNumericButton(string buttonText, string commandArgument, string commandName)
        {
            IButtonControl control;
            switch (this.ButtonType)
            {
                case System.Web.UI.WebControls.ButtonType.Button:
                    control = new Button();
                    break;

                default:
                    control = new LinkButton();
                    break;
            }
            control.Text = buttonText;
            control.CausesValidation = false;
            control.CommandName = commandName;
            control.CommandArgument = commandArgument;
            WebControl control2 = control as WebControl;
            if ((control2 != null) && !string.IsNullOrEmpty(this.NumericButtonCssClass))
            {
                control2.CssClass = this.NumericButtonCssClass;
            }
            return (control as Control);
        }

        private HyperLink CreateNumericLink(int pageIndex)
        {
            int pageNumber = pageIndex + 1;
            HyperLink link = new HyperLink {
                Text = pageNumber.ToString(CultureInfo.InvariantCulture),
                NavigateUrl = base.GetQueryStringNavigateUrl(pageNumber)
            };
            if (!string.IsNullOrEmpty(this.NumericButtonCssClass))
            {
                link.CssClass = this.NumericButtonCssClass;
            }
            return link;
        }

        public override bool Equals(object o)
        {
            NumericPagerField field = o as NumericPagerField;
            return (((((field != null) && object.Equals(field.ButtonCount, this.ButtonCount)) && ((field.ButtonType == this.ButtonType) && string.Equals(field.CurrentPageLabelCssClass, this.CurrentPageLabelCssClass))) && ((string.Equals(field.NextPageImageUrl, this.NextPageImageUrl) && string.Equals(field.NextPageText, this.NextPageText)) && (string.Equals(field.NextPreviousButtonCssClass, this.NextPreviousButtonCssClass) && string.Equals(field.NumericButtonCssClass, this.NumericButtonCssClass)))) && (string.Equals(field.PreviousPageImageUrl, this.PreviousPageImageUrl) && string.Equals(field.PreviousPageText, this.PreviousPageText)));
        }

        public override int GetHashCode() => 
            ((((((((this.ButtonCount.GetHashCode() | this.ButtonType.GetHashCode()) | this.CurrentPageLabelCssClass.GetHashCode()) | this.NextPageImageUrl.GetHashCode()) | this.NextPageText.GetHashCode()) | this.NextPreviousButtonCssClass.GetHashCode()) | this.NumericButtonCssClass.GetHashCode()) | this.PreviousPageImageUrl.GetHashCode()) | this.PreviousPageText.GetHashCode());

        public override void HandleEvent(CommandEventArgs e)
        {
            if (string.IsNullOrEmpty(base.DataPager.QueryStringField))
            {
                int startRowIndex = -1;
                int num1 = this._startRowIndex / base.DataPager.PageSize;
                int num2 = (this._startRowIndex / (this.ButtonCount * base.DataPager.PageSize)) * this.ButtonCount;
                int num3 = (num2 + this.ButtonCount) - 1;
                int num4 = ((num3 + 1) * base.DataPager.PageSize) - 1;
                if (string.Equals(e.CommandName, "Prev"))
                {
                    startRowIndex = (num2 - 1) * base.DataPager.PageSize;
                    if (startRowIndex < 0)
                    {
                        startRowIndex = 0;
                    }
                }
                else if (string.Equals(e.CommandName, "Next"))
                {
                    startRowIndex = num4 + 1;
                    if (startRowIndex > this._totalRowCount)
                    {
                        startRowIndex = this._totalRowCount - base.DataPager.PageSize;
                    }
                }
                else
                {
                    startRowIndex = Convert.ToInt32(e.CommandName, CultureInfo.InvariantCulture) * base.DataPager.PageSize;
                }
                if (startRowIndex != -1)
                {
                    base.DataPager.SetPageProperties(startRowIndex, base.DataPager.PageSize, true);
                }
            }
        }

        [ResourceDescription("NumericPagerField_ButtonCount"), DefaultValue(5), Category("Appearance")]
        public int ButtonCount
        {
            get
            {
                object obj2 = base.ViewState["ButtonCount"];
                if (obj2 != null)
                {
                    return (int) obj2;
                }
                return 5;
            }
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                if (value != this.ButtonCount)
                {
                    base.ViewState["ButtonCount"] = value;
                    this.OnFieldChanged();
                }
            }
        }

        [DefaultValue(2), Category("Appearance"), ResourceDescription("NumericPagerField_ButtonType")]
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

        [CssClassProperty, Category("Appearance"), ResourceDescription("NumericPagerField_CurrentPageLabelCssClass"), DefaultValue("")]
        public string CurrentPageLabelCssClass
        {
            get
            {
                object obj2 = base.ViewState["CurrentPageLabelCssClass"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return string.Empty;
            }
            set
            {
                if (value != this.CurrentPageLabelCssClass)
                {
                    base.ViewState["CurrentPageLabelCssClass"] = value;
                    this.OnFieldChanged();
                }
            }
        }

        [DefaultValue(""), UrlProperty, Category("Appearance"), Editor(typeof(ImageUrlEditor), typeof(UITypeEditor)), ResourceDescription("NumericPagerField_NextPageImageUrl")]
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

        [ResourceDescription("NumericPagerField_NextPageText"), Category("Appearance"), Localizable(true), ResourceDefaultValue("NumericPagerField_DefaultNextPageText")]
        public string NextPageText
        {
            get
            {
                object obj2 = base.ViewState["NextPageText"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return AtlasWeb.NumericPagerField_DefaultNextPageText;
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

        [DefaultValue(""), Category("Appearance"), CssClassProperty, ResourceDescription("NumericPagerField_NextPreviousButtonCssClass")]
        public string NextPreviousButtonCssClass
        {
            get
            {
                object obj2 = base.ViewState["NextPreviousButtonCssClass"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return string.Empty;
            }
            set
            {
                if (value != this.NextPreviousButtonCssClass)
                {
                    base.ViewState["NextPreviousButtonCssClass"] = value;
                    this.OnFieldChanged();
                }
            }
        }

        [Category("Appearance"), DefaultValue(""), ResourceDescription("NumericPagerField_NumericButtonCssClass"), CssClassProperty]
        public string NumericButtonCssClass
        {
            get
            {
                object obj2 = base.ViewState["NumericButtonCssClass"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return string.Empty;
            }
            set
            {
                if (value != this.NumericButtonCssClass)
                {
                    base.ViewState["NumericButtonCssClass"] = value;
                    this.OnFieldChanged();
                }
            }
        }

        [UrlProperty, ResourceDescription("NumericPagerField_PreviousPageImageUrl"), Category("Appearance"), DefaultValue(""), Editor(typeof(ImageUrlEditor), typeof(UITypeEditor))]
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

        [ResourceDefaultValue("NumericPagerField_DefaultPreviousPageText"), Localizable(true), ResourceDescription("NumericPagerField_PreviousPageText"), Category("Appearance")]
        public string PreviousPageText
        {
            get
            {
                object obj2 = base.ViewState["PreviousPageText"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return AtlasWeb.NumericPagerField_DefaultPreviousPageText;
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

        [Category("Behavior"), ResourceDescription("NumericPagerField_RenderNonBreakingSpacesBetweenControls"), DefaultValue(true)]
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
    }
}

