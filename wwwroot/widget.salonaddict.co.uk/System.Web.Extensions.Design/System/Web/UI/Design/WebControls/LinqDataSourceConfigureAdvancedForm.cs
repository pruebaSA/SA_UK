namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Web.Resources.Design;
    using System.Web.UI.Design.Util;
    using System.Windows.Forms;

    internal class LinqDataSourceConfigureAdvancedForm : DesignerForm, ILinqDataSourceConfigureAdvancedForm
    {
        private Button _cancelButton;
        private LinqDataSourceConfigureAdvanced _configure;
        private CheckBox _enableDeleteCheckBox;
        private CheckBox _enableInsertCheckBox;
        private CheckBox _enableUpdateCheckBox;
        private Label _headerLabel;
        private Button _okButton;
        private IContainer components;

        public LinqDataSourceConfigureAdvancedForm(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            this.InitializeComponent();
            this.InitializeUI();
            base.FormBorderStyle = FormBorderStyle.FixedDialog;
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
            this._enableDeleteCheckBox = new CheckBox();
            this._enableInsertCheckBox = new CheckBox();
            this._enableUpdateCheckBox = new CheckBox();
            this._okButton = new Button();
            this._cancelButton = new Button();
            this._headerLabel = new Label();
            base.SuspendLayout();
            this.InitializeSizes();
            this._enableDeleteCheckBox.AutoSize = true;
            this._enableDeleteCheckBox.Name = "_enableDeleteCheckBox";
            this._enableDeleteCheckBox.TabIndex = 20;
            this._enableDeleteCheckBox.UseVisualStyleBackColor = true;
            this._enableDeleteCheckBox.CheckedChanged += new EventHandler(this.OnEnableDeleteCheckBoxCheckedChanged);
            this._enableDeleteCheckBox.CheckAlign = ContentAlignment.TopLeft;
            this._enableDeleteCheckBox.TextAlign = ContentAlignment.TopLeft;
            this._enableInsertCheckBox.AutoSize = true;
            this._enableInsertCheckBox.Name = "_enableInsertCheckBox";
            this._enableInsertCheckBox.TabIndex = 40;
            this._enableInsertCheckBox.UseVisualStyleBackColor = true;
            this._enableInsertCheckBox.CheckedChanged += new EventHandler(this.OnEnableInsertCheckBoxCheckedChanged);
            this._enableInsertCheckBox.CheckAlign = ContentAlignment.TopLeft;
            this._enableInsertCheckBox.TextAlign = ContentAlignment.TopLeft;
            this._enableUpdateCheckBox.AutoSize = true;
            this._enableUpdateCheckBox.Name = "_enableUpdateCheckBox";
            this._enableUpdateCheckBox.TabIndex = 60;
            this._enableUpdateCheckBox.UseVisualStyleBackColor = true;
            this._enableUpdateCheckBox.CheckedChanged += new EventHandler(this.OnEnableUpdateCheckBoxCheckedChanged);
            this._enableUpdateCheckBox.CheckAlign = ContentAlignment.TopLeft;
            this._enableUpdateCheckBox.TextAlign = ContentAlignment.TopLeft;
            this._okButton.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            this._okButton.Name = "_okButton";
            this._okButton.TabIndex = 80;
            this._okButton.UseVisualStyleBackColor = true;
            this._okButton.Click += new EventHandler(this.OnOkButtonClick);
            this._cancelButton.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.TabIndex = 90;
            this._cancelButton.UseVisualStyleBackColor = true;
            this._cancelButton.Click += new EventHandler(this.OnCancelButtonClick);
            this._headerLabel.AutoSize = true;
            this._headerLabel.Name = "_headerLabel";
            this._headerLabel.TabIndex = 10;
            base.AcceptButton = this._okButton;
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.CancelButton = this._cancelButton;
            base.Controls.Add(this._headerLabel);
            base.Controls.Add(this._cancelButton);
            base.Controls.Add(this._okButton);
            base.Controls.Add(this._enableUpdateCheckBox);
            base.Controls.Add(this._enableInsertCheckBox);
            base.Controls.Add(this._enableDeleteCheckBox);
            base.Name = "LinqDataSourceConfigureAdvancedForm";
            base.ResumeLayout(false);
            base.PerformLayout();
            base.InitializeForm();
        }

        private void InitializeSizes()
        {
            this._headerLabel.Location = new Point(12, 12);
            this._headerLabel.Size = new Size(0x1d, 13);
            this._enableDeleteCheckBox.Location = new Point(12, 0x30);
            this._enableDeleteCheckBox.Size = new Size(80, 0x11);
            this._enableInsertCheckBox.Location = new Point(12, 0x54);
            this._enableInsertCheckBox.Size = new Size(80, 0x11);
            this._enableUpdateCheckBox.Location = new Point(12, 120);
            this._enableUpdateCheckBox.Size = new Size(80, 0x11);
            this._okButton.Location = new Point(0xe0, 0xa8);
            this._okButton.Size = new Size(0x4b, 0x17);
            this._cancelButton.Location = new Point(0x131, 0xa8);
            this._cancelButton.Size = new Size(0x4b, 0x17);
            this.AutoSize = true;
            base.ClientSize = new Size(0x188, 0xcb);
            this.MinimumSize = new Size(400, 230);
        }

        private void InitializeUI()
        {
            this._okButton.Text = AtlasWebDesign.OK;
            this._cancelButton.Text = AtlasWebDesign.Cancel;
            this._headerLabel.Text = AtlasWebDesign.LinqDataSourceConfigureAdvancedForm_HeaderLabel;
            this._enableDeleteCheckBox.Text = AtlasWebDesign.LinqDataSourceConfigureAdvancedForm_EnableDeleteCheckBox;
            this._enableInsertCheckBox.Text = AtlasWebDesign.LinqDataSourceConfigureAdvancedForm_EnableInsertCheckBox;
            this._enableUpdateCheckBox.Text = AtlasWebDesign.LinqDataSourceConfigureAdvancedForm_EnableUpdateCheckBox;
            this.Text = AtlasWebDesign.LinqDataSourceConfigureAdvancedForm_Caption;
        }

        private void OnCancelButtonClick(object sender, EventArgs e)
        {
            base.DialogResult = DialogResult.Cancel;
            base.Close();
        }

        private void OnEnableDeleteCheckBoxCheckedChanged(object sender, EventArgs e)
        {
            this._configure.SetEnableDelete(this._enableDeleteCheckBox.Checked);
        }

        private void OnEnableInsertCheckBoxCheckedChanged(object sender, EventArgs e)
        {
            this._configure.SetEnableInsert(this._enableInsertCheckBox.Checked);
        }

        private void OnEnableUpdateCheckBoxCheckedChanged(object sender, EventArgs e)
        {
            this._configure.SetEnableUpdate(this._enableUpdateCheckBox.Checked);
        }

        private void OnOkButtonClick(object sender, EventArgs e)
        {
            this._configure.SaveState();
            base.DialogResult = DialogResult.OK;
        }

        public void Register(LinqDataSourceConfigureAdvanced configureAdvanced)
        {
            this._configure = configureAdvanced;
        }

        public void SetEnableDelete(bool isChecked)
        {
            this._enableDeleteCheckBox.Checked = isChecked;
        }

        public void SetEnableInsert(bool isChecked)
        {
            this._enableInsertCheckBox.Checked = isChecked;
        }

        public void SetEnableUpdate(bool isChecked)
        {
            this._enableUpdateCheckBox.Checked = isChecked;
        }

        protected override string HelpTopic =>
            "net.Asp.LinqDataSource.AdvancedDialog";
    }
}

