namespace PdfSharp.Drawing
{
    using System;

    internal class InternalGraphicsState
    {
        internal XGraphics gfx;
        internal bool invalid;
        internal XGraphicsState state;
        private XMatrix transform;

        public InternalGraphicsState(XGraphics gfx)
        {
            this.transform = new XMatrix();
            this.gfx = gfx;
        }

        public InternalGraphicsState(XGraphics gfx, XGraphicsContainer container)
        {
            this.transform = new XMatrix();
            this.gfx = gfx;
            container.InternalState = this;
        }

        public InternalGraphicsState(XGraphics gfx, XGraphicsState state)
        {
            this.transform = new XMatrix();
            this.gfx = gfx;
            this.state = state;
            state.InternalState = this;
        }

        public void Popped()
        {
            this.invalid = true;
        }

        public void Pushed()
        {
        }

        public XMatrix Transform
        {
            get => 
                this.transform;
            set
            {
                this.transform = value;
            }
        }
    }
}

