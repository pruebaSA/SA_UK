namespace System.Windows.Forms.Design
{
    using System;
    using System.Collections;
    using System.Windows.Forms;

    internal class ToolStripItemDataObject : DataObject
    {
        private ArrayList dragComponents;
        private ToolStrip owner;
        private ToolStripItem primarySelection;

        internal ToolStripItemDataObject(ArrayList dragComponents, ToolStripItem primarySelection, ToolStrip owner)
        {
            this.dragComponents = dragComponents;
            this.owner = owner;
            this.primarySelection = primarySelection;
        }

        internal ArrayList DragComponents =>
            this.dragComponents;

        internal ToolStrip Owner =>
            this.owner;

        internal ToolStripItem PrimarySelection =>
            this.primarySelection;
    }
}

