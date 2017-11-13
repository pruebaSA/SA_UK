﻿namespace System.Web.UI.WebControls
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;

    [Bindable(false), Designer("System.Web.UI.Design.WebControls.PreviewControlDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), ToolboxItem(false), ParseChildren(true, "Cells"), DefaultProperty("Cells"), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class TableRow : WebControl
    {
        private TableCellCollection cells;

        public TableRow() : base(HtmlTextWriterTag.Tr)
        {
            base.PreventAutoID();
        }

        protected override ControlCollection CreateControlCollection() => 
            new CellControlCollection(this);

        protected override Style CreateControlStyle() => 
            new TableItemStyle(this.ViewState);

        [PersistenceMode(PersistenceMode.InnerDefaultProperty), MergableProperty(false), WebSysDescription("TableRow_Cells")]
        public virtual TableCellCollection Cells
        {
            get
            {
                if (this.cells == null)
                {
                    this.cells = new TableCellCollection(this);
                }
                return this.cells;
            }
        }

        [DefaultValue(0), WebSysDescription("TableItem_HorizontalAlign"), WebCategory("Layout")]
        public virtual System.Web.UI.WebControls.HorizontalAlign HorizontalAlign
        {
            get
            {
                if (!base.ControlStyleCreated)
                {
                    return System.Web.UI.WebControls.HorizontalAlign.NotSet;
                }
                return ((TableItemStyle) base.ControlStyle).HorizontalAlign;
            }
            set
            {
                ((TableItemStyle) base.ControlStyle).HorizontalAlign = value;
            }
        }

        [WebSysDescription("TableRow_TableSection"), DefaultValue(1), WebCategory("Accessibility")]
        public virtual TableRowSection TableSection
        {
            get
            {
                object obj2 = this.ViewState["TableSection"];
                if (obj2 != null)
                {
                    return (TableRowSection) obj2;
                }
                return TableRowSection.TableBody;
            }
            set
            {
                if ((value < TableRowSection.TableHeader) || (value > TableRowSection.TableFooter))
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                this.ViewState["TableSection"] = value;
                if (value != TableRowSection.TableBody)
                {
                    Control parent = this.Parent;
                    if (parent != null)
                    {
                        Table table = parent as Table;
                        if (table != null)
                        {
                            table.HasRowSections = true;
                        }
                    }
                }
            }
        }

        [DefaultValue(0), WebCategory("Layout"), WebSysDescription("TableItem_VerticalAlign")]
        public virtual System.Web.UI.WebControls.VerticalAlign VerticalAlign
        {
            get
            {
                if (!base.ControlStyleCreated)
                {
                    return System.Web.UI.WebControls.VerticalAlign.NotSet;
                }
                return ((TableItemStyle) base.ControlStyle).VerticalAlign;
            }
            set
            {
                ((TableItemStyle) base.ControlStyle).VerticalAlign = value;
            }
        }

        protected class CellControlCollection : ControlCollection
        {
            internal CellControlCollection(Control owner) : base(owner)
            {
            }

            public override void Add(Control child)
            {
                if (!(child is TableCell))
                {
                    throw new ArgumentException(System.Web.SR.GetString("Cannot_Have_Children_Of_Type", new object[] { "TableRow", child.GetType().Name.ToString(CultureInfo.InvariantCulture) }));
                }
                base.Add(child);
            }

            public override void AddAt(int index, Control child)
            {
                if (!(child is TableCell))
                {
                    throw new ArgumentException(System.Web.SR.GetString("Cannot_Have_Children_Of_Type", new object[] { "TableRow", child.GetType().Name.ToString(CultureInfo.InvariantCulture) }));
                }
                base.AddAt(index, child);
            }
        }
    }
}

