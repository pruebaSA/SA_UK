﻿namespace System.Web.UI.HtmlControls
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;

    [ParseChildren(true, "Cells"), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class HtmlTableRow : HtmlContainerControl
    {
        private HtmlTableCellCollection cells;

        public HtmlTableRow() : base("tr")
        {
        }

        protected override ControlCollection CreateControlCollection() => 
            new HtmlTableCellControlCollection(this);

        protected internal override void RenderChildren(HtmlTextWriter writer)
        {
            writer.WriteLine();
            writer.Indent++;
            base.RenderChildren(writer);
            writer.Indent--;
        }

        protected override void RenderEndTag(HtmlTextWriter writer)
        {
            base.RenderEndTag(writer);
            writer.WriteLine();
        }

        [WebCategory("Layout"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), DefaultValue("")]
        public string Align
        {
            get
            {
                string str = base.Attributes["align"];
                if (str == null)
                {
                    return string.Empty;
                }
                return str;
            }
            set
            {
                base.Attributes["align"] = HtmlControl.MapStringAttributeToString(value);
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), WebCategory("Appearance"), DefaultValue("")]
        public string BgColor
        {
            get
            {
                string str = base.Attributes["bgcolor"];
                if (str == null)
                {
                    return string.Empty;
                }
                return str;
            }
            set
            {
                base.Attributes["bgcolor"] = HtmlControl.MapStringAttributeToString(value);
            }
        }

        [DefaultValue(""), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), WebCategory("Appearance")]
        public string BorderColor
        {
            get
            {
                string str = base.Attributes["bordercolor"];
                if (str == null)
                {
                    return string.Empty;
                }
                return str;
            }
            set
            {
                base.Attributes["bordercolor"] = HtmlControl.MapStringAttributeToString(value);
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual HtmlTableCellCollection Cells
        {
            get
            {
                if (this.cells == null)
                {
                    this.cells = new HtmlTableCellCollection(this);
                }
                return this.cells;
            }
        }

        [WebCategory("Layout"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), DefaultValue("")]
        public string Height
        {
            get
            {
                string str = base.Attributes["height"];
                if (str == null)
                {
                    return string.Empty;
                }
                return str;
            }
            set
            {
                base.Attributes["height"] = HtmlControl.MapStringAttributeToString(value);
            }
        }

        public override string InnerHtml
        {
            get
            {
                throw new NotSupportedException(System.Web.SR.GetString("InnerHtml_not_supported", new object[] { base.GetType().Name }));
            }
            set
            {
                throw new NotSupportedException(System.Web.SR.GetString("InnerHtml_not_supported", new object[] { base.GetType().Name }));
            }
        }

        public override string InnerText
        {
            get
            {
                throw new NotSupportedException(System.Web.SR.GetString("InnerText_not_supported", new object[] { base.GetType().Name }));
            }
            set
            {
                throw new NotSupportedException(System.Web.SR.GetString("InnerText_not_supported", new object[] { base.GetType().Name }));
            }
        }

        [DefaultValue(""), WebCategory("Layout"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string VAlign
        {
            get
            {
                string str = base.Attributes["valign"];
                if (str == null)
                {
                    return string.Empty;
                }
                return str;
            }
            set
            {
                base.Attributes["valign"] = HtmlControl.MapStringAttributeToString(value);
            }
        }

        protected class HtmlTableCellControlCollection : ControlCollection
        {
            internal HtmlTableCellControlCollection(Control owner) : base(owner)
            {
            }

            public override void Add(Control child)
            {
                if (!(child is HtmlTableCell))
                {
                    throw new ArgumentException(System.Web.SR.GetString("Cannot_Have_Children_Of_Type", new object[] { "HtmlTableRow", child.GetType().Name.ToString(CultureInfo.InvariantCulture) }));
                }
                base.Add(child);
            }

            public override void AddAt(int index, Control child)
            {
                if (!(child is HtmlTableCell))
                {
                    throw new ArgumentException(System.Web.SR.GetString("Cannot_Have_Children_Of_Type", new object[] { "HtmlTableRow", child.GetType().Name.ToString(CultureInfo.InvariantCulture) }));
                }
                base.AddAt(index, child);
            }
        }
    }
}

