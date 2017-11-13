﻿namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.Design;
    using System.Drawing;
    using System.Web.UI.Design.Util;
    using System.Web.UI.WebControls;
    using System.Windows.Forms;
    using System.Windows.Forms.Design;

    internal sealed class TreeNodeCollectionEditorDialog : CollectionEditorDialog
    {
        private ToolStripButton _addChildButton;
        private ToolStripButton _addRootButton;
        private System.Windows.Forms.Button _cancelButton;
        private ToolStripButton _indentButton;
        private ToolStripButton _moveDownButton;
        private ToolStripButton _moveUpButton;
        private System.Windows.Forms.Label _nodesLabel;
        private System.Windows.Forms.Button _okButton;
        private System.Windows.Forms.Label _propertiesLabel;
        private PropertyGrid _propertyGrid;
        private ToolStripButton _removeButton;
        private ToolStripSeparator _toolBarSeparator;
        private System.Windows.Forms.TreeView _treeView;
        private System.Web.UI.Design.WebControls.TreeViewDesigner _treeViewDesigner;
        private System.Windows.Forms.Panel _treeViewPanel;
        private ToolStrip _treeViewToolBar;
        private ToolStripButton _unindentButton;
        private System.Web.UI.WebControls.TreeView _webTreeView;

        public TreeNodeCollectionEditorDialog(System.Web.UI.WebControls.TreeView treeView, System.Web.UI.Design.WebControls.TreeViewDesigner treeViewDesigner) : base(treeView.Site)
        {
            this._webTreeView = treeView;
            this._treeViewDesigner = treeViewDesigner;
            this._treeViewPanel = new System.Windows.Forms.Panel();
            this._treeView = new System.Windows.Forms.TreeView();
            this._treeViewToolBar = new ToolStrip();
            ToolStripRenderer toolStripRenderer = UIServiceHelper.GetToolStripRenderer(base.ServiceProvider);
            if (toolStripRenderer != null)
            {
                this._treeViewToolBar.Renderer = toolStripRenderer;
            }
            this._propertyGrid = new VsPropertyGrid(base.ServiceProvider);
            this._okButton = new System.Windows.Forms.Button();
            this._cancelButton = new System.Windows.Forms.Button();
            this._propertiesLabel = new System.Windows.Forms.Label();
            this._nodesLabel = new System.Windows.Forms.Label();
            this._addRootButton = base.CreatePushButton(System.Design.SR.GetString("TreeNodeCollectionEditor_AddRoot"), 3);
            this._addChildButton = base.CreatePushButton(System.Design.SR.GetString("TreeNodeCollectionEditor_AddChild"), 2);
            this._removeButton = base.CreatePushButton(System.Design.SR.GetString("TreeNodeCollectionEditor_Remove"), 4);
            this._moveUpButton = base.CreatePushButton(System.Design.SR.GetString("TreeNodeCollectionEditor_MoveUp"), 5);
            this._moveDownButton = base.CreatePushButton(System.Design.SR.GetString("TreeNodeCollectionEditor_MoveDown"), 6);
            this._indentButton = base.CreatePushButton(System.Design.SR.GetString("TreeNodeCollectionEditor_Indent"), 1);
            this._unindentButton = base.CreatePushButton(System.Design.SR.GetString("TreeNodeCollectionEditor_Unindent"), 0);
            this._toolBarSeparator = new ToolStripSeparator();
            this._treeViewPanel.SuspendLayout();
            base.SuspendLayout();
            this._treeViewPanel.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            this._treeViewPanel.BackColor = SystemColors.ControlDark;
            this._treeViewPanel.Controls.Add(this._treeView);
            this._treeViewPanel.DockPadding.Left = 1;
            this._treeViewPanel.DockPadding.Right = 1;
            this._treeViewPanel.DockPadding.Bottom = 1;
            this._treeViewPanel.DockPadding.Top = 1;
            this._treeViewPanel.Location = new Point(12, 0x36);
            this._treeViewPanel.Name = "_treeViewPanel";
            this._treeViewPanel.Size = new Size(0xe3, 0xe9);
            this._treeViewPanel.TabIndex = 1;
            this._treeView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._treeView.Dock = DockStyle.Fill;
            this._treeView.ImageIndex = -1;
            this._treeView.HideSelection = false;
            this._treeView.Location = new Point(1, 1);
            this._treeView.Name = "_treeView";
            this._treeView.SelectedImageIndex = -1;
            this._treeView.TabIndex = 0;
            this._treeView.AfterSelect += new TreeViewEventHandler(this.OnTreeViewAfterSelect);
            this._treeView.KeyDown += new KeyEventHandler(this.OnTreeViewKeyDown);
            this._treeViewToolBar.Items.AddRange(new ToolStripItem[] { this._addRootButton, this._addChildButton, this._removeButton, this._toolBarSeparator, this._moveUpButton, this._moveDownButton, this._unindentButton, this._indentButton });
            this._treeViewToolBar.Location = new Point(12, 0x1c);
            this._treeViewToolBar.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            this._treeViewToolBar.AutoSize = false;
            this._treeViewToolBar.Size = new Size(0xe3, 0x1a);
            this._treeViewToolBar.CanOverflow = false;
            Padding padding = this._treeViewToolBar.Padding;
            padding.Left = 2;
            this._treeViewToolBar.Padding = padding;
            this._treeViewToolBar.Name = "_treeViewToolBar";
            this._treeViewToolBar.ShowItemToolTips = true;
            this._treeViewToolBar.GripStyle = ToolStripGripStyle.Hidden;
            this._treeViewToolBar.TabIndex = 1;
            this._treeViewToolBar.TabStop = true;
            this._treeViewToolBar.ItemClicked += new ToolStripItemClickedEventHandler(this.OnTreeViewToolBarButtonClick);
            this._propertyGrid.Anchor = AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Top;
            this._propertyGrid.CommandsVisibleIfAvailable = true;
            this._propertyGrid.LargeButtons = false;
            this._propertyGrid.LineColor = SystemColors.ScrollBar;
            this._propertyGrid.Location = new Point(260, 0x1c);
            this._propertyGrid.Name = "_propertyGrid";
            this._propertyGrid.PropertySort = PropertySort.Alphabetical;
            this._propertyGrid.Size = new Size(0xcc, 0x103);
            this._propertyGrid.TabIndex = 3;
            this._propertyGrid.Text = System.Design.SR.GetString("MenuItemCollectionEditor_PropertyGrid");
            this._propertyGrid.ToolbarVisible = true;
            this._propertyGrid.ViewBackColor = SystemColors.Window;
            this._propertyGrid.ViewForeColor = SystemColors.WindowText;
            this._propertyGrid.PropertyValueChanged += new PropertyValueChangedEventHandler(this.OnPropertyGridPropertyValueChanged);
            this._propertyGrid.Site = this._webTreeView.Site;
            this._okButton.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            this._okButton.FlatStyle = FlatStyle.System;
            this._okButton.Location = new Point(0x135, 0x128);
            this._okButton.Name = "_okButton";
            this._okButton.Size = new Size(0x4b, 0x17);
            this._okButton.TabIndex = 9;
            this._okButton.Text = System.Design.SR.GetString("TreeNodeCollectionEditor_OK");
            this._okButton.Click += new EventHandler(this.OnOkButtonClick);
            this._cancelButton.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            this._cancelButton.FlatStyle = FlatStyle.System;
            this._cancelButton.Location = new Point(0x185, 0x128);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new Size(0x4b, 0x17);
            this._cancelButton.TabIndex = 10;
            this._cancelButton.Text = System.Design.SR.GetString("TreeNodeCollectionEditor_Cancel");
            this._cancelButton.Click += new EventHandler(this.OnCancelButtonClick);
            this._propertiesLabel.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            this._propertiesLabel.Location = new Point(260, 12);
            this._propertiesLabel.Name = "_propertiesLabel";
            this._propertiesLabel.Size = new Size(0xcc, 14);
            this._propertiesLabel.TabIndex = 2;
            this._propertiesLabel.Text = System.Design.SR.GetString("TreeNodeCollectionEditor_Properties");
            this._nodesLabel.Location = new Point(12, 12);
            this._nodesLabel.Name = "_nodesLabel";
            this._nodesLabel.Size = new Size(100, 14);
            this._nodesLabel.TabIndex = 0;
            this._nodesLabel.Text = System.Design.SR.GetString("TreeNodeCollectionEditor_Nodes");
            ImageList list = new ImageList {
                ImageSize = new Size(0x10, 0x10),
                TransparentColor = Color.Magenta
            };
            list.Images.AddStrip(new Bitmap(base.GetType(), "Commands.bmp"));
            this._treeViewToolBar.ImageList = list;
            base.ClientSize = new Size(0x1de, 0x14b);
            base.CancelButton = this._cancelButton;
            base.Controls.AddRange(new Control[] { this._nodesLabel, this._propertiesLabel, this._cancelButton, this._okButton, this._propertyGrid, this._treeViewPanel, this._treeViewToolBar });
            base.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MinimumSize = new Size(0x1e4, 0x14b);
            base.Name = "TreeNodeEditor";
            base.SizeGripStyle = SizeGripStyle.Hide;
            this.Text = System.Design.SR.GetString("TreeNodeCollectionEditor_Title");
            base.InitializeForm();
            this._treeViewPanel.ResumeLayout(false);
            base.ResumeLayout(false);
        }

        private void LoadNodes(System.Windows.Forms.TreeNodeCollection destNodes, System.Web.UI.WebControls.TreeNodeCollection sourceNodes)
        {
            foreach (System.Web.UI.WebControls.TreeNode node in sourceNodes)
            {
                TreeNodeContainer container = new TreeNodeContainer();
                destNodes.Add(container);
                container.Text = node.Text;
                System.Web.UI.WebControls.TreeNode clone = (System.Web.UI.WebControls.TreeNode) ((ICloneable) node).Clone();
                this._treeViewDesigner.RegisterClone(node, clone);
                container.WebTreeNode = clone;
                if (node.ChildNodes.Count > 0)
                {
                    this.LoadNodes(container.Nodes, node.ChildNodes);
                }
            }
        }

        private void OnAddChildButtonClick()
        {
            System.Windows.Forms.TreeNode selectedNode = this._treeView.SelectedNode;
            if (selectedNode != null)
            {
                TreeNodeContainer node = new TreeNodeContainer();
                selectedNode.Nodes.Add(node);
                string str = System.Design.SR.GetString("TreeNodeCollectionEditor_NewNodeText");
                node.Text = str;
                node.WebTreeNode.Text = str;
                if (!selectedNode.IsExpanded)
                {
                    selectedNode.Expand();
                }
                this._treeView.SelectedNode = node;
            }
        }

        private void OnAddRootButtonClick()
        {
            TreeNodeContainer node = new TreeNodeContainer();
            this._treeView.Nodes.Add(node);
            string str = System.Design.SR.GetString("TreeNodeCollectionEditor_NewNodeText");
            node.Text = str;
            node.WebTreeNode.Text = str;
            this._treeView.SelectedNode = node;
        }

        private void OnCancelButtonClick(object sender, EventArgs e)
        {
            base.DialogResult = DialogResult.Cancel;
            base.Close();
        }

        private void OnIndentButtonClick()
        {
            System.Windows.Forms.TreeNode selectedNode = this._treeView.SelectedNode;
            if (selectedNode != null)
            {
                System.Windows.Forms.TreeNode prevNode = selectedNode.PrevNode;
                if (prevNode != null)
                {
                    selectedNode.Remove();
                    prevNode.Nodes.Add(selectedNode);
                    this._treeView.SelectedNode = selectedNode;
                }
            }
        }

        protected override void OnInitialActivated(EventArgs e)
        {
            base.OnInitialActivated(e);
            this.LoadNodes(this._treeView.Nodes, this._webTreeView.Nodes);
            if (this._treeView.Nodes.Count > 0)
            {
                this._treeView.SelectedNode = this._treeView.Nodes[0];
            }
            this.UpdateEnabledState();
        }

        private void OnMoveDownButtonClick()
        {
            System.Windows.Forms.TreeNode selectedNode = this._treeView.SelectedNode;
            if (selectedNode != null)
            {
                System.Windows.Forms.TreeNode nextNode = selectedNode.NextNode;
                System.Windows.Forms.TreeNodeCollection nodes = this._treeView.Nodes;
                if (selectedNode.Parent != null)
                {
                    nodes = selectedNode.Parent.Nodes;
                }
                if (nextNode != null)
                {
                    selectedNode.Remove();
                    nodes.Insert(nextNode.Index + 1, selectedNode);
                    this._treeView.SelectedNode = selectedNode;
                }
            }
        }

        private void OnMoveUpButtonClick()
        {
            System.Windows.Forms.TreeNode selectedNode = this._treeView.SelectedNode;
            if (selectedNode != null)
            {
                System.Windows.Forms.TreeNode prevNode = selectedNode.PrevNode;
                System.Windows.Forms.TreeNodeCollection nodes = this._treeView.Nodes;
                if (selectedNode.Parent != null)
                {
                    nodes = selectedNode.Parent.Nodes;
                }
                if (prevNode != null)
                {
                    selectedNode.Remove();
                    nodes.Insert(prevNode.Index, selectedNode);
                    this._treeView.SelectedNode = selectedNode;
                }
            }
        }

        private void OnOkButtonClick(object sender, EventArgs e)
        {
            this.SaveNodes(this._webTreeView.Nodes, this._treeView.Nodes);
            base.DialogResult = DialogResult.OK;
            base.Close();
        }

        private void OnPropertyGridPropertyValueChanged(object sender, PropertyValueChangedEventArgs e)
        {
            TreeNodeContainer selectedNode = (TreeNodeContainer) this._treeView.SelectedNode;
            if (selectedNode != null)
            {
                selectedNode.Text = selectedNode.WebTreeNode.Text;
            }
            this._propertyGrid.Refresh();
        }

        private void OnRemoveButtonClick()
        {
            System.Windows.Forms.TreeNode selectedNode = this._treeView.SelectedNode;
            if (selectedNode != null)
            {
                System.Windows.Forms.TreeNodeCollection nodes = null;
                if (selectedNode.Parent != null)
                {
                    nodes = selectedNode.Parent.Nodes;
                }
                else
                {
                    nodes = this._treeView.Nodes;
                }
                if (nodes.Count == 1)
                {
                    this._treeView.SelectedNode = selectedNode.Parent;
                }
                else if (selectedNode.NextNode != null)
                {
                    this._treeView.SelectedNode = selectedNode.NextNode;
                }
                else
                {
                    this._treeView.SelectedNode = selectedNode.PrevNode;
                }
                selectedNode.Remove();
                if (this._treeView.SelectedNode == null)
                {
                    this._propertyGrid.SelectedObject = null;
                }
                this.UpdateEnabledState();
            }
        }

        private void OnTreeViewAfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node != null)
            {
                this._propertyGrid.SelectedObject = ((TreeNodeContainer) e.Node).WebTreeNode;
            }
            else
            {
                this._propertyGrid.SelectedObject = null;
            }
            this.UpdateEnabledState();
        }

        private void OnTreeViewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Insert)
            {
                if ((Control.ModifierKeys & Keys.Alt) != Keys.None)
                {
                    this.OnAddChildButtonClick();
                }
                else
                {
                    this.OnAddRootButtonClick();
                }
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Delete)
            {
                this.OnRemoveButtonClick();
                e.Handled = true;
            }
            else if ((Control.ModifierKeys & Keys.Shift) != Keys.None)
            {
                if (e.KeyCode == Keys.Up)
                {
                    this.OnMoveUpButtonClick();
                }
                else if (e.KeyCode == Keys.Down)
                {
                    this.OnMoveDownButtonClick();
                }
                else if (e.KeyCode == Keys.Left)
                {
                    this.OnUnindentButtonClick();
                }
                else if (e.KeyCode == Keys.Right)
                {
                    this.OnIndentButtonClick();
                }
                e.Handled = true;
            }
        }

        private void OnTreeViewToolBarButtonClick(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem == this._addRootButton)
            {
                this.OnAddRootButtonClick();
            }
            else if (e.ClickedItem == this._addChildButton)
            {
                this.OnAddChildButtonClick();
            }
            else if (e.ClickedItem == this._removeButton)
            {
                this.OnRemoveButtonClick();
            }
            else if (e.ClickedItem == this._moveUpButton)
            {
                this.OnMoveUpButtonClick();
            }
            else if (e.ClickedItem == this._unindentButton)
            {
                this.OnUnindentButtonClick();
            }
            else if (e.ClickedItem == this._indentButton)
            {
                this.OnIndentButtonClick();
            }
            else if (e.ClickedItem == this._moveDownButton)
            {
                this.OnMoveDownButtonClick();
            }
        }

        private void OnUnindentButtonClick()
        {
            System.Windows.Forms.TreeNode selectedNode = this._treeView.SelectedNode;
            if (selectedNode != null)
            {
                System.Windows.Forms.TreeNode parent = selectedNode.Parent;
                if (parent != null)
                {
                    System.Windows.Forms.TreeNodeCollection nodes = this._treeView.Nodes;
                    if (parent.Parent != null)
                    {
                        nodes = parent.Parent.Nodes;
                    }
                    if (parent != null)
                    {
                        selectedNode.Remove();
                        nodes.Insert(parent.Index + 1, selectedNode);
                        this._treeView.SelectedNode = selectedNode;
                    }
                }
            }
        }

        private void SaveNodes(System.Web.UI.WebControls.TreeNodeCollection destNodes, System.Windows.Forms.TreeNodeCollection sourceNodes)
        {
            destNodes.Clear();
            foreach (TreeNodeContainer container in sourceNodes)
            {
                System.Web.UI.WebControls.TreeNode webTreeNode = container.WebTreeNode;
                destNodes.Add(webTreeNode);
                if (container.Nodes.Count > 0)
                {
                    this.SaveNodes(webTreeNode.ChildNodes, container.Nodes);
                }
            }
        }

        private void UpdateEnabledState()
        {
            System.Windows.Forms.TreeNode selectedNode = this._treeView.SelectedNode;
            if (selectedNode != null)
            {
                this._addChildButton.Enabled = true;
                this._removeButton.Enabled = true;
                this._moveUpButton.Enabled = selectedNode.PrevNode != null;
                this._moveDownButton.Enabled = selectedNode.NextNode != null;
                this._indentButton.Enabled = selectedNode.PrevNode != null;
                this._unindentButton.Enabled = selectedNode.Parent != null;
            }
            else
            {
                this._addChildButton.Enabled = false;
                this._removeButton.Enabled = false;
                this._moveUpButton.Enabled = false;
                this._moveDownButton.Enabled = false;
                this._indentButton.Enabled = false;
                this._unindentButton.Enabled = false;
            }
        }

        protected override string HelpTopic =>
            "net.Asp.TreeView.CollectionEditor";

        private class TreeNodeContainer : System.Windows.Forms.TreeNode
        {
            private System.Web.UI.WebControls.TreeNode _webTreeNode;

            public System.Web.UI.WebControls.TreeNode WebTreeNode
            {
                get
                {
                    if (this._webTreeNode == null)
                    {
                        this._webTreeNode = new System.Web.UI.WebControls.TreeNode();
                    }
                    return this._webTreeNode;
                }
                set
                {
                    this._webTreeNode = value;
                }
            }
        }
    }
}

