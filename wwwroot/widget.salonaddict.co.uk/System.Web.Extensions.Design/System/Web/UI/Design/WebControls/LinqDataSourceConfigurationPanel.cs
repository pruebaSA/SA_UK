namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Web.Resources.Design;
    using System.Web.UI.Design.Util;
    using System.Windows.Forms;

    internal class LinqDataSourceConfigurationPanel : WizardPanel, ILinqDataSourceConfigurationPanel
    {
        private Button _advancedButton;
        private ILinqDataSourceConfiguration _configuration;
        private Panel _groupByPanel;
        private bool _ignoreEvents;
        private Button _orderByButton;
        private Panel _selectPanel;
        private TableLayoutPanel _selectTableLayoutPanel;
        private AutoSizeComboBox _tableCombo;
        private Label _tableLabel;
        private Button _whereButton;
        private IContainer components;

        public LinqDataSourceConfigurationPanel()
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
            this._tableLabel.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this._tableCombo.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this._groupByPanel.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this._selectTableLayoutPanel.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            this._selectPanel.Dock = DockStyle.Fill;
            this._whereButton.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            this._orderByButton.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            this._advancedButton.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
        }

        private void InitializeComponent()
        {
            this._tableCombo = new AutoSizeComboBox();
            this._selectPanel = new Panel();
            this._groupByPanel = new Panel();
            this._tableLabel = new Label();
            this._selectTableLayoutPanel = new TableLayoutPanel();
            this._whereButton = new Button();
            this._orderByButton = new Button();
            this._advancedButton = new Button();
            this._selectTableLayoutPanel.SuspendLayout();
            base.SuspendLayout();
            this.InitializeSizes();
            this._tableCombo.DropDownStyle = ComboBoxStyle.DropDownList;
            this._tableCombo.Enabled = false;
            this._tableCombo.FormattingEnabled = true;
            this._tableCombo.Name = "_tableCombo";
            this._tableCombo.TabIndex = 20;
            this._tableCombo.SelectedIndexChanged += new EventHandler(this.OnSelectFromComboSelectedIndexChanged);
            this._selectPanel.Name = "_selectPanel";
            this._selectPanel.Dock = DockStyle.Fill;
            this._selectPanel.Margin = new Padding(0, 0, 0, 0);
            this._selectTableLayoutPanel.SetRowSpan(this._selectPanel, 5);
            this._selectPanel.TabIndex = 40;
            this._groupByPanel.Name = "_groupByPanel";
            this._groupByPanel.TabIndex = 30;
            this._tableLabel.AutoSize = true;
            this._tableLabel.Name = "_tableLabel";
            this._tableLabel.TabIndex = 10;
            this._selectTableLayoutPanel.Name = "tableLayoutPanel1";
            this._whereButton.AutoSize = true;
            this._whereButton.Dock = DockStyle.Fill;
            this._whereButton.Name = "_whereButton";
            this._whereButton.TabIndex = 1;
            this._whereButton.UseVisualStyleBackColor = true;
            this._whereButton.Click += new EventHandler(this.OnWhereButtonClick);
            this._orderByButton.AutoSize = true;
            this._orderByButton.Dock = DockStyle.Fill;
            this._orderByButton.Name = "_orderByButton";
            this._orderByButton.TabIndex = 0;
            this._orderByButton.UseVisualStyleBackColor = true;
            this._orderByButton.Click += new EventHandler(this.OnOrderByButtonClick);
            this._advancedButton.AutoSize = true;
            this._advancedButton.Dock = DockStyle.Fill;
            this._advancedButton.Name = "_advancedButton";
            this._advancedButton.TabIndex = 2;
            this._advancedButton.UseVisualStyleBackColor = true;
            this._advancedButton.Click += new EventHandler(this.OnAdvancedButtonClick);
            base.Controls.Add(this._tableLabel);
            base.Controls.Add(this._groupByPanel);
            base.Controls.Add(this._tableCombo);
            base.Controls.Add(this._selectTableLayoutPanel);
            base.Name = "LinqDataSourceConfigurationPanel";
            base.Size = new Size(0x220, 0x112);
            this._selectTableLayoutPanel.ResumeLayout(false);
            this._selectTableLayoutPanel.PerformLayout();
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void InitializeSizes()
        {
            int bottom = 0;
            this._tableLabel.Location = new Point(0, 0);
            this._tableLabel.Size = new Size(0x23, 13);
            bottom = this._tableLabel.Bottom;
            this._tableCombo.Location = new Point(3, bottom + 3);
            this._tableCombo.Size = new Size(0xe3, 0x15);
            bottom = this._tableCombo.Bottom;
            this._groupByPanel.Location = new Point(0, bottom + 4);
            this._groupByPanel.Size = new Size(0x220, 0x25);
            bottom = this._groupByPanel.Bottom;
            this._selectTableLayoutPanel.Location = new Point(0, bottom + 4);
            this._selectTableLayoutPanel.Size = new Size(0x220, 0xbc);
            this._selectTableLayoutPanel.ColumnCount = 2;
            this._selectTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            this._selectTableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
            this._selectTableLayoutPanel.Controls.Add(this._whereButton, 1, 0);
            this._selectTableLayoutPanel.Controls.Add(this._whereButton, 1, 1);
            this._selectTableLayoutPanel.Controls.Add(this._orderByButton, 1, 2);
            this._selectTableLayoutPanel.Controls.Add(this._advancedButton, 1, 3);
            this._selectTableLayoutPanel.Controls.Add(this._selectPanel, 0, 0);
            this._selectTableLayoutPanel.RowCount = 5;
            this._selectTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 56f));
            this._selectTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 23f));
            this._selectTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 29f));
            this._selectTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 29f));
            this._selectTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            this._selectTableLayoutPanel.TabIndex = 0;
            this._selectPanel.Location = new Point(0, 0);
            this._selectPanel.Size = new Size(0x1d0, 0xbc);
            this._selectPanel.Margin = new Padding(0, 0, 0, 0);
            this._whereButton.MinimumSize = new Size(80, 0x17);
            this._whereButton.Size = new Size(80, 0x17);
            this._whereButton.Margin = new Padding(0, 0, 0, 0);
            bottom = this._whereButton.Bottom;
            this._orderByButton.MinimumSize = new Size(80, 0x17);
            this._orderByButton.Size = new Size(80, 0x17);
            this._orderByButton.Margin = new Padding(0, 6, 0, 0);
            bottom = this._orderByButton.Bottom;
            this._advancedButton.MinimumSize = new Size(80, 0x17);
            this._advancedButton.Size = new Size(80, 0x17);
            this._advancedButton.Margin = new Padding(0, 6, 0, 0);
        }

        private void InitializeTabIndexes()
        {
            this._tableLabel.TabIndex = 10;
            this._tableCombo.TabIndex = 20;
            this._groupByPanel.TabIndex = 30;
            this._selectTableLayoutPanel.TabIndex = 40;
            this._selectPanel.TabIndex = 10;
            this._whereButton.TabIndex = 50;
            this._orderByButton.TabIndex = 60;
            this._advancedButton.TabIndex = 70;
        }

        private void InitializeUI()
        {
            this._tableLabel.Text = AtlasWebDesign.LinqDataSourceConfigurationPanel_TableLabel;
            this._orderByButton.Text = AtlasWebDesign.LinqDataSourceConfigurationPanel_OrderByButton;
            this._whereButton.Text = AtlasWebDesign.LinqDataSourceConfigurationPanel_WhereButton;
            this._advancedButton.Text = AtlasWebDesign.LinqDataSourceConfigurationPanel_AdvancedButton;
            base.Caption = AtlasWebDesign.LinqDataSourceConfigurationPanel_Caption;
        }

        private void OnAdvancedButtonClick(object sender, EventArgs e)
        {
            this._configuration.ShowAdvanced();
        }

        private void OnOrderByButtonClick(object sender, EventArgs e)
        {
            this._configuration.ShowOrderBy();
        }

        private void OnSelectFromComboSelectedIndexChanged(object sender, EventArgs e)
        {
            if (!this._ignoreEvents)
            {
                this._configuration.SelectTable(this._tableCombo.SelectedItem as ILinqDataSourcePropertyItem);
            }
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if (base.Visible)
            {
                this._configuration.UpdateWizardState((ILinqDataSourceWizardForm) base.ParentWizard);
            }
        }

        private void OnWhereButtonClick(object sender, EventArgs e)
        {
            this._configuration.ShowWhere();
        }

        public void Register(ILinqDataSourceConfiguration configuration)
        {
            this._configuration = configuration;
        }

        public void SetAdvancedEnabled(bool enabled)
        {
            this._advancedButton.Enabled = enabled;
        }

        public void SetConfigureGroupByForm(ILinqDataSourceConfigureGroupByPanel form)
        {
            Panel panel = (Panel) form;
            panel.Dock = DockStyle.Fill;
            this._groupByPanel.Controls.Add(panel);
            this._groupByPanel.Invalidate();
        }

        public void SetConfigureSelectForm(ILinqDataSourceConfigureSelectPanel form)
        {
            Panel panel = (Panel) form;
            panel.Dock = DockStyle.Fill;
            this._selectPanel.Controls.Add(panel);
            this._selectPanel.Invalidate();
        }

        public void SetPanelEnabled(bool enabled)
        {
            this._orderByButton.Enabled = enabled;
            this._whereButton.Enabled = enabled;
            this._groupByPanel.Enabled = enabled;
            this._selectPanel.Enabled = enabled;
            if (!enabled)
            {
                this._advancedButton.Enabled = enabled;
            }
        }

        public void SetSelectedTable(ILinqDataSourcePropertyItem selected)
        {
            this._ignoreEvents = true;
            this._tableCombo.SelectedItem = selected;
            this._ignoreEvents = false;
        }

        public void SetTableComboEnabled(bool enabled)
        {
            this._tableCombo.Enabled = enabled;
        }

        public void SetTables(List<ILinqDataSourcePropertyItem> tableProperties)
        {
            this._ignoreEvents = true;
            this._tableCombo.Items.Clear();
            this._tableCombo.Items.AddRange(tableProperties.ToArray());
            this._tableCombo.InvalidateDropDownWidth();
            this._ignoreEvents = false;
        }
    }
}

