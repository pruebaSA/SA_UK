namespace MigraDoc.Rendering
{
    using PdfSharp.Drawing;
    using System;

    internal class Rectangle : Area
    {
        private XUnit height;
        private XUnit width;
        private XUnit x;
        private XUnit y;

        internal Rectangle(Rectangle rect)
        {
            this.x = rect.x;
            this.y = rect.y;
            this.width = rect.width;
            this.height = rect.height;
        }

        internal Rectangle(XUnit x, XUnit y, XUnit width, XUnit height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        internal override Rectangle GetFittingRect(XUnit yPosition, XUnit height)
        {
            if ((((double) yPosition) + ((double) height)) > ((((double) this.y) + ((double) this.height)) + ((double) Renderer.Tolerance)))
            {
                return null;
            }
            return new Rectangle(this.x, yPosition, this.width, height);
        }

        internal override Area Lower(XUnit verticalOffset) => 
            new Rectangle(this.x, ((double) this.y) + ((double) verticalOffset), this.width, ((double) this.height) - ((double) verticalOffset));

        internal override Area Unite(Area area)
        {
            if (area == null)
            {
                return this;
            }
            XUnit y = Math.Min((double) this.y, (double) area.Y);
            XUnit x = Math.Min((double) this.x, (double) area.X);
            XUnit unit3 = Math.Max((double) (((double) this.x) + ((double) this.width)), (double) (((double) area.X) + ((double) area.Width)));
            XUnit unit4 = Math.Max((double) (((double) this.y) + ((double) this.height)), (double) (((double) area.Y) + ((double) area.Height)));
            return new Rectangle(x, y, ((double) unit3) - ((double) x), ((double) unit4) - ((double) y));
        }

        internal override XUnit Height
        {
            get => 
                this.height;
            set
            {
                this.height = value;
            }
        }

        internal override XUnit Width
        {
            get => 
                this.width;
            set
            {
                this.width = value;
            }
        }

        internal override XUnit X
        {
            get => 
                this.x;
            set
            {
                this.x = value;
            }
        }

        internal override XUnit Y
        {
            get => 
                this.y;
            set
            {
                this.y = value;
            }
        }
    }
}

