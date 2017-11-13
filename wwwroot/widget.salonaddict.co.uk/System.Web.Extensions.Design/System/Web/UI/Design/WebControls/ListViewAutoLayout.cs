namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel.Design;
    using System.Globalization;
    using System.Web.UI;
    using System.Web.UI.Design;
    using System.Web.UI.WebControls;

    internal abstract class ListViewAutoLayout
    {
        private IDesignerHost _designerHost;
        private IDataSourceFieldSchema[] _fieldSchema;
        protected const string BoolEditItemTemplateString = "<asp:CheckBox Checked='<%# {1} %>' runat=\"server\" id=\"{2}CheckBox\" Text=\"{0}\" />";
        protected const string BoolEditItemTemplateStringNoLabel = "<asp:CheckBox Checked='<%# {1} %>' runat=\"server\" id=\"{2}CheckBox\" />";
        protected const string BoolInsertItemTemplateString = "<asp:CheckBox Checked='<%# {1} %>' runat=\"server\" id=\"{2}CheckBox\" Text=\"{0}\" />";
        protected const string BoolInsertItemTemplateStringNoLabel = "<asp:CheckBox Checked='<%# {1} %>' runat=\"server\" id=\"{2}CheckBox\" />";
        protected const string BoolItemTemplateString = "<asp:CheckBox Checked='<%# {1} %>' runat=\"server\" id=\"{2}CheckBox\" Enabled=\"false\" Text=\"{0}\" />";
        protected const string BoolItemTemplateStringNoLabel = "<asp:CheckBox Checked='<%# {1} %>' runat=\"server\" id=\"{2}CheckBox\" Enabled=\"false\" />";
        protected const string BreakString = "<br />";
        protected const string ButtonString = "<asp:Button runat=\"server\" CommandName=\"{0}\" Text=\"{1}\" id=\"{0}Button\"/>";
        protected const string DynamicInsertItemTemplateString = "{0}: <asp:DynamicControl DataField=\"{0}\" Mode=\"Insert\" ValidationGroup=\"Insert\" runat=\"server\" />";
        protected const string DynamicInsertItemTemplateStringNoLabel = "<asp:DynamicControl DataField=\"{0}\" Mode=\"Insert\" ValidationGroup=\"Insert\" runat=\"server\" />";
        protected const string DynamicItemTemplateString = "{0}: <asp:DynamicControl DataField=\"{0}\" Mode=\"{1}\" runat=\"server\" />";
        protected const string DynamicItemTemplateStringNoLabel = "<asp:DynamicControl DataField=\"{0}\" Mode=\"{1}\" runat=\"server\" />";
        protected const string EditItemTemplateString = "{0}: <asp:TextBox Text='<%# {1} %>' runat=\"server\" id=\"{2}TextBox\" />";
        protected const string EditItemTemplateStringNoLabel = "<asp:TextBox Text='<%# {1} %>' runat=\"server\" id=\"{2}TextBox\" />";
        protected const string GroupPlaceholderContainerID = "groupPlaceholderContainer";
        protected const string InsertButtonWithValidationGroupString = "<asp:Button runat=\"server\" CommandName=\"Insert\" Text=\"{0}\" id=\"InsertButton\" ValidationGroup=\"Insert\"/>";
        protected const string InsertItemTemplateString = "{0}: <asp:TextBox Text='<%# {1} %>' runat=\"server\" id=\"{2}TextBox\" />";
        protected const string InsertItemTemplateStringNoLabel = "<asp:TextBox Text='<%# {1} %>' runat=\"server\" id=\"{2}TextBox\" />";
        protected const string ItemPlaceholderContainerID = "itemPlaceholderContainer";
        protected const string ItemTemplateString = "{0}: <asp:Label Text='<%# {1} %>' runat=\"server\" id=\"{2}Label\" />";
        protected const string ItemTemplateStringNoLabel = "<asp:Label Text='<%# {1} %>' runat=\"server\" id=\"{2}Label\" />";
        protected const string KeyEditItemTemplateString = "{0}: <asp:Label Text='<%# {1} %>' runat=\"server\" id=\"{2}Label1\" />";
        protected const string KeyEditItemTemplateStringNoLabel = "<asp:Label Text='<%# {1} %>' runat=\"server\" id=\"{2}Label1\" />";
        protected const string KeyItemTemplateString = "{0}: <asp:Label Text='<%# {1} %>' runat=\"server\" id=\"{2}Label\" />";
        protected const string KeyItemTemplateStringNoLabel = "<asp:Label Text='<%# {1} %>' runat=\"server\" id=\"{2}Label\" />";

        public ListViewAutoLayout(IDesignerHost designerHost, IDataSourceFieldSchema[] fieldSchema)
        {
            this._fieldSchema = fieldSchema;
            this._designerHost = designerHost;
        }

        public virtual void ApplyLayout(ListView listView, string styleID, bool enableDeleting, bool enableEditing, bool enableInserting, DesignerPagerStyle pagerStyle, bool isDynamic)
        {
            ListViewAutoStyle style = null;
            foreach (ListViewAutoStyle style2 in this.Styles)
            {
                if (style2.ID == styleID)
                {
                    style = style2;
                    break;
                }
            }
            if (style != null)
            {
                if (enableInserting)
                {
                    listView.InsertItemPosition = InsertItemPosition.LastItem;
                }
                else
                {
                    listView.InsertItemPosition = InsertItemPosition.None;
                }
                listView.GroupItemCount = 1;
                bool hasButtons = (enableEditing || enableDeleting) || enableInserting;
                listView.LayoutTemplate = this.CreateLayoutTemplate(style, pagerStyle, listView.ItemPlaceholderID, listView.GroupPlaceholderID, hasButtons);
                listView.ItemTemplate = this.CreateItemTemplate(style, enableDeleting, enableEditing, hasButtons, isDynamic);
                listView.AlternatingItemTemplate = this.CreateAlternatingItemTemplate(style, enableDeleting, enableEditing, hasButtons, isDynamic);
                listView.SelectedItemTemplate = this.CreateSelectedItemTemplate(style, enableDeleting, enableEditing, hasButtons, isDynamic);
                listView.ItemSeparatorTemplate = this.CreateItemSeparatorTemplate(style);
                listView.GroupTemplate = this.CreateGroupTemplate(style, listView.ItemPlaceholderID);
                listView.GroupSeparatorTemplate = this.CreateGroupSeparatorTemplate(style);
                listView.EditItemTemplate = this.CreateEditItemTemplate(style, isDynamic);
                listView.InsertItemTemplate = this.CreateInsertItemTemplate(style, isDynamic);
                listView.EmptyDataTemplate = this.CreateEmptyDataTemplate(style);
                listView.EmptyItemTemplate = this.CreateEmptyItemTemplate(style);
            }
        }

        protected virtual ITemplate CreateAlternatingItemTemplate(ListViewAutoStyle style, bool enableDeleting, bool enableEditing, bool hasButtons, bool isDynamic) => 
            null;

        protected abstract ITemplate CreateEditItemTemplate(ListViewAutoStyle style, bool isDynamic);
        protected abstract ITemplate CreateEmptyDataTemplate(ListViewAutoStyle style);
        protected virtual ITemplate CreateEmptyItemTemplate(ListViewAutoStyle style) => 
            null;

        protected virtual ITemplate CreateGroupSeparatorTemplate(ListViewAutoStyle style) => 
            null;

        protected virtual ITemplate CreateGroupTemplate(ListViewAutoStyle style, string itemPlaceholderID) => 
            null;

        protected abstract ITemplate CreateInsertItemTemplate(ListViewAutoStyle style, bool isDynamic);
        protected virtual ITemplate CreateItemSeparatorTemplate(ListViewAutoStyle style) => 
            null;

        protected abstract ITemplate CreateItemTemplate(ListViewAutoStyle style, bool enableDeleting, bool enableEditing, bool hasButtons, bool isDynamic);
        protected abstract ITemplate CreateLayoutTemplate(ListViewAutoStyle style, DesignerPagerStyle pagerStyle, string itemPlaceholderID, string groupPlaceholderID, bool hasButtons);
        protected virtual ITemplate CreateSelectedItemTemplate(ListViewAutoStyle style, bool enableDeleting, bool enableEditing, bool hasButtons, bool isDynamic) => 
            null;

        protected string GetButtonString(string command, string text) => 
            string.Format(CultureInfo.InvariantCulture, "<asp:Button runat=\"server\" CommandName=\"{0}\" Text=\"{1}\" id=\"{0}Button\"/>", new object[] { command, text });

        protected static string GetFieldDerivedID(string fieldName)
        {
            char[] chArray = new char[fieldName.Length];
            for (int i = 0; i < fieldName.Length; i++)
            {
                char c = fieldName[i];
                if (char.IsLetterOrDigit(c) || (c == '_'))
                {
                    chArray[i] = c;
                }
                else
                {
                    chArray[i] = '_';
                }
            }
            return new string(chArray);
        }

        protected string GetInsertButtonWithValidationGroupString(string text) => 
            string.Format(CultureInfo.InvariantCulture, "<asp:Button runat=\"server\" CommandName=\"Insert\" Text=\"{0}\" id=\"InsertButton\" ValidationGroup=\"Insert\"/>", new object[] { text });

        public override string ToString() => 
            this.LayoutName;

        protected IDesignerHost DesignerHost =>
            this._designerHost;

        protected IDataSourceFieldSchema[] FieldSchema =>
            this._fieldSchema;

        public abstract string ID { get; }

        public abstract string LayoutName { get; }

        public abstract Collection<ListViewAutoStyle> Styles { get; }
    }
}

