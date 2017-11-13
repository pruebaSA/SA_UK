namespace PdfSharp.Pdf.Annotations
{
    using PdfSharp.Pdf;
    using PdfSharp.Pdf.Advanced;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;

    public sealed class PdfAnnotations : PdfArray
    {
        private PdfPage page;

        internal PdfAnnotations(PdfArray array) : base(array)
        {
        }

        internal PdfAnnotations(PdfDocument document) : base(document)
        {
        }

        public void Add(PdfAnnotation annotation)
        {
            annotation.Document = this.Owner;
            this.Owner.irefTable.Add(annotation);
            base.Elements.Add(annotation.Reference);
        }

        public void Clear()
        {
            for (int i = this.Count - 1; i >= 0; i--)
            {
                this.Page.Annotations.Remove(this.page.Annotations[i]);
            }
        }

        internal static void FixImportedAnnotation(PdfPage page)
        {
            PdfArray array = page.Elements.GetArray("/Annots");
            if (array != null)
            {
                int count = array.Elements.Count;
                for (int i = 0; i < count; i++)
                {
                    PdfDictionary dictionary = array.Elements.GetDictionary(i);
                    if ((dictionary != null) && dictionary.Elements.ContainsKey("/P"))
                    {
                        dictionary.Elements["/P"] = page.Reference;
                    }
                }
            }
        }

        public override IEnumerator<PdfItem> GetEnumerator() => 
            ((IEnumerator<PdfItem>) new AnnotationsIterator(this));

        public void Remove(PdfAnnotation annotation)
        {
            if (annotation.Owner != this.Owner)
            {
                throw new InvalidOperationException("The annotation does not belong to this document.");
            }
            this.Owner.Internals.RemoveObject(annotation);
            base.Elements.Remove(annotation.Reference);
        }

        public int Count =>
            base.Elements.Count;

        public PdfAnnotation this[int index]
        {
            get
            {
                PdfReference reference = null;
                PdfDictionary dict = null;
                PdfItem item = base.Elements[index];
                reference = item as PdfReference;
                if (reference != null)
                {
                    dict = (PdfDictionary) reference.Value;
                }
                else
                {
                    dict = (PdfDictionary) item;
                }
                PdfAnnotation annotation = dict as PdfAnnotation;
                if (annotation == null)
                {
                    annotation = new PdfGenericAnnotation(dict);
                    if (reference == null)
                    {
                        base.Elements[index] = annotation;
                    }
                }
                return annotation;
            }
        }

        internal PdfPage Page
        {
            get => 
                this.page;
            set
            {
                this.page = value;
            }
        }

        private class AnnotationsIterator : IEnumerator<PdfAnnotation>, IDisposable, IEnumerator
        {
            private PdfAnnotations annotations;
            private int index;

            public AnnotationsIterator(PdfAnnotations annotations)
            {
                this.annotations = annotations;
                this.index = -1;
            }

            public void Dispose()
            {
            }

            public bool MoveNext() => 
                (++this.index < this.annotations.Count);

            public void Reset()
            {
                this.index = -1;
            }

            public PdfAnnotation Current =>
                this.annotations[this.index];

            object IEnumerator.Current =>
                this.Current;
        }
    }
}

