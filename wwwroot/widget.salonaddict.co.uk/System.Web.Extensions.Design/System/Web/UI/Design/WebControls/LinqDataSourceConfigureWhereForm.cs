namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Reflection;
    using System.Web.Resources.Design;
    using System.Web.UI;
    using System.Web.UI.Design.Util;
    using System.Web.UI.WebControls;
    using System.Windows.Forms;

    internal class LinqDataSourceConfigureWhereForm : DesignerForm, ILinqDataSourceConfigureWhereForm
    {
        private System.Windows.Forms.Button _addButton;
        private System.Windows.Forms.Button _cancelButton;
        private AutoSizeComboBox _columnsComboBox;
        private ILinqDataSourceConfigureWhere _configureWhere;
        private System.Web.UI.Control _dataSource;
        private ColumnHeader _expressionColumnHeader;
        private System.Windows.Forms.Label _expressionLabel;
        private System.Windows.Forms.TextBox _expressionTextBox;
        private System.Windows.Forms.Label _fieldLabel;
        private System.Windows.Forms.Label _helpLabel;
        private System.Windows.Forms.Button _okButton;
        private List<OperatorItem> _operatorItems;
        private System.Windows.Forms.Label _operatorLabel;
        private AutoSizeComboBox _operatorsComboBox;
        private static IDictionary<System.Type, object> _parameterEditors;
        private GroupBox _propertiesGroupBox;
        private System.Windows.Forms.Panel _propertiesPanel;
        private System.Windows.Forms.Button _removeButton;
        private IServiceProvider _serviceProvider;
        private AutoSizeComboBox _sourceComboBox;
        private System.Windows.Forms.Label _sourceLabel;
        private ColumnHeader _valueColumnHeader;
        private System.Windows.Forms.Label _valueLabel;
        private System.Windows.Forms.TextBox _valueTextBox;
        private System.Windows.Forms.Label _whereClausesLabel;
        private System.Windows.Forms.ListView _whereClausesListView;
        private IContainer components;

        public LinqDataSourceConfigureWhereForm(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            this._serviceProvider = serviceProvider;
            this.InitializeComponent();
            this.InitializeUI();
            this._okButton.Enabled = false;
        }

        public void AddNewWhereExpressionItem(LinqDataSourceWhereExpression whereExpression, LinqDataSourceWhereStatement whereStatement, IServiceProvider serviceProvider, System.Web.UI.Control dataSource)
        {
            WhereExpressionItem item = new WhereExpressionItem(serviceProvider, whereStatement, whereExpression, dataSource);
            this._whereClausesListView.Items.Add(item);
            item.Selected = true;
            item.Focused = true;
            item.EnsureVisible();
            this._okButton.Enabled = true;
            item.Refresh();
        }

        internal void CreateParameterList()
        {
            Assembly assembly = typeof(SqlDataSourceDesigner).Assembly;
            string str = "System.Web.UI.Design.WebControls.SqlDataSourceConfigureFilterForm";
            _parameterEditors = new Dictionary<System.Type, object>();
            System.Type type = assembly.GetType(str + "+StaticParameterEditor");
            _parameterEditors.Add(typeof(Parameter), Activator.CreateInstance(type, new object[] { this._serviceProvider }));
            type = assembly.GetType(str + "+ControlParameterEditor");
            _parameterEditors.Add(typeof(ControlParameter), Activator.CreateInstance(type, new object[] { this._serviceProvider, this._dataSource }));
            type = assembly.GetType(str + "+CookieParameterEditor");
            _parameterEditors.Add(typeof(CookieParameter), Activator.CreateInstance(type, new object[] { this._serviceProvider }));
            type = assembly.GetType(str + "+FormParameterEditor");
            _parameterEditors.Add(typeof(FormParameter), Activator.CreateInstance(type, new object[] { this._serviceProvider }));
            type = assembly.GetType(str + "+ProfileParameterEditor");
            _parameterEditors.Add(typeof(ProfileParameter), Activator.CreateInstance(type, new object[] { this._serviceProvider }));
            type = assembly.GetType(str + "+QueryStringParameterEditor");
            _parameterEditors.Add(typeof(QueryStringParameter), Activator.CreateInstance(type, new object[] { this._serviceProvider }));
            type = assembly.GetType(str + "+SessionParameterEditor");
            _parameterEditors.Add(typeof(SessionParameter), Activator.CreateInstance(type, new object[] { this._serviceProvider }));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this._helpLabel = new System.Windows.Forms.Label();
            this._fieldLabel = new System.Windows.Forms.Label();
            this._columnsComboBox = new AutoSizeComboBox();
            this._operatorsComboBox = new AutoSizeComboBox();
            this._operatorLabel = new System.Windows.Forms.Label();
            this._whereClausesLabel = new System.Windows.Forms.Label();
            this._addButton = new System.Windows.Forms.Button();
            this._removeButton = new System.Windows.Forms.Button();
            this._cancelButton = new System.Windows.Forms.Button();
            this._okButton = new System.Windows.Forms.Button();
            this._expressionLabel = new System.Windows.Forms.Label();
            this._propertiesGroupBox = new GroupBox();
            this._propertiesPanel = new System.Windows.Forms.Panel();
            this._sourceComboBox = new AutoSizeComboBox();
            this._sourceLabel = new System.Windows.Forms.Label();
            this._expressionTextBox = new System.Windows.Forms.TextBox();
            this._whereClausesListView = new System.Windows.Forms.ListView();
            this._expressionColumnHeader = new ColumnHeader();
            this._valueColumnHeader = new ColumnHeader();
            this._valueTextBox = new System.Windows.Forms.TextBox();
            this._valueLabel = new System.Windows.Forms.Label();
            this._propertiesGroupBox.SuspendLayout();
            base.SuspendLayout();
            this._helpLabel.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this._helpLabel.Location = new Point(12, 11);
            this._helpLabel.Name = "_helpLabel";
            this._helpLabel.Size = new Size(0x20c, 0x2a);
            this._helpLabel.TabIndex = 10;
            this._fieldLabel.Location = new Point(12, 0x3b);
            this._fieldLabel.Name = "_columnLabel";
            this._fieldLabel.Size = new Size(0xac, 15);
            this._fieldLabel.TabIndex = 20;
            this._columnsComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            this._columnsComboBox.Location = new Point(15, 0x4d);
            this._columnsComboBox.Name = "_columnsComboBox";
            this._columnsComboBox.Size = new Size(0xac, 0x15);
            this._columnsComboBox.Sorted = true;
            this._columnsComboBox.TabIndex = 30;
            this._columnsComboBox.SelectedIndexChanged += new EventHandler(this.OnColumnsComboBoxSelectedIndexChanged);
            this._operatorsComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            this._operatorsComboBox.Location = new Point(15, 0x7a);
            this._operatorsComboBox.Name = "_operatorsComboBox";
            this._operatorsComboBox.Size = new Size(0xac, 0x15);
            this._operatorsComboBox.TabIndex = 50;
            this._operatorsComboBox.SelectedIndexChanged += new EventHandler(this.OnOperatorsComboBoxSelectedIndexChanged);
            this._operatorLabel.Location = new Point(12, 0x68);
            this._operatorLabel.Name = "_operatorLabel";
            this._operatorLabel.Size = new Size(0xac, 15);
            this._operatorLabel.TabIndex = 40;
            this._whereClausesLabel.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this._whereClausesLabel.Location = new Point(12, 0xf2);
            this._whereClausesLabel.Name = "_whereClausesLabel";
            this._whereClausesLabel.Size = new Size(0x1a9, 15);
            this._whereClausesLabel.TabIndex = 130;
            this._addButton.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            this._addButton.Location = new Point(0x1ba, 0xd4);
            this._addButton.Name = "_addButton";
            this._addButton.Size = new Size(90, 0x17);
            this._addButton.TabIndex = 0x7d;
            this._addButton.Click += new EventHandler(this.OnAddButtonClick);
            this._removeButton.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            this._removeButton.Location = new Point(0x1ba, 260);
            this._removeButton.Name = "_removeButton";
            this._removeButton.Size = new Size(90, 0x17);
            this._removeButton.TabIndex = 140;
            this._removeButton.Click += new EventHandler(this.OnRemoveButtonClick);
            this._cancelButton.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            this._cancelButton.DialogResult = DialogResult.Cancel;
            this._cancelButton.Location = new Point(0x1c9, 350);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new Size(0x4b, 0x17);
            this._cancelButton.TabIndex = 160;
            this._cancelButton.Click += new EventHandler(this.OnCancelButtonClick);
            this._okButton.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            this._okButton.Location = new Point(0x178, 350);
            this._okButton.Name = "_okButton";
            this._okButton.Size = new Size(0x4b, 0x17);
            this._okButton.TabIndex = 150;
            this._okButton.Click += new EventHandler(this.OnOkButtonClick);
            this._expressionLabel.Location = new Point(12, 0xc2);
            this._expressionLabel.Name = "_expressionLabel";
            this._expressionLabel.Size = new Size(0xe1, 15);
            this._expressionLabel.TabIndex = 90;
            this._propertiesGroupBox.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this._propertiesGroupBox.Controls.Add(this._propertiesPanel);
            this._propertiesGroupBox.Location = new Point(0xf3, 0x3b);
            this._propertiesGroupBox.Name = "_propertiesGroupBox";
            this._propertiesGroupBox.Size = new Size(0xc2, 0x7f);
            this._propertiesGroupBox.TabIndex = 80;
            this._propertiesGroupBox.TabStop = false;
            this._propertiesPanel.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            this._propertiesPanel.Location = new Point(10, 15);
            this._propertiesPanel.Name = "_propertiesPanel";
            this._propertiesPanel.Size = new Size(0xa4, 100);
            this._propertiesPanel.TabIndex = 10;
            this._sourceComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            this._sourceComboBox.Location = new Point(15, 0xa6);
            this._sourceComboBox.Name = "_sourceComboBox";
            this._sourceComboBox.Size = new Size(0xac, 0x15);
            this._sourceComboBox.TabIndex = 70;
            this._sourceComboBox.SelectedIndexChanged += new EventHandler(this.OnSourceComboBoxSelectedIndexChanged);
            this._sourceLabel.Location = new Point(12, 0x94);
            this._sourceLabel.Name = "_sourceLabel";
            this._sourceLabel.Size = new Size(0xac, 15);
            this._sourceLabel.TabIndex = 60;
            this._expressionTextBox.Location = new Point(15, 0xd4);
            this._expressionTextBox.Name = "_expressionTextBox";
            this._expressionTextBox.ReadOnly = true;
            this._expressionTextBox.Size = new Size(0xdd, 20);
            this._expressionTextBox.TabIndex = 100;
            this._whereClausesListView.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            this._whereClausesListView.Columns.AddRange(new ColumnHeader[] { this._expressionColumnHeader, this._valueColumnHeader });
            this._whereClausesListView.FullRowSelect = true;
            this._whereClausesListView.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            this._whereClausesListView.HideSelection = false;
            this._whereClausesListView.Location = new Point(15, 260);
            this._whereClausesListView.MultiSelect = false;
            this._whereClausesListView.Name = "_whereClausesListView";
            this._whereClausesListView.Size = new Size(0x1a6, 0x4e);
            this._whereClausesListView.TabIndex = 0x87;
            this._whereClausesListView.UseCompatibleStateImageBehavior = false;
            this._whereClausesListView.View = System.Windows.Forms.View.Details;
            this._whereClausesListView.SelectedIndexChanged += new EventHandler(this.OnWhereClausesListViewSelectedIndexChanged);
            this._expressionColumnHeader.Width = 0xe1;
            this._valueColumnHeader.Width = 160;
            this._valueTextBox.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this._valueTextBox.Location = new Point(0xf3, 0xd4);
            this._valueTextBox.Name = "_valueTextBox";
            this._valueTextBox.ReadOnly = true;
            this._valueTextBox.Size = new Size(0xc2, 20);
            this._valueTextBox.TabIndex = 120;
            this._valueLabel.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this._valueLabel.Location = new Point(0xf3, 0xc2);
            this._valueLabel.Name = "_valueLabel";
            this._valueLabel.Size = new Size(0xc2, 15);
            this._valueLabel.TabIndex = 110;
            base.AcceptButton = this._okButton;
            base.CancelButton = this._cancelButton;
            base.ClientSize = new Size(0x220, 0x181);
            base.Controls.Add(this._valueTextBox);
            base.Controls.Add(this._valueLabel);
            base.Controls.Add(this._whereClausesListView);
            base.Controls.Add(this._expressionTextBox);
            base.Controls.Add(this._propertiesGroupBox);
            base.Controls.Add(this._expressionLabel);
            base.Controls.Add(this._okButton);
            base.Controls.Add(this._cancelButton);
            base.Controls.Add(this._removeButton);
            base.Controls.Add(this._addButton);
            base.Controls.Add(this._whereClausesLabel);
            base.Controls.Add(this._operatorsComboBox);
            base.Controls.Add(this._operatorLabel);
            base.Controls.Add(this._columnsComboBox);
            base.Controls.Add(this._fieldLabel);
            base.Controls.Add(this._helpLabel);
            base.Controls.Add(this._sourceLabel);
            base.Controls.Add(this._sourceComboBox);
            this.MinimumSize = new Size(0x228, 0x19c);
            base.Name = "LinqDataSourceConfigureWhere";
            base.SizeGripStyle = SizeGripStyle.Show;
            this._propertiesGroupBox.ResumeLayout(false);
            base.ResumeLayout(false);
            base.PerformLayout();
            base.InitializeForm();
        }

        private void InitializeUI()
        {
            this._helpLabel.Text = AtlasWebDesign.LinqDataSourceConfigureWhereForm_HelpLabel;
            this._fieldLabel.Text = AtlasWebDesign.LinqDataSourceConfigureWhereForm_ColumnLabel;
            this._operatorLabel.Text = AtlasWebDesign.LinqDataSourceConfigureWhereForm_OperatorLabel;
            this._whereClausesLabel.Text = AtlasWebDesign.LinqDataSourceConfigureWhereForm_WhereLabel;
            this._expressionLabel.Text = AtlasWebDesign.LinqDataSourceConfigureWhereForm_ExpressionLabel;
            this._valueLabel.Text = AtlasWebDesign.LinqDataSourceConfigureWhereForm_ValueLabel;
            this._expressionColumnHeader.Text = AtlasWebDesign.LinqDataSourceConfigureWhereForm_ExpressionColumnHeader;
            this._valueColumnHeader.Text = AtlasWebDesign.LinqDataSourceConfigureWhereForm_ValueColumnHeader;
            this._propertiesGroupBox.Text = AtlasWebDesign.LinqDataSourceConfigureWhereForm_ParameterPropertiesGroup;
            this._sourceLabel.Text = AtlasWebDesign.LinqDataSourceConfigureWhereForm_SourceLabel;
            this._addButton.Text = AtlasWebDesign.LinqDataSourceConfigureWhereForm_AddButton;
            this._removeButton.Text = AtlasWebDesign.LinqDataSourceConfigureWhereForm_RemoveButton;
            this._okButton.Text = AtlasWebDesign.OK;
            this._cancelButton.Text = AtlasWebDesign.Cancel;
            this.Text = AtlasWebDesign.LinqDataSourceConfigureWhereForm_Caption;
        }

        private void OnAddButtonClick(object sender, EventArgs e)
        {
            this._configureWhere.AddCurrentWhereExpression();
        }

        private void OnCancelButtonClick(object sender, EventArgs e)
        {
            base.DialogResult = DialogResult.Cancel;
            base.Close();
        }

        public void OnColumnsComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            this._configureWhere.SelectWhereField(this._columnsComboBox.SelectedItem as ILinqDataSourcePropertyItem);
        }

        private void OnOkButtonClick(object sender, EventArgs e)
        {
            this._configureWhere.SaveState();
            base.DialogResult = DialogResult.OK;
        }

        private void OnOperatorsComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            OperatorItem selectedItem = this._operatorsComboBox.SelectedItem as OperatorItem;
            this._configureWhere.SelectOperator(selectedItem.Operator);
        }

        private void OnParameterChanged(object sender, EventArgs e)
        {
            this._configureWhere.InvalidateSelectedParameter();
        }

        private void OnRemoveButtonClick(object sender, EventArgs e)
        {
            this._configureWhere.RemoveCurrentWhereExpression();
        }

        private void OnSourceComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            this._configureWhere.SelectParameterEditor(this._sourceComboBox.SelectedItem);
        }

        private void OnWhereClausesListViewSelectedIndexChanged(object sender, EventArgs e)
        {
            if (this._whereClausesListView.SelectedItems.Count > 0)
            {
                WhereExpressionItem item = this._whereClausesListView.SelectedItems[0] as WhereExpressionItem;
                this._configureWhere.SelectWhereExpression(item.WhereExpression);
            }
            else
            {
                this._configureWhere.SelectWhereExpression(null);
            }
        }

        public void Register(ILinqDataSourceConfigureWhere configureWhere, System.Web.UI.Control dataSource)
        {
            this._configureWhere = configureWhere;
            this._dataSource = dataSource;
            this.CreateParameterList();
            foreach (object obj2 in _parameterEditors.Values)
            {
                this._configureWhere.SetParameterEditorVisible(obj2, false);
                this._propertiesPanel.Controls.Add((System.Windows.Forms.Control) obj2);
                this._sourceComboBox.Items.Add(obj2);
                this._configureWhere.AttachParameterEditorChangedHandler(obj2, new EventHandler(this.OnParameterChanged));
            }
            this._sourceComboBox.InvalidateDropDownWidth();
        }

        public void RemoveWhereExpressionItem(LinqDataSourceWhereExpression whereExpression)
        {
            int num = 0;
            WhereExpressionItem item = null;
            for (int i = 0; i < this._whereClausesListView.Items.Count; i++)
            {
                item = this._whereClausesListView.Items[i] as WhereExpressionItem;
                if (item.WhereExpression == whereExpression)
                {
                    num = i;
                    break;
                }
            }
            this._whereClausesListView.Items.Remove(item);
            this._okButton.Enabled = true;
            if (num < this._whereClausesListView.Items.Count)
            {
                System.Windows.Forms.ListViewItem item2 = this._whereClausesListView.Items[num];
                item2.Selected = true;
                item2.Focused = true;
                item2.EnsureVisible();
                this._whereClausesListView.Focus();
            }
            else if (this._whereClausesListView.Items.Count > 0)
            {
                System.Windows.Forms.ListViewItem item3 = this._whereClausesListView.Items[num - 1];
                item3.Selected = true;
                item3.Focused = true;
                item3.EnsureVisible();
                this._whereClausesListView.Focus();
            }
            else
            {
                this._whereClausesListView.SelectedItems.Clear();
            }
        }

        public void SetCanAddSelectExpression(bool enabled)
        {
            this._addButton.Enabled = enabled;
        }

        public void SetCanRemoveSelectExpression(bool enabled)
        {
            this._removeButton.Enabled = enabled;
        }

        public void SetExpressionPreviewEnabled(bool enabled)
        {
            this._expressionLabel.Enabled = enabled;
            this._expressionTextBox.Enabled = enabled;
            this._valueLabel.Enabled = enabled;
            this._valueTextBox.Enabled = enabled;
        }

        public void SetOperators(List<LinqDataSourceOperators> operators)
        {
            this._operatorItems = new List<OperatorItem>();
            foreach (LinqDataSourceOperators operators2 in operators)
            {
                this._operatorItems.Add(new OperatorItem(operators2));
            }
            this._operatorsComboBox.Items.Clear();
            this._operatorsComboBox.Items.AddRange(this._operatorItems.ToArray());
        }

        public void SetOperatorsEnabled(bool enabled)
        {
            if (!enabled)
            {
                this._operatorsComboBox.SelectedItem = -1;
                this._operatorsComboBox.Items.Clear();
            }
            this._operatorsComboBox.Enabled = enabled;
            this._operatorLabel.Enabled = enabled;
        }

        public void SetParameterEditorToShow(object editor)
        {
            foreach (object obj2 in _parameterEditors.Values)
            {
                this._configureWhere.SetParameterEditorVisible(obj2, false);
            }
            if (editor != null)
            {
                this._configureWhere.SetParameterEditorVisible(editor, true);
                this._configureWhere.InitializeParameterEditor(editor);
                this._propertiesPanel.Visible = true;
            }
            else
            {
                this._propertiesPanel.Visible = false;
            }
        }

        public void SetParametersEnabled(bool enabled)
        {
            this._propertiesGroupBox.Enabled = enabled;
            this._sourceLabel.Enabled = enabled;
            this._sourceComboBox.Enabled = enabled;
            if (!enabled)
            {
                this._sourceComboBox.SelectedIndex = -1;
            }
        }

        public void SetParameterValuePreview(string preview)
        {
            this._valueTextBox.Text = preview;
        }

        public void SetSelectedWhereField(ILinqDataSourcePropertyItem selected)
        {
            this._columnsComboBox.SelectedItem = selected;
        }

        public void SetWhereExpressionPreview(string preview)
        {
            this._expressionTextBox.Text = preview;
        }

        public void SetWhereFields(List<ILinqDataSourcePropertyItem> fields)
        {
            this._columnsComboBox.Items.Clear();
            this._columnsComboBox.Items.AddRange(fields.ToArray());
            this._columnsComboBox.InvalidateDropDownWidth();
        }

        public void SetWhereStatement(LinqDataSourceWhereStatement whereStatement, IServiceProvider serviceProvider, System.Web.UI.Control dataSource)
        {
            this._whereClausesListView.BeginUpdate();
            this._whereClausesListView.Items.Clear();
            foreach (LinqDataSourceWhereExpression expression in whereStatement)
            {
                WhereExpressionItem item = new WhereExpressionItem(serviceProvider, whereStatement, expression, dataSource);
                this._whereClausesListView.Items.Add(item);
                item.Refresh();
            }
            this._whereClausesListView.EndUpdate();
        }

        protected override string HelpTopic =>
            "net.Asp.LinqDataSource.WhereDialog";

        private sealed class OperatorItem
        {
            private LinqDataSourceOperators _operator;

            public OperatorItem(LinqDataSourceOperators op)
            {
                this._operator = op;
            }

            public override string ToString() => 
                this._operator.ToString();

            public LinqDataSourceOperators Operator =>
                this._operator;
        }

        private sealed class WhereExpressionItem : System.Windows.Forms.ListViewItem
        {
            private System.Web.UI.Control _dataSource;
            private IServiceProvider _serviceProvider;
            private LinqDataSourceWhereExpression _whereExpression;
            private LinqDataSourceWhereStatement _whereStatement;

            public WhereExpressionItem(IServiceProvider serviceProvider, LinqDataSourceWhereStatement whereStatement, LinqDataSourceWhereExpression whereExpression, System.Web.UI.Control dataSource)
            {
                this._serviceProvider = serviceProvider;
                this._whereStatement = whereStatement;
                this._whereExpression = whereExpression;
                this._dataSource = dataSource;
            }

            public void Refresh()
            {
                string str;
                base.SubItems.Clear();
                base.Text = this._whereExpression.ToString();
                System.Windows.Forms.ListView listView = base.ListView;
                Parameter parameter = this._whereStatement.Parameters[this._whereExpression.ParameterName];
                if (parameter == null)
                {
                    str = string.Empty;
                }
                else
                {
                    bool flag;
                    str = new LinqDataSourceConfigureWhereHelper().GetParameterExpression(this._serviceProvider, parameter, this._dataSource, out flag);
                    if (flag)
                    {
                        str = string.Empty;
                    }
                }
                System.Windows.Forms.ListViewItem.ListViewSubItem item = new System.Windows.Forms.ListViewItem.ListViewSubItem {
                    Text = str
                };
                base.SubItems.Add(item);
            }

            public override string ToString() => 
                this._whereExpression.ToString();

            public LinqDataSourceWhereExpression WhereExpression =>
                this._whereExpression;
        }
    }
}

