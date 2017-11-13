namespace System.Windows.Forms.Layout
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    public abstract class LayoutEngine
    {
        protected LayoutEngine()
        {
        }

        internal IArrangedElement CastToArrangedElement(object obj)
        {
            IArrangedElement element = obj as IArrangedElement;
            if (obj == null)
            {
                throw new NotSupportedException(System.Windows.Forms.SR.GetString("LayoutEngineUnsupportedType", new object[] { obj.GetType() }));
            }
            return element;
        }

        internal virtual Size GetPreferredSize(IArrangedElement container, Size proposedConstraints) => 
            Size.Empty;

        public virtual void InitLayout(object child, BoundsSpecified specified)
        {
            this.InitLayoutCore(this.CastToArrangedElement(child), specified);
        }

        internal virtual void InitLayoutCore(IArrangedElement element, BoundsSpecified bounds)
        {
        }

        public virtual bool Layout(object container, LayoutEventArgs layoutEventArgs) => 
            this.LayoutCore(this.CastToArrangedElement(container), layoutEventArgs);

        internal virtual bool LayoutCore(IArrangedElement container, LayoutEventArgs layoutEventArgs) => 
            false;

        internal virtual void ProcessSuspendedLayoutEventArgs(IArrangedElement container, LayoutEventArgs args)
        {
        }
    }
}

