namespace System.Windows.Forms.PropertyGridInternal
{
    using System;
    using System.Collections;
    using System.Drawing;
    using System.Windows.Forms;

    internal class CategoryGridEntry : GridEntry
    {
        private Brush backBrush;
        private static Hashtable categoryStates;
        internal string name;

        public CategoryGridEntry(PropertyGrid ownerGrid, GridEntry peParent, string name, GridEntry[] childGridEntries) : base(ownerGrid, peParent)
        {
            this.name = name;
            if (categoryStates == null)
            {
                categoryStates = new Hashtable();
            }
            lock (categoryStates)
            {
                if (!categoryStates.ContainsKey(name))
                {
                    categoryStates.Add(name, true);
                }
            }
            this.IsExpandable = true;
            for (int i = 0; i < childGridEntries.Length; i++)
            {
                childGridEntries[i].ParentGridEntry = this;
            }
            base.ChildCollection = new GridEntryCollection(this, childGridEntries);
            lock (categoryStates)
            {
                this.InternalExpanded = (bool) categoryStates[name];
            }
            this.SetFlag(0x40, true);
        }

        protected override bool CreateChildren(bool diffOldChildren) => 
            true;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.backBrush != null)
                {
                    this.backBrush.Dispose();
                    this.backBrush = null;
                }
                if (base.ChildCollection != null)
                {
                    base.ChildCollection = null;
                }
            }
            base.Dispose(disposing);
        }

        public override void DisposeChildren()
        {
        }

        protected override Brush GetBackgroundBrush(Graphics g) => 
            this.GridEntryHost.GetLineBrush(g);

        public override object GetChildValueOwner(GridEntry childEntry) => 
            this.ParentGridEntry.GetChildValueOwner(childEntry);

        public override string GetPropertyTextValue(object o) => 
            "";

        public override string GetTestingInfo()
        {
            string str = "object = (";
            return ((str + base.FullLabel) + "), Category = (" + this.PropertyLabel + ")");
        }

        internal override bool NotifyChildValue(GridEntry pe, int type) => 
            base.parentPE.NotifyChildValue(pe, type);

        public override void PaintLabel(Graphics g, Rectangle rect, Rectangle clipRect, bool selected, bool paintFullLabel)
        {
            base.PaintLabel(g, rect, clipRect, false, true);
            if (selected && base.hasFocus)
            {
                bool boldFont = (this.Flags & 0x40) != 0;
                Font f = base.GetFont(boldFont);
                int num = base.GetLabelTextWidth(this.PropertyLabel, g, f);
                int x = this.PropertyLabelIndent - 2;
                Rectangle rectangle = new Rectangle(x, rect.Y, num + 3, rect.Height - 1);
                ControlPaint.DrawFocusRectangle(g, rectangle);
            }
            if (base.parentPE.GetChildIndex(this) > 0)
            {
                g.DrawLine(SystemPens.Control, (int) (rect.X - 1), (int) (rect.Y - 1), (int) (rect.Width + 2), (int) (rect.Y - 1));
            }
        }

        public override void PaintOutline(Graphics g, Rectangle r)
        {
            if (this.Expandable)
            {
                bool expanded = this.Expanded;
                Rectangle outlineRect = base.OutlineRect;
                outlineRect = Rectangle.Intersect(r, outlineRect);
                if (!outlineRect.IsEmpty)
                {
                    bool flag2 = false;
                    bool flag3 = false;
                    Color lineColor = this.GridEntryHost.GetLineColor();
                    Brush brush = new SolidBrush(g.GetNearestColor(lineColor));
                    flag2 = true;
                    lineColor = this.GridEntryHost.GetTextColor();
                    Pen pen = new Pen(g.GetNearestColor(lineColor));
                    flag3 = true;
                    g.FillRectangle(brush, outlineRect);
                    g.DrawRectangle(pen, outlineRect.X, outlineRect.Y, outlineRect.Width - 1, outlineRect.Height - 1);
                    int num = 2;
                    g.DrawLine(SystemPens.WindowText, (int) (outlineRect.X + num), (int) (outlineRect.Y + (outlineRect.Height / 2)), (int) (((outlineRect.X + outlineRect.Width) - num) - 1), (int) (outlineRect.Y + (outlineRect.Height / 2)));
                    if (!expanded)
                    {
                        g.DrawLine(SystemPens.WindowText, (int) (outlineRect.X + (outlineRect.Width / 2)), (int) (outlineRect.Y + num), (int) (outlineRect.X + (outlineRect.Width / 2)), (int) (((outlineRect.Y + outlineRect.Height) - num) - 1));
                    }
                    if (flag3)
                    {
                        pen.Dispose();
                    }
                    if (flag2)
                    {
                        brush.Dispose();
                    }
                }
            }
        }

        public override void PaintValue(object val, Graphics g, Rectangle rect, Rectangle clipRect, GridEntry.PaintValueFlags paintFlags)
        {
            base.PaintValue(val, g, rect, clipRect, paintFlags & ~GridEntry.PaintValueFlags.DrawSelected);
            if (base.parentPE.GetChildIndex(this) > 0)
            {
                g.DrawLine(SystemPens.Control, (int) (rect.X - 2), (int) (rect.Y - 1), (int) (rect.Width + 1), (int) (rect.Y - 1));
            }
        }

        public override bool Expandable =>
            !this.GetFlagSet(0x80000);

        public override System.Windows.Forms.GridItemType GridItemType =>
            System.Windows.Forms.GridItemType.Category;

        internal override bool HasValue =>
            false;

        public override string HelpKeyword =>
            null;

        internal override bool InternalExpanded
        {
            set
            {
                base.InternalExpanded = value;
                lock (categoryStates)
                {
                    categoryStates[this.name] = value;
                }
            }
        }

        protected override Color LabelTextColor =>
            base.ownerGrid.CategoryForeColor;

        public override int PropertyDepth =>
            (base.PropertyDepth - 1);

        public override string PropertyLabel =>
            this.name;

        internal override int PropertyLabelIndent
        {
            get
            {
                PropertyGridView gridEntryHost = this.GridEntryHost;
                return (((1 + gridEntryHost.GetOutlineIconSize()) + 5) + (base.PropertyDepth * gridEntryHost.GetDefaultOutlineIndent()));
            }
        }

        public override System.Type PropertyType =>
            typeof(void);
    }
}

