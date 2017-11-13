namespace System.Windows.Forms
{
    using System;
    using System.Globalization;

    internal class MergeHistoryItem
    {
        private int index = -1;
        private ToolStripItemCollection indexCollection;
        private System.Windows.Forms.MergeAction mergeAction;
        private int previousIndex = -1;
        private ToolStripItemCollection previousIndexCollection;
        private ToolStripItem targetItem;

        public MergeHistoryItem(System.Windows.Forms.MergeAction mergeAction)
        {
            this.mergeAction = mergeAction;
        }

        public override string ToString() => 
            ("MergeAction: " + this.mergeAction.ToString() + " | TargetItem: " + ((this.TargetItem == null) ? "null" : this.TargetItem.Text) + " Index: " + this.index.ToString(CultureInfo.CurrentCulture));

        public int Index
        {
            get => 
                this.index;
            set
            {
                this.index = value;
            }
        }

        public ToolStripItemCollection IndexCollection
        {
            get => 
                this.indexCollection;
            set
            {
                this.indexCollection = value;
            }
        }

        public System.Windows.Forms.MergeAction MergeAction =>
            this.mergeAction;

        public int PreviousIndex
        {
            get => 
                this.previousIndex;
            set
            {
                this.previousIndex = value;
            }
        }

        public ToolStripItemCollection PreviousIndexCollection
        {
            get => 
                this.previousIndexCollection;
            set
            {
                this.previousIndexCollection = value;
            }
        }

        public ToolStripItem TargetItem
        {
            get => 
                this.targetItem;
            set
            {
                this.targetItem = value;
            }
        }
    }
}

