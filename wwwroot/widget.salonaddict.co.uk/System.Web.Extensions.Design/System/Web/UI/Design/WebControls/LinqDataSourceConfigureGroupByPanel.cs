namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Web.Resources.Design;
    using System.Web.UI.Design.Util;
    using System.Windows.Forms;

    internal class LinqDataSourceConfigureGroupByPanel : Panel, ILinqDataSourceConfigureGroupByPanel
    {
        private LinqDataSourceConfigureGroupBy _configureGroupBy;
        private AutoSizeComboBox _groupByCombo;
        private Label _groupByLabel;
        private bool _ignoreGroupByChanged;
        private AutoSizeComboBox _orderGroupsByCombo;
        private Label _orderGroupsByLabel;
        private IContainer components;

        public LinqDataSourceConfigureGroupByPanel()
        {
            this.InitializeComponent();
            this._groupByLabel.Text = AtlasWebDesign.LinqDataSourceConfigureGroupByPanel_GroupByLabel;
            this._orderGroupsByLabel.Text = AtlasWebDesign.LinqDataSourceConfigureGroupByPanel_OrderGroupsByLabel;
            this._orderGroupsByCombo.Items.Add(AtlasWebDesign.Combo_NoneOption);
            this._orderGroupsByCombo.Items.Add("key");
            this._orderGroupsByCombo.Items.Add("key desc");
            this.InitializeAnchors();
            this.InitializeTabIndexes();
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
            this._groupByLabel.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this._groupByCombo.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this._orderGroupsByLabel.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            this._orderGroupsByCombo.Anchor = AnchorStyles.Right | AnchorStyles.Top;
        }

        private void InitializeComponent()
        {
            this._groupByLabel = new Label();
            this._groupByCombo = new AutoSizeComboBox();
            this._orderGroupsByLabel = new Label();
            this._orderGroupsByCombo = new AutoSizeComboBox();
            base.SuspendLayout();
            this.InitializeSizes();
            this._groupByLabel.AutoSize = true;
            this._groupByLabel.Name = "_groupByLabel";
            this._groupByLabel.TabIndex = 10;
            this._groupByCombo.DropDownStyle = ComboBoxStyle.DropDownList;
            this._groupByCombo.FormattingEnabled = true;
            this._groupByCombo.Name = "_groupByCombo";
            this._groupByCombo.TabIndex = 20;
            this._groupByCombo.SelectedIndexChanged += new EventHandler(this.OnGroupByComboSelectedIndexChanged);
            this._orderGroupsByLabel.AutoSize = true;
            this._orderGroupsByLabel.Name = "_orderGroupsByLabel";
            this._orderGroupsByLabel.TabIndex = 30;
            this._orderGroupsByCombo.Name = "_orderGroupsByCombo";
            this._orderGroupsByCombo.TabIndex = 40;
            this._orderGroupsByCombo.SelectedIndexChanged += new EventHandler(this.OnOrderGroupsByComboSelectedIndexChanged);
            base.Controls.Add(this._groupByLabel);
            base.Controls.Add(this._groupByCombo);
            base.Controls.Add(this._orderGroupsByLabel);
            base.Controls.Add(this._orderGroupsByCombo);
            base.Name = "LinqDataSourceConfigureGroupByPanel";
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void InitializeSizes()
        {
            this._groupByLabel.Location = new Point(0, 0);
            this._groupByLabel.Size = new Size(0x23, 13);
            this._groupByCombo.Location = new Point(3, this._groupByLabel.Bottom + 3);
            this._groupByCombo.Size = new Size(230, 0x15);
            this._orderGroupsByLabel.Location = new Point(250, 0);
            this._orderGroupsByLabel.Size = new Size(50, 13);
            this._orderGroupsByCombo.Location = new Point(250, this._groupByLabel.Bottom + 3);
            this._orderGroupsByCombo.Size = new Size(230, 0x15);
            base.Size = new Size(0x220, this._groupByCombo.Bottom);
        }

        private void InitializeTabIndexes()
        {
            this._groupByLabel.TabIndex = 10;
            this._groupByCombo.TabIndex = 20;
            this._orderGroupsByLabel.TabIndex = 30;
            this._orderGroupsByCombo.TabIndex = 40;
        }

        private void OnGroupByComboSelectedIndexChanged(object sender, EventArgs e)
        {
            if (!this._ignoreGroupByChanged)
            {
                this._configureGroupBy.SelectGroupBy(this._groupByCombo.SelectedItem as ILinqDataSourcePropertyItem);
            }
        }

        private void OnOrderGroupsByComboSelectedIndexChanged(object sender, EventArgs e)
        {
            if (this._orderGroupsByCombo.Text == AtlasWebDesign.Combo_NoneOption)
            {
                this._orderGroupsByCombo.SelectedText = string.Empty;
            }
        }

        public void Register(LinqDataSourceConfigureGroupBy configureGroupBy)
        {
            this._configureGroupBy = configureGroupBy;
        }

        public void SetGroupBy(ILinqDataSourcePropertyItem field)
        {
            this._ignoreGroupByChanged = true;
            this._groupByCombo.SelectedItem = field;
            this._ignoreGroupByChanged = false;
        }

        public void SetGroupByFields(List<ILinqDataSourcePropertyItem> fields)
        {
            this._groupByCombo.Items.Clear();
            this._groupByCombo.Items.AddRange(fields.ToArray());
            this._groupByCombo.InvalidateDropDownWidth();
        }

        public void SetSelectedGroupByField(ILinqDataSourcePropertyItem selected)
        {
            this._groupByCombo.SelectedItem = selected;
        }

        public void ShowOrderGroupsBy(bool show)
        {
            this._orderGroupsByLabel.Visible = show;
            this._orderGroupsByCombo.Visible = show;
        }

        public string OrderGroupsBy
        {
            get => 
                this._orderGroupsByCombo.Text;
            set
            {
                this._orderGroupsByCombo.Text = value;
            }
        }
    }
}

