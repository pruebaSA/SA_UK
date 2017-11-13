namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Globalization;
    using System.Web.Resources.Design;
    using System.Web.UI.Design.Util;
    using System.Windows.Forms;

    internal class LinqDataSourceChooseContextTypePanel : WizardPanel, ILinqDataSourceChooseContextTypePanel
    {
        private Label _chooseContextHelpTextLabel;
        private ILinqDataSourceChooseContextType _chooseContextType;
        private AutoSizeComboBox _contextTypesCombo;
        private Label _contextTypesLabel;
        private bool _ignoreEvents;
        private CheckBox _showDataContextsOnlyCheckBox;
        private IContainer components;

        public LinqDataSourceChooseContextTypePanel()
        {
            this.InitializeComponent();
            this.InitializeUI();
            this.InitializeTabIndexes();
            this.InitializeAnchors();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeAnchors()
        {
            this._chooseContextHelpTextLabel.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this._contextTypesLabel.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this._contextTypesCombo.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this._showDataContextsOnlyCheckBox.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
        }

        private void InitializeComponent()
        {
            this._contextTypesCombo = new AutoSizeComboBox();
            this._contextTypesLabel = new Label();
            this._showDataContextsOnlyCheckBox = new CheckBox();
            this._chooseContextHelpTextLabel = new Label();
            base.SuspendLayout();
            this.InitializeSizes();
            this._contextTypesCombo.DropDownStyle = ComboBoxStyle.DropDownList;
            this._contextTypesCombo.FormattingEnabled = true;
            this._contextTypesCombo.Name = "_contextTypesCombo";
            this._contextTypesCombo.TabIndex = 20;
            this._contextTypesCombo.SelectedIndexChanged += new EventHandler(this.OnContextTypesComboSelectedIndexChanged);
            this._contextTypesLabel.AutoSize = true;
            this._contextTypesLabel.Name = "_contextTypesLabel";
            this._contextTypesLabel.TabIndex = 10;
            this._showDataContextsOnlyCheckBox.Name = "_showDataContextsOnlyCheckBox";
            this._showDataContextsOnlyCheckBox.TabIndex = 30;
            this._showDataContextsOnlyCheckBox.UseVisualStyleBackColor = true;
            this._showDataContextsOnlyCheckBox.CheckedChanged += new EventHandler(this.OnShowDataContextsOnlyCheckBoxCheckedChanged);
            this._chooseContextHelpTextLabel.AutoSize = true;
            this._chooseContextHelpTextLabel.Name = "_chooseContextHelpTextLabel";
            this._chooseContextHelpTextLabel.TabIndex = 0;
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.Controls.Add(this._chooseContextHelpTextLabel);
            base.Controls.Add(this._showDataContextsOnlyCheckBox);
            base.Controls.Add(this._contextTypesLabel);
            base.Controls.Add(this._contextTypesCombo);
            base.Name = "LinqDataSourceChooseContextTypePanel";
            base.Size = new Size(0x220, 0x112);
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void InitializeSizes()
        {
            int bottom = 0;
            this._chooseContextHelpTextLabel.Location = new Point(0, 0);
            this._chooseContextHelpTextLabel.Size = new Size(0x23, 13);
            bottom = this._chooseContextHelpTextLabel.Bottom;
            this._showDataContextsOnlyCheckBox.Location = new Point(3, bottom + 30);
            this._showDataContextsOnlyCheckBox.Size = new Size(0xe3, 0x11);
            bottom = this._showDataContextsOnlyCheckBox.Bottom;
            this._contextTypesLabel.Location = new Point(0, bottom + 12);
            this._contextTypesLabel.Size = new Size(0x23, 13);
            bottom = this._contextTypesLabel.Bottom;
            this._contextTypesCombo.Location = new Point(3, bottom + 3);
            this._contextTypesCombo.Size = new Size(0xe3, 0x15);
        }

        private void InitializeTabIndexes()
        {
            this._chooseContextHelpTextLabel.TabIndex = 10;
            this._showDataContextsOnlyCheckBox.TabIndex = 20;
            this._contextTypesLabel.TabIndex = 30;
            this._contextTypesCombo.TabIndex = 40;
        }

        private void InitializeUI()
        {
            this._chooseContextHelpTextLabel.Text = AtlasWebDesign.LinqDataSourceChooseContextTypePanel_ContextHelpText;
            this._contextTypesLabel.Text = AtlasWebDesign.LinqDataSourceChooseContextTypePanel_ContextTypesLabel;
            this._showDataContextsOnlyCheckBox.Text = AtlasWebDesign.LinqDataSourceChooseContextTypePanel_ShowOnlyDataContextsCheckBox;
            base.Caption = AtlasWebDesign.LinqDataSourceChooseContextTypePanel_Caption;
        }

        private void OnContextTypesComboSelectedIndexChanged(object sender, EventArgs e)
        {
            if (!this._ignoreEvents)
            {
                this._chooseContextType.SelectContextType(this._contextTypesCombo.SelectedItem as ILinqDataSourceContextTypeItem);
                this._chooseContextType.UpdateWizardState((ILinqDataSourceWizardForm) base.ParentWizard);
            }
        }

        public override bool OnNext()
        {
            bool flag = this._chooseContextType.OnNext();
            if (!flag)
            {
                UIServiceHelper.ShowError(base.ServiceProvider, string.Format(CultureInfo.InvariantCulture, AtlasWebDesign.LinqDataSourceDesigner_CannotGetType, new object[] { LinqDataSourceChooseContextType.PlaceholderContextType }));
            }
            return flag;
        }

        private void OnShowDataContextsOnlyCheckBoxCheckedChanged(object sender, EventArgs e)
        {
            if (!this._ignoreEvents)
            {
                this._chooseContextType.SelectShowDataContextsOnly(this._showDataContextsOnlyCheckBox.Checked);
                this._chooseContextType.UpdateWizardState((ILinqDataSourceWizardForm) base.ParentWizard);
            }
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if (base.Visible)
            {
                this._chooseContextType.UpdateWizardState((ILinqDataSourceWizardForm) base.ParentWizard);
            }
        }

        public void Register(ILinqDataSourceChooseContextType chooseContextType)
        {
            this._chooseContextType = chooseContextType;
        }

        public void SetContextTypes(List<ILinqDataSourceContextTypeItem> contextTypes)
        {
            this._ignoreEvents = true;
            this._contextTypesCombo.Items.Clear();
            this._contextTypesCombo.Items.AddRange(contextTypes.ToArray());
            this._contextTypesCombo.InvalidateDropDownWidth();
            this._ignoreEvents = false;
        }

        public void SetSelectedContextType(ILinqDataSourceContextTypeItem selected)
        {
            this._ignoreEvents = true;
            this._contextTypesCombo.SelectedItem = selected;
            this._ignoreEvents = false;
        }

        public void SetShowOnlyDataContexts(bool showOnlyDataContexts)
        {
            this._ignoreEvents = true;
            this._showDataContextsOnlyCheckBox.Checked = showOnlyDataContexts;
            this._ignoreEvents = false;
        }
    }
}

