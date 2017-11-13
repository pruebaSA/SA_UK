namespace PdfSharp.Pdf.Internal
{
    using PdfSharp.Pdf;
    using PdfSharp.Pdf.IO;
    using System;
    using System.Collections.Generic;

    internal class ThreadLocalStorage
    {
        private readonly Dictionary<string, PdfDocument.DocumentHandle> importedDocuments = new Dictionary<string, PdfDocument.DocumentHandle>(StringComparer.InvariantCultureIgnoreCase);

        public void AddDocument(string path, PdfDocument document)
        {
            this.importedDocuments.Add(path, document.Handle);
        }

        public void DetachDocument(PdfDocument.DocumentHandle handle)
        {
            if (handle.IsAlive)
            {
                foreach (string str in this.importedDocuments.Keys)
                {
                    if (this.importedDocuments[str] == handle)
                    {
                        this.importedDocuments.Remove(str);
                        break;
                    }
                }
            }
            bool flag = true;
            while (flag)
            {
                flag = false;
                foreach (string str2 in this.importedDocuments.Keys)
                {
                    if (!this.importedDocuments[str2].IsAlive)
                    {
                        this.importedDocuments.Remove(str2);
                        flag = true;
                        continue;
                    }
                }
            }
        }

        public PdfDocument GetDocument(string path)
        {
            PdfDocument target = null;
            PdfDocument.DocumentHandle handle;
            if (this.importedDocuments.TryGetValue(path, out handle))
            {
                target = handle.Target;
                if (target == null)
                {
                    this.RemoveDocument(path);
                }
            }
            if (target == null)
            {
                target = PdfReader.Open(path, PdfDocumentOpenMode.Import);
                this.importedDocuments.Add(path, target.Handle);
            }
            return target;
        }

        public void RemoveDocument(string path)
        {
            this.importedDocuments.Remove(path);
        }

        public PdfDocument[] Documents
        {
            get
            {
                List<PdfDocument> list = new List<PdfDocument>();
                foreach (PdfDocument.DocumentHandle handle in this.importedDocuments.Values)
                {
                    if (handle.IsAlive)
                    {
                        list.Add(handle.Target);
                    }
                }
                return list.ToArray();
            }
        }
    }
}

