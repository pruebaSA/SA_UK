namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Web.Resources.Design;
    using System.Web.UI.Design.Util;
    using System.Windows.Forms;

    internal class LinqDataSourceConfigureOrderByForm : DesignerForm, ILinqDataSourceConfigureOrderByForm
    {
        private RadioButton[] _ascendingRadios;
        private Button _cancelButton;
        private LinqDataSourceConfigureOrderBy _configureOrderBy;
        private RadioButton[] _descendingRadios;
        private AutoSizeComboBox _fieldComboBox1;
        private AutoSizeComboBox _fieldComboBox2;
        private AutoSizeComboBox _fieldComboBox3;
        private Label _helpLabel;
        private Button _okButton;
        private AutoSizeComboBox[] _orderByCombos;
        private Panel[] _orderByDirectionPanels;
        private GroupBox[] _orderByGroupBoxes;
        private Label _previewLabel;
        private TextBox _previewTextBox;
        private RadioButton _sortAscendingRadioButton1;
        private RadioButton _sortAscendingRadioButton2;
        private RadioButton _sortAscendingRadioButton3;
        private GroupBox _sortByGroupBox1;
        private GroupBox _sortByGroupBox2;
        private GroupBox _sortByGroupBox3;
        private RadioButton _sortDescendingRadioButton1;
        private RadioButton _sortDescendingRadioButton2;
        private RadioButton _sortDescendingRadioButton3;
        private Panel _sortDirectionPanel1;
        private Panel _sortDirectionPanel2;
        private Panel _sortDirectionPanel3;
        private IContainer components;

        public LinqDataSourceConfigureOrderByForm(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            this.InitializeComponent();
            this.InitializeUI();
            this._orderByCombos = new AutoSizeComboBox[] { this._fieldComboBox1, this._fieldComboBox2, this._fieldComboBox3 };
            this._ascendingRadios = new RadioButton[] { this._sortAscendingRadioButton1, this._sortAscendingRadioButton2, this._sortAscendingRadioButton3 };
            this._descendingRadios = new RadioButton[] { this._sortDescendingRadioButton1, this._sortDescendingRadioButton2, this._sortDescendingRadioButton3 };
            this._orderByDirectionPanels = new Panel[] { this._sortDirectionPanel1, this._sortDirectionPanel2, this._sortDirectionPanel3 };
            this._orderByGroupBoxes = new GroupBox[] { this._sortByGroupBox1, this._sortByGroupBox2, this._sortByGroupBox3 };
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
            this._helpLabel = new Label();
            this._previewLabel = new Label();
            this._previewTextBox = new TextBox();
            this._sortAscendingRadioButton1 = new RadioButton();
            this._sortDescendingRadioButton1 = new RadioButton();
            this._sortDirectionPanel1 = new Panel();
            this._fieldComboBox1 = new AutoSizeComboBox();
            this._okButton = new Button();
            this._cancelButton = new Button();
            this._sortDescendingRadioButton2 = new RadioButton();
            this._sortAscendingRadioButton2 = new RadioButton();
            this._fieldComboBox2 = new AutoSizeComboBox();
            this._sortDirectionPanel2 = new Panel();
            this._sortDescendingRadioButton3 = new RadioButton();
            this._sortAscendingRadioButton3 = new RadioButton();
            this._fieldComboBox3 = new AutoSizeComboBox();
            this._sortDirectionPanel3 = new Panel();
            this._sortByGroupBox1 = new GroupBox();
            this._sortByGroupBox2 = new GroupBox();
            this._sortByGroupBox3 = new GroupBox();
            this._sortDirectionPanel1.SuspendLayout();
            this._sortDirectionPanel2.SuspendLayout();
            this._sortDirectionPanel3.SuspendLayout();
            this._sortByGroupBox1.SuspendLayout();
            this._sortByGroupBox2.SuspendLayout();
            this._sortByGroupBox3.SuspendLayout();
            base.SuspendLayout();
            this._helpLabel.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this._helpLabel.Location = new Point(12, 12);
            this._helpLabel.Name = "_helpLabel";
            this._helpLabel.Size = new Size(0x17e, 0x10);
            this._helpLabel.TabIndex = 10;
            this._previewLabel.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this._previewLabel.Location = new Point(9, 0xdb);
            this._previewLabel.Name = "_previewLabel";
            this._previewLabel.Size = new Size(0x180, 13);
            this._previewLabel.TabIndex = 50;
            this._previewTextBox.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            this._previewTextBox.BackColor = SystemColors.Control;
            this._previewTextBox.Location = new Point(12, 0xed);
            this._previewTextBox.Multiline = true;
            this._previewTextBox.Name = "_previewTextBox";
            this._previewTextBox.ReadOnly = true;
            this._previewTextBox.ScrollBars = ScrollBars.Vertical;
            this._previewTextBox.Size = new Size(0x180, 0x48);
            this._previewTextBox.TabIndex = 60;
            this._sortAscendingRadioButton1.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this._sortAscendingRadioButton1.Location = new Point(0, 0);
            this._sortAscendingRadioButton1.Name = "_sortAscendingRadioButton1";
            this._sortAscendingRadioButton1.Size = new Size(200, 0x12);
            this._sortAscendingRadioButton1.TabIndex = 10;
            this._sortAscendingRadioButton1.CheckedChanged += new EventHandler(this.OnSortAscendingRadioButton1CheckedChanged);
            this._sortDescendingRadioButton1.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this._sortDescendingRadioButton1.Location = new Point(0, 0x12);
            this._sortDescendingRadioButton1.Name = "_sortDescendingRadioButton1";
            this._sortDescendingRadioButton1.Size = new Size(200, 0x12);
            this._sortDescendingRadioButton1.TabIndex = 20;
            this._sortDescendingRadioButton1.CheckedChanged += new EventHandler(this.OnSortDescendingRadioButton1CheckedChanged);
            this._sortDirectionPanel1.Controls.Add(this._sortDescendingRadioButton1);
            this._sortDirectionPanel1.Controls.Add(this._sortAscendingRadioButton1);
            this._sortDirectionPanel1.Location = new Point(0xa9, 12);
            this._sortDirectionPanel1.Name = "_sortDirectionPanel1";
            this._sortDirectionPanel1.Size = new Size(200, 0x26);
            this._sortDirectionPanel1.TabIndex = 20;
            this._fieldComboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            this._fieldComboBox1.Location = new Point(9, 20);
            this._fieldComboBox1.Name = "_fieldComboBox1";
            this._fieldComboBox1.Size = new Size(0x99, 0x15);
            this._fieldComboBox1.Sorted = true;
            this._fieldComboBox1.TabIndex = 10;
            this._fieldComboBox1.SelectedIndexChanged += new EventHandler(this.OnFieldComboBox1SelectedIndexChanged);
            this._okButton.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            this._okButton.Location = new Point(240, 0x141);
            this._okButton.Name = "_okButton";
            this._okButton.Size = new Size(0x4b, 0x17);
            this._okButton.TabIndex = 70;
            this._okButton.Click += new EventHandler(this.OnOkButtonClick);
            this._cancelButton.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            this._cancelButton.DialogResult = DialogResult.Cancel;
            this._cancelButton.Location = new Point(0x141, 0x141);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new Size(0x4b, 0x17);
            this._cancelButton.TabIndex = 80;
            this._cancelButton.Click += new EventHandler(this.OnCancelButtonClick);
            this._sortDescendingRadioButton2.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this._sortDescendingRadioButton2.Location = new Point(0, 0x12);
            this._sortDescendingRadioButton2.Name = "_sortDescendingRadioButton2";
            this._sortDescendingRadioButton2.Size = new Size(200, 0x12);
            this._sortDescendingRadioButton2.TabIndex = 20;
            this._sortDescendingRadioButton2.CheckedChanged += new EventHandler(this.OnSortDescendingRadioButton2CheckedChanged);
            this._sortAscendingRadioButton2.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this._sortAscendingRadioButton2.Location = new Point(0, 0);
            this._sortAscendingRadioButton2.Name = "_sortAscendingRadioButton2";
            this._sortAscendingRadioButton2.Size = new Size(200, 0x12);
            this._sortAscendingRadioButton2.TabIndex = 10;
            this._sortAscendingRadioButton2.CheckedChanged += new EventHandler(this.OnSortAscendingRadioButton2CheckedChanged);
            this._fieldComboBox2.DropDownStyle = ComboBoxStyle.DropDownList;
            this._fieldComboBox2.Location = new Point(9, 20);
            this._fieldComboBox2.Name = "_fieldComboBox2";
            this._fieldComboBox2.Size = new Size(0x99, 0x15);
            this._fieldComboBox2.Sorted = true;
            this._fieldComboBox2.TabIndex = 10;
            this._fieldComboBox2.SelectedIndexChanged += new EventHandler(this.OnFieldComboBox2SelectedIndexChanged);
            this._sortDirectionPanel2.Controls.Add(this._sortDescendingRadioButton2);
            this._sortDirectionPanel2.Controls.Add(this._sortAscendingRadioButton2);
            this._sortDirectionPanel2.Location = new Point(0xa9, 12);
            this._sortDirectionPanel2.Name = "_sortDirectionPanel2";
            this._sortDirectionPanel2.Size = new Size(200, 0x26);
            this._sortDirectionPanel2.TabIndex = 20;
            this._sortDescendingRadioButton3.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this._sortDescendingRadioButton3.Location = new Point(0, 0x12);
            this._sortDescendingRadioButton3.Name = "_sortDescendingRadioButton3";
            this._sortDescendingRadioButton3.Size = new Size(200, 0x12);
            this._sortDescendingRadioButton3.TabIndex = 20;
            this._sortDescendingRadioButton3.CheckedChanged += new EventHandler(this.OnSortDescendingRadioButton3CheckedChanged);
            this._sortAscendingRadioButton3.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this._sortAscendingRadioButton3.Location = new Point(0, 0);
            this._sortAscendingRadioButton3.Name = "_sortAscendingRadioButton3";
            this._sortAscendingRadioButton3.Size = new Size(200, 0x12);
            this._sortAscendingRadioButton3.TabIndex = 10;
            this._sortAscendingRadioButton3.CheckedChanged += new EventHandler(this.OnSortAscendingRadioButton3CheckedChanged);
            this._fieldComboBox3.DropDownStyle = ComboBoxStyle.DropDownList;
            this._fieldComboBox3.Location = new Point(9, 20);
            this._fieldComboBox3.Name = "_fieldComboBox3";
            this._fieldComboBox3.Size = new Size(0x99, 0x15);
            this._fieldComboBox3.Sorted = true;
            this._fieldComboBox3.TabIndex = 10;
            this._fieldComboBox3.SelectedIndexChanged += new EventHandler(this.OnFieldComboBox3SelectedIndexChanged);
            this._sortDirectionPanel3.Controls.Add(this._sortDescendingRadioButton3);
            this._sortDirectionPanel3.Controls.Add(this._sortAscendingRadioButton3);
            this._sortDirectionPanel3.Location = new Point(0xa9, 12);
            this._sortDirectionPanel3.Name = "_sortDirectionPanel3";
            this._sortDirectionPanel3.Size = new Size(200, 0x26);
            this._sortDirectionPanel3.TabIndex = 20;
            this._sortByGroupBox1.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this._sortByGroupBox1.Controls.Add(this._fieldComboBox1);
            this._sortByGroupBox1.Controls.Add(this._sortDirectionPanel1);
            this._sortByGroupBox1.Location = new Point(12, 0x21);
            this._sortByGroupBox1.Name = "_sortByGroupBox1";
            this._sortByGroupBox1.Size = new Size(0x180, 0x38);
            this._sortByGroupBox1.TabIndex = 20;
            this._sortByGroupBox1.TabStop = false;
            this._sortByGroupBox2.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this._sortByGroupBox2.Controls.Add(this._fieldComboBox2);
            this._sortByGroupBox2.Controls.Add(this._sortDirectionPanel2);
            this._sortByGroupBox2.Location = new Point(12, 0x5f);
            this._sortByGroupBox2.Name = "_sortByGroupBox2";
            this._sortByGroupBox2.Size = new Size(0x180, 0x38);
            this._sortByGroupBox2.TabIndex = 30;
            this._sortByGroupBox2.TabStop = false;
            this._sortByGroupBox3.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this._sortByGroupBox3.Controls.Add(this._fieldComboBox3);
            this._sortByGroupBox3.Controls.Add(this._sortDirectionPanel3);
            this._sortByGroupBox3.Location = new Point(12, 0x9d);
            this._sortByGroupBox3.Name = "_sortByGroupBox3";
            this._sortByGroupBox3.Size = new Size(0x180, 0x38);
            this._sortByGroupBox3.TabIndex = 40;
            this._sortByGroupBox3.TabStop = false;
            base.AcceptButton = this._okButton;
            base.CancelButton = this._cancelButton;
            base.ClientSize = new Size(0x198, 0x164);
            base.Controls.Add(this._sortByGroupBox2);
            base.Controls.Add(this._sortByGroupBox3);
            base.Controls.Add(this._sortByGroupBox1);
            base.Controls.Add(this._cancelButton);
            base.Controls.Add(this._okButton);
            base.Controls.Add(this._previewTextBox);
            base.Controls.Add(this._previewLabel);
            base.Controls.Add(this._helpLabel);
            this.MinimumSize = new Size(0x1a0, 0x17f);
            base.Name = "LinqDataSourceConfigureOrderByForm";
            base.SizeGripStyle = SizeGripStyle.Show;
            this._sortDirectionPanel1.ResumeLayout(false);
            this._sortDirectionPanel2.ResumeLayout(false);
            this._sortDirectionPanel3.ResumeLayout(false);
            this._sortByGroupBox1.ResumeLayout(false);
            this._sortByGroupBox2.ResumeLayout(false);
            this._sortByGroupBox3.ResumeLayout(false);
            base.ResumeLayout(false);
            base.PerformLayout();
            base.InitializeForm();
        }

        private void InitializeUI()
        {
            this._helpLabel.Text = AtlasWebDesign.LinqDataSourceConfigureOrderByForm_HelpLabel;
            this._sortByGroupBox1.Text = AtlasWebDesign.LinqDataSourceConfigureOrderByForm_SortByLabel;
            this._sortByGroupBox2.Text = AtlasWebDesign.LinqDataSourceConfigureOrderByForm_ThenByLabel;
            this._sortByGroupBox3.Text = AtlasWebDesign.LinqDataSourceConfigureOrderByForm_ThenByLabel;
            this._sortAscendingRadioButton1.Text = AtlasWebDesign.LinqDataSourceConfigureOrderByForm_AscendingLabel;
            this._sortDescendingRadioButton1.Text = AtlasWebDesign.LinqDataSourceConfigureOrderByForm_DescendingLabel;
            this._sortAscendingRadioButton2.Text = AtlasWebDesign.LinqDataSourceConfigureOrderByForm_AscendingLabel;
            this._sortDescendingRadioButton2.Text = AtlasWebDesign.LinqDataSourceConfigureOrderByForm_DescendingLabel;
            this._sortAscendingRadioButton3.Text = AtlasWebDesign.LinqDataSourceConfigureOrderByForm_AscendingLabel;
            this._sortDescendingRadioButton3.Text = AtlasWebDesign.LinqDataSourceConfigureOrderByForm_DescendingLabel;
            this._sortAscendingRadioButton1.AccessibleDescription = AtlasWebDesign.LinqDataSourceConfigureOrderByForm_SortDirection1;
            this._sortDescendingRadioButton1.AccessibleDescription = AtlasWebDesign.LinqDataSourceConfigureOrderByForm_SortDirection1;
            this._sortAscendingRadioButton2.AccessibleDescription = AtlasWebDesign.LinqDataSourceConfigureOrderByForm_SortDirection2;
            this._sortDescendingRadioButton2.AccessibleDescription = AtlasWebDesign.LinqDataSourceConfigureOrderByForm_SortDirection2;
            this._sortAscendingRadioButton3.AccessibleDescription = AtlasWebDesign.LinqDataSourceConfigureOrderByForm_SortDirection3;
            this._sortDescendingRadioButton3.AccessibleDescription = AtlasWebDesign.LinqDataSourceConfigureOrderByForm_SortDirection3;
            this._fieldComboBox1.AccessibleName = AtlasWebDesign.LinqDataSourceConfigureOrderByForm_SortField1;
            this._fieldComboBox2.AccessibleName = AtlasWebDesign.LinqDataSourceConfigureOrderByForm_SortField2;
            this._fieldComboBox3.AccessibleName = AtlasWebDesign.LinqDataSourceConfigureOrderByForm_SortField3;
            this._okButton.Text = AtlasWebDesign.OK;
            this._cancelButton.Text = AtlasWebDesign.Cancel;
            this._previewLabel.Text = AtlasWebDesign.LinqDataSourceConfigureOrderByForm_PreviewLabel;
            this.Text = AtlasWebDesign.LinqDataSourceConfigureOrderByForm_Caption;
        }

        private void OnCancelButtonClick(object sender, EventArgs e)
        {
            base.DialogResult = DialogResult.Cancel;
            base.Close();
        }

        private void OnFieldComboBox1SelectedIndexChanged(object sender, EventArgs e)
        {
            this._configureOrderBy.SelectOrderByField(0, this._fieldComboBox1.SelectedItem as ILinqDataSourcePropertyItem);
        }

        private void OnFieldComboBox2SelectedIndexChanged(object sender, EventArgs e)
        {
            this._configureOrderBy.SelectOrderByField(1, this._fieldComboBox2.SelectedItem as ILinqDataSourcePropertyItem);
        }

        private void OnFieldComboBox3SelectedIndexChanged(object sender, EventArgs e)
        {
            this._configureOrderBy.SelectOrderByField(2, this._fieldComboBox3.SelectedItem as ILinqDataSourcePropertyItem);
        }

        private void OnOkButtonClick(object sender, EventArgs e)
        {
            this._configureOrderBy.SaveState();
            base.DialogResult = DialogResult.OK;
        }

        private void OnSortAscendingRadioButton1CheckedChanged(object sender, EventArgs e)
        {
            this._configureOrderBy.SelectOrderByDirection(0, this._sortAscendingRadioButton1.Checked);
        }

        private void OnSortAscendingRadioButton2CheckedChanged(object sender, EventArgs e)
        {
            this._configureOrderBy.SelectOrderByDirection(1, this._sortAscendingRadioButton2.Checked);
        }

        private void OnSortAscendingRadioButton3CheckedChanged(object sender, EventArgs e)
        {
            this._configureOrderBy.SelectOrderByDirection(2, this._sortAscendingRadioButton3.Checked);
        }

        private void OnSortDescendingRadioButton1CheckedChanged(object sender, EventArgs e)
        {
            this._configureOrderBy.SelectOrderByDirection(0, !this._sortDescendingRadioButton1.Checked);
        }

        private void OnSortDescendingRadioButton2CheckedChanged(object sender, EventArgs e)
        {
            this._configureOrderBy.SelectOrderByDirection(1, !this._sortDescendingRadioButton2.Checked);
        }

        private void OnSortDescendingRadioButton3CheckedChanged(object sender, EventArgs e)
        {
            this._configureOrderBy.SelectOrderByDirection(2, !this._sortDescendingRadioButton3.Checked);
        }

        public void Register(LinqDataSourceConfigureOrderBy configureOrderBy)
        {
            this._configureOrderBy = configureOrderBy;
        }

        public void SetOrderByDirectionEnabled(int clauseNumber, bool enabled)
        {
            this._orderByDirectionPanels[clauseNumber].Enabled = enabled;
            if (!enabled)
            {
                this._ascendingRadios[clauseNumber].Checked = true;
                this._descendingRadios[clauseNumber].Checked = false;
            }
        }

        public void SetOrderByFieldEnabled(int clauseNumber, bool enabled)
        {
            this._orderByCombos[clauseNumber].Enabled = enabled;
            this._orderByGroupBoxes[clauseNumber].Enabled = enabled;
            this._orderByDirectionPanels[clauseNumber].Enabled = false;
            this._ascendingRadios[clauseNumber].Checked = true;
            if (!enabled)
            {
                this._orderByCombos[clauseNumber].SelectedItem = null;
            }
        }

        public void SetOrderByFields(List<ILinqDataSourcePropertyItem> fields)
        {
            foreach (AutoSizeComboBox box in this._orderByCombos)
            {
                box.Items.Clear();
                box.Items.AddRange(fields.ToArray());
                box.InvalidateDropDownWidth();
            }
        }

        public void SetPreview(string preview)
        {
            this._previewTextBox.Text = preview;
        }

        public void SetSelectedOrderByField(int clauseNumber, ILinqDataSourcePropertyItem field, bool isAsc)
        {
            this._orderByCombos[clauseNumber].SelectedItem = field;
            this._ascendingRadios[clauseNumber].Checked = isAsc;
            this._descendingRadios[clauseNumber].Checked = !isAsc;
        }

        protected override string HelpTopic =>
            "net.Asp.LinqDataSource.OrderByDialog";
    }
}

