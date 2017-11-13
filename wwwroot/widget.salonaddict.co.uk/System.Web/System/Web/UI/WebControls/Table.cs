namespace System.Web.UI.WebControls
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Design;
    using System.Globalization;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;

    [SupportsEventValidation, Designer("System.Web.UI.Design.WebControls.TableDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), ParseChildren(true, "Rows"), DefaultProperty("Rows"), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class Table : WebControl, IPostBackEventHandler
    {
        private bool _hasRowSections;
        private TableRowCollection _rows;

        public Table() : base(HtmlTextWriterTag.Table)
        {
        }

        protected override void AddAttributesToRender(HtmlTextWriter writer)
        {
            base.AddAttributesToRender(writer);
            string str = "0";
            if (base.ControlStyleCreated)
            {
                if (base.EnableLegacyRendering || (writer is Html32TextWriter))
                {
                    Color borderColor = this.BorderColor;
                    if (!borderColor.IsEmpty)
                    {
                        writer.AddAttribute(HtmlTextWriterAttribute.Bordercolor, ColorTranslator.ToHtml(borderColor));
                    }
                }
                Unit borderWidth = this.BorderWidth;
                if (this.GridLines != System.Web.UI.WebControls.GridLines.None)
                {
                    if (borderWidth.IsEmpty || (borderWidth.Type != UnitType.Pixel))
                    {
                        str = "1";
                    }
                    else
                    {
                        str = ((int) borderWidth.Value).ToString(NumberFormatInfo.InvariantInfo);
                    }
                }
            }
            writer.AddAttribute(HtmlTextWriterAttribute.Border, str);
        }

        protected override ControlCollection CreateControlCollection() => 
            new RowControlCollection(this);

        protected override Style CreateControlStyle() => 
            new TableStyle(this.ViewState);

        protected virtual void RaisePostBackEvent(string argument)
        {
            base.ValidateEvent(this.UniqueID, argument);
            if (base._adapter != null)
            {
                IPostBackEventHandler handler = base._adapter as IPostBackEventHandler;
                if (handler != null)
                {
                    handler.RaisePostBackEvent(argument);
                }
            }
        }

        public override void RenderBeginTag(HtmlTextWriter writer)
        {
            base.RenderBeginTag(writer);
            string caption = this.Caption;
            if (caption.Length != 0)
            {
                TableCaptionAlign captionAlign = this.CaptionAlign;
                if (captionAlign != TableCaptionAlign.NotSet)
                {
                    string str2 = "Right";
                    switch (captionAlign)
                    {
                        case TableCaptionAlign.Top:
                            str2 = "Top";
                            break;

                        case TableCaptionAlign.Bottom:
                            str2 = "Bottom";
                            break;

                        case TableCaptionAlign.Left:
                            str2 = "Left";
                            break;
                    }
                    writer.AddAttribute(HtmlTextWriterAttribute.Align, str2);
                }
                writer.RenderBeginTag(HtmlTextWriterTag.Caption);
                writer.Write(caption);
                writer.RenderEndTag();
            }
        }

        protected internal override void RenderContents(HtmlTextWriter writer)
        {
            TableRowCollection rows = this.Rows;
            if (rows.Count > 0)
            {
                if (this.HasRowSections)
                {
                    TableRowSection tableHeader = TableRowSection.TableHeader;
                    bool flag = false;
                    foreach (TableRow row in rows)
                    {
                        if (row.TableSection < tableHeader)
                        {
                            throw new HttpException(System.Web.SR.GetString("Table_SectionsMustBeInOrder", new object[] { this.ID }));
                        }
                        if ((tableHeader < row.TableSection) || ((row.TableSection == TableRowSection.TableHeader) && !flag))
                        {
                            if (flag)
                            {
                                writer.RenderEndTag();
                            }
                            tableHeader = row.TableSection;
                            flag = true;
                            switch (tableHeader)
                            {
                                case TableRowSection.TableHeader:
                                    writer.RenderBeginTag(HtmlTextWriterTag.Thead);
                                    break;

                                case TableRowSection.TableBody:
                                    writer.RenderBeginTag(HtmlTextWriterTag.Tbody);
                                    break;

                                case TableRowSection.TableFooter:
                                    writer.RenderBeginTag(HtmlTextWriterTag.Tfoot);
                                    break;
                            }
                        }
                        row.RenderControl(writer);
                    }
                    writer.RenderEndTag();
                }
                else
                {
                    foreach (TableRow row2 in rows)
                    {
                        row2.RenderControl(writer);
                    }
                }
            }
        }

        void IPostBackEventHandler.RaisePostBackEvent(string eventArgument)
        {
            this.RaisePostBackEvent(eventArgument);
        }

        [WebSysDescription("Table_BackImageUrl"), UrlProperty, WebCategory("Appearance"), DefaultValue(""), Editor("System.Web.UI.Design.ImageUrlEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        public virtual string BackImageUrl
        {
            get
            {
                if (!base.ControlStyleCreated)
                {
                    return string.Empty;
                }
                return ((TableStyle) base.ControlStyle).BackImageUrl;
            }
            set
            {
                ((TableStyle) base.ControlStyle).BackImageUrl = value;
            }
        }

        [WebSysDescription("Table_Caption"), Localizable(true), WebCategory("Accessibility"), DefaultValue("")]
        public virtual string Caption
        {
            get
            {
                string str = (string) this.ViewState["Caption"];
                if (str == null)
                {
                    return string.Empty;
                }
                return str;
            }
            set
            {
                this.ViewState["Caption"] = value;
            }
        }

        [DefaultValue(0), WebCategory("Accessibility"), WebSysDescription("WebControl_CaptionAlign")]
        public virtual TableCaptionAlign CaptionAlign
        {
            get
            {
                object obj2 = this.ViewState["CaptionAlign"];
                if (obj2 == null)
                {
                    return TableCaptionAlign.NotSet;
                }
                return (TableCaptionAlign) obj2;
            }
            set
            {
                if ((value < TableCaptionAlign.NotSet) || (value > TableCaptionAlign.Right))
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                this.ViewState["CaptionAlign"] = value;
            }
        }

        [WebCategory("Appearance"), DefaultValue(-1), WebSysDescription("Table_CellPadding")]
        public virtual int CellPadding
        {
            get
            {
                if (!base.ControlStyleCreated)
                {
                    return -1;
                }
                return ((TableStyle) base.ControlStyle).CellPadding;
            }
            set
            {
                ((TableStyle) base.ControlStyle).CellPadding = value;
            }
        }

        [WebSysDescription("Table_CellSpacing"), DefaultValue(-1), WebCategory("Appearance")]
        public virtual int CellSpacing
        {
            get
            {
                if (!base.ControlStyleCreated)
                {
                    return -1;
                }
                return ((TableStyle) base.ControlStyle).CellSpacing;
            }
            set
            {
                ((TableStyle) base.ControlStyle).CellSpacing = value;
            }
        }

        [DefaultValue(0), WebSysDescription("Table_GridLines"), WebCategory("Appearance")]
        public virtual System.Web.UI.WebControls.GridLines GridLines
        {
            get
            {
                if (!base.ControlStyleCreated)
                {
                    return System.Web.UI.WebControls.GridLines.None;
                }
                return ((TableStyle) base.ControlStyle).GridLines;
            }
            set
            {
                ((TableStyle) base.ControlStyle).GridLines = value;
            }
        }

        internal bool HasRowSections
        {
            get => 
                this._hasRowSections;
            set
            {
                this._hasRowSections = value;
            }
        }

        [WebCategory("Layout"), DefaultValue(0), WebSysDescription("Table_HorizontalAlign")]
        public virtual System.Web.UI.WebControls.HorizontalAlign HorizontalAlign
        {
            get
            {
                if (!base.ControlStyleCreated)
                {
                    return System.Web.UI.WebControls.HorizontalAlign.NotSet;
                }
                return ((TableStyle) base.ControlStyle).HorizontalAlign;
            }
            set
            {
                ((TableStyle) base.ControlStyle).HorizontalAlign = value;
            }
        }

        [MergableProperty(false), WebSysDescription("Table_Rows"), PersistenceMode(PersistenceMode.InnerDefaultProperty)]
        public virtual TableRowCollection Rows
        {
            get
            {
                if (this._rows == null)
                {
                    this._rows = new TableRowCollection(this);
                }
                return this._rows;
            }
        }

        protected class RowControlCollection : ControlCollection
        {
            internal RowControlCollection(Control owner) : base(owner)
            {
            }

            public override void Add(Control child)
            {
                if (!(child is TableRow))
                {
                    throw new ArgumentException(System.Web.SR.GetString("Cannot_Have_Children_Of_Type", new object[] { "Table", child.GetType().Name.ToString(CultureInfo.InvariantCulture) }));
                }
                base.Add(child);
            }

            public override void AddAt(int index, Control child)
            {
                if (!(child is TableRow))
                {
                    throw new ArgumentException(System.Web.SR.GetString("Cannot_Have_Children_Of_Type", new object[] { "Table", child.GetType().Name.ToString(CultureInfo.InvariantCulture) }));
                }
                base.AddAt(index, child);
            }
        }
    }
}

