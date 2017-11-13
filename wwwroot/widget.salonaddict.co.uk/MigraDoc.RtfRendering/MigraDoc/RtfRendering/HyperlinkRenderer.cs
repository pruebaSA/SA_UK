namespace MigraDoc.RtfRendering
{
    using MigraDoc.DocumentObjectModel;
    using System;
    using System.IO;

    internal class HyperlinkRenderer : RendererBase
    {
        private Hyperlink hyperlink;

        internal HyperlinkRenderer(DocumentObject domObj, RtfDocumentRenderer docRenderer) : base(domObj, docRenderer)
        {
            this.hyperlink = domObj as Hyperlink;
        }

        internal override void Render()
        {
            base.useEffectiveValue = true;
            base.rtfWriter.StartContent();
            base.rtfWriter.WriteControl("field");
            base.rtfWriter.StartContent();
            base.rtfWriter.WriteControl("fldinst", true);
            base.rtfWriter.WriteText("HYPERLINK ");
            string name = this.hyperlink.Name;
            if (this.hyperlink.IsNull("Type") || (this.hyperlink.Type == HyperlinkType.Local))
            {
                name = BookmarkFieldRenderer.MakeValidBookmarkName(this.hyperlink.Name);
                base.rtfWriter.WriteText(@"\l ");
            }
            else if (this.hyperlink.Type == HyperlinkType.File)
            {
                if (base.docRenderer.WorkingDirectory != null)
                {
                    name = Path.Combine(base.docRenderer.WorkingDirectory, name);
                }
                name = name.Replace(@"\", @"\\");
            }
            base.rtfWriter.WriteText("\"" + name + "\"");
            base.rtfWriter.EndContent();
            base.rtfWriter.StartContent();
            base.rtfWriter.WriteControl("fldrslt");
            base.rtfWriter.StartContent();
            base.rtfWriter.WriteControl("cs", base.docRenderer.GetStyleIndex("Hyperlink"));
            new FontRenderer(this.hyperlink.Font, base.docRenderer).Render();
            if (!this.hyperlink.IsNull("Elements"))
            {
                foreach (DocumentObject obj2 in this.hyperlink.Elements)
                {
                    RendererFactory.CreateRenderer(obj2, base.docRenderer).Render();
                }
            }
            base.rtfWriter.EndContent();
            base.rtfWriter.EndContent();
            base.rtfWriter.EndContent();
        }
    }
}

