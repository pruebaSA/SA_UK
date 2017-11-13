namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;
    using System.Reflection;
    using System.Web.Resources.Design;
    using System.Web.UI;
    using System.Web.UI.Design.Util;
    using System.Web.UI.WebControls;
    using System.Windows.Forms;

    internal class LinqDataSourceStatementEditorForm : DesignerForm, ILinqDataSourceStatementEditorForm
    {
        private System.Windows.Forms.CheckBox _autoGenerateCheckBox;
        private string _cachedStatementText;
        private System.Windows.Forms.Button _cancelButton;
        private System.Windows.Forms.Panel _checkBoxPanel;
        private System.Web.UI.Control _linqDataSource;
        private System.Windows.Forms.Button _okButton;
        private ParameterEditorUserControl _parameterEditorUserControl;
        private ParameterCollection _parameters;
        private System.Windows.Forms.Button _refreshParametersButton;
        private System.Windows.Forms.Label _statementLabel;
        private System.Windows.Forms.Panel _statementPanel;
        private System.Windows.Forms.TextBox _statementTextBox;

        public LinqDataSourceStatementEditorForm(System.Web.UI.Control linqDataSource, IServiceProvider serviceProvider, bool hasAutoGen, bool isInsertUpdateDelete, bool isAutoGen, string statement, ParameterCollection parameters, string operationText) : base(serviceProvider)
        {
            this._linqDataSource = linqDataSource;
            this.InitializeComponent();
            this.InitializeUI(operationText);
            this.InitializeTabIndexes();
            this.InitializeAnchors();
            if (!hasAutoGen)
            {
                this.HideCheckBox();
            }
            if (isInsertUpdateDelete)
            {
                this.HideStatement();
            }
            this._parameters = parameters;
            this._autoGenerateCheckBox.Checked = isAutoGen;
            this._statementPanel.Enabled = !isAutoGen;
            this._statementTextBox.Text = statement;
            this._statementTextBox.Select(0, 0);
            List<Parameter> list = new List<Parameter>();
            foreach (Parameter parameter in parameters)
            {
                list.Add(parameter);
            }
            this._parameterEditorUserControl.AddParameters(list.ToArray());
            this._cachedStatementText = null;
        }

        private void HideCheckBox()
        {
            this._autoGenerateCheckBox.Checked = false;
            this._checkBoxPanel.Visible = false;
            int num = this._statementPanel.Location.Y - this._checkBoxPanel.Location.Y;
            Point location = this._statementPanel.Location;
            location.Y -= num;
            this._statementPanel.Location = location;
            location = this._parameterEditorUserControl.Location;
            location.Y -= num;
            this._parameterEditorUserControl.Location = location;
            Size minimumSize = this._parameterEditorUserControl.Size;
            minimumSize.Height += num;
            this._parameterEditorUserControl.Size = minimumSize;
            minimumSize = this.MinimumSize;
            minimumSize.Height -= num;
            this.MinimumSize = minimumSize;
            base.Size = minimumSize;
        }

        private void HideStatement()
        {
            this._statementPanel.Visible = false;
            int num = this._parameterEditorUserControl.Location.Y - this._statementPanel.Location.Y;
            Point location = this._parameterEditorUserControl.Location;
            location.Y -= num;
            this._parameterEditorUserControl.Location = location;
            Size minimumSize = this._parameterEditorUserControl.Size;
            minimumSize.Height += num;
            this._parameterEditorUserControl.Size = minimumSize;
            minimumSize = this.MinimumSize;
            minimumSize.Height -= num;
            this.MinimumSize = minimumSize;
            base.Size = minimumSize;
        }

        private void InitializeAnchors()
        {
            this._checkBoxPanel.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this._autoGenerateCheckBox.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            this._statementPanel.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this._statementLabel.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this._statementTextBox.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this._refreshParametersButton.Anchor = AnchorStyles.Left | AnchorStyles.Top;
            this._parameterEditorUserControl.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            this._okButton.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            this._cancelButton.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
        }

        private void InitializeComponent()
        {
            this._okButton = new System.Windows.Forms.Button();
            this._cancelButton = new System.Windows.Forms.Button();
            this._refreshParametersButton = new System.Windows.Forms.Button();
            this._statementLabel = new System.Windows.Forms.Label();
            this._statementTextBox = new System.Windows.Forms.TextBox();
            this._autoGenerateCheckBox = new System.Windows.Forms.CheckBox();
            this._parameterEditorUserControl = (ParameterEditorUserControl) Activator.CreateInstance(typeof(ParameterEditorUserControl), BindingFlags.NonPublic | BindingFlags.Instance, null, new object[] { base.ServiceProvider, this._linqDataSource }, null);
            this._checkBoxPanel = new System.Windows.Forms.Panel();
            this._statementPanel = new System.Windows.Forms.Panel();
            this._checkBoxPanel.SuspendLayout();
            this._statementPanel.SuspendLayout();
            base.SuspendLayout();
            this.InitializeSizes();
            this._okButton.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            this._okButton.Name = "_okButton";
            this._okButton.TabIndex = 150;
            this._okButton.Click += new EventHandler(this.OnOkButtonClick);
            this._cancelButton.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            this._cancelButton.DialogResult = DialogResult.Cancel;
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.TabIndex = 160;
            this._cancelButton.Click += new EventHandler(this.OnCancelButtonClick);
            this._refreshParametersButton.Name = "_inferParametersButton";
            this._refreshParametersButton.TabIndex = 30;
            this._refreshParametersButton.Click += new EventHandler(this.OnInferParametersButtonClick);
            this._statementLabel.Name = "_commandLabel";
            this._statementLabel.TabIndex = 10;
            this._statementTextBox.AcceptsReturn = true;
            this._statementTextBox.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this._statementTextBox.Multiline = true;
            this._statementTextBox.Name = "_statementTextBox";
            this._statementTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this._statementTextBox.TabIndex = 20;
            this._autoGenerateCheckBox.CheckAlign = ContentAlignment.TopLeft;
            this._autoGenerateCheckBox.TextAlign = ContentAlignment.TopLeft;
            this._autoGenerateCheckBox.Name = "_autoGenerateCheckBox";
            this._autoGenerateCheckBox.TabIndex = 0xa1;
            this._autoGenerateCheckBox.TabStop = true;
            this._autoGenerateCheckBox.UseVisualStyleBackColor = true;
            this._autoGenerateCheckBox.CheckedChanged += new EventHandler(this.OnAutoGenerateCheckBoxCheckedChanged);
            this._checkBoxPanel.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this._checkBoxPanel.Controls.Add(this._autoGenerateCheckBox);
            this._checkBoxPanel.Name = "_radioPanel";
            this._checkBoxPanel.TabIndex = 0xa3;
            this._statementPanel.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this._statementPanel.Controls.Add(this._statementLabel);
            this._statementPanel.Controls.Add(this._statementTextBox);
            this._statementPanel.Controls.Add(this._refreshParametersButton);
            this._statementPanel.Name = "_statementPanel";
            this._statementPanel.TabIndex = 0xa4;
            this._parameterEditorUserControl.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            this._parameterEditorUserControl.Name = "_parameterEditorUserControl";
            this._parameterEditorUserControl.TabIndex = 50;
            base.AcceptButton = this._okButton;
            base.CancelButton = this._cancelButton;
            base.Controls.Add(this._statementPanel);
            base.Controls.Add(this._checkBoxPanel);
            base.Controls.Add(this._cancelButton);
            base.Controls.Add(this._okButton);
            base.Controls.Add(this._parameterEditorUserControl);
            base.Name = "LinqDataSourceStatementEditorForm";
            this._checkBoxPanel.ResumeLayout(false);
            this._checkBoxPanel.PerformLayout();
            this._statementPanel.ResumeLayout(false);
            this._statementPanel.PerformLayout();
            base.ResumeLayout(false);
            base.InitializeForm();
        }

        private void InitializeSizes()
        {
            int bottom = 0;
            this._checkBoxPanel.Location = new Point(12, 12);
            this._checkBoxPanel.Size = new Size(0x1c8, 0x20);
            this._autoGenerateCheckBox.Location = new Point(0, 0);
            this._autoGenerateCheckBox.Size = new Size(0x1c8, 30);
            bottom = this._checkBoxPanel.Bottom;
            this._statementPanel.Location = new Point(12, bottom + 4);
            this._statementPanel.Size = new Size(0x1c8, 0x7c);
            bottom = 0;
            this._statementLabel.Location = new Point(0, 0);
            this._statementLabel.Size = new Size(200, 0x10);
            bottom = this._statementLabel.Bottom;
            this._statementTextBox.Location = new Point(0, bottom + 3);
            this._statementTextBox.Size = new Size(0x1c8, 0x4e);
            bottom = this._statementTextBox.Bottom;
            this._refreshParametersButton.Location = new Point(0, bottom + 6);
            this._refreshParametersButton.Size = new Size(0x80, 0x17);
            bottom = this._statementPanel.Bottom;
            this._parameterEditorUserControl.Location = new Point(12, bottom + 5);
            this._parameterEditorUserControl.Size = new Size(460, 0xd8);
            this._parameterEditorUserControl.MinimumSize = new Size(460, 0xd8);
            bottom = this._parameterEditorUserControl.Bottom;
            this._okButton.Location = new Point(0x139, bottom + 6);
            this._okButton.Size = new Size(0x4b, 0x17);
            this._cancelButton.Location = new Point(0x189, bottom + 6);
            this._cancelButton.Size = new Size(0x4b, 0x17);
            bottom = this._cancelButton.Bottom;
            base.ClientSize = new Size(480, bottom + 12);
            this.MinimumSize = new Size(0x1e8, (bottom + 12) + 0x1b);
        }

        private void InitializeTabIndexes()
        {
            this._checkBoxPanel.TabIndex = 10;
            this._autoGenerateCheckBox.TabIndex = 10;
            this._statementPanel.TabIndex = 20;
            this._statementLabel.TabIndex = 10;
            this._statementTextBox.TabIndex = 20;
            this._refreshParametersButton.TabIndex = 30;
            this._parameterEditorUserControl.TabIndex = 30;
            this._okButton.TabIndex = 40;
            this._cancelButton.TabIndex = 50;
        }

        private void InitializeUI(string operationText)
        {
            this.Text = AtlasWebDesign.LinqDataSourceStatementEditorForm_Caption;
            this._okButton.Text = AtlasWebDesign.OK;
            this._cancelButton.Text = AtlasWebDesign.Cancel;
            this._statementLabel.Text = string.Format(CultureInfo.InvariantCulture, AtlasWebDesign.LinqDataSourceStatementEditorForm_StatementLabel, new object[] { operationText });
            this._refreshParametersButton.Text = AtlasWebDesign.LinqDataSourceStatementEditorForm_RefreshParametersButton;
            if (string.Equals(operationText, "Where", StringComparison.OrdinalIgnoreCase))
            {
                this._autoGenerateCheckBox.Text = AtlasWebDesign.LinqDataSourceStatementEditorForm_AutoGenerateWhereCheckBox;
            }
            else if (string.Equals(operationText, "OrderBy", StringComparison.OrdinalIgnoreCase))
            {
                this._autoGenerateCheckBox.Text = AtlasWebDesign.LinqDataSourceStatementEditorForm_AutoGenerateOrderByCheckBox;
            }
        }

        private void OnAutoGenerateCheckBoxCheckedChanged(object sender, EventArgs e)
        {
            if (this._autoGenerateCheckBox.Checked)
            {
                this._cachedStatementText = this._statementTextBox.Text;
                this._statementTextBox.Text = null;
            }
            else if (!string.IsNullOrEmpty(this._cachedStatementText))
            {
                this._statementTextBox.Text = this._cachedStatementText;
            }
            this._statementPanel.Enabled = !this._autoGenerateCheckBox.Checked;
        }

        private void OnCancelButtonClick(object sender, EventArgs e)
        {
            base.DialogResult = DialogResult.Cancel;
            base.Close();
        }

        private void OnInferParametersButtonClick(object sender, EventArgs e)
        {
            List<Parameter> neededParameters = LinqDataSourceStatementEditor.GetNeededParameters(this.Statement);
            List<Parameter> list = new List<Parameter>(this._parameterEditorUserControl.GetParameters());
            List<Parameter> list3 = new List<Parameter>();
            foreach (Parameter parameter in neededParameters)
            {
                if (!LinqDataSourceStatementEditor.ContainsParameter(list, parameter.Name))
                {
                    list3.Add(parameter);
                }
            }
            this._parameterEditorUserControl.AddParameters(list3.ToArray());
        }

        private void OnOkButtonClick(object sender, EventArgs e)
        {
            this._parameters.Clear();
            foreach (Parameter parameter in this._parameterEditorUserControl.GetParameters())
            {
                this._parameters.Add(parameter);
            }
            base.DialogResult = DialogResult.OK;
            base.Close();
        }

        DialogResult ILinqDataSourceStatementEditorForm.ShowDialog() => 
            base.ShowDialog();

        public bool AutoGen =>
            this._autoGenerateCheckBox.Checked;

        protected override string HelpTopic =>
            "net.Asp.LinqDataSource.ExpressionEditor";

        public ParameterCollection Parameters =>
            this._parameters;

        public string Statement =>
            this._statementTextBox.Text;
    }
}

