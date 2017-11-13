namespace MigraDoc.RtfRendering
{
    using MigraDoc.DocumentObjectModel;
    using System;

    internal class BorderRenderer : BorderRendererBase
    {
        private Border border;
        private MigraDoc.DocumentObjectModel.BorderType borderType;

        public BorderRenderer(DocumentObject domObj, RtfDocumentRenderer docRenderer) : base(domObj, docRenderer)
        {
            this.border = domObj as Border;
        }

        internal override void Render()
        {
            base.useEffectiveValue = true;
            base.RenderBorder(base.GetBorderControl(this.borderType));
        }

        internal MigraDoc.DocumentObjectModel.BorderType BorderType
        {
            set
            {
                this.borderType = value;
            }
        }
    }
}

