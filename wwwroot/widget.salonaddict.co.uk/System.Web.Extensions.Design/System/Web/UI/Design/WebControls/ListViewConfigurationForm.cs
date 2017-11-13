namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Drawing;
    using System.Security.Permissions;
    using System.Web.Resources.Design;
    using System.Web.UI.Design;
    using System.Web.UI.Design.Util;
    using System.Web.UI.WebControls;
    using System.Windows.Forms;

    [SecurityPermission(SecurityAction.Demand, Flags=SecurityPermissionFlag.UnmanagedCode)]
    internal sealed class ListViewConfigurationForm : DesignerForm
    {
        private System.Windows.Forms.Button _cancelButton;
        private ListViewDesigner _controlDesigner;
        private System.Windows.Forms.CheckBox _deletingCheckBox;
        private System.Windows.Forms.CheckBox _dynamicCheckBox;
        private System.Windows.Forms.CheckBox _editingCheckBox;
        private bool _firstActivate;
        private System.Windows.Forms.CheckBox _insertingCheckBox;
        private System.Windows.Forms.ListBox _layoutListBox;
        private System.Windows.Forms.Button _okButton;
        private System.Windows.Forms.Label _optionsLabel;
        private ComboBox _pagerComboBox;
        private System.Windows.Forms.CheckBox _pagingCheckBox;
        private System.Windows.Forms.Label _previewLabel;
        private System.Windows.Forms.Panel _previewPanel;
        private MSHTMLHost _schemePreview;
        private System.Windows.Forms.Label _selectLayoutLabel;
        private System.Windows.Forms.Label _selectStyleLabel;
        private System.Windows.Forms.ListBox _styleListBox;
        private IContainer components;

        public ListViewConfigurationForm(ListViewDesigner controlDesigner) : base(controlDesigner.Component.Site)
        {
            this._firstActivate = true;
            this._controlDesigner = controlDesigner;
            this.InitializeComponent();
            this.InitForm();
        }

        private void ApplySelectedStyle(System.Web.UI.WebControls.ListView listView)
        {
            ListViewAutoLayout selectedItem = this._layoutListBox.SelectedItem as ListViewAutoLayout;
            ListViewAutoStyle style = this._styleListBox.SelectedItem as ListViewAutoStyle;
            bool enableDeleting = this._deletingCheckBox.Enabled && this._deletingCheckBox.Checked;
            bool enableEditing = this._editingCheckBox.Enabled && this._editingCheckBox.Checked;
            bool enableInserting = this._insertingCheckBox.Enabled && this._insertingCheckBox.Checked;
            DesignerPagerStyle pagerStyle = new NullPagerStyle(this._controlDesigner.Component.Site);
            if (this._pagingCheckBox.Enabled && this._pagingCheckBox.Checked)
            {
                pagerStyle = this._pagerComboBox.SelectedItem as DesignerPagerStyle;
            }
            bool isDynamic = this._dynamicCheckBox.Visible && this._dynamicCheckBox.Checked;
            if ((selectedItem != null) && (style != null))
            {
                selectedItem.ApplyLayout(listView, style.ID, enableDeleting, enableEditing, enableInserting, pagerStyle, isDynamic);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void DoDelayLoadActions()
        {
            this._schemePreview.CreateTrident();
            this._schemePreview.ActivateTrident();
        }

        private System.Web.UI.WebControls.ListView GetPreviewListView()
        {
            System.Web.UI.WebControls.ListView listView = new System.Web.UI.WebControls.ListView {
                ItemPlaceholderID = this.ListView.ItemPlaceholderID,
                GroupPlaceholderID = this.ListView.GroupPlaceholderID
            };
            this.ApplySelectedStyle(listView);
            return listView;
        }

        private void InitForm()
        {
            base.SuspendLayout();
            this._cancelButton.Text = AtlasWebDesign.CancelCaption;
            this._okButton.Text = AtlasWebDesign.OKCaption;
            this._selectLayoutLabel.Text = AtlasWebDesign.ListViewConfigForm_SelectLayout;
            this._previewLabel.Text = AtlasWebDesign.ListViewConfigForm_Preview;
            this._selectStyleLabel.Text = AtlasWebDesign.ListViewConfigForm_SelectStyle;
            this._optionsLabel.Text = AtlasWebDesign.ListViewConfigForm_Options;
            this._editingCheckBox.Text = AtlasWebDesign.ListViewConfigForm_EnableEditing;
            this._insertingCheckBox.Text = AtlasWebDesign.ListViewConfigForm_EnableInserting;
            this._deletingCheckBox.Text = AtlasWebDesign.ListViewConfigForm_EnableDeleting;
            this._pagingCheckBox.Text = AtlasWebDesign.ListViewConfigForm_EnablePaging;
            this._dynamicCheckBox.Text = AtlasWebDesign.ListViewConfigForm_EnableDynamicData;
            this.Text = AtlasWebDesign.ListViewConfigForm_Title;
            this._deletingCheckBox.Enabled = this.DesignerView.CanDelete;
            this._editingCheckBox.Enabled = this.DesignerView.CanUpdate;
            this._insertingCheckBox.Enabled = this.DesignerView.CanInsert;
            this._pagerComboBox.Items.AddRange(new object[] { new NextPreviousPagerStyle(this._controlDesigner.Component.Site), new NumericPagerStyle(this._controlDesigner.Component.Site) });
            this._pagerComboBox.Enabled = this._pagingCheckBox.Checked;
            IDesignerHost service = (IDesignerHost) this._controlDesigner.Component.Site.GetService(typeof(IDesignerHost));
            WebFormsRootDesigner designer = service.GetDesigner(service.RootComponent) as WebFormsRootDesigner;
            if (designer != null)
            {
                WebFormsReferenceManager referenceManager = designer.ReferenceManager;
                this._dynamicCheckBox.Visible = referenceManager.GetType("asp", "DynamicControl") != null;
            }
            this.PopulateLayoutBox();
            this._layoutListBox.SelectedIndex = 0;
            base.InitializeForm();
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void InitializeComponent()
        {
            this._layoutListBox = new System.Windows.Forms.ListBox();
            this._selectLayoutLabel = new System.Windows.Forms.Label();
            this._previewLabel = new System.Windows.Forms.Label();
            this._selectStyleLabel = new System.Windows.Forms.Label();
            this._styleListBox = new System.Windows.Forms.ListBox();
            this._optionsLabel = new System.Windows.Forms.Label();
            this._editingCheckBox = new System.Windows.Forms.CheckBox();
            this._insertingCheckBox = new System.Windows.Forms.CheckBox();
            this._deletingCheckBox = new System.Windows.Forms.CheckBox();
            this._pagingCheckBox = new System.Windows.Forms.CheckBox();
            this._pagerComboBox = new ComboBox();
            this._dynamicCheckBox = new System.Windows.Forms.CheckBox();
            this._cancelButton = new System.Windows.Forms.Button();
            this._okButton = new System.Windows.Forms.Button();
            this._previewPanel = new System.Windows.Forms.Panel();
            this._schemePreview = new MSHTMLHost();
            base.SuspendLayout();
            this._layoutListBox.FormattingEnabled = true;
            this._layoutListBox.Location = new Point(12, 0x1c);
            this._layoutListBox.Name = "_layoutListBox";
            this._layoutListBox.Size = new Size(160, 0x5f);
            this._layoutListBox.TabIndex = 1;
            this._layoutListBox.TabStop = true;
            this._layoutListBox.SelectedIndexChanged += new EventHandler(this.OnSelectedLayoutChanged);
            this._selectLayoutLabel.Location = new Point(12, 12);
            this._selectLayoutLabel.Name = "_selectLayoutLabel";
            this._selectLayoutLabel.Size = new Size(160, 0x10);
            this._selectLayoutLabel.TabIndex = 0;
            this._selectLayoutLabel.TabStop = false;
            this._previewLabel.Location = new Point(0xcf, 12);
            this._previewLabel.Name = "_previewLabel";
            this._previewLabel.Size = new Size(300, 0x10);
            this._previewLabel.TabIndex = 11;
            this._selectStyleLabel.Location = new Point(12, 0x81);
            this._selectStyleLabel.Name = "_selectStyleLabel";
            this._selectStyleLabel.Size = new Size(160, 0x10);
            this._selectStyleLabel.TabIndex = 2;
            this._styleListBox.FormattingEnabled = true;
            this._styleListBox.Location = new Point(12, 0x91);
            this._styleListBox.Name = "_styleListBox";
            this._styleListBox.Size = new Size(160, 0x5f);
            this._styleListBox.TabIndex = 3;
            this._styleListBox.SelectedIndexChanged += new EventHandler(this.OnSelectedStyleChanged);
            this._optionsLabel.Location = new Point(12, 0xf6);
            this._optionsLabel.Name = "_optionsLabel";
            this._optionsLabel.Size = new Size(160, 0x10);
            this._optionsLabel.TabIndex = 4;
            this._editingCheckBox.Location = new Point(12, 0x106);
            this._editingCheckBox.Name = "_editingCheckBox";
            this._editingCheckBox.Size = new Size(160, 0x12);
            this._editingCheckBox.TabIndex = 5;
            this._editingCheckBox.CheckedChanged += new EventHandler(this.OnOptionChanged);
            this._insertingCheckBox.Location = new Point(12, 280);
            this._insertingCheckBox.Name = "_insertingCheckBox";
            this._insertingCheckBox.Size = new Size(160, 0x12);
            this._insertingCheckBox.TabIndex = 6;
            this._insertingCheckBox.CheckedChanged += new EventHandler(this.OnOptionChanged);
            this._deletingCheckBox.Location = new Point(12, 0x12a);
            this._deletingCheckBox.Name = "_deletingCheckBox";
            this._deletingCheckBox.Size = new Size(160, 0x12);
            this._deletingCheckBox.TabIndex = 7;
            this._deletingCheckBox.CheckedChanged += new EventHandler(this.OnOptionChanged);
            this._pagingCheckBox.Location = new Point(12, 0x13c);
            this._pagingCheckBox.Name = "_pagingCheckBox";
            this._pagingCheckBox.Size = new Size(160, 0x12);
            this._pagingCheckBox.TabIndex = 8;
            this._pagingCheckBox.CheckedChanged += new EventHandler(this.OnPagingCheckedChanged);
            this._pagerComboBox.FormattingEnabled = true;
            this._pagerComboBox.Location = new Point(0x21, 0x14f);
            this._pagerComboBox.Name = "_pagerComboBox";
            this._pagerComboBox.Size = new Size(0x8f, 0x15);
            this._pagerComboBox.TabIndex = 9;
            this._pagerComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            this._pagerComboBox.SelectedIndexChanged += new EventHandler(this.OnOptionChanged);
            this._dynamicCheckBox.Location = new Point(12, 0x164);
            this._dynamicCheckBox.Name = "_dynamicCheckBox";
            this._dynamicCheckBox.Size = new Size(160, 0x12);
            this._dynamicCheckBox.TabIndex = 10;
            this._dynamicCheckBox.Checked = false;
            this._dynamicCheckBox.Visible = false;
            this._dynamicCheckBox.CheckedChanged += new EventHandler(this.OnDynamicDataCheckedChanged);
            this._cancelButton.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            this._cancelButton.DialogResult = DialogResult.Cancel;
            this._cancelButton.Location = new Point(0x1f9, 0x16d);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new Size(0x4b, 0x17);
            this._cancelButton.TabIndex = 14;
            this._cancelButton.DialogResult = DialogResult.Cancel;
            this._okButton.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            this._okButton.Location = new Point(0x1a8, 0x16d);
            this._okButton.Name = "_okButton";
            this._okButton.Size = new Size(0x4b, 0x17);
            this._okButton.TabIndex = 13;
            this._okButton.DialogResult = DialogResult.OK;
            this._previewPanel.Location = new Point(0xcf, 0x1c);
            this._previewPanel.Name = "_previewPanel";
            this._previewPanel.Size = new Size(0x175, 0x148);
            this._previewPanel.TabIndex = 12;
            this._previewPanel.TabStop = false;
            this._previewPanel.BackColor = Color.White;
            this._previewPanel.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            this._schemePreview.Size = new Size(0x175, 0x148);
            this._schemePreview.TabStop = false;
            this._schemePreview.TabIndex = 0;
            this._schemePreview.TabStop = false;
            this._schemePreview.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            this._previewPanel.Controls.Add(this._schemePreview);
            base.AcceptButton = this._okButton;
            base.CancelButton = this._cancelButton;
            base.ClientSize = new Size(0x250, 400);
            base.Controls.Add(this._previewPanel);
            base.Controls.Add(this._okButton);
            base.Controls.Add(this._cancelButton);
            base.Controls.Add(this._dynamicCheckBox);
            base.Controls.Add(this._pagerComboBox);
            base.Controls.Add(this._pagingCheckBox);
            base.Controls.Add(this._deletingCheckBox);
            base.Controls.Add(this._insertingCheckBox);
            base.Controls.Add(this._editingCheckBox);
            base.Controls.Add(this._optionsLabel);
            base.Controls.Add(this._styleListBox);
            base.Controls.Add(this._selectStyleLabel);
            base.Controls.Add(this._previewLabel);
            base.Controls.Add(this._selectLayoutLabel);
            base.Controls.Add(this._layoutListBox);
            base.FormBorderStyle = FormBorderStyle.Sizable;
            base.SizeGripStyle = SizeGripStyle.Show;
            this.MinimumSize = new Size(600, 0x1ab);
            base.ResumeLayout(false);
        }

        private bool IsListViewEmpty() => 
            (((((this.ListView.GroupItemCount == 1) && string.IsNullOrEmpty(this.ListView.CssClass)) && ((this.ListView.DataKeyNames.Length == 0) && (this.ListView.InsertItemPosition == InsertItemPosition.None))) && (((this.ListView.AlternatingItemTemplate == null) && (this.ListView.EditItemTemplate == null)) && ((this.ListView.EmptyDataTemplate == null) && (this.ListView.EmptyItemTemplate == null)))) && ((((this.ListView.GroupSeparatorTemplate == null) && (this.ListView.GroupTemplate == null)) && ((this.ListView.InsertItemTemplate == null) && (this.ListView.ItemSeparatorTemplate == null))) && (((this.ListView.ItemTemplate == null) && (this.ListView.LayoutTemplate == null)) && (this.ListView.SelectedItemTemplate == null))));

        protected override void OnActivated(EventArgs e)
        {
            if (this._firstActivate)
            {
                this.DoDelayLoadActions();
                this._layoutListBox.SelectedIndex = 0;
                this._firstActivate = false;
                this.UpdateSchemePreview();
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (base.DialogResult == DialogResult.OK)
            {
                if (!this.IsListViewEmpty() && (DialogResult.No == UIServiceHelper.ShowMessage(this.ListView.Site, AtlasWebDesign.ListView_RegenerateListView, AtlasWebDesign.ListView_ResetCaption, MessageBoxButtons.YesNo)))
                {
                    e.Cancel = true;
                }
                this.ApplySelectedStyle(this.ListView);
                this.SetDataKeyNames();
            }
        }

        private void OnDynamicDataCheckedChanged(object sender, EventArgs e)
        {
            this.UpdateSchemePreview();
        }

        private void OnOptionChanged(object sender, EventArgs e)
        {
            this.UpdateSchemePreview();
        }

        private void OnPagingCheckedChanged(object sender, EventArgs e)
        {
            if (this._pagingCheckBox.Checked && (this._pagerComboBox.SelectedIndex < 0))
            {
                this._pagerComboBox.SelectedIndex = 0;
            }
            this._pagerComboBox.Enabled = this._pagingCheckBox.Checked;
            this.UpdateSchemePreview();
        }

        private void OnSelectedLayoutChanged(object sender, EventArgs e)
        {
            string str = string.Empty;
            object selectedItem = this._styleListBox.SelectedItem;
            if (selectedItem != null)
            {
                str = selectedItem.ToString();
            }
            this._styleListBox.Items.Clear();
            ListViewAutoLayout layout = this._layoutListBox.SelectedItem as ListViewAutoLayout;
            Collection<ListViewAutoStyle> styles = layout.Styles;
            foreach (ListViewAutoStyle style in styles)
            {
                this._styleListBox.Items.Add(style);
                if (!string.IsNullOrEmpty(str) && string.Equals(style.ToString(), str))
                {
                    this._styleListBox.SelectedItem = style;
                }
            }
            if ((this._styleListBox.SelectedIndex > styles.Count) || (this._styleListBox.SelectedIndex < 0))
            {
                this._styleListBox.SelectedIndex = 0;
            }
            this.UpdateSchemePreview();
        }

        private void OnSelectedStyleChanged(object sender, EventArgs e)
        {
            this.UpdateSchemePreview();
        }

        private void PopulateLayoutBox()
        {
            foreach (ListViewAutoLayout layout in this._controlDesigner.AutoLayouts)
            {
                this._layoutListBox.Items.Add(layout);
            }
        }

        private void SetDataKeyNames()
        {
            IDataSourceFieldSchema[] fields = null;
            IDataSourceViewSchema schema = null;
            DesignerDataSourceView designerView = this._controlDesigner.DesignerView;
            if (designerView != null)
            {
                schema = designerView.Schema;
                if (schema != null)
                {
                    fields = schema.GetFields();
                    if ((fields != null) && (fields.Length > 0))
                    {
                        ArrayList list = new ArrayList();
                        foreach (IDataSourceFieldSchema schema2 in fields)
                        {
                            if (schema2.PrimaryKey)
                            {
                                list.Add(schema2.Name);
                            }
                        }
                        int count = list.Count;
                        if (count > 0)
                        {
                            string[] array = new string[count];
                            list.CopyTo(array, 0);
                            this.ListView.DataKeyNames = array;
                        }
                    }
                }
            }
        }

        private void UpdateSchemePreview()
        {
            if (!this._firstActivate)
            {
                System.Web.UI.WebControls.ListView previewListView = this.GetPreviewListView();
                IDesigner designer = TypeDescriptor.CreateDesigner(previewListView, typeof(IDesigner));
                previewListView.Site = this.ListView.Site;
                designer.Initialize(previewListView);
                string designTimeHtml = ((ListViewDesigner) designer).GetDesignTimeHtml();
                this._schemePreview.GetDocument().GetBody().SetInnerHTML(designTimeHtml);
            }
        }

        private DesignerDataSourceView DesignerView =>
            this._controlDesigner.DesignerView;

        protected override string HelpTopic =>
            "net.Asp.ListView.ListViewConfigurationForm";

        private System.Web.UI.WebControls.ListView ListView =>
            (this._controlDesigner.Component as System.Web.UI.WebControls.ListView);
    }
}

