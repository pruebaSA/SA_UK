namespace MigraDoc.RtfRendering
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Fields;
    using System;
    using System.Text;

    internal class BookmarkFieldRenderer : RendererBase
    {
        private BookmarkField bookmark;

        public BookmarkFieldRenderer(DocumentObject domObj, RtfDocumentRenderer docRenderer) : base(domObj, docRenderer)
        {
            this.bookmark = domObj as BookmarkField;
        }

        internal static string MakeValidBookmarkName(string originalName)
        {
            StringBuilder builder = new StringBuilder(originalName.Length);
            if (!char.IsLetter(originalName[0]))
            {
                builder.Append("BM__");
            }
            for (int i = 0; i < originalName.Length; i++)
            {
                char c = originalName[i];
                if (char.IsLetterOrDigit(c))
                {
                    builder.Append(c);
                }
                else
                {
                    builder.Append('_');
                }
            }
            return builder.ToString();
        }

        internal override void Render()
        {
            base.rtfWriter.StartContent();
            base.rtfWriter.WriteControl("bkmkstart", true);
            string text = MakeValidBookmarkName(this.bookmark.Name);
            base.rtfWriter.WriteText(text);
            base.rtfWriter.EndContent();
            base.rtfWriter.StartContent();
            base.rtfWriter.WriteControl("bkmkend", true);
            base.rtfWriter.WriteText(text);
            base.rtfWriter.EndContent();
        }
    }
}

