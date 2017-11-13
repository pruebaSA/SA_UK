namespace PdfSharp.Pdf.Internal
{
    using PdfSharp.Pdf;
    using System;
    using System.Collections.Generic;

    internal class GlobalObjectTable
    {
        private List<object> documentHandles = new List<object>();

        public void AttatchDocument(PdfDocument.DocumentHandle handle)
        {
            lock (this.documentHandles)
            {
                this.documentHandles.Add(handle);
            }
        }

        public void DetatchDocument(PdfDocument.DocumentHandle handle)
        {
            lock (this.documentHandles)
            {
                int count = this.documentHandles.Count;
                for (int i = 0; i < count; i++)
                {
                    if (((PdfDocument.DocumentHandle) this.documentHandles[i]).IsAlive)
                    {
                        PdfDocument target = ((PdfDocument.DocumentHandle) this.documentHandles[i]).Target;
                        if (target != null)
                        {
                            target.OnExternalDocumentFinalized(handle);
                        }
                    }
                }
                for (int j = 0; j < this.documentHandles.Count; j++)
                {
                    if (((PdfDocument.DocumentHandle) this.documentHandles[j]).Target == null)
                    {
                        this.documentHandles.RemoveAt(j);
                        j--;
                    }
                }
            }
        }
    }
}

