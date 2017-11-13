namespace System.Web.UI.WebControls
{
    using System;
    using System.ComponentModel;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class TemplateColumn : DataGridColumn
    {
        private ITemplate editItemTemplate;
        private ITemplate footerTemplate;
        private ITemplate headerTemplate;
        private ITemplate itemTemplate;

        public override void InitializeCell(TableCell cell, int columnIndex, ListItemType itemType)
        {
            base.InitializeCell(cell, columnIndex, itemType);
            ITemplate itemTemplate = null;
            switch (itemType)
            {
                case ListItemType.Header:
                    itemTemplate = this.headerTemplate;
                    goto Label_0057;

                case ListItemType.Footer:
                    itemTemplate = this.footerTemplate;
                    goto Label_0057;

                case ListItemType.Item:
                case ListItemType.AlternatingItem:
                case ListItemType.SelectedItem:
                    break;

                case ListItemType.EditItem:
                    if (this.editItemTemplate == null)
                    {
                        break;
                    }
                    itemTemplate = this.editItemTemplate;
                    goto Label_0057;

                default:
                    goto Label_0057;
            }
            itemTemplate = this.itemTemplate;
        Label_0057:
            if (itemTemplate != null)
            {
                cell.Text = string.Empty;
                itemTemplate.InstantiateIn(cell);
            }
            else if (((itemType == ListItemType.Item) || (itemType == ListItemType.AlternatingItem)) || ((itemType == ListItemType.SelectedItem) || (itemType == ListItemType.EditItem)))
            {
                cell.Text = "&nbsp;";
            }
        }

        [TemplateContainer(typeof(DataGridItem)), Browsable(false), DefaultValue((string) null), WebSysDescription("TemplateColumn_EditItemTemplate"), PersistenceMode(PersistenceMode.InnerProperty)]
        public virtual ITemplate EditItemTemplate
        {
            get => 
                this.editItemTemplate;
            set
            {
                this.editItemTemplate = value;
                this.OnColumnChanged();
            }
        }

        [PersistenceMode(PersistenceMode.InnerProperty), TemplateContainer(typeof(DataGridItem)), DefaultValue((string) null), WebSysDescription("TemplateColumn_FooterTemplate"), Browsable(false)]
        public virtual ITemplate FooterTemplate
        {
            get => 
                this.footerTemplate;
            set
            {
                this.footerTemplate = value;
                this.OnColumnChanged();
            }
        }

        [Browsable(false), TemplateContainer(typeof(DataGridItem)), DefaultValue((string) null), WebSysDescription("TemplateColumn_HeaderTemplate"), PersistenceMode(PersistenceMode.InnerProperty)]
        public virtual ITemplate HeaderTemplate
        {
            get => 
                this.headerTemplate;
            set
            {
                this.headerTemplate = value;
                this.OnColumnChanged();
            }
        }

        [WebSysDescription("TemplateColumn_ItemTemplate"), PersistenceMode(PersistenceMode.InnerProperty), TemplateContainer(typeof(DataGridItem)), DefaultValue((string) null), Browsable(false)]
        public virtual ITemplate ItemTemplate
        {
            get => 
                this.itemTemplate;
            set
            {
                this.itemTemplate = value;
                this.OnColumnChanged();
            }
        }
    }
}

