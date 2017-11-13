namespace PdfSharp.Pdf.Advanced
{
    using PdfSharp.Drawing;
    using PdfSharp.Pdf;
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    internal sealed class PdfFormXObjectTable : PdfResourceTable
    {
        private readonly Dictionary<Selector, PdfImportedObjectTable> forms;

        public PdfFormXObjectTable(PdfDocument document) : base(document)
        {
            this.forms = new Dictionary<Selector, PdfImportedObjectTable>();
        }

        public void DetachDocument(PdfDocument.DocumentHandle handle)
        {
            if (handle.IsAlive)
            {
                foreach (Selector selector in this.forms.Keys)
                {
                    PdfImportedObjectTable table = this.forms[selector];
                    if ((table.ExternalDocument != null) && (table.ExternalDocument.Handle == handle))
                    {
                        this.forms.Remove(selector);
                        break;
                    }
                }
            }
            bool flag = true;
            while (flag)
            {
                flag = false;
                foreach (Selector selector2 in this.forms.Keys)
                {
                    PdfImportedObjectTable table2 = this.forms[selector2];
                    if (table2.ExternalDocument == null)
                    {
                        this.forms.Remove(selector2);
                        flag = true;
                        continue;
                    }
                }
            }
        }

        public PdfFormXObject GetForm(XForm form)
        {
            if (form.pdfForm != null)
            {
                if (object.ReferenceEquals(form.pdfForm.Owner, base.owner))
                {
                    return form.pdfForm;
                }
                form.pdfForm = null;
            }
            XPdfForm form2 = form as XPdfForm;
            if (form2 != null)
            {
                PdfImportedObjectTable table;
                Selector key = new Selector(form);
                if (!this.forms.TryGetValue(key, out table))
                {
                    PdfDocument externalDocument = form2.ExternalDocument;
                    table = new PdfImportedObjectTable(base.owner, externalDocument);
                    this.forms[key] = table;
                }
                PdfFormXObject xObject = table.GetXObject(form2.PageNumber);
                if (xObject == null)
                {
                    xObject = new PdfFormXObject(base.owner, table, form2);
                    table.SetXObject(form2.PageNumber, xObject);
                }
                return xObject;
            }
            form.pdfForm = new PdfFormXObject(base.owner, form);
            return form.pdfForm;
        }

        public PdfImportedObjectTable GetImportedObjectTable(PdfPage page)
        {
            PdfImportedObjectTable table;
            Selector key = new Selector(page);
            if (!this.forms.TryGetValue(key, out table))
            {
                table = new PdfImportedObjectTable(base.owner, page.Owner);
                this.forms[key] = table;
            }
            return table;
        }

        public class Selector
        {
            private string path;

            public Selector(XForm form)
            {
                this.path = form.path.ToLower(CultureInfo.InvariantCulture);
            }

            public Selector(PdfPage page)
            {
                PdfDocument owner = page.Owner;
                this.path = "*" + owner.Guid.ToString("B");
                this.path = this.path.ToLower(CultureInfo.InvariantCulture);
            }

            public override bool Equals(object obj)
            {
                PdfFormXObjectTable.Selector selector = obj as PdfFormXObjectTable.Selector;
                if (obj == null)
                {
                    return false;
                }
                return (this.path == selector.path);
            }

            public override int GetHashCode() => 
                this.path.GetHashCode();

            public string Path
            {
                get => 
                    this.path;
                set
                {
                    this.path = value;
                }
            }
        }
    }
}

