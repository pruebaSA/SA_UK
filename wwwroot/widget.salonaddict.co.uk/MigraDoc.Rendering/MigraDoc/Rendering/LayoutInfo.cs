namespace MigraDoc.Rendering
{
    using PdfSharp.Drawing;
    using System;

    internal class LayoutInfo
    {
        private Area contentArea;
        private MigraDoc.Rendering.Floating floating;
        private ElementAlignment horizontalAlignment;
        private MigraDoc.Rendering.HorizontalReference horizontalReference;
        private bool keepTogether;
        private bool keepWithNext;
        private XUnit left;
        private XUnit marginBottom;
        private XUnit marginLeft;
        private XUnit marginRight;
        private XUnit marginTop;
        private XUnit minWidth;
        private bool pageBreakBefore;
        protected XUnit startingHeight;
        private XUnit top;
        private XUnit trailingHeight;
        private ElementAlignment verticalAlignment;
        private MigraDoc.Rendering.VerticalReference verticalReference;

        internal LayoutInfo()
        {
        }

        internal Area ContentArea
        {
            get => 
                this.contentArea;
            set
            {
                this.contentArea = value;
            }
        }

        internal MigraDoc.Rendering.Floating Floating
        {
            get => 
                this.floating;
            set
            {
                this.floating = value;
            }
        }

        internal ElementAlignment HorizontalAlignment
        {
            get => 
                this.horizontalAlignment;
            set
            {
                this.horizontalAlignment = value;
            }
        }

        internal MigraDoc.Rendering.HorizontalReference HorizontalReference
        {
            get => 
                this.horizontalReference;
            set
            {
                this.horizontalReference = value;
            }
        }

        internal bool KeepTogether
        {
            get => 
                this.keepTogether;
            set
            {
                this.keepTogether = value;
            }
        }

        internal bool KeepWithNext
        {
            get => 
                this.keepWithNext;
            set
            {
                this.keepWithNext = value;
            }
        }

        internal XUnit Left
        {
            get => 
                this.left;
            set
            {
                this.left = value;
            }
        }

        internal XUnit MarginBottom
        {
            get => 
                this.marginBottom;
            set
            {
                this.marginBottom = value;
            }
        }

        internal XUnit MarginLeft
        {
            get => 
                this.marginLeft;
            set
            {
                this.marginLeft = value;
            }
        }

        internal XUnit MarginRight
        {
            get => 
                this.marginRight;
            set
            {
                this.marginRight = value;
            }
        }

        internal virtual XUnit MarginTop
        {
            get => 
                this.marginTop;
            set
            {
                this.marginTop = value;
            }
        }

        internal XUnit MinWidth
        {
            get => 
                this.minWidth;
            set
            {
                this.minWidth = value;
            }
        }

        internal bool PageBreakBefore
        {
            get => 
                this.pageBreakBefore;
            set
            {
                this.pageBreakBefore = value;
            }
        }

        internal XUnit StartingHeight
        {
            get => 
                this.startingHeight;
            set
            {
                this.startingHeight = value;
            }
        }

        internal XUnit Top
        {
            get => 
                this.top;
            set
            {
                this.top = value;
            }
        }

        internal XUnit TrailingHeight
        {
            get => 
                this.trailingHeight;
            set
            {
                this.trailingHeight = value;
            }
        }

        internal ElementAlignment VerticalAlignment
        {
            get => 
                this.verticalAlignment;
            set
            {
                this.verticalAlignment = value;
            }
        }

        internal MigraDoc.Rendering.VerticalReference VerticalReference
        {
            get => 
                this.verticalReference;
            set
            {
                this.verticalReference = value;
            }
        }
    }
}

