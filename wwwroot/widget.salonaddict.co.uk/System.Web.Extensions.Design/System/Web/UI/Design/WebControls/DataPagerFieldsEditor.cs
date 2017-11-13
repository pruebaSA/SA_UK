namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.Drawing;
    using System.Globalization;
    using System.Security.Permissions;
    using System.Web.Resources.Design;
    using System.Web.UI.Design.Util;
    using System.Web.UI.WebControls;
    using System.Windows.Forms;

    [SecurityPermission(SecurityAction.Demand, Flags=SecurityPermissionFlag.UnmanagedCode)]
    internal sealed class DataPagerFieldsEditor : DesignerForm
    {
        private System.Windows.Forms.Button _addFieldButton;
        private System.Windows.Forms.Label _availableFieldsLabel;
        private TreeViewWithEnter _availableFieldsTree;
        private System.Windows.Forms.Button _cancelButton;
        private DataPagerFieldCollection _clonedFieldCollection;
        private DataPagerDesigner _controlDesigner;
        private FieldItem _currentFieldItem;
        private PropertyGrid _currentFieldProps;
        private System.Windows.Forms.Button _deleteFieldButton;
        private bool _fieldMovePending;
        private bool _initialActivate;
        private bool _isLoading;
        private System.Windows.Forms.Button _moveFieldDownButton;
        private System.Windows.Forms.Button _moveFieldUpButton;
        private System.Windows.Forms.Button _okButton;
        private bool _propChangesPending;
        private System.Windows.Forms.Label _selFieldLabel;
        private System.Windows.Forms.Label _selFieldsLabel;
        private ListViewWithEnter _selFieldsList;
        private const int CF_DELETE = 3;
        private const int CF_EDIT = 0;
        private const int CF_INSERT = 1;
        private const int CF_SELECT = 2;
        private const int ILI_CUSTOM = 3;
        private const int ILI_NEXTPREVIOUS = 0;
        private const int ILI_NUMERIC = 1;
        private const int ILI_TEMPLATE = 2;
        private const int MODE_EDIT = 1;
        private const int MODE_INSERT = 2;
        private const int MODE_READONLY = 0;

        public DataPagerFieldsEditor(DataPagerDesigner controlDesigner) : base(controlDesigner.Component.Site)
        {
            this._controlDesigner = controlDesigner;
            this.InitializeComponent();
            this.InitForm();
            this._initialActivate = true;
        }

        private void EnterLoadingMode()
        {
            this._isLoading = true;
        }

        private void ExitLoadingMode()
        {
            this._isLoading = false;
        }

        private void InitForm()
        {
            System.Drawing.Image image = new Bitmap(base.GetType().Assembly.GetManifestResourceStream("System.Web.Resources.Design.FieldNodes.bmp"));
            ImageList list = new ImageList {
                TransparentColor = Color.Magenta
            };
            list.Images.AddStrip(image);
            this._availableFieldsTree.ImageList = list;
            this._addFieldButton.Text = AtlasWebDesign.DPFEditor_Add;
            ColumnHeader header = new ColumnHeader {
                Width = this._selFieldsList.Width - 4
            };
            this._selFieldsList.Columns.Add(header);
            this._selFieldsList.SmallImageList = list;
            Bitmap bitmap = new Bitmap(base.GetType().Assembly.GetManifestResourceStream("System.Web.Resources.Design.SortUp.ico"));
            bitmap.MakeTransparent();
            this._moveFieldUpButton.Image = bitmap;
            this._moveFieldUpButton.AccessibleDescription = AtlasWebDesign.DPFEditor_MoveFieldUpDesc;
            this._moveFieldUpButton.AccessibleName = AtlasWebDesign.DPFEditor_MoveFieldUpName;
            Bitmap bitmap2 = new Bitmap(base.GetType().Assembly.GetManifestResourceStream("System.Web.Resources.Design.SortDown.ico"));
            bitmap2.MakeTransparent();
            this._moveFieldDownButton.Image = bitmap2;
            this._moveFieldDownButton.AccessibleDescription = AtlasWebDesign.DPFEditor_MoveFieldDownDesc;
            this._moveFieldDownButton.AccessibleName = AtlasWebDesign.DPFEditor_MoveFieldDownName;
            Bitmap bitmap3 = new Bitmap(base.GetType().Assembly.GetManifestResourceStream("System.Web.Resources.Design.Delete.ico"));
            bitmap3.MakeTransparent();
            this._deleteFieldButton.Image = bitmap3;
            this._deleteFieldButton.AccessibleDescription = AtlasWebDesign.DPFEditor_DeleteFieldDesc;
            this._deleteFieldButton.AccessibleName = AtlasWebDesign.DPFEditor_DeleteFieldName;
            this._okButton.Text = AtlasWebDesign.OKCaption;
            this._cancelButton.Text = AtlasWebDesign.CancelCaption;
            this._selFieldLabel.Text = AtlasWebDesign.DPFEditor_FieldProps;
            this._availableFieldsLabel.Text = AtlasWebDesign.DPFEditor_AvailableFields;
            this._selFieldsLabel.Text = AtlasWebDesign.DPFEditor_SelectedFields;
            this._currentFieldProps.Site = this._controlDesigner.Component.Site;
            this.Text = AtlasWebDesign.DPFEditor_Text;
        }

        private void InitializeComponent()
        {
            this._availableFieldsTree = new TreeViewWithEnter();
            this._selFieldsList = new ListViewWithEnter();
            this._okButton = new System.Windows.Forms.Button();
            this._cancelButton = new System.Windows.Forms.Button();
            this._moveFieldUpButton = new System.Windows.Forms.Button();
            this._moveFieldDownButton = new System.Windows.Forms.Button();
            this._addFieldButton = new System.Windows.Forms.Button();
            this._deleteFieldButton = new System.Windows.Forms.Button();
            this._currentFieldProps = new PropertyGrid();
            this._selFieldLabel = new System.Windows.Forms.Label();
            this._availableFieldsLabel = new System.Windows.Forms.Label();
            this._selFieldsLabel = new System.Windows.Forms.Label();
            base.SuspendLayout();
            this._availableFieldsTree.HideSelection = false;
            this._availableFieldsTree.ImageIndex = -1;
            this._availableFieldsTree.Indent = 15;
            this._availableFieldsTree.Location = new Point(12, 0x1c);
            this._availableFieldsTree.Name = "_availableFieldsTree";
            this._availableFieldsTree.SelectedImageIndex = -1;
            this._availableFieldsTree.Size = new Size(0xc4, 0x74);
            this._availableFieldsTree.TabIndex = 1;
            this._availableFieldsTree.NodeMouseDoubleClick += new TreeNodeMouseClickEventHandler(this.OnAvailableFieldsDoubleClick);
            this._availableFieldsTree.AfterSelect += new TreeViewEventHandler(this.OnSelChangedAvailableFields);
            this._availableFieldsTree.GotFocus += new EventHandler(this.OnAvailableFieldsGotFocus);
            this._availableFieldsTree.KeyPress += new KeyPressEventHandler(this.OnAvailableFieldsKeyPress);
            this._selFieldsList.HeaderStyle = ColumnHeaderStyle.None;
            this._selFieldsList.HideSelection = false;
            this._selFieldsList.LabelWrap = false;
            this._selFieldsList.Location = new Point(12, 0xc5);
            this._selFieldsList.MultiSelect = false;
            this._selFieldsList.Name = "_selFieldsList";
            this._selFieldsList.Size = new Size(0xa4, 0x70);
            this._selFieldsList.TabIndex = 4;
            this._selFieldsList.View = System.Windows.Forms.View.Details;
            this._selFieldsList.KeyDown += new KeyEventHandler(this.OnSelFieldsListKeyDown);
            this._selFieldsList.SelectedIndexChanged += new EventHandler(this.OnSelIndexChangedSelFieldsList);
            this._selFieldsList.ItemActivate += new EventHandler(this.OnClickDeleteField);
            this._selFieldsList.GotFocus += new EventHandler(this.OnSelFieldsListGotFocus);
            this._okButton.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            this._okButton.DialogResult = DialogResult.OK;
            this._okButton.FlatStyle = FlatStyle.System;
            this._okButton.Location = new Point(340, 330);
            this._okButton.Name = "_okButton";
            this._okButton.TabIndex = 100;
            this._okButton.Click += new EventHandler(this.OnClickOK);
            this._cancelButton.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            this._cancelButton.DialogResult = DialogResult.Cancel;
            this._cancelButton.FlatStyle = FlatStyle.System;
            this._cancelButton.Location = new Point(420, 330);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.TabIndex = 0x65;
            this._moveFieldUpButton.Location = new Point(0xba, 0xc5);
            this._moveFieldUpButton.Name = "_moveFieldUpButton";
            this._moveFieldUpButton.Size = new Size(0x1a, 0x17);
            this._moveFieldUpButton.TabIndex = 5;
            this._moveFieldUpButton.Click += new EventHandler(this.OnClickMoveFieldUp);
            this._moveFieldDownButton.Location = new Point(0xba, 0xdd);
            this._moveFieldDownButton.Name = "_moveFieldDownButton";
            this._moveFieldDownButton.Size = new Size(0x1a, 0x17);
            this._moveFieldDownButton.TabIndex = 6;
            this._moveFieldDownButton.Click += new EventHandler(this.OnClickMoveFieldDown);
            this._addFieldButton.FlatStyle = FlatStyle.System;
            this._addFieldButton.Location = new Point(0x7b, 150);
            this._addFieldButton.Name = "_addFieldButton";
            this._addFieldButton.Size = new Size(0x55, 0x17);
            this._addFieldButton.TabIndex = 2;
            this._addFieldButton.Click += new EventHandler(this.OnClickAddField);
            this._deleteFieldButton.Location = new Point(0xba, 0xf5);
            this._deleteFieldButton.Name = "_deleteFieldButton";
            this._deleteFieldButton.Size = new Size(0x1a, 0x17);
            this._deleteFieldButton.TabIndex = 7;
            this._deleteFieldButton.Click += new EventHandler(this.OnClickDeleteField);
            this._currentFieldProps.CommandsVisibleIfAvailable = true;
            this._currentFieldProps.Enabled = false;
            this._currentFieldProps.LargeButtons = false;
            this._currentFieldProps.LineColor = SystemColors.ScrollBar;
            this._currentFieldProps.Location = new Point(0xf4, 0x1c);
            this._currentFieldProps.Name = "_currentFieldProps";
            this._currentFieldProps.Size = new Size(0xf8, 0x119);
            this._currentFieldProps.TabIndex = 9;
            this._currentFieldProps.ToolbarVisible = true;
            this._currentFieldProps.ViewBackColor = SystemColors.Window;
            this._currentFieldProps.ViewForeColor = SystemColors.WindowText;
            this._currentFieldProps.PropertyValueChanged += new PropertyValueChangedEventHandler(this.OnChangedPropertyValues);
            this._selFieldLabel.Location = new Point(0xf4, 12);
            this._selFieldLabel.Name = "_selFieldLabel";
            this._selFieldLabel.Size = new Size(0xf8, 0x10);
            this._selFieldLabel.TabIndex = 8;
            this._availableFieldsLabel.Location = new Point(12, 12);
            this._availableFieldsLabel.Name = "_availableFieldsLabel";
            this._availableFieldsLabel.Size = new Size(0xc4, 0x10);
            this._availableFieldsLabel.TabIndex = 0;
            this._selFieldsLabel.Location = new Point(12, 0xb5);
            this._selFieldsLabel.Name = "_selFieldsLabel";
            this._selFieldsLabel.Size = new Size(0xc4, 0x10);
            this._selFieldsLabel.TabIndex = 3;
            base.AcceptButton = this._okButton;
            base.CancelButton = this._cancelButton;
            base.ClientSize = new Size(0x1fb, 0x16d);
            base.Controls.Add(this._selFieldsLabel);
            base.Controls.Add(this._availableFieldsLabel);
            base.Controls.Add(this._selFieldLabel);
            base.Controls.Add(this._currentFieldProps);
            base.Controls.Add(this._deleteFieldButton);
            base.Controls.Add(this._addFieldButton);
            base.Controls.Add(this._moveFieldDownButton);
            base.Controls.Add(this._moveFieldUpButton);
            base.Controls.Add(this._cancelButton);
            base.Controls.Add(this._okButton);
            base.Controls.Add(this._selFieldsList);
            base.Controls.Add(this._availableFieldsTree);
            base.FormBorderStyle = FormBorderStyle.FixedDialog;
            base.Name = "Form1";
            base.InitializeForm();
            base.ResumeLayout(false);
        }

        private void InitPage()
        {
            this._availableFieldsTree.Nodes.Clear();
            this._selFieldsList.Items.Clear();
            this._currentFieldItem = null;
            this._propChangesPending = false;
        }

        private void LoadAvailableFieldsTree()
        {
            NextPreviousNode node = new NextPreviousNode(this);
            this._availableFieldsTree.Nodes.Add(node);
            NumericNode node2 = new NumericNode(this);
            this._availableFieldsTree.Nodes.Add(node2);
            TemplateNode node3 = new TemplateNode(this);
            this._availableFieldsTree.Nodes.Add(node3);
        }

        private void LoadComponent()
        {
            this.InitPage();
            this.LoadAvailableFieldsTree();
            this.LoadFields();
            this.UpdateEnabledVisibleState();
        }

        private void LoadFields()
        {
            DataPagerFieldCollection fieldCollection = this.FieldCollection;
            if (fieldCollection != null)
            {
                int count = fieldCollection.Count;
                for (int i = 0; i < count; i++)
                {
                    DataPagerField runtimeField = fieldCollection[i];
                    FieldItem item = null;
                    System.Type type = runtimeField.GetType();
                    if (type == typeof(NextPreviousPagerField))
                    {
                        item = new NextPreviousFieldItem(this, (NextPreviousPagerField) runtimeField);
                    }
                    else if (type == typeof(NumericPagerField))
                    {
                        item = new NumericFieldItem(this, (NumericPagerField) runtimeField);
                    }
                    else if (type == typeof(TemplatePagerField))
                    {
                        item = new TemplateFieldItem(this, (TemplatePagerField) runtimeField);
                    }
                    else
                    {
                        item = new CustomFieldItem(this, runtimeField);
                    }
                    item.LoadFieldInfo();
                    this._selFieldsList.Items.Add(item);
                }
                if (this._selFieldsList.Items.Count != 0)
                {
                    this._currentFieldItem = (FieldItem) this._selFieldsList.Items[0];
                    this._currentFieldItem.Selected = true;
                }
            }
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            if (this._initialActivate)
            {
                this.LoadComponent();
                this._initialActivate = false;
            }
        }

        private void OnAvailableFieldsDoubleClick(object source, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.OnClickAddField(source, e);
            }
        }

        private void OnAvailableFieldsGotFocus(object source, EventArgs e)
        {
            this._currentFieldProps.SelectedObject = null;
        }

        private void OnAvailableFieldsKeyPress(object source, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                this.OnClickAddField(source, e);
                e.Handled = true;
            }
        }

        private void OnChangedPropertyValues(object source, PropertyValueChangedEventArgs e)
        {
            bool flag1 = this._isLoading;
        }

        private void OnClickAddField(object source, EventArgs e)
        {
            AvailableFieldNode selectedNode = (AvailableFieldNode) this._availableFieldsTree.SelectedNode;
            if (this._addFieldButton.Enabled)
            {
                if (this._propChangesPending)
                {
                    this.SaveFieldProperties();
                }
                FieldItem item = selectedNode.CreateField();
                this._selFieldsList.Items.Add(item);
                this._currentFieldItem = item;
                this._currentFieldItem.Selected = true;
                this._currentFieldItem.EnsureVisible();
                this._selFieldsList.Focus();
                this._selFieldsList.FocusedItem = this._currentFieldItem;
                this.UpdateEnabledVisibleState();
            }
        }

        private void OnClickDeleteField(object source, EventArgs e)
        {
            int index = this._currentFieldItem.Index;
            int num2 = -1;
            int count = this._selFieldsList.Items.Count;
            if (count > 1)
            {
                if (index == (count - 1))
                {
                    num2 = index - 1;
                }
                else
                {
                    num2 = index;
                }
            }
            this._propChangesPending = false;
            this._currentFieldItem.Remove();
            this._currentFieldItem = null;
            if (num2 != -1)
            {
                this._currentFieldItem = (FieldItem) this._selFieldsList.Items[num2];
                this._currentFieldItem.Selected = true;
                this._currentFieldItem.EnsureVisible();
                this._deleteFieldButton.Focus();
            }
            this.UpdateEnabledVisibleState();
        }

        private void OnClickMoveFieldDown(object source, EventArgs e)
        {
            this._fieldMovePending = true;
            int index = this._currentFieldItem.Index;
            System.Windows.Forms.ListViewItem item = this._selFieldsList.Items[index];
            this._selFieldsList.Items.RemoveAt(index);
            this._selFieldsList.Items.Insert(index + 1, item);
            this._currentFieldItem = (FieldItem) this._selFieldsList.Items[index + 1];
            this._currentFieldItem.Selected = true;
            this._currentFieldItem.EnsureVisible();
            this.UpdateFieldPositionButtonsState();
            if (this._moveFieldUpButton.Enabled && !this._moveFieldDownButton.Enabled)
            {
                this._moveFieldUpButton.Focus();
            }
            this._fieldMovePending = false;
        }

        private void OnClickMoveFieldUp(object source, EventArgs e)
        {
            this._fieldMovePending = true;
            int index = this._currentFieldItem.Index;
            System.Windows.Forms.ListViewItem item = this._selFieldsList.Items[index];
            this._selFieldsList.Items.RemoveAt(index);
            this._selFieldsList.Items.Insert(index - 1, item);
            this._currentFieldItem = (FieldItem) this._selFieldsList.Items[index - 1];
            this._currentFieldItem.Selected = true;
            this._currentFieldItem.EnsureVisible();
            this.UpdateFieldPositionButtonsState();
            this._fieldMovePending = false;
        }

        private void OnClickOK(object source, EventArgs e)
        {
            this.SaveComponent();
            this.PersistClonedFieldsToControl();
        }

        protected override void OnClosed(EventArgs e)
        {
        }

        private void OnSelChangedAvailableFields(object source, TreeViewEventArgs e)
        {
            this.UpdateEnabledVisibleState();
        }

        private void OnSelFieldsListGotFocus(object source, EventArgs e)
        {
            this.UpdateEnabledVisibleState();
        }

        private void OnSelFieldsListKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Delete)
            {
                if (this._currentFieldItem != null)
                {
                    this.OnClickDeleteField(sender, e);
                }
                e.Handled = true;
            }
        }

        private void OnSelIndexChangedSelFieldsList(object source, EventArgs e)
        {
            if (!this._fieldMovePending)
            {
                if (this._propChangesPending)
                {
                    this.SaveFieldProperties();
                }
                if (this._selFieldsList.SelectedItems.Count == 0)
                {
                    this._currentFieldItem = null;
                }
                else
                {
                    this._currentFieldItem = (FieldItem) this._selFieldsList.SelectedItems[0];
                }
                this.SetFieldPropertyHeader();
                this.UpdateEnabledVisibleState();
            }
        }

        private void PersistClonedFieldsToControl()
        {
            DataPagerFieldCollection fields = this.Control.Fields;
            if (fields != null)
            {
                fields.Clear();
                foreach (DataPagerField field in this.FieldCollection)
                {
                    fields.Add(field);
                }
            }
        }

        private void SaveComponent()
        {
            if (this._propChangesPending)
            {
                this.SaveFieldProperties();
            }
            DataPagerFieldCollection fieldCollection = this.FieldCollection;
            if (fieldCollection != null)
            {
                fieldCollection.Clear();
                int count = this._selFieldsList.Items.Count;
                for (int i = 0; i < count; i++)
                {
                    FieldItem item = (FieldItem) this._selFieldsList.Items[i];
                    fieldCollection.Add(item.RuntimeField);
                }
            }
        }

        private void SaveFieldProperties()
        {
            if ((this._currentFieldItem != null) && this._currentFieldProps.Visible)
            {
                this._currentFieldProps.Refresh();
            }
            this._propChangesPending = false;
        }

        private void SetFieldPropertyHeader()
        {
            string str = AtlasWebDesign.DPFEditor_FieldProps;
            if (this._currentFieldItem != null)
            {
                this.EnterLoadingMode();
                System.Type type = this._currentFieldItem.GetType();
                if (type == typeof(NextPreviousFieldItem))
                {
                    str = string.Format(CultureInfo.CurrentCulture, AtlasWebDesign.DPFEditor_FieldPropsFormat, new object[] { AtlasWebDesign.DPFEditor_Node_NextPrev });
                }
                else if (type == typeof(NumericFieldItem))
                {
                    str = string.Format(CultureInfo.CurrentCulture, AtlasWebDesign.DPFEditor_FieldPropsFormat, new object[] { AtlasWebDesign.DPFEditor_Node_Numeric });
                }
                else if (type == typeof(TemplateFieldItem))
                {
                    str = string.Format(CultureInfo.CurrentCulture, AtlasWebDesign.DPFEditor_FieldPropsFormat, new object[] { AtlasWebDesign.DPFEditor_Node_Template });
                }
                else
                {
                    str = string.Format(CultureInfo.CurrentCulture, AtlasWebDesign.DPFEditor_FieldPropsFormat, new object[] { AtlasWebDesign.DPFEditor_Node_Custom });
                }
                this.ExitLoadingMode();
            }
            this._selFieldLabel.Text = str;
        }

        private void UpdateEnabledVisibleState()
        {
            AvailableFieldNode selectedNode = (AvailableFieldNode) this._availableFieldsTree.SelectedNode;
            int count = this._selFieldsList.Items.Count;
            int num = this._selFieldsList.SelectedItems.Count;
            FieldItem item = null;
            int index = -1;
            if (num != 0)
            {
                item = (FieldItem) this._selFieldsList.SelectedItems[0];
            }
            if (item != null)
            {
                index = item.Index;
            }
            bool flag = index != -1;
            this._addFieldButton.Enabled = selectedNode != null;
            this._deleteFieldButton.Enabled = flag;
            this.UpdateFieldPositionButtonsState();
            this._currentFieldProps.Enabled = item != null;
            this._currentFieldProps.SelectedObject = ((item != null) && this._selFieldsList.Focused) ? item.RuntimeField : null;
            if (item != null)
            {
                item.RuntimeField.GetType();
            }
        }

        private void UpdateFieldPositionButtonsState()
        {
            int index = -1;
            int count = this._selFieldsList.SelectedItems.Count;
            FieldItem item = null;
            if (count > 0)
            {
                item = this._selFieldsList.SelectedItems[0] as FieldItem;
            }
            if (item != null)
            {
                index = item.Index;
            }
            this._moveFieldUpButton.Enabled = index > 0;
            this._moveFieldDownButton.Enabled = (index >= 0) && (index < (this._selFieldsList.Items.Count - 1));
        }

        private DataPager Control =>
            (this._controlDesigner.Component as DataPager);

        private DataPagerFieldCollection FieldCollection
        {
            get
            {
                if (this._clonedFieldCollection == null)
                {
                    DataPagerFieldCollection fields = this.Control.Fields;
                    this._clonedFieldCollection = fields.CloneFields(this.Control);
                    for (int i = 0; i < fields.Count; i++)
                    {
                        this._controlDesigner.RegisterClone(fields[i], this._clonedFieldCollection[i]);
                    }
                }
                return this._clonedFieldCollection;
            }
        }

        protected override string HelpTopic =>
            "net.Asp.DataPagerField.DataPagerFieldEditor";

        private abstract class AvailableFieldNode : System.Windows.Forms.TreeNode
        {
            public AvailableFieldNode(string text, int icon) : base(text, icon, icon)
            {
            }

            public virtual DataPagerFieldsEditor.FieldItem CreateField() => 
                null;
        }

        private class CustomFieldItem : DataPagerFieldsEditor.FieldItem
        {
            public CustomFieldItem(DataPagerFieldsEditor fieldsEditor, DataPagerField runtimeField) : base(fieldsEditor, runtimeField, 3)
            {
            }

            protected override string GetDefaultNodeText() => 
                AtlasWebDesign.DPFEditor_Node_Custom;
        }

        private abstract class FieldItem : System.Windows.Forms.ListViewItem
        {
            protected DataPagerFieldsEditor fieldsEditor;
            protected DataPagerField runtimeField;

            public FieldItem(DataPagerFieldsEditor fieldsEditor, DataPagerField runtimeField, int image) : base(string.Empty, image)
            {
                this.fieldsEditor = fieldsEditor;
                this.runtimeField = runtimeField;
                base.Text = this.GetNodeText();
            }

            protected virtual string GetDefaultNodeText() => 
                this.runtimeField.GetType().Name;

            public string GetNodeText() => 
                this.GetDefaultNodeText();

            public virtual void LoadFieldInfo()
            {
                this.UpdateDisplayText();
            }

            protected void UpdateDisplayText()
            {
                base.Text = this.GetNodeText();
            }

            public DataPagerField RuntimeField =>
                this.runtimeField;
        }

        private class ListViewWithEnter : System.Windows.Forms.ListView
        {
            protected override bool IsInputKey(Keys keyCode) => 
                ((keyCode == Keys.Enter) || base.IsInputKey(keyCode));
        }

        private class NextPreviousFieldItem : DataPagerFieldsEditor.FieldItem
        {
            public NextPreviousFieldItem(DataPagerFieldsEditor fieldsEditor, NextPreviousPagerField runtimeField) : base(fieldsEditor, runtimeField, 0)
            {
            }

            protected override string GetDefaultNodeText() => 
                AtlasWebDesign.DPFEditor_Node_NextPrev;
        }

        private class NextPreviousNode : DataPagerFieldsEditor.AvailableFieldNode
        {
            private DataPagerFieldsEditor _fieldsEditor;

            public NextPreviousNode(DataPagerFieldsEditor fieldsEditor) : base(AtlasWebDesign.DPFEditor_Node_NextPrev, 0)
            {
                this._fieldsEditor = fieldsEditor;
                base.Text = AtlasWebDesign.DPFEditor_Node_NextPrev;
            }

            public override DataPagerFieldsEditor.FieldItem CreateField()
            {
                NextPreviousPagerField runtimeField = new NextPreviousPagerField();
                DataPagerFieldsEditor.FieldItem item = new DataPagerFieldsEditor.NextPreviousFieldItem(this._fieldsEditor, runtimeField);
                item.LoadFieldInfo();
                return item;
            }
        }

        private class NumericFieldItem : DataPagerFieldsEditor.FieldItem
        {
            public NumericFieldItem(DataPagerFieldsEditor fieldsEditor, NumericPagerField runtimeField) : base(fieldsEditor, runtimeField, 1)
            {
            }

            protected override string GetDefaultNodeText() => 
                AtlasWebDesign.DPFEditor_Node_Numeric;
        }

        private class NumericNode : DataPagerFieldsEditor.AvailableFieldNode
        {
            private DataPagerFieldsEditor _fieldsEditor;

            public NumericNode(DataPagerFieldsEditor fieldsEditor) : base(AtlasWebDesign.DPFEditor_Node_Numeric, 1)
            {
                this._fieldsEditor = fieldsEditor;
                base.Text = AtlasWebDesign.DPFEditor_Node_Numeric;
            }

            public override DataPagerFieldsEditor.FieldItem CreateField()
            {
                NumericPagerField runtimeField = new NumericPagerField();
                DataPagerFieldsEditor.FieldItem item = new DataPagerFieldsEditor.NumericFieldItem(this._fieldsEditor, runtimeField);
                item.LoadFieldInfo();
                return item;
            }
        }

        private class TemplateFieldItem : DataPagerFieldsEditor.FieldItem
        {
            public TemplateFieldItem(DataPagerFieldsEditor fieldsEditor, TemplatePagerField runtimeField) : base(fieldsEditor, runtimeField, 2)
            {
            }

            protected override string GetDefaultNodeText() => 
                AtlasWebDesign.DPFEditor_Node_Template;
        }

        private class TemplateNode : DataPagerFieldsEditor.AvailableFieldNode
        {
            private DataPagerFieldsEditor _fieldsEditor;

            public TemplateNode(DataPagerFieldsEditor fieldsEditor) : base(AtlasWebDesign.DPFEditor_Node_Template, 2)
            {
                this._fieldsEditor = fieldsEditor;
                base.Text = AtlasWebDesign.DPFEditor_Node_Template;
            }

            public override DataPagerFieldsEditor.FieldItem CreateField()
            {
                TemplatePagerField runtimeField = new TemplatePagerField();
                DataPagerFieldsEditor.FieldItem item = new DataPagerFieldsEditor.TemplateFieldItem(this._fieldsEditor, runtimeField);
                item.LoadFieldInfo();
                return item;
            }
        }

        private class TreeViewWithEnter : System.Windows.Forms.TreeView
        {
            protected override bool IsInputKey(Keys keyCode) => 
                ((keyCode == Keys.Enter) || base.IsInputKey(keyCode));
        }
    }
}

