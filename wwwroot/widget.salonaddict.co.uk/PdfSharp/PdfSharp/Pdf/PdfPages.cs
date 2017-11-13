namespace PdfSharp.Pdf
{
    using PdfSharp;
    using PdfSharp.Pdf.Advanced;
    using PdfSharp.Pdf.Annotations;
    using PdfSharp.Pdf.IO;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;

    [DebuggerDisplay("(PageCount={Count})")]
    public sealed class PdfPages : PdfDictionary, IEnumerable
    {
        private PdfArray pagesArray;

        internal PdfPages(PdfDictionary dictionary) : base(dictionary)
        {
        }

        internal PdfPages(PdfDocument document) : base(document)
        {
            base.Elements.SetName("/Type", "/Pages");
            base.Elements["/Count"] = new PdfInteger(0);
        }

        public PdfPage Add()
        {
            PdfPage page = new PdfPage();
            this.Insert(this.Count, page);
            return page;
        }

        public PdfPage Add(PdfPage page) => 
            this.Insert(this.Count, page);

        private void CloneElement(PdfPage page, PdfPage importPage, string key, bool deepcopy)
        {
            PdfItem item = importPage.Elements[key];
            if (item != null)
            {
                PdfImportedObjectTable importedObjectTable = null;
                if (!deepcopy)
                {
                    importedObjectTable = this.Owner.FormTable.GetImportedObjectTable(importPage);
                }
                if (item is PdfReference)
                {
                    item = ((PdfReference) item).Value;
                }
                if (item is PdfObject)
                {
                    PdfObject externalObject = (PdfObject) item;
                    if (deepcopy)
                    {
                        externalObject = PdfObject.DeepCopyClosure(base.document, externalObject);
                    }
                    else
                    {
                        if (externalObject.Owner == null)
                        {
                            externalObject.Document = importPage.Owner;
                        }
                        externalObject = PdfObject.ImportClosure(importedObjectTable, page.Owner, externalObject);
                    }
                    if (externalObject.Reference == null)
                    {
                        page.Elements[key] = externalObject;
                    }
                    else
                    {
                        page.Elements[key] = externalObject.Reference;
                    }
                }
                else
                {
                    page.Elements[key] = item.Clone();
                }
            }
        }

        internal void FlattenPageTree()
        {
            PdfPage.InheritedValues values = new PdfPage.InheritedValues();
            PdfPage.InheritValues(this, ref values);
            PdfDictionary[] dictionaryArray = this.GetKids(base.Reference, values, null);
            PdfArray array = new PdfArray(this.Owner);
            foreach (PdfDictionary dictionary in dictionaryArray)
            {
                dictionary.Elements["/Parent"] = base.Reference;
                array.Elements.Add(dictionary.Reference);
            }
            base.Elements.SetName("/Type", "/Pages");
            base.Elements.SetValue("/Kids", array);
            base.Elements.SetInteger("/Count", array.Elements.Count);
        }

        public IEnumerator GetEnumerator() => 
            new PdfPagesEnumerator(this);

        private PdfDictionary[] GetKids(PdfReference iref, PdfPage.InheritedValues values, PdfDictionary parent)
        {
            PdfDictionary page = (PdfDictionary) iref.Value;
            if (page.Elements.GetName("/Type") == "/Page")
            {
                PdfPage.InheritValues(page, values);
                return new PdfDictionary[] { page };
            }
            PdfPage.InheritValues(page, ref values);
            List<PdfDictionary> list = new List<PdfDictionary>();
            PdfArray array = page.Elements["/Kids"] as PdfArray;
            if (array == null)
            {
                PdfReference reference = page.Elements["/Kids"] as PdfReference;
                array = reference.Value as PdfArray;
            }
            foreach (PdfReference reference2 in array)
            {
                list.AddRange(this.GetKids(reference2, values, page));
            }
            int count = list.Count;
            return list.ToArray();
        }

        private PdfPage ImportExternalPage(PdfPage importPage)
        {
            if (importPage.Owner.openMode != PdfDocumentOpenMode.Import)
            {
                throw new InvalidOperationException("A PDF document must be opened with PdfDocumentOpenMode.Import to import pages from it.");
            }
            PdfPage page = new PdfPage(base.document);
            this.CloneElement(page, importPage, "/Resources", false);
            this.CloneElement(page, importPage, "/Contents", false);
            this.CloneElement(page, importPage, "/MediaBox", true);
            this.CloneElement(page, importPage, "/CropBox", true);
            this.CloneElement(page, importPage, "/Rotate", true);
            this.CloneElement(page, importPage, "/BleedBox", true);
            this.CloneElement(page, importPage, "/TrimBox", true);
            this.CloneElement(page, importPage, "/ArtBox", true);
            this.CloneElement(page, importPage, "/Annots", false);
            return page;
        }

        public PdfPage Insert(int index)
        {
            PdfPage page = new PdfPage();
            this.Insert(index, page);
            return page;
        }

        public PdfPage Insert(int index, PdfPage page)
        {
            if (page == null)
            {
                throw new ArgumentNullException("page");
            }
            if (page.Owner == this.Owner)
            {
                int count = this.Count;
                for (int i = 0; i < count; i++)
                {
                    if (object.ReferenceEquals(this[i], page))
                    {
                        throw new InvalidOperationException(PSSR.MultiplePageInsert);
                    }
                }
                this.Owner.irefTable.Add(page);
                this.PagesArray.Elements.Insert(index, page.Reference);
                base.Elements.SetInteger("/Count", this.PagesArray.Elements.Count);
                return page;
            }
            if (page.Owner == null)
            {
                page.Document = this.Owner;
                this.Owner.irefTable.Add(page);
                this.PagesArray.Elements.Insert(index, page.Reference);
                base.Elements.SetInteger("/Count", this.PagesArray.Elements.Count);
            }
            else
            {
                page = this.ImportExternalPage(page);
                this.Owner.irefTable.Add(page);
                this.PagesArray.Elements.Insert(index, page.Reference);
                base.Elements.SetInteger("/Count", this.PagesArray.Elements.Count);
                PdfAnnotations.FixImportedAnnotation(page);
            }
            if (this.Owner.Settings.TrimMargins.AreSet)
            {
                page.TrimMargins = this.Owner.Settings.TrimMargins;
            }
            return page;
        }

        public void MovePage(int oldIndex, int newIndex)
        {
            if ((oldIndex < 0) || (oldIndex >= this.Count))
            {
                throw new ArgumentOutOfRangeException("oldIndex");
            }
            if ((newIndex < 0) || (newIndex >= this.Count))
            {
                throw new ArgumentOutOfRangeException("newIndex");
            }
            if (oldIndex != newIndex)
            {
                PdfReference reference = (PdfReference) this.pagesArray.Elements[oldIndex];
                this.pagesArray.Elements.RemoveAt(oldIndex);
                this.pagesArray.Elements.Insert(newIndex, reference);
            }
        }

        internal override void PrepareForSave()
        {
            int count = this.pagesArray.Elements.Count;
            for (int i = 0; i < count; i++)
            {
                this[i].PrepareForSave();
            }
        }

        public void Remove(PdfPage page)
        {
            this.PagesArray.Elements.Remove(page.Reference);
            base.Elements.SetInteger("/Count", this.PagesArray.Elements.Count);
        }

        public void RemoveAt(int index)
        {
            this.PagesArray.Elements.RemoveAt(index);
            base.Elements.SetInteger("/Count", this.PagesArray.Elements.Count);
        }

        public int Count =>
            this.PagesArray.Elements.Count;

        public PdfPage this[int index]
        {
            get
            {
                if ((index < 0) || (index >= this.Count))
                {
                    throw new ArgumentOutOfRangeException("index", index, PSSR.PageIndexOutOfRange);
                }
                PdfDictionary dict = (PdfDictionary) ((PdfReference) this.PagesArray.Elements[index]).Value;
                if (!(dict is PdfPage))
                {
                    dict = new PdfPage(dict);
                }
                return (PdfPage) dict;
            }
        }

        internal override DictionaryMeta Meta =>
            Keys.Meta;

        public PdfArray PagesArray
        {
            get
            {
                if (this.pagesArray == null)
                {
                    this.pagesArray = (PdfArray) base.Elements.GetValue("/Kids", VCF.Create);
                }
                return this.pagesArray;
            }
        }

        internal sealed class Keys : PdfPage.InheritablePageKeys
        {
            [KeyInfo(KeyType.Required | KeyType.Integer)]
            public const string Count = "/Count";
            [KeyInfo(KeyType.Required | KeyType.Array)]
            public const string Kids = "/Kids";
            private static DictionaryMeta meta;
            [KeyInfo(KeyType.Required | KeyType.Dictionary)]
            public const string Parent = "/Parent";
            [KeyInfo(KeyType.Required | KeyType.Name, FixedValue="Pages")]
            public const string Type = "/Type";

            public static DictionaryMeta Meta
            {
                get
                {
                    if (meta == null)
                    {
                        meta = KeysBase.CreateMeta(typeof(PdfPages.Keys));
                    }
                    return meta;
                }
            }
        }

        private class PdfPagesEnumerator : IEnumerator
        {
            private PdfPage currentElement;
            private int index;
            private PdfPages list;

            internal PdfPagesEnumerator(PdfPages list)
            {
                this.list = list;
                this.index = -1;
            }

            public bool MoveNext()
            {
                if (this.index < (this.list.Count - 1))
                {
                    this.index++;
                    this.currentElement = this.list[this.index];
                    return true;
                }
                this.index = this.list.Count;
                return false;
            }

            public void Reset()
            {
                this.currentElement = null;
                this.index = -1;
            }

            public PdfPage Current
            {
                get
                {
                    if ((this.index == -1) || (this.index >= this.list.Count))
                    {
                        throw new InvalidOperationException(PSSR.ListEnumCurrentOutOfRange);
                    }
                    return this.currentElement;
                }
            }

            object IEnumerator.Current =>
                this.Current;
        }
    }
}

