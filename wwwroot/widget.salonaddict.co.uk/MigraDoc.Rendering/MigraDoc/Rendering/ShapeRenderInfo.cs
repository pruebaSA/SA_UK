namespace MigraDoc.Rendering
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Shapes;
    using System;

    internal abstract class ShapeRenderInfo : RenderInfo
    {
        internal Shape shape;

        internal ShapeRenderInfo()
        {
        }

        internal override MigraDoc.DocumentObjectModel.DocumentObject DocumentObject =>
            this.shape;
    }
}

