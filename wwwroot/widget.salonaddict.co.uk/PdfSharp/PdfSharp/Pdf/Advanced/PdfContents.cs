namespace PdfSharp.Pdf.Advanced
{
    using PdfSharp.Pdf;
    using PdfSharp.Pdf.IO;
    using System;

    public sealed class PdfContents : PdfArray
    {
        private bool modified;

        internal PdfContents(PdfArray array) : base(array)
        {
            int count = base.Elements.Count;
            for (int i = 0; i < count; i++)
            {
                PdfItem item = base.Elements[i];
                PdfReference reference = item as PdfReference;
                if ((reference == null) || !(reference.Value is PdfDictionary))
                {
                    throw new InvalidOperationException("Unexpected item in a content stream array.");
                }
                new PdfContent((PdfDictionary) reference.Value);
            }
        }

        public PdfContents(PdfDocument document) : base(document)
        {
        }

        public PdfContent AppendContent()
        {
            this.SetModified();
            PdfContent content = new PdfContent(this.Owner);
            this.Owner.irefTable.Add(content);
            base.Elements.Add(content.Reference);
            return content;
        }

        public PdfContent CreateSingleContent()
        {
            byte[] array = new byte[0];
            foreach (PdfItem item in base.Elements)
            {
                PdfDictionary dictionary = (PdfDictionary) ((PdfReference) item).Value;
                byte[] buffer2 = array;
                byte[] unfilteredValue = dictionary.Stream.UnfilteredValue;
                array = new byte[(buffer2.Length + unfilteredValue.Length) + 1];
                buffer2.CopyTo(array, 0);
                array[buffer2.Length] = 10;
                unfilteredValue.CopyTo(array, (int) (buffer2.Length + 1));
            }
            PdfContent owner = new PdfContent(this.Owner);
            owner.Stream = new PdfDictionary.PdfStream(array, owner);
            return owner;
        }

        public PdfContent PrependContent()
        {
            this.SetModified();
            PdfContent content = new PdfContent(this.Owner);
            this.Owner.irefTable.Add(content);
            base.Elements.Insert(0, content.Reference);
            return content;
        }

        private void SetModified()
        {
            if (!this.modified)
            {
                this.modified = true;
                int count = base.Elements.Count;
                if (count == 1)
                {
                    ((PdfContent) ((PdfReference) base.Elements[0]).Value).PreserveGraphicsState();
                }
                else if (count > 1)
                {
                    byte[] buffer;
                    int length;
                    PdfContent content2 = (PdfContent) ((PdfReference) base.Elements[0]).Value;
                    if ((content2 != null) && (content2.Stream != null))
                    {
                        length = content2.Stream.Length;
                        buffer = new byte[length + 2];
                        buffer[0] = 0x71;
                        buffer[1] = 10;
                        Array.Copy(content2.Stream.Value, 0, buffer, 2, length);
                        content2.Stream.Value = buffer;
                        content2.Elements.SetInteger("/Length", length + 2);
                    }
                    content2 = (PdfContent) ((PdfReference) base.Elements[count - 1]).Value;
                    if ((content2 != null) && (content2.Stream != null))
                    {
                        length = content2.Stream.Length;
                        buffer = new byte[length + 3];
                        Array.Copy(content2.Stream.Value, 0, buffer, 0, length);
                        buffer[length] = 0x20;
                        buffer[length + 1] = 0x51;
                        buffer[length + 2] = 10;
                        content2.Stream.Value = buffer;
                        content2.Elements.SetInteger("/Length", length + 3);
                    }
                }
            }
        }

        internal override void WriteObject(PdfWriter writer)
        {
            if (base.Elements.Count == 1)
            {
                base.Elements[0].WriteObject(writer);
            }
            else
            {
                base.WriteObject(writer);
            }
        }
    }
}

