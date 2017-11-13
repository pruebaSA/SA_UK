namespace PdfSharp.Pdf.Advanced
{
    using PdfSharp.Pdf;
    using PdfSharp.Pdf.IO;
    using System;
    using System.IO;
    using System.Reflection;
    using System.Text;

    public class PdfInternals
    {
        public string CustomValueKey = "/PdfSharp.CustomValue";
        private PdfDocument document;

        internal PdfInternals(PdfDocument document)
        {
            this.document = document;
        }

        public void AddObject(PdfObject obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }
            if (obj.Owner == null)
            {
                obj.Document = this.document;
            }
            else if (obj.Owner != this.document)
            {
                throw new InvalidOperationException("Object does not belong to this document.");
            }
            this.document.irefTable.Add(obj);
        }

        public T CreateIndirectObject<T>() where T: PdfObject
        {
            T local = default(T);
            ConstructorInfo info = typeof(T).GetConstructor(BindingFlags.ExactBinding | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(PdfDocument) }, null);
            if (info != null)
            {
                local = (T) info.Invoke(new object[] { this.document });
                this.AddObject(local);
            }
            return local;
        }

        public static int GenerationNumber(PdfObject obj) => 
            obj?.GenerationNumber;

        public PdfObject[] GetAllObjects()
        {
            PdfReference[] allReferences = this.document.irefTable.AllReferences;
            int length = allReferences.Length;
            PdfObject[] objArray = new PdfObject[length];
            for (int i = 0; i < length; i++)
            {
                objArray[i] = allReferences[i].Value;
            }
            return objArray;
        }

        public PdfObject[] GetClosure(PdfObject obj) => 
            this.GetClosure(obj, 0x7fffffff);

        public PdfObject[] GetClosure(PdfObject obj, int depth)
        {
            PdfReference[] referenceArray = this.document.irefTable.TransitiveClosure(obj, depth);
            int num = referenceArray.Length + 1;
            PdfObject[] objArray = new PdfObject[num];
            objArray[0] = obj;
            for (int i = 1; i < num; i++)
            {
                objArray[i] = referenceArray[i - 1].Value;
            }
            return objArray;
        }

        public PdfObject GetObject(PdfObjectID objectID) => 
            this.document.irefTable[objectID].Value;

        public static PdfObjectID GetObjectID(PdfObject obj) => 
            obj?.ObjectID;

        public static int GetObjectNumber(PdfObject obj) => 
            obj?.ObjectNumber;

        public static PdfReference GetReference(PdfObject obj) => 
            obj?.Reference;

        private Guid GuidFromString(string id)
        {
            if ((id == null) || (id.Length != 0x10))
            {
                return Guid.Empty;
            }
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < 0x10; i++)
            {
                builder.AppendFormat("{0:X2}", (byte) id[i]);
            }
            return new Guid(builder.ToString());
        }

        public void RemoveObject(PdfObject obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }
            if (obj.Reference == null)
            {
                throw new InvalidOperationException("Only indirect objects can be removed.");
            }
            if (obj.Owner != this.document)
            {
                throw new InvalidOperationException("Object does not belong to this document.");
            }
            this.document.irefTable.Remove(obj.Reference);
        }

        public void WriteObject(Stream stream, PdfItem item)
        {
            PdfWriter writer = new PdfWriter(stream, null) {
                Options = PdfWriterOptions.OmitStream
            };
            item.WriteObject(writer);
        }

        [Obsolete("Use GetAllObjects.")]
        public PdfObject[] AllObjects =>
            this.GetAllObjects();

        public PdfCatalog Catalog =>
            this.document.Catalog;

        public Guid FirstDocumentGuid =>
            this.GuidFromString(this.document.trailer.GetDocumentID(0));

        public string FirstDocumentID
        {
            get => 
                this.document.trailer.GetDocumentID(0);
            set
            {
                this.document.trailer.SetDocumentID(0, value);
            }
        }

        public Guid SecondDocumentGuid =>
            this.GuidFromString(this.document.trailer.GetDocumentID(0));

        public string SecondDocumentID
        {
            get => 
                this.document.trailer.GetDocumentID(1);
            set
            {
                this.document.trailer.SetDocumentID(1, value);
            }
        }
    }
}

