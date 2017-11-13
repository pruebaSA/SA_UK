namespace PdfSharp.Pdf
{
    using PdfSharp.Drawing;
    using PdfSharp.Pdf.Advanced;
    using PdfSharp.Pdf.Filters;
    using PdfSharp.Pdf.Internal;
    using PdfSharp.Pdf.IO;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Text;

    [DebuggerDisplay("(pairs={Elements.Count})")]
    public class PdfDictionary : PdfObject, IEnumerable
    {
        protected DictionaryElements elements;
        private PdfStream stream;

        public PdfDictionary()
        {
        }

        protected PdfDictionary(PdfDictionary dict) : base(dict)
        {
            if (dict.elements != null)
            {
                dict.elements.ChangeOwner(this);
            }
            if (dict.stream != null)
            {
                dict.stream.SetOwner(this);
            }
        }

        public PdfDictionary(PdfDocument document) : base(document)
        {
        }

        public PdfDictionary Clone() => 
            ((PdfDictionary) this.Copy());

        protected override object Copy()
        {
            PdfDictionary dict = (PdfDictionary) base.Copy();
            if (dict.elements != null)
            {
                dict.elements = dict.elements.Clone();
                dict.elements.ChangeOwner(dict);
                foreach (PdfName name in dict.elements.KeyNames)
                {
                    PdfObject obj2 = dict.elements[name] as PdfObject;
                    if (obj2 != null)
                    {
                        obj2 = obj2.Clone();
                        dict.elements[name] = obj2;
                    }
                }
            }
            if (dict.stream != null)
            {
                dict.stream = dict.stream.Clone();
                dict.stream.SetOwner(dict);
            }
            return dict;
        }

        public PdfStream CreateStream(byte[] value)
        {
            if (this.stream != null)
            {
                throw new InvalidOperationException("The dictionary already has a stream.");
            }
            this.stream = new PdfStream(value, this);
            this.Elements["/Length"] = new PdfInteger(this.stream.Length);
            return this.stream;
        }

        public IEnumerator GetEnumerator() => 
            this.Elements.GetEnumerator();

        public override string ToString()
        {
            PdfName[] keyNames = this.Elements.KeyNames;
            List<PdfName> list = new List<PdfName>(keyNames);
            list.Sort(PdfName.Comparer);
            list.CopyTo(keyNames, 0);
            StringBuilder builder = new StringBuilder();
            builder.Append("<< ");
            foreach (PdfName name in keyNames)
            {
                builder.Append(string.Concat(new object[] { name, " ", this.Elements[name], " " }));
            }
            builder.Append(">>");
            return builder.ToString();
        }

        internal virtual void WriteDictionaryElement(PdfWriter writer, PdfName key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            PdfItem item = this.Elements[key];
            key.WriteObject(writer);
            item.WriteObject(writer);
            writer.NewLine();
        }

        internal virtual void WriteDictionaryStream(PdfWriter writer)
        {
            writer.WriteStream(this, (writer.Options & PdfWriterOptions.OmitStream) == PdfWriterOptions.OmitStream);
        }

        internal override void WriteObject(PdfWriter writer)
        {
            writer.WriteBeginObject(this);
            foreach (PdfName name in this.Elements.KeyNames)
            {
                this.WriteDictionaryElement(writer, name);
            }
            if (this.Stream != null)
            {
                this.WriteDictionaryStream(writer);
            }
            writer.WriteEndObject();
        }

        public DictionaryElements Elements
        {
            get
            {
                if (this.elements == null)
                {
                    this.elements = new DictionaryElements(this);
                }
                return this.elements;
            }
        }

        internal virtual DictionaryMeta Meta =>
            null;

        public PdfStream Stream
        {
            get => 
                this.stream;
            set
            {
                this.stream = value;
            }
        }

        public sealed class DictionaryElements : IDictionary<string, PdfItem>, ICollection<KeyValuePair<string, PdfItem>>, IEnumerable<KeyValuePair<string, PdfItem>>, IEnumerable, ICloneable
        {
            private Dictionary<string, PdfItem> elements = new Dictionary<string, PdfItem>();
            private PdfDictionary owner;

            internal DictionaryElements(PdfDictionary dict)
            {
                this.owner = dict;
            }

            public void Add(KeyValuePair<string, PdfItem> item)
            {
                this.Add(item.Key, item.Value);
            }

            public void Add(string key, PdfItem value)
            {
                if (string.IsNullOrEmpty(key))
                {
                    throw new ArgumentNullException("key");
                }
                if (key[0] != '/')
                {
                    throw new ArgumentException("The key must start with a slash '/'.");
                }
                PdfObject obj2 = value as PdfObject;
                if ((obj2 != null) && obj2.IsIndirect)
                {
                    value = obj2.Reference;
                }
                this.elements.Add(key, value);
            }

            internal void ChangeOwner(PdfDictionary dict)
            {
                this.owner = dict;
                dict.elements = this;
            }

            public void Clear()
            {
                this.elements.Clear();
            }

            public PdfDictionary.DictionaryElements Clone() => 
                ((PdfDictionary.DictionaryElements) ((ICloneable) this).Clone());

            public bool Contains(KeyValuePair<string, PdfItem> item)
            {
                throw new NotImplementedException();
            }

            [Obsolete("Use ContainsKey.")]
            public bool Contains(string key) => 
                this.elements.ContainsKey(key);

            public bool ContainsKey(string key) => 
                this.elements.ContainsKey(key);

            public void CopyTo(KeyValuePair<string, PdfItem>[] array, int arrayIndex)
            {
                throw new NotImplementedException();
            }

            private PdfArray CreateArray(Type type, PdfArray oldArray)
            {
                if (oldArray == null)
                {
                    return (type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(PdfDocument) }, null).Invoke(new object[] { this.owner.Owner }) as PdfArray);
                }
                return (type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(PdfArray) }, null).Invoke(new object[] { oldArray }) as PdfArray);
            }

            private PdfDictionary CreateDictionary(Type type, PdfDictionary oldDictionary)
            {
                if (oldDictionary == null)
                {
                    return (type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(PdfDocument) }, null).Invoke(new object[] { this.owner.Owner }) as PdfDictionary);
                }
                return (type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(PdfDictionary) }, null).Invoke(new object[] { oldDictionary }) as PdfDictionary);
            }

            private PdfItem CreateValue(Type type, PdfDictionary oldValue)
            {
                PdfObject obj2 = type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(PdfDocument) }, null).Invoke(new object[] { this.owner.Owner }) as PdfObject;
                if (oldValue != null)
                {
                    obj2.Reference = oldValue.Reference;
                    obj2.Reference.Value = obj2;
                    if (obj2 is PdfDictionary)
                    {
                        PdfDictionary dictionary = (PdfDictionary) obj2;
                        dictionary.elements = oldValue.elements;
                    }
                }
                return obj2;
            }

            public PdfArray GetArray(string key) => 
                (this.GetObject(key) as PdfArray);

            public bool GetBoolean(string key) => 
                this.GetBoolean(key, false);

            public bool GetBoolean(string key, bool create)
            {
                object obj2 = this[key];
                if (obj2 == null)
                {
                    if (create)
                    {
                        this[key] = new PdfBoolean();
                    }
                    return false;
                }
                if (obj2 is PdfReference)
                {
                    obj2 = ((PdfReference) obj2).Value;
                }
                if (obj2 is PdfBoolean)
                {
                    return ((PdfBoolean) obj2).Value;
                }
                if (!(obj2 is PdfBooleanObject))
                {
                    throw new InvalidCastException("GetBoolean: Object is not a boolean.");
                }
                return ((PdfBooleanObject) obj2).Value;
            }

            public DateTime GetDateTime(string key, DateTime defaultValue)
            {
                object obj2 = this[key];
                if (obj2 != null)
                {
                    string str;
                    if (obj2 is PdfReference)
                    {
                        obj2 = ((PdfReference) obj2).Value;
                    }
                    if (obj2 is PdfDate)
                    {
                        return ((PdfDate) obj2).Value;
                    }
                    if (obj2 is PdfString)
                    {
                        str = ((PdfString) obj2).Value;
                    }
                    else
                    {
                        if (!(obj2 is PdfStringObject))
                        {
                            throw new InvalidCastException("GetName: Object is not a name.");
                        }
                        str = ((PdfStringObject) obj2).Value;
                    }
                    if (str == "")
                    {
                        return defaultValue;
                    }
                    try
                    {
                        defaultValue = Parser.ParseDateTime(str, defaultValue);
                    }
                    catch
                    {
                    }
                }
                return defaultValue;
            }

            public PdfDictionary GetDictionary(string key) => 
                (this.GetObject(key) as PdfDictionary);

            public IEnumerator<KeyValuePair<string, PdfItem>> GetEnumerator() => 
                this.elements.GetEnumerator();

            internal int GetEnumFromName(string key, object defaultValue) => 
                this.GetEnumFromName(key, defaultValue, false);

            internal int GetEnumFromName(string key, object defaultValue, bool create)
            {
                if (!(defaultValue is Enum))
                {
                    throw new ArgumentException("defaultValue");
                }
                object obj2 = this[key];
                if (obj2 != null)
                {
                    return (int) Enum.Parse(defaultValue.GetType(), obj2.ToString().Substring(1), false);
                }
                if (create)
                {
                    this[key] = new PdfName(defaultValue.ToString());
                }
                return (int) defaultValue;
            }

            [Obsolete("Use GetObject, GetDictionary, GetArray, or GetReference")]
            public PdfObject GetIndirectObject(string key)
            {
                PdfItem item = this[key];
                if (item is PdfReference)
                {
                    return ((PdfReference) item).Value;
                }
                return null;
            }

            public int GetInteger(string key) => 
                this.GetInteger(key, false);

            public int GetInteger(string key, bool create)
            {
                object obj2 = this[key];
                if (obj2 == null)
                {
                    if (create)
                    {
                        this[key] = new PdfInteger();
                    }
                    return 0;
                }
                if (obj2 is PdfReference)
                {
                    obj2 = ((PdfReference) obj2).Value;
                }
                if (obj2 is PdfInteger)
                {
                    return ((PdfInteger) obj2).Value;
                }
                if (!(obj2 is PdfIntegerObject))
                {
                    throw new InvalidCastException("GetInteger: Object is not an integer.");
                }
                return ((PdfIntegerObject) obj2).Value;
            }

            public XMatrix GetMatrix(string key) => 
                this.GetMatrix(key, false);

            public XMatrix GetMatrix(string key, bool create)
            {
                XMatrix matrix = new XMatrix();
                object obj2 = this[key];
                if (obj2 == null)
                {
                    if (create)
                    {
                        this[key] = new PdfLiteral("[1 0 0 1 0 0]");
                    }
                    return matrix;
                }
                if (obj2 is PdfReference)
                {
                    obj2 = ((PdfReference) obj2).Value;
                }
                PdfArray array = obj2 as PdfArray;
                if ((array != null) && (array.Elements.Count == 6))
                {
                    return new XMatrix(array.Elements.GetReal(0), array.Elements.GetReal(1), array.Elements.GetReal(2), array.Elements.GetReal(3), array.Elements.GetReal(4), array.Elements.GetReal(5));
                }
                if (obj2 is PdfLiteral)
                {
                    throw new NotImplementedException("Parsing matrix from literal.");
                }
                throw new InvalidCastException("Element is not an array with 6 values.");
            }

            public string GetName(string key)
            {
                object obj2 = this[key];
                if (obj2 == null)
                {
                    return string.Empty;
                }
                if (obj2 is PdfReference)
                {
                    obj2 = ((PdfReference) obj2).Value;
                }
                if (obj2 is PdfName)
                {
                    return ((PdfName) obj2).Value;
                }
                if (!(obj2 is PdfNameObject))
                {
                    throw new InvalidCastException("GetName: Object is not a name.");
                }
                return ((PdfNameObject) obj2).Value;
            }

            public PdfObject GetObject(string key)
            {
                PdfItem item = this[key];
                if (item is PdfReference)
                {
                    return ((PdfReference) item).Value;
                }
                return (item as PdfObject);
            }

            public double GetReal(string key) => 
                this.GetReal(key, false);

            public double GetReal(string key, bool create)
            {
                object obj2 = this[key];
                if (obj2 == null)
                {
                    if (create)
                    {
                        this[key] = new PdfReal();
                    }
                    return 0.0;
                }
                if (obj2 is PdfReference)
                {
                    obj2 = ((PdfReference) obj2).Value;
                }
                if (obj2 is PdfReal)
                {
                    return ((PdfReal) obj2).Value;
                }
                if (obj2 is PdfRealObject)
                {
                    return ((PdfRealObject) obj2).Value;
                }
                if (obj2 is PdfInteger)
                {
                    return (double) ((PdfInteger) obj2).Value;
                }
                if (!(obj2 is PdfIntegerObject))
                {
                    throw new InvalidCastException("GetReal: Object is not a number.");
                }
                return (double) ((PdfIntegerObject) obj2).Value;
            }

            public PdfRectangle GetRectangle(string key) => 
                this.GetRectangle(key, false);

            public PdfRectangle GetRectangle(string key, bool create)
            {
                PdfRectangle rectangle = new PdfRectangle();
                object obj2 = this[key];
                if (obj2 == null)
                {
                    if (create)
                    {
                        this[key] = rectangle = new PdfRectangle();
                    }
                    return rectangle;
                }
                if (obj2 is PdfReference)
                {
                    obj2 = ((PdfReference) obj2).Value;
                }
                PdfArray array = obj2 as PdfArray;
                if ((array != null) && (array.Elements.Count == 4))
                {
                    rectangle = new PdfRectangle(array.Elements.GetReal(0), array.Elements.GetReal(1), array.Elements.GetReal(2), array.Elements.GetReal(3));
                    this[key] = rectangle;
                    return rectangle;
                }
                return (PdfRectangle) obj2;
            }

            public PdfReference GetReference(string key)
            {
                PdfItem item = this[key];
                return (item as PdfReference);
            }

            public string GetString(string key) => 
                this.GetString(key, false);

            public string GetString(string key, bool create)
            {
                object obj2 = this[key];
                if (obj2 == null)
                {
                    if (create)
                    {
                        this[key] = new PdfString();
                    }
                    return "";
                }
                if (obj2 is PdfReference)
                {
                    obj2 = ((PdfReference) obj2).Value;
                }
                if (obj2 is PdfString)
                {
                    return ((PdfString) obj2).Value;
                }
                if (obj2 is PdfStringObject)
                {
                    return ((PdfStringObject) obj2).Value;
                }
                if (obj2 is PdfName)
                {
                    return ((PdfName) obj2).Value;
                }
                if (!(obj2 is PdfNameObject))
                {
                    throw new InvalidCastException("GetString: Object is not a string.");
                }
                return ((PdfNameObject) obj2).Value;
            }

            public PdfItem GetValue(string key) => 
                this.GetValue(key, VCF.None);

            public PdfItem GetValue(string key, VCF options)
            {
                PdfItem item = this[key];
                if (item == null)
                {
                    if (options != VCF.None)
                    {
                        PdfObject obj2;
                        Type c = this.GetValueType(key);
                        if (c == null)
                        {
                            throw new NotImplementedException("Cannot create value for key: " + key);
                        }
                        if (typeof(PdfDictionary).IsAssignableFrom(c))
                        {
                            item = obj2 = this.CreateDictionary(c, null);
                        }
                        else
                        {
                            if (!typeof(PdfArray).IsAssignableFrom(c))
                            {
                                throw new NotImplementedException("Type other than array or dictionary.");
                            }
                            item = obj2 = this.CreateArray(c, null);
                        }
                        if (options == VCF.CreateIndirect)
                        {
                            this.owner.Owner.irefTable.Add(obj2);
                            this[key] = obj2.Reference;
                            return item;
                        }
                        this[key] = obj2;
                    }
                    return item;
                }
                PdfReference reference = item as PdfReference;
                if (reference != null)
                {
                    item = reference.Value;
                    if (item == null)
                    {
                        throw new InvalidOperationException("Indirect reference without value.");
                    }
                    Type type2 = this.GetValueType(key);
                    if ((type2 == null) || (type2 == item.GetType()))
                    {
                        return item;
                    }
                    if (typeof(PdfDictionary).IsAssignableFrom(type2))
                    {
                        return this.CreateDictionary(type2, (PdfDictionary) item);
                    }
                    if (!typeof(PdfArray).IsAssignableFrom(type2))
                    {
                        throw new NotImplementedException("Type other than array or dictionary.");
                    }
                    return this.CreateArray(type2, (PdfArray) item);
                }
                PdfDictionary oldDictionary = item as PdfDictionary;
                if (oldDictionary != null)
                {
                    Type type = this.GetValueType(key);
                    if (oldDictionary.GetType() != type)
                    {
                        oldDictionary = this.CreateDictionary(type, oldDictionary);
                    }
                    return oldDictionary;
                }
                PdfArray oldArray = item as PdfArray;
                if (oldArray == null)
                {
                    return item;
                }
                Type valueType = this.GetValueType(key);
                if (oldArray.GetType() != valueType)
                {
                    oldArray = this.CreateArray(valueType, oldArray);
                }
                return oldArray;
            }

            private Type GetValueType(string key)
            {
                Type valueType = null;
                DictionaryMeta meta = this.owner.Meta;
                if (meta != null)
                {
                    KeyDescriptor descriptor = meta[key];
                    if (descriptor != null)
                    {
                        valueType = descriptor.GetValueType();
                    }
                }
                return valueType;
            }

            public bool Remove(KeyValuePair<string, PdfItem> item)
            {
                throw new NotImplementedException();
            }

            public bool Remove(string key) => 
                this.elements.Remove(key);

            public void SetBoolean(string key, bool value)
            {
                this[key] = new PdfBoolean(value);
            }

            public void SetDateTime(string key, DateTime value)
            {
                this.elements[key] = new PdfDate(value);
            }

            internal void SetEnumAsName(string key, object value)
            {
                if (!(value is Enum))
                {
                    throw new ArgumentException("value");
                }
                this.elements[key] = new PdfName("/" + value);
            }

            public void SetInteger(string key, int value)
            {
                this[key] = new PdfInteger(value);
            }

            public void SetMatrix(string key, XMatrix matrix)
            {
                this.elements[key] = PdfLiteral.FromMatrix(matrix);
            }

            public void SetName(string key, string value)
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                if ((value.Length == 0) || (value[0] != '/'))
                {
                    value = "/" + value;
                }
                this[key] = new PdfName(value);
            }

            public void SetObject(string key, PdfObject obj)
            {
                if (obj.Reference != null)
                {
                    throw new ArgumentException("PdfObject must not be an indirect object.", "obj");
                }
                this[key] = obj;
            }

            [Obsolete("Renamed to ChangeOwner for consistency.")]
            internal void SetOwner(PdfDictionary dict)
            {
                this.ChangeOwner(dict);
            }

            public void SetReal(string key, double value)
            {
                this[key] = new PdfReal(value);
            }

            public void SetRectangle(string key, PdfRectangle rect)
            {
                this.elements[key] = rect;
            }

            public void SetReference(string key, PdfObject obj)
            {
                if (obj.Reference == null)
                {
                    throw new ArgumentException("PdfObject must be an indirect object.", "obj");
                }
                this[key] = obj.Reference;
            }

            public void SetString(string key, string value)
            {
                this[key] = new PdfString(value);
            }

            public void SetValue(string key, PdfItem value)
            {
                this.elements[key] = value;
            }

            IEnumerator IEnumerable.GetEnumerator() => 
                this.elements.GetEnumerator();

            object ICloneable.Clone()
            {
                PdfDictionary.DictionaryElements elements = (PdfDictionary.DictionaryElements) base.MemberwiseClone();
                elements.elements = new Dictionary<string, PdfItem>(elements.elements);
                elements.owner = null;
                return elements;
            }

            public bool TryGetValue(string key, out PdfItem value) => 
                this.elements.TryGetValue(key, out value);

            public int Count =>
                this.elements.Count;

            public bool IsFixedSize =>
                false;

            public bool IsReadOnly =>
                false;

            public bool IsSynchronized =>
                false;

            public PdfItem this[string key]
            {
                get
                {
                    PdfItem item;
                    this.elements.TryGetValue(key, out item);
                    return item;
                }
                set
                {
                    if (value == null)
                    {
                        throw new ArgumentNullException("value");
                    }
                    PdfObject obj2 = value as PdfObject;
                    if ((obj2 != null) && obj2.IsIndirect)
                    {
                        value = obj2.Reference;
                    }
                    this.elements[key] = value;
                }
            }

            public PdfItem this[PdfName key]
            {
                get => 
                    this[key.Value];
                set
                {
                    if (value == null)
                    {
                        throw new ArgumentNullException("value");
                    }
                    PdfObject obj2 = value as PdfObject;
                    if ((obj2 != null) && obj2.IsIndirect)
                    {
                        value = obj2.Reference;
                    }
                    this.elements[key.Value] = value;
                }
            }

            public PdfName[] KeyNames
            {
                get
                {
                    ICollection keys = this.elements.Keys;
                    int count = keys.Count;
                    string[] array = new string[count];
                    keys.CopyTo(array, 0);
                    PdfName[] nameArray = new PdfName[count];
                    for (int i = 0; i < count; i++)
                    {
                        nameArray[i] = new PdfName(array[i]);
                    }
                    return nameArray;
                }
            }

            public ICollection<string> Keys
            {
                get
                {
                    ICollection keys = this.elements.Keys;
                    string[] array = new string[keys.Count];
                    keys.CopyTo(array, 0);
                    return array;
                }
            }

            internal PdfDictionary Owner =>
                this.owner;

            public object SyncRoot =>
                null;

            public ICollection<PdfItem> Values
            {
                get
                {
                    ICollection values = this.elements.Values;
                    PdfItem[] array = new PdfItem[values.Count];
                    values.CopyTo(array, 0);
                    return array;
                }
            }
        }

        public sealed class PdfStream
        {
            private PdfDictionary owner;
            private byte[] value;

            internal PdfStream(PdfDictionary owner)
            {
                if (owner == null)
                {
                    throw new ArgumentNullException("owner");
                }
                this.owner = owner;
            }

            internal PdfStream(byte[] value, PdfDictionary owner) : this(owner)
            {
                this.value = value;
            }

            public PdfDictionary.PdfStream Clone()
            {
                PdfDictionary.PdfStream stream = (PdfDictionary.PdfStream) base.MemberwiseClone();
                stream.owner = null;
                if (stream.value != null)
                {
                    stream.value = new byte[stream.value.Length];
                    this.value.CopyTo(stream.value, 0);
                }
                return stream;
            }

            public static byte[] RawEncode(string content) => 
                PdfEncoders.RawEncoding.GetBytes(content);

            internal void SetOwner(PdfDictionary dict)
            {
                this.owner = dict;
                this.owner.stream = this;
            }

            public override string ToString()
            {
                if (this.value == null)
                {
                    return "\x00abnull\x00bb";
                }
                PdfItem filterItem = this.owner.Elements["/Filter"];
                if (filterItem != null)
                {
                    byte[] bytes = Filtering.Decode(this.value, filterItem);
                    if (bytes == null)
                    {
                        throw new NotImplementedException("Unknown filter");
                    }
                    return PdfEncoders.RawEncoding.GetString(bytes, 0, bytes.Length);
                }
                return PdfEncoders.RawEncoding.GetString(this.value, 0, this.value.Length);
            }

            public bool TryUnfilter()
            {
                if (this.value != null)
                {
                    PdfItem filterItem = this.owner.Elements["/Filter"];
                    if (filterItem != null)
                    {
                        byte[] buffer = Filtering.Decode(this.value, filterItem);
                        if (buffer != null)
                        {
                            this.owner.Elements.Remove("/Filter");
                            this.Value = buffer;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                return true;
            }

            public void Zip()
            {
                if ((this.value != null) && !this.owner.Elements.ContainsKey("/Filter"))
                {
                    this.value = Filtering.FlateDecode.Encode(this.value);
                    this.owner.Elements["/Filter"] = new PdfName("/FlateDecode");
                    this.owner.Elements["/Length"] = new PdfInteger(this.value.Length);
                }
            }

            public int Length =>
                this.value?.Length;

            public byte[] UnfilteredValue
            {
                get
                {
                    byte[] array = null;
                    if (this.value != null)
                    {
                        PdfItem filterItem = this.owner.Elements["/Filter"];
                        if (filterItem != null)
                        {
                            array = Filtering.Decode(this.value, filterItem);
                            if (array == null)
                            {
                                string s = $"«Cannot decode filter '{filterItem}'»";
                                array = PdfEncoders.RawEncoding.GetBytes(s);
                            }
                        }
                        else
                        {
                            array = new byte[this.value.Length];
                            this.value.CopyTo(array, 0);
                        }
                    }
                    return (array ?? new byte[0]);
                }
            }

            public byte[] Value
            {
                get => 
                    this.value;
                set
                {
                    if (value == null)
                    {
                        throw new ArgumentNullException("value");
                    }
                    this.value = value;
                    this.owner.Elements.SetInteger("/Length", value.Length);
                }
            }

            public class Keys : KeysBase
            {
                [KeyInfo(KeyType.Optional | KeyType.ArrayOrDictionary)]
                public const string DecodeParms = "/DecodeParms";
                [KeyInfo("1.5", KeyType.Optional | KeyType.Integer)]
                public const string DL = "/DL";
                [KeyInfo("1.2", KeyType.Optional | KeyType.String)]
                public const string F = "/F";
                [KeyInfo("1.2", KeyType.Optional | KeyType.ArrayOrDictionary)]
                public const string FDecodeParms = "/FDecodeParms";
                [KeyInfo("1.2", KeyType.Optional | KeyType.NameOrArray)]
                public const string FFilter = "/FFilter";
                [KeyInfo(KeyType.Optional | KeyType.NameOrArray)]
                public const string Filter = "/Filter";
                [KeyInfo(KeyType.Required | KeyType.Integer)]
                public const string Length = "/Length";
            }
        }
    }
}

