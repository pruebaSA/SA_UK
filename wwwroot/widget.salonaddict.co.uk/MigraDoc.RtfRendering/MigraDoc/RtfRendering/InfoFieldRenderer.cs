namespace MigraDoc.RtfRendering
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Fields;
    using System;

    internal class InfoFieldRenderer : FieldRenderer
    {
        private InfoField infoField;

        internal InfoFieldRenderer(DocumentObject domObj, RtfDocumentRenderer docRenderer) : base(domObj, docRenderer)
        {
            this.infoField = domObj as InfoField;
        }

        protected override string GetFieldResult()
        {
            Document document = this.infoField.Document;
            if (!document.IsNull("Info." + this.infoField.Name))
            {
                return (document.Info.GetValue(this.infoField.Name) as string);
            }
            return "";
        }

        internal override void Render()
        {
            base.StartField();
            base.rtfWriter.WriteText("INFO ");
            string str = this.infoField.Name.ToLower();
            if (str != null)
            {
                if (str == "author")
                {
                    base.rtfWriter.WriteText("Author");
                }
                else if (str == "title")
                {
                    base.rtfWriter.WriteText("Title");
                }
                else if (str == "keywords")
                {
                    base.rtfWriter.WriteText("Keywords");
                }
                else if (str == "subject")
                {
                    base.rtfWriter.WriteText("Subject");
                }
            }
            base.EndField();
        }
    }
}

