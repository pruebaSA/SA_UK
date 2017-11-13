namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Web.Resources.Design;
    using System.Windows.Forms;

    internal class LinqDataSourceConfigureSelectPanel : Panel, ILinqDataSourceConfigureSelectPanel
    {
        private DataGridViewTextBoxColumn _aliasColumn;
        private LinqDataSourceConfigureSelect _configureSelect;
        private TextBox _customGroupByTextBox;
        private Label _customGroupLabel;
        private TextBox _customSelectTextBox;
        private Button _downButton;
        private DataGridViewComboBoxColumn _fieldColumn;
        private DataGridViewComboBoxColumn _functionColumn;
        private Panel _groupByCustomPanel;
        private Label _groupByCustomSelectLabel;
        private Panel _groupByFieldPanel;
        private Label _groupByFieldSelectLabel;
        private Panel _groupByNonePanel;
        private Label _groupByNoneSelectLabel;
        private bool _ignoreCheckEvents;
        private CheckedListBox _projectionsCheckedListBox;
        private DataGridView _projectionsGrid;
        private Button _removeButton;
        private Button _upButton;
        private IContainer components;

        public LinqDataSourceConfigureSelectPanel() : this(false)
        {
        }

        public LinqDataSourceConfigureSelectPanel(bool isDebug)
        {
            this.InitializeComponent();
            this.InitializeUI(isDebug);
            this.InitializeTabIndexes();
            this.InitializeAnchors();
        }

        public void AddField(ILinqDataSourcePropertyItem field)
        {
            this._ignoreCheckEvents = true;
            for (int i = 1; i < this._projectionsCheckedListBox.Items.Count; i++)
            {
                if (string.Equals(this._projectionsCheckedListBox.Items[i] as string, field.Name, StringComparison.Ordinal))
                {
                    this._projectionsCheckedListBox.SetItemChecked(i, true);
                    break;
                }
            }
            this._ignoreCheckEvents = false;
        }

        public void AddProjection(ILinqDataSourcePropertyItem field, List<LinqDataSourceAggregateFunctions> aggregates, LinqDataSourceAggregateFunctions aggregateFunction, string alias, bool isDefaultProjection)
        {
            DataGridViewCell cell;
            DataGridViewRow dataGridViewRow = new DataGridViewRow();
            if (!isDefaultProjection)
            {
                DataGridViewComboBoxCell dataGridViewCell = new DataGridViewComboBoxCell();
                foreach (string str in this._fieldColumn.Items)
                {
                    dataGridViewCell.Items.Add(str);
                }
                dataGridViewCell.Value = field.DisplayName;
                dataGridViewRow.Cells.Add(dataGridViewCell);
                dataGridViewCell.ReadOnly = false;
                dataGridViewCell = new DataGridViewComboBoxCell();
                foreach (LinqDataSourceAggregateFunctions functions in aggregates)
                {
                    dataGridViewCell.Items.Add(functions.ToString());
                }
                dataGridViewCell.Value = aggregateFunction.ToString();
                dataGridViewRow.Cells.Add(dataGridViewCell);
                dataGridViewCell.ReadOnly = false;
            }
            else
            {
                cell = new DataGridViewTextBoxCell {
                    Value = field.ToString()
                };
                dataGridViewRow.Cells.Add(cell);
                cell.ReadOnly = true;
                cell = new DataGridViewTextBoxCell {
                    Value = aggregateFunction.ToString()
                };
                dataGridViewRow.Cells.Add(cell);
                cell.ReadOnly = true;
            }
            cell = new DataGridViewTextBoxCell {
                Value = alias
            };
            dataGridViewRow.Cells.Add(cell);
            cell.ReadOnly = false;
            this._projectionsGrid.Rows.Add(dataGridViewRow);
        }

        public void ClearGridProjections()
        {
            this._projectionsGrid.Rows.Clear();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        public void EnableAlias(int rowIndex)
        {
            this._projectionsGrid.Rows[rowIndex].Cells[2].ReadOnly = false;
        }

        internal static Bitmap GetBitmap(string resourceName)
        {
            Bitmap bitmap = new Bitmap(typeof(LinqDataSourceConfigureSelectPanel).Assembly.GetManifestResourceStream(resourceName));
            bitmap.MakeTransparent();
            return bitmap;
        }

        private void InitializeAnchors()
        {
            this._groupByNonePanel.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            this._groupByNoneSelectLabel.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this._projectionsCheckedListBox.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            this._groupByFieldPanel.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            this._groupByFieldSelectLabel.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this._projectionsGrid.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            this._upButton.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            this._downButton.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            this._removeButton.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            this._groupByCustomPanel.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            this._customGroupLabel.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this._customGroupByTextBox.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this._groupByCustomSelectLabel.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this._customSelectTextBox.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
        }

        private void InitializeComponent()
        {
            this._projectionsGrid = new DataGridView();
            this._fieldColumn = new DataGridViewComboBoxColumn();
            this._functionColumn = new DataGridViewComboBoxColumn();
            this._aliasColumn = new DataGridViewTextBoxColumn();
            this._projectionsCheckedListBox = new CheckedListBox();
            this._groupByFieldPanel = new Panel();
            this._removeButton = new Button();
            this._downButton = new Button();
            this._upButton = new Button();
            this._groupByFieldSelectLabel = new Label();
            this._groupByCustomPanel = new Panel();
            this._groupByCustomSelectLabel = new Label();
            this._customGroupLabel = new Label();
            this._customSelectTextBox = new TextBox();
            this._customGroupByTextBox = new TextBox();
            this._groupByNonePanel = new Panel();
            this._groupByNoneSelectLabel = new Label();
            ((ISupportInitialize) this._projectionsGrid).BeginInit();
            this._groupByFieldPanel.SuspendLayout();
            this._groupByCustomPanel.SuspendLayout();
            this._groupByNonePanel.SuspendLayout();
            base.SuspendLayout();
            this.InitializeSizes();
            this._projectionsGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._projectionsGrid.Columns.AddRange(new DataGridViewColumn[] { this._fieldColumn, this._functionColumn, this._aliasColumn });
            this._projectionsGrid.MultiSelect = false;
            this._projectionsGrid.Name = "_projectionsGrid";
            this._projectionsGrid.RowHeadersWidth = 0x1f;
            this._projectionsGrid.TabIndex = 30;
            this._projectionsGrid.UserDeletingRow += new DataGridViewRowCancelEventHandler(this.OnProjectionsGridUserDeletingRow);
            this._projectionsGrid.RowsAdded += new DataGridViewRowsAddedEventHandler(this.OnProjectionsGridRowsAdded);
            this._projectionsGrid.CellEndEdit += new DataGridViewCellEventHandler(this.OnProjectionsGridCellEndEdit);
            this._projectionsGrid.SelectionChanged += new EventHandler(this.OnProjectionsGridSelectionChanged);
            this._fieldColumn.HeaderText = "Column1";
            this._fieldColumn.Name = "_fieldColumn";
            this._fieldColumn.Width = 110;
            this._functionColumn.HeaderText = "Column1";
            this._functionColumn.Name = "_functionColumn";
            this._aliasColumn.HeaderText = "Column1";
            this._aliasColumn.Name = "_aliasColumn";
            this._aliasColumn.Width = 110;
            this._projectionsCheckedListBox.CheckOnClick = true;
            this._projectionsCheckedListBox.FormattingEnabled = true;
            this._projectionsCheckedListBox.MultiColumn = true;
            this._projectionsCheckedListBox.Name = "_projectionsCheckedListBox";
            this._projectionsCheckedListBox.TabIndex = 30;
            this._projectionsCheckedListBox.ItemCheck += new ItemCheckEventHandler(this.OnProjectionsCheckedListBoxItemCheck);
            this._groupByFieldPanel.Controls.Add(this._removeButton);
            this._groupByFieldPanel.Controls.Add(this._downButton);
            this._groupByFieldPanel.Controls.Add(this._upButton);
            this._groupByFieldPanel.Controls.Add(this._projectionsGrid);
            this._groupByFieldPanel.Controls.Add(this._groupByFieldSelectLabel);
            this._groupByFieldPanel.Name = "_groupByFieldPanel";
            this._groupByFieldPanel.TabIndex = 30;
            this._groupByFieldPanel.Visible = false;
            this._removeButton.Name = "_removeButton";
            this._removeButton.TabIndex = 60;
            this._removeButton.UseVisualStyleBackColor = true;
            this._removeButton.Click += new EventHandler(this.OnRemoveButtonClick);
            this._downButton.Name = "_downButton";
            this._downButton.TabIndex = 50;
            this._downButton.UseVisualStyleBackColor = true;
            this._downButton.Click += new EventHandler(this.OnDownButtonClick);
            this._upButton.Name = "_upButton";
            this._upButton.TabIndex = 40;
            this._upButton.UseVisualStyleBackColor = true;
            this._upButton.Click += new EventHandler(this.OnUpButtonClick);
            this._groupByFieldSelectLabel.AutoSize = true;
            this._groupByFieldSelectLabel.Name = "_groupByFieldSelectLabel";
            this._groupByFieldSelectLabel.TabIndex = 20;
            this._groupByCustomPanel.Controls.Add(this._groupByCustomSelectLabel);
            this._groupByCustomPanel.Controls.Add(this._customGroupLabel);
            this._groupByCustomPanel.Controls.Add(this._customSelectTextBox);
            this._groupByCustomPanel.Controls.Add(this._customGroupByTextBox);
            this._groupByCustomPanel.Name = "_groupByCustomPanel";
            this._groupByCustomPanel.TabIndex = 40;
            this._groupByCustomSelectLabel.AutoSize = true;
            this._groupByCustomSelectLabel.Name = "_groupByCustomSelectLabel";
            this._groupByCustomSelectLabel.TabIndex = 40;
            this._customGroupLabel.AutoSize = true;
            this._customGroupLabel.Name = "_customGroupLabel";
            this._customGroupLabel.TabIndex = 20;
            this._customSelectTextBox.Multiline = true;
            this._customSelectTextBox.Name = "_customSelectTextBox";
            this._customSelectTextBox.TabIndex = 50;
            this._customSelectTextBox.TextChanged += new EventHandler(this.OnCustomSelectTextBoxTextChanged);
            this._customGroupByTextBox.Name = "_customGroupByTextBox";
            this._customGroupByTextBox.TabIndex = 30;
            this._customGroupByTextBox.TextChanged += new EventHandler(this.OnCustomGroupByTextBoxTextChanged);
            this._groupByNonePanel.Controls.Add(this._groupByNoneSelectLabel);
            this._groupByNonePanel.Controls.Add(this._projectionsCheckedListBox);
            this._groupByNonePanel.Name = "_groupByNonePanel";
            this._groupByNonePanel.TabIndex = 20;
            this._groupByNonePanel.Visible = false;
            this._groupByNoneSelectLabel.AutoSize = true;
            this._groupByNoneSelectLabel.Name = "_groupByNoneSelectLabel";
            this._groupByNoneSelectLabel.TabIndex = 10;
            base.Controls.Add(this._groupByFieldPanel);
            base.Controls.Add(this._groupByNonePanel);
            base.Controls.Add(this._groupByCustomPanel);
            base.Name = "LinqDataSourceConfigureSelectPanel";
            ((ISupportInitialize) this._projectionsGrid).EndInit();
            this._groupByFieldPanel.ResumeLayout(false);
            this._groupByFieldPanel.PerformLayout();
            this._groupByCustomPanel.ResumeLayout(false);
            this._groupByCustomPanel.PerformLayout();
            this._groupByNonePanel.ResumeLayout(false);
            this._groupByNonePanel.PerformLayout();
            base.ResumeLayout(false);
        }

        private void InitializeSizes()
        {
            int y = 0x29;
            int x = 0x1a6;
            int width = (x - 3) - 6;
            this._groupByNonePanel.Location = new Point(0, 0);
            this._groupByNonePanel.Size = new Size(0x1d0, 0xbc);
            this._groupByNoneSelectLabel.Location = new Point(0, y);
            this._groupByNoneSelectLabel.Size = new Size(0x23, 13);
            y = this._groupByNoneSelectLabel.Bottom;
            this._projectionsCheckedListBox.IntegralHeight = false;
            this._projectionsCheckedListBox.Location = new Point(3, y + 3);
            y = this._projectionsCheckedListBox.Top;
            this._projectionsCheckedListBox.Size = new Size(width, 0xbc - y);
            y = 0x29;
            this._groupByFieldPanel.Location = new Point(0, 0);
            this._groupByFieldPanel.Size = new Size(0x1d0, 0xbc);
            this._groupByFieldSelectLabel.Location = new Point(0, y);
            this._groupByFieldSelectLabel.Size = new Size(0x23, 13);
            y = this._groupByFieldSelectLabel.Bottom;
            this._projectionsGrid.Location = new Point(3, y + 3);
            y = this._projectionsGrid.Top;
            this._projectionsGrid.Size = new Size(width, 0xbc - y);
            y = 0x39;
            this._upButton.Location = new Point(x, y);
            this._upButton.Size = new Size(0x1a, 0x17);
            y = this._upButton.Bottom;
            this._downButton.Location = new Point(x, y + 1);
            this._downButton.Size = new Size(0x1a, 0x17);
            y = this._downButton.Bottom;
            this._removeButton.Location = new Point(x, y + 6);
            this._removeButton.Size = new Size(0x1a, 0x17);
            y = 0;
            this._groupByCustomPanel.Location = new Point(0, 0);
            this._groupByCustomPanel.Size = new Size(0x1d0, 0xbc);
            this._customGroupLabel.Location = new Point(0, y);
            this._customGroupLabel.Size = new Size(0x23, 13);
            y = this._customGroupLabel.Bottom;
            this._customGroupByTextBox.Location = new Point(3, y + 3);
            this._customGroupByTextBox.Size = new Size(0xe3, 20);
            y = this._customGroupByTextBox.Bottom;
            this._groupByCustomSelectLabel.Location = new Point(0, y + 5);
            this._groupByCustomSelectLabel.Size = new Size(0x23, 13);
            y = this._groupByCustomSelectLabel.Bottom;
            this._customSelectTextBox.Location = new Point(3, y + 3);
            y = this._customSelectTextBox.Top;
            this._customSelectTextBox.Size = new Size(width, 0xbc - y);
            base.ClientSize = new Size(0x1d0, 0xbc);
        }

        private void InitializeTabIndexes()
        {
            this._groupByNonePanel.TabIndex = 10;
            this._groupByNoneSelectLabel.TabIndex = 10;
            this._projectionsCheckedListBox.TabIndex = 20;
            this._groupByFieldPanel.TabIndex = 20;
            this._groupByFieldSelectLabel.TabIndex = 10;
            this._projectionsGrid.TabIndex = 20;
            this._upButton.TabIndex = 30;
            this._downButton.TabIndex = 40;
            this._removeButton.TabIndex = 50;
            this._groupByCustomPanel.TabIndex = 30;
            this._customGroupLabel.TabIndex = 10;
            this._customGroupByTextBox.TabIndex = 20;
            this._groupByCustomSelectLabel.TabIndex = 30;
            this._customSelectTextBox.TabIndex = 40;
        }

        private void InitializeUI(bool isDebug)
        {
            this._fieldColumn.HeaderText = AtlasWebDesign.LinqDataSourceConfigureSelectForm_FieldHeader;
            this._functionColumn.HeaderText = AtlasWebDesign.LinqDataSourceConfigureSelectForm_AggregateFunctionHeader;
            this._aliasColumn.HeaderText = AtlasWebDesign.LinqDataSourceConfigureSelectForm_AliasHeader;
            this._groupByFieldSelectLabel.Text = AtlasWebDesign.LinqDataSourceConfigureSelectForm_SelectLabel;
            this._groupByCustomSelectLabel.Text = AtlasWebDesign.LinqDataSourceConfigureSelectForm_SelectLabel;
            this._groupByNoneSelectLabel.Text = AtlasWebDesign.LinqDataSourceConfigureSelectForm_SelectLabel;
            this._customGroupLabel.Text = AtlasWebDesign.LinqDataSourceConfigureSelectForm_CustomGroupByLabel;
            this._upButton.Text = null;
            if (!isDebug)
            {
                this._upButton.Image = GetBitmap("System.Web.Resources.Design.SortUp.ico");
            }
            this._upButton.AccessibleName = AtlasWebDesign.LinqDataSourceConfigureSelectForm_MoveUp;
            this._upButton.AccessibleDescription = AtlasWebDesign.LinqDataSourceConfigureSelectForm_MoveUpDescription;
            this._downButton.Text = null;
            if (!isDebug)
            {
                this._downButton.Image = GetBitmap("System.Web.Resources.Design.SortDown.ico");
            }
            this._downButton.AccessibleName = AtlasWebDesign.LinqDataSourceConfigureSelectForm_MoveDown;
            this._downButton.AccessibleDescription = AtlasWebDesign.LinqDataSourceConfigureSelectForm_MoveDownDescription;
            this._removeButton.Text = null;
            if (!isDebug)
            {
                this._removeButton.Image = GetBitmap("System.Web.Resources.Design.Delete.ico");
            }
            this._removeButton.AccessibleName = AtlasWebDesign.LinqDataSourceConfigureSelectForm_Delete;
            this._removeButton.AccessibleDescription = AtlasWebDesign.LinqDataSourceConfigureSelectForm_DeleteDescription;
        }

        public void MoveProjection(int oldIndex, int newIndex)
        {
            DataGridViewRow dataGridViewRow = this._projectionsGrid.Rows[oldIndex];
            this._projectionsGrid.Rows.RemoveAt(oldIndex);
            this._projectionsGrid.Rows.Insert(newIndex, dataGridViewRow);
            this._projectionsGrid.Rows[newIndex].Cells[0].Selected = true;
            this._projectionsGrid.Rows[newIndex].Selected = true;
            this._projectionsGrid.BeginEdit(false);
            this._projectionsGrid.EndEdit();
        }

        private void OnCustomGroupByTextBoxTextChanged(object sender, EventArgs e)
        {
            this._configureSelect.SetCustomGroupBy(this._customGroupByTextBox.Text);
        }

        private void OnCustomSelectTextBoxTextChanged(object sender, EventArgs e)
        {
            this._configureSelect.SetCustomSelect(this._customSelectTextBox.Text);
        }

        private void OnDownButtonClick(object sender, EventArgs e)
        {
            int rowIndex = this._projectionsGrid.SelectedCells[0].RowIndex;
            this._configureSelect.MoveProjection(rowIndex, rowIndex + 1);
        }

        private void OnProjectionsCheckedListBoxItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (!this._ignoreCheckEvents)
            {
                if (e.NewValue == CheckState.Checked)
                {
                    this._configureSelect.AddField(e.Index - 1);
                }
                else
                {
                    this._configureSelect.RemoveField(e.Index - 1);
                }
            }
        }

        private void OnProjectionsGridCellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewCell cell = this._projectionsGrid.Rows[e.RowIndex].Cells[e.ColumnIndex];
            string str = null;
            if (cell.Value != null)
            {
                str = cell.Value.ToString();
            }
            switch (e.ColumnIndex)
            {
                case 0:
                    if (string.IsNullOrEmpty(str))
                    {
                        break;
                    }
                    this._configureSelect.ChangeProjectionField(e.RowIndex, str);
                    return;

                case 1:
                    if (string.IsNullOrEmpty(str))
                    {
                        break;
                    }
                    this._configureSelect.ChangeProjectionAggregateFunction(e.RowIndex, str);
                    return;

                case 2:
                    if (cell.Value != null)
                    {
                        str = cell.Value.ToString();
                    }
                    this._configureSelect.ChangeProjectionAlias(e.RowIndex, str);
                    break;

                default:
                    return;
            }
        }

        private void OnProjectionsGridRowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            DataGridViewRow row = this._projectionsGrid.Rows[e.RowIndex];
            if ((row.Cells[1].Value == null) && (row.Cells[2].Value == null))
            {
                row.Cells[2].ReadOnly = true;
            }
        }

        private void OnProjectionsGridSelectionChanged(object sender, EventArgs e)
        {
            if (this._projectionsGrid.SelectedRows.Count > 0)
            {
                this._projectionsGrid.EndEdit();
            }
            if (this._projectionsGrid.SelectedCells.Count > 0)
            {
                this._configureSelect.SelectProjection(this._projectionsGrid.SelectedCells[0].RowIndex);
            }
        }

        private void OnProjectionsGridUserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (e.Row.Cells[0].ReadOnly)
            {
                e.Cancel = true;
            }
            else
            {
                this._configureSelect.RemoveProjectionDeleteKey(e.Row.Index);
            }
        }

        private void OnRemoveButtonClick(object sender, EventArgs e)
        {
            int rowIndex = this._projectionsGrid.SelectedCells[0].RowIndex;
            if (!this._projectionsGrid.SelectedCells[0].ReadOnly)
            {
                this._configureSelect.RemoveProjectionDeleteButton(rowIndex);
            }
        }

        private void OnUpButtonClick(object sender, EventArgs e)
        {
            int rowIndex = this._projectionsGrid.SelectedCells[0].RowIndex;
            this._configureSelect.MoveProjection(rowIndex, rowIndex - 1);
        }

        public void Register(LinqDataSourceConfigureSelect configureSelect)
        {
            this._configureSelect = configureSelect;
        }

        public void RemoveProjection(int index)
        {
            this._projectionsGrid.Rows.RemoveAt(index);
        }

        public void SetAggregateFunctions(int rowIndex, List<LinqDataSourceAggregateFunctions> aggregates)
        {
            DataGridViewComboBoxCell cell = this._projectionsGrid.Rows[rowIndex].Cells[1] as DataGridViewComboBoxCell;
            cell.Items.Clear();
            foreach (LinqDataSourceAggregateFunctions functions in aggregates)
            {
                cell.Items.Add(functions.ToString());
            }
            if ((cell.Value != null) && !cell.Items.Contains(cell.Value))
            {
                cell.Value = null;
            }
        }

        public void SetCanMoveDown(bool enabled)
        {
            this._downButton.Enabled = enabled;
        }

        public void SetCanMoveUp(bool enabled)
        {
            this._upButton.Enabled = enabled;
        }

        public void SetCanRemove(bool enabled)
        {
            this._removeButton.Enabled = enabled;
        }

        public void SetCheckBoxFields(List<ILinqDataSourcePropertyItem> fields)
        {
            this._projectionsCheckedListBox.Items.Clear();
            this._projectionsCheckedListBox.Items.Add("*");
            if (fields != null)
            {
                foreach (ILinqDataSourcePropertyItem item in fields)
                {
                    this._projectionsCheckedListBox.Items.Add(item.ToString());
                }
            }
        }

        public void SetCustomGroupBy(string newGroupBy)
        {
            this._customGroupByTextBox.Text = newGroupBy;
        }

        public void SetCustomSelect(string newSelect)
        {
            this._customSelectTextBox.Text = newSelect;
        }

        public void SetGridAggregateFunctions(List<LinqDataSourceAggregateFunctions> aggregates)
        {
            this.ClearGridProjections();
            if (aggregates != null)
            {
                this._functionColumn.Items.Clear();
                foreach (LinqDataSourceAggregateFunctions functions in aggregates)
                {
                    this._functionColumn.Items.Add(functions.ToString());
                }
            }
        }

        public void SetGridFields(List<ILinqDataSourcePropertyItem> fields)
        {
            this.ClearGridProjections();
            if (fields != null)
            {
                this._fieldColumn.Items.Clear();
                foreach (ILinqDataSourcePropertyItem item in fields)
                {
                    this._fieldColumn.Items.Add(item.ToString());
                }
            }
        }

        public void SetProjectionAggregateFunction(int rowIndex, LinqDataSourceAggregateFunctions function)
        {
            this._projectionsGrid.Rows[rowIndex].Cells[1].Value = function.ToString();
        }

        public void SetProjectionAlias(int rowIndex, string alias)
        {
            this._projectionsGrid.Rows[rowIndex].Cells[2].Value = alias;
        }

        public void SetProjectionField(int rowIndex, string field)
        {
            this._projectionsGrid.Rows[rowIndex].Cells[0].Value = field;
        }

        public void SetSelectMode(LinqDataSourceGroupByMode SelectMode)
        {
            switch (SelectMode)
            {
                case LinqDataSourceGroupByMode.GroupByNone:
                    this._groupByNonePanel.Visible = true;
                    this._groupByFieldPanel.Visible = false;
                    this._groupByCustomPanel.Visible = false;
                    return;

                case LinqDataSourceGroupByMode.GroupByField:
                    this._groupByNonePanel.Visible = false;
                    this._groupByFieldPanel.Visible = true;
                    this._groupByCustomPanel.Visible = false;
                    return;
            }
            this._groupByNonePanel.Visible = false;
            this._groupByFieldPanel.Visible = false;
            this._groupByCustomPanel.Visible = true;
        }

        public void UncheckFieldCheckboxes()
        {
            this._ignoreCheckEvents = true;
            for (int i = 1; i < this._projectionsCheckedListBox.Items.Count; i++)
            {
                this._projectionsCheckedListBox.SetItemChecked(i, false);
            }
            this._projectionsCheckedListBox.SetItemChecked(0, true);
            this._ignoreCheckEvents = false;
        }

        public void UncheckStarCheckbox()
        {
            this._ignoreCheckEvents = true;
            this._projectionsCheckedListBox.SetItemChecked(0, false);
            this._ignoreCheckEvents = false;
        }
    }
}

