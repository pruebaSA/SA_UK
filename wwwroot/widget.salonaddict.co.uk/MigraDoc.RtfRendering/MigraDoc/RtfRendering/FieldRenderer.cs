namespace MigraDoc.RtfRendering
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.RtfRendering.Resources;
    using System;

    internal abstract class FieldRenderer : RendererBase
    {
        internal FieldRenderer(DocumentObject domObj, RtfDocumentRenderer docRenderer) : base(domObj, docRenderer)
        {
        }

        protected void EndField()
        {
            base.rtfWriter.EndContent();
            base.rtfWriter.StartContent();
            base.rtfWriter.WriteControl("fldrslt");
            base.rtfWriter.WriteText(this.GetFieldResult());
            base.rtfWriter.EndContent();
            base.rtfWriter.EndContent();
        }

        protected virtual string GetFieldResult() => 
            Messages.UpdateField;

        protected void StartField()
        {
            base.rtfWriter.StartContent();
            base.rtfWriter.WriteControl("field");
            base.rtfWriter.WriteControl("flddirty");
            base.rtfWriter.StartContent();
            base.rtfWriter.WriteControl("fldinst", true);
        }
    }
}

