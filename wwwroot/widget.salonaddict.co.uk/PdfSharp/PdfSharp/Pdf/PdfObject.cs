namespace PdfSharp.Pdf
{
    using PdfSharp.Pdf.Advanced;
    using PdfSharp.Pdf.IO;
    using System;

    public abstract class PdfObject : PdfItem
    {
        internal PdfDocument document;
        private PdfObjectInternals internals;
        internal PdfReference iref;

        protected PdfObject()
        {
        }

        protected PdfObject(PdfDocument document)
        {
            this.Document = document;
        }

        protected PdfObject(PdfObject obj)
        {
            this.Document = obj.Owner;
            if (obj.iref != null)
            {
                obj.iref.Value = this;
            }
        }

        public PdfObject Clone() => 
            ((PdfObject) this.Copy());

        protected override object Copy()
        {
            PdfObject obj2 = (PdfObject) base.Copy();
            obj2.document = null;
            obj2.iref = null;
            return obj2;
        }

        internal static PdfObject DeepCopyClosure(PdfDocument owner, PdfObject externalObject)
        {
            PdfObject[] closure = externalObject.Owner.Internals.GetClosure(externalObject);
            int length = closure.Length;
            PdfImportedObjectTable iot = new PdfImportedObjectTable(owner, externalObject.Owner);
            for (int i = 0; i < length; i++)
            {
                PdfObject obj2 = closure[i];
                PdfObject obj3 = obj2.Clone();
                obj3.Document = owner;
                if (obj2.Reference != null)
                {
                    owner.irefTable.Add(obj3);
                    iot.Add(obj2.ObjectID, obj3.Reference);
                }
                closure[i] = obj3;
            }
            for (int j = 0; j < length; j++)
            {
                PdfObject obj4 = closure[j];
                FixUpObject(iot, owner, obj4);
            }
            return closure[0];
        }

        internal static void FixUpObject(PdfImportedObjectTable iot, PdfDocument owner, PdfObject value)
        {
            PdfDictionary dictionary = value as PdfDictionary;
            if (dictionary != null)
            {
                if (dictionary.Owner == null)
                {
                    dictionary.Document = owner;
                }
                foreach (PdfName name in dictionary.Elements.KeyNames)
                {
                    PdfItem item = dictionary.Elements[name];
                    PdfReference reference = item as PdfReference;
                    if (reference != null)
                    {
                        if (reference.Document != owner)
                        {
                            PdfReference reference2 = iot[reference.ObjectID];
                            dictionary.Elements[name] = reference2;
                        }
                    }
                    else if (item is PdfObject)
                    {
                        FixUpObject(iot, owner, (PdfObject) item);
                    }
                }
            }
            else
            {
                PdfArray array = value as PdfArray;
                if (array != null)
                {
                    if (array.Owner == null)
                    {
                        array.Document = owner;
                    }
                    int count = array.Elements.Count;
                    for (int i = 0; i < count; i++)
                    {
                        PdfItem item2 = array.Elements[i];
                        PdfReference reference3 = item2 as PdfReference;
                        if (reference3 != null)
                        {
                            if (reference3.Document != owner)
                            {
                                PdfReference reference4 = iot[reference3.ObjectID];
                                array.Elements[i] = reference4;
                            }
                        }
                        else if (item2 is PdfObject)
                        {
                            FixUpObject(iot, owner, (PdfObject) item2);
                        }
                    }
                }
            }
        }

        internal static PdfObject ImportClosure(PdfImportedObjectTable importedObjectTable, PdfDocument owner, PdfObject externalObject)
        {
            PdfObject[] closure = externalObject.Owner.Internals.GetClosure(externalObject);
            int length = closure.Length;
            for (int i = 0; i < length; i++)
            {
                PdfObject obj2 = closure[i];
                if (importedObjectTable.Contains(obj2.ObjectID))
                {
                    PdfReference reference = importedObjectTable[obj2.ObjectID];
                    closure[i] = reference.Value;
                }
                else
                {
                    PdfObject obj3 = obj2.Clone();
                    obj3.Document = owner;
                    if (obj2.Reference != null)
                    {
                        owner.irefTable.Add(obj3);
                        importedObjectTable.Add(obj2.ObjectID, obj3.Reference);
                    }
                    closure[i] = obj3;
                }
            }
            for (int j = 0; j < length; j++)
            {
                PdfObject obj4 = closure[j];
                FixUpObject(importedObjectTable, importedObjectTable.Owner, obj4);
            }
            return closure[0];
        }

        internal virtual void PrepareForSave()
        {
        }

        internal void SetObjectID(int objectNumber, int generationNumber)
        {
            PdfObjectID tid = new PdfObjectID(objectNumber, generationNumber);
            if (this.iref == null)
            {
                this.iref = this.document.irefTable[tid];
            }
            if (this.iref == null)
            {
                this.iref = new PdfReference(this);
                this.iref.ObjectID = tid;
            }
            this.iref.Value = this;
            this.iref.Document = this.document;
        }

        internal override void WriteObject(PdfWriter writer)
        {
        }

        internal virtual PdfDocument Document
        {
            set
            {
                if (!object.ReferenceEquals(this.document, value))
                {
                    if (this.document != null)
                    {
                        throw new InvalidOperationException("Cannot change document.");
                    }
                    this.document = value;
                    if (this.iref != null)
                    {
                        this.iref.Document = value;
                    }
                }
            }
        }

        internal int GenerationNumber =>
            this.ObjectID.GenerationNumber;

        public PdfObjectInternals Internals
        {
            get
            {
                if (this.internals == null)
                {
                    this.internals = new PdfObjectInternals(this);
                }
                return this.internals;
            }
        }

        public bool IsIndirect =>
            (this.iref != null);

        internal PdfObjectID ObjectID =>
            this.iref?.ObjectID;

        internal int ObjectNumber =>
            this.ObjectID.ObjectNumber;

        public virtual PdfDocument Owner =>
            this.document;

        public PdfReference Reference
        {
            get => 
                this.iref;
            set
            {
                this.iref = value;
            }
        }
    }
}

