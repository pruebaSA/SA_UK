namespace System.Windows.Forms.Design.Behavior
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;
    using System.Windows.Forms.Design;

    internal abstract class SelectionGlyphBase : Glyph
    {
        protected Rectangle bounds;
        protected Rectangle hitBounds;
        protected Cursor hitTestCursor;
        protected System.Windows.Forms.Design.SelectionRules rules;

        internal SelectionGlyphBase(System.Windows.Forms.Design.Behavior.Behavior behavior) : base(behavior)
        {
        }

        public override Cursor GetHitTest(Point p)
        {
            if (this.hitBounds.Contains(p))
            {
                return this.hitTestCursor;
            }
            return null;
        }

        public override void Paint(PaintEventArgs pe)
        {
        }

        public override Rectangle Bounds =>
            this.bounds;

        public Cursor HitTestCursor =>
            this.hitTestCursor;

        public System.Windows.Forms.Design.SelectionRules SelectionRules =>
            this.rules;
    }
}

