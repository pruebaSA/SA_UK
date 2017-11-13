namespace PdfSharp.Pdf
{
    using PdfSharp.Pdf.Advanced;
    using PdfSharp.Pdf.IO;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;

    internal sealed class PdfReferenceTable
    {
        private PdfDictionary deadObject;
        private PdfDocument document;
        private bool isUnderConstruction;
        internal int maxObjectNumber;
        private static int nestingLevel;
        public Dictionary<PdfObjectID, PdfReference> objectTable = new Dictionary<PdfObjectID, PdfReference>();
        private Dictionary<PdfItem, object> overflow = new Dictionary<PdfItem, object>();

        public PdfReferenceTable(PdfDocument document)
        {
            this.document = document;
        }

        public void Add(PdfReference iref)
        {
            if (iref.ObjectID.IsEmpty)
            {
                iref.ObjectID = new PdfObjectID(this.GetNewObjectNumber());
            }
            if (this.objectTable.ContainsKey(iref.ObjectID))
            {
                throw new InvalidOperationException("Object already in table.");
            }
            this.objectTable.Add(iref.ObjectID, iref);
        }

        public void Add(PdfObject value)
        {
            if (value.Owner == null)
            {
                value.Document = this.document;
            }
            if (value.ObjectID.IsEmpty)
            {
                value.SetObjectID(this.GetNewObjectNumber(), 0);
            }
            if (this.objectTable.ContainsKey(value.ObjectID))
            {
                throw new InvalidOperationException("Object already in table.");
            }
            this.objectTable.Add(value.ObjectID, value.Reference);
        }

        [Conditional("DEBUG_")]
        public void CheckConsistence()
        {
            Dictionary<PdfReference, object> dictionary = new Dictionary<PdfReference, object>();
            foreach (PdfReference reference in this.objectTable.Values)
            {
                dictionary.Add(reference, null);
            }
            Dictionary<PdfObjectID, object> dictionary2 = new Dictionary<PdfObjectID, object>();
            foreach (PdfReference reference2 in this.objectTable.Values)
            {
                dictionary2.Add(reference2.ObjectID, null);
            }
            ICollection values = this.objectTable.Values;
            PdfReference[] array = new PdfReference[values.Count];
            values.CopyTo(array, 0);
        }

        internal int Compact()
        {
            int count = this.objectTable.Count;
            PdfReference[] referenceArray = this.TransitiveClosure(this.document.trailer);
            this.maxObjectNumber = 0;
            this.objectTable.Clear();
            foreach (PdfReference reference in referenceArray)
            {
                this.objectTable.Add(reference.ObjectID, reference);
                this.maxObjectNumber = Math.Max(this.maxObjectNumber, reference.ObjectNumber);
            }
            return (count - this.objectTable.Count);
        }

        public bool Contains(PdfObjectID objectID) => 
            this.objectTable.ContainsKey(objectID);

        public int GetNewObjectNumber() => 
            ++this.maxObjectNumber;

        internal void HandleOrphanedReferences()
        {
        }

        public void Remove(PdfReference iref)
        {
            this.objectTable.Remove(iref.ObjectID);
        }

        internal void Renumber()
        {
            PdfReference[] allReferences = this.AllReferences;
            this.objectTable.Clear();
            int length = allReferences.Length;
            for (int i = 0; i < length; i++)
            {
                PdfReference reference = allReferences[i];
                reference.ObjectID = new PdfObjectID(i + 1);
                this.objectTable.Add(reference.ObjectID, reference);
            }
            this.maxObjectNumber = length;
        }

        public PdfReference[] TransitiveClosure(PdfObject pdfObject) => 
            this.TransitiveClosure(pdfObject, 0x7fff);

        public PdfReference[] TransitiveClosure(PdfObject pdfObject, int depth)
        {
            Dictionary<PdfItem, object> objects = new Dictionary<PdfItem, object>();
            this.overflow = new Dictionary<PdfItem, object>();
            this.TransitiveClosureImplementation(objects, pdfObject, ref depth);
            while (true)
            {
                if (this.overflow.Count <= 0)
                {
                    break;
                }
                PdfObject[] objArray = new PdfObject[this.overflow.Count];
                this.overflow.Keys.CopyTo(objArray, 0);
                this.overflow = new Dictionary<PdfItem, object>();
                for (int i = 0; i < objArray.Length; i++)
                {
                    PdfObject obj2 = objArray[i];
                    this.TransitiveClosureImplementation(objects, obj2, ref depth);
                }
            }
            ICollection keys = objects.Keys;
            PdfReference[] array = new PdfReference[keys.Count];
            keys.CopyTo(array, 0);
            return array;
        }

        private void TransitiveClosureImplementation(Dictionary<PdfItem, object> objects, PdfObject pdfObject, ref int depth)
        {
            if (depth-- != 0)
            {
                try
                {
                    nestingLevel++;
                    if (nestingLevel >= 0x3e8)
                    {
                        if (!this.overflow.ContainsKey(pdfObject))
                        {
                            this.overflow.Add(pdfObject, null);
                        }
                    }
                    else
                    {
                        IEnumerable values = null;
                        if (pdfObject is PdfDictionary)
                        {
                            values = ((PdfDictionary) pdfObject).Elements.Values;
                        }
                        else if (pdfObject is PdfArray)
                        {
                            values = ((PdfArray) pdfObject).Elements;
                        }
                        if (values != null)
                        {
                            foreach (PdfItem item in values)
                            {
                                PdfReference key = item as PdfReference;
                                if (key != null)
                                {
                                    if (!object.ReferenceEquals(key.Document, this.document))
                                    {
                                        base.GetType();
                                    }
                                    if (!objects.ContainsKey(key))
                                    {
                                        PdfObject obj2 = key.Value;
                                        if (key.Document != null)
                                        {
                                            if (obj2 == null)
                                            {
                                                key = this.objectTable[key.ObjectID];
                                                obj2 = key.Value;
                                            }
                                            objects.Add(key, null);
                                            if ((obj2 is PdfArray) || (obj2 is PdfDictionary))
                                            {
                                                this.TransitiveClosureImplementation(objects, obj2, ref depth);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    PdfObject obj3 = item as PdfObject;
                                    if ((obj3 != null) && ((obj3 is PdfDictionary) || (obj3 is PdfArray)))
                                    {
                                        this.TransitiveClosureImplementation(objects, obj3, ref depth);
                                    }
                                }
                            }
                        }
                    }
                }
                finally
                {
                    nestingLevel--;
                }
            }
        }

        internal void WriteObject(PdfWriter writer)
        {
            writer.WriteRaw("xref\n");
            PdfReference[] allReferences = this.AllReferences;
            int length = allReferences.Length;
            writer.WriteRaw($"0 {length + 1}
");
            writer.WriteRaw($"{0:0000000000} {0xffff:00000} {"f"} 
");
            for (int i = 0; i < length; i++)
            {
                PdfReference reference = allReferences[i];
                writer.WriteRaw($"{reference.Position:0000000000} {reference.GenerationNumber:00000} {"n"} 
");
            }
        }

        internal PdfObjectID[] AllObjectIDs
        {
            get
            {
                ICollection keys = this.objectTable.Keys;
                PdfObjectID[] array = new PdfObjectID[keys.Count];
                keys.CopyTo(array, 0);
                return array;
            }
        }

        internal PdfReference[] AllReferences
        {
            get
            {
                Dictionary<PdfObjectID, PdfReference>.ValueCollection collection = this.objectTable.Values;
                List<PdfReference> list = new List<PdfReference>(collection);
                list.Sort(PdfReference.Comparer);
                PdfReference[] array = new PdfReference[collection.Count];
                list.CopyTo(array, 0);
                return array;
            }
        }

        public PdfReference DeadObject =>
            this.deadObject?.Reference;

        internal bool IsUnderConstruction
        {
            get => 
                this.isUnderConstruction;
            set
            {
                this.isUnderConstruction = value;
            }
        }

        public PdfReference this[PdfObjectID objectID]
        {
            get
            {
                PdfReference reference;
                this.objectTable.TryGetValue(objectID, out reference);
                return reference;
            }
        }
    }
}

