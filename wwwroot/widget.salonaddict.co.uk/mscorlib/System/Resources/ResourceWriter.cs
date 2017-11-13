namespace System.Resources
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Text;

    [ComVisible(true)]
    public sealed class ResourceWriter : IResourceWriter, IDisposable
    {
        private Hashtable _caseInsensitiveDups;
        private const int _ExpectedNumberOfResources = 0x3e8;
        private Stream _output;
        private Hashtable _preserializedData;
        private Hashtable _resourceList;
        private const int AverageNameSize = 40;
        private const int AverageValueSize = 40;

        public ResourceWriter(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            if (!stream.CanWrite)
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_StreamNotWritable"));
            }
            this._output = stream;
            this._resourceList = new Hashtable(0x3e8, FastResourceComparer.Default);
            this._caseInsensitiveDups = new Hashtable(StringComparer.OrdinalIgnoreCase);
        }

        public ResourceWriter(string fileName)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }
            this._output = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None);
            this._resourceList = new Hashtable(0x3e8, FastResourceComparer.Default);
            this._caseInsensitiveDups = new Hashtable(StringComparer.OrdinalIgnoreCase);
        }

        public void AddResource(string name, object value)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (this._resourceList == null)
            {
                throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_ResourceWriterSaved"));
            }
            this._caseInsensitiveDups.Add(name, null);
            this._resourceList.Add(name, value);
        }

        public void AddResource(string name, string value)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (this._resourceList == null)
            {
                throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_ResourceWriterSaved"));
            }
            this._caseInsensitiveDups.Add(name, null);
            this._resourceList.Add(name, value);
        }

        public void AddResource(string name, byte[] value)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (this._resourceList == null)
            {
                throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_ResourceWriterSaved"));
            }
            this._caseInsensitiveDups.Add(name, null);
            this._resourceList.Add(name, value);
        }

        public void AddResourceData(string name, string typeName, byte[] serializedData)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (typeName == null)
            {
                throw new ArgumentNullException("typeName");
            }
            if (serializedData == null)
            {
                throw new ArgumentNullException("serializedData");
            }
            if (this._resourceList == null)
            {
                throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_ResourceWriterSaved"));
            }
            this._caseInsensitiveDups.Add(name, null);
            if (this._preserializedData == null)
            {
                this._preserializedData = new Hashtable(FastResourceComparer.Default);
            }
            this._preserializedData.Add(name, new PrecannedResource(typeName, serializedData));
        }

        public void Close()
        {
            this.Dispose(true);
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this._resourceList != null)
                {
                    this.Generate();
                }
                if (this._output != null)
                {
                    this._output.Close();
                }
            }
            this._output = null;
            this._caseInsensitiveDups = null;
        }

        private ResourceTypeCode FindTypeCode(object value, List<string> types)
        {
            string typeName;
            if (value == null)
            {
                return ResourceTypeCode.Null;
            }
            Type type = value.GetType();
            if (type == typeof(string))
            {
                return ResourceTypeCode.String;
            }
            if (type == typeof(int))
            {
                return ResourceTypeCode.Int32;
            }
            if (type == typeof(bool))
            {
                return ResourceTypeCode.Boolean;
            }
            if (type == typeof(char))
            {
                return ResourceTypeCode.Char;
            }
            if (type == typeof(byte))
            {
                return ResourceTypeCode.Byte;
            }
            if (type == typeof(sbyte))
            {
                return ResourceTypeCode.SByte;
            }
            if (type == typeof(short))
            {
                return ResourceTypeCode.Int16;
            }
            if (type == typeof(long))
            {
                return ResourceTypeCode.Int64;
            }
            if (type == typeof(ushort))
            {
                return ResourceTypeCode.UInt16;
            }
            if (type == typeof(uint))
            {
                return ResourceTypeCode.UInt32;
            }
            if (type == typeof(ulong))
            {
                return ResourceTypeCode.UInt64;
            }
            if (type == typeof(float))
            {
                return ResourceTypeCode.Single;
            }
            if (type == typeof(double))
            {
                return ResourceTypeCode.Double;
            }
            if (type == typeof(decimal))
            {
                return ResourceTypeCode.Decimal;
            }
            if (type == typeof(DateTime))
            {
                return ResourceTypeCode.DateTime;
            }
            if (type == typeof(TimeSpan))
            {
                return ResourceTypeCode.TimeSpan;
            }
            if (type == typeof(byte[]))
            {
                return ResourceTypeCode.ByteArray;
            }
            if (type == typeof(MemoryStream))
            {
                return ResourceTypeCode.Stream;
            }
            if (type == typeof(PrecannedResource))
            {
                typeName = ((PrecannedResource) value).TypeName;
                if (typeName.StartsWith("ResourceTypeCode.", StringComparison.Ordinal))
                {
                    typeName = typeName.Substring(0x11);
                    return (ResourceTypeCode) Enum.Parse(typeof(ResourceTypeCode), typeName);
                }
            }
            else
            {
                typeName = type.AssemblyQualifiedName;
            }
            int index = types.IndexOf(typeName);
            if (index == -1)
            {
                index = types.Count;
                types.Add(typeName);
            }
            return (ResourceTypeCode) (index + 0x40);
        }

        public void Generate()
        {
            if (this._resourceList == null)
            {
                throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_ResourceWriterSaved"));
            }
            BinaryWriter writer = new BinaryWriter(this._output, Encoding.UTF8);
            List<string> types = new List<string>();
            writer.Write(ResourceManager.MagicNumber);
            writer.Write(ResourceManager.HeaderVersionNumber);
            MemoryStream output = new MemoryStream(240);
            BinaryWriter writer2 = new BinaryWriter(output);
            writer2.Write(typeof(ResourceReader).AssemblyQualifiedName);
            writer2.Write(ResourceManager.ResSetTypeName);
            writer2.Flush();
            writer.Write((int) output.Length);
            writer.Write(output.GetBuffer(), 0, (int) output.Length);
            writer.Write(2);
            int count = this._resourceList.Count;
            if (this._preserializedData != null)
            {
                count += this._preserializedData.Count;
            }
            writer.Write(count);
            int[] keys = new int[count];
            int[] items = new int[count];
            int index = 0;
            MemoryStream stream2 = new MemoryStream(count * 40);
            BinaryWriter writer3 = new BinaryWriter(stream2, Encoding.Unicode);
            MemoryStream stream3 = new MemoryStream(count * 40);
            BinaryWriter store = new BinaryWriter(stream3, Encoding.UTF8);
            IFormatter objFormatter = new BinaryFormatter(null, new StreamingContext(StreamingContextStates.Persistence | StreamingContextStates.File));
            SortedList list2 = new SortedList(this._resourceList, FastResourceComparer.Default);
            if (this._preserializedData != null)
            {
                foreach (DictionaryEntry entry in this._preserializedData)
                {
                    list2.Add(entry.Key, entry.Value);
                }
            }
            IDictionaryEnumerator enumerator = list2.GetEnumerator();
            while (enumerator.MoveNext())
            {
                keys[index] = FastResourceComparer.HashFunction((string) enumerator.Key);
                items[index++] = (int) writer3.Seek(0, SeekOrigin.Current);
                writer3.Write((string) enumerator.Key);
                writer3.Write((int) store.Seek(0, SeekOrigin.Current));
                object obj2 = enumerator.Value;
                ResourceTypeCode typeCode = this.FindTypeCode(obj2, types);
                Write7BitEncodedInt(store, (int) typeCode);
                PrecannedResource resource = obj2 as PrecannedResource;
                if (resource != null)
                {
                    store.Write(resource.Data);
                }
                else
                {
                    this.WriteValue(typeCode, obj2, store, objFormatter);
                }
            }
            writer.Write(types.Count);
            for (int i = 0; i < types.Count; i++)
            {
                writer.Write(types[i]);
            }
            Array.Sort<int, int>(keys, items);
            writer.Flush();
            int num4 = ((int) writer.BaseStream.Position) & 7;
            if (num4 > 0)
            {
                for (int j = 0; j < (8 - num4); j++)
                {
                    writer.Write("PAD"[j % 3]);
                }
            }
            foreach (int num6 in keys)
            {
                writer.Write(num6);
            }
            foreach (int num7 in items)
            {
                writer.Write(num7);
            }
            writer.Flush();
            writer3.Flush();
            store.Flush();
            int num8 = (int) (writer.Seek(0, SeekOrigin.Current) + stream2.Length);
            num8 += 4;
            writer.Write(num8);
            writer.Write(stream2.GetBuffer(), 0, (int) stream2.Length);
            writer3.Close();
            writer.Write(stream3.GetBuffer(), 0, (int) stream3.Length);
            store.Close();
            writer.Flush();
            this._resourceList = null;
        }

        private static void Write7BitEncodedInt(BinaryWriter store, int value)
        {
            uint num = (uint) value;
            while (num >= 0x80)
            {
                store.Write((byte) (num | 0x80));
                num = num >> 7;
            }
            store.Write((byte) num);
        }

        private void WriteValue(ResourceTypeCode typeCode, object value, BinaryWriter writer, IFormatter objFormatter)
        {
            switch (typeCode)
            {
                case ResourceTypeCode.Null:
                    return;

                case ResourceTypeCode.String:
                    writer.Write((string) value);
                    return;

                case ResourceTypeCode.Boolean:
                    writer.Write((bool) value);
                    return;

                case ResourceTypeCode.Char:
                    writer.Write((ushort) ((char) value));
                    return;

                case ResourceTypeCode.Byte:
                    writer.Write((byte) value);
                    return;

                case ResourceTypeCode.SByte:
                    writer.Write((sbyte) value);
                    return;

                case ResourceTypeCode.Int16:
                    writer.Write((short) value);
                    return;

                case ResourceTypeCode.UInt16:
                    writer.Write((ushort) value);
                    return;

                case ResourceTypeCode.Int32:
                    writer.Write((int) value);
                    return;

                case ResourceTypeCode.UInt32:
                    writer.Write((uint) value);
                    return;

                case ResourceTypeCode.Int64:
                    writer.Write((long) value);
                    return;

                case ResourceTypeCode.UInt64:
                    writer.Write((ulong) value);
                    return;

                case ResourceTypeCode.Single:
                    writer.Write((float) value);
                    return;

                case ResourceTypeCode.Double:
                    writer.Write((double) value);
                    return;

                case ResourceTypeCode.Decimal:
                    writer.Write((decimal) value);
                    return;

                case ResourceTypeCode.DateTime:
                {
                    long num = ((DateTime) value).ToBinary();
                    writer.Write(num);
                    return;
                }
                case ResourceTypeCode.TimeSpan:
                {
                    TimeSpan span = (TimeSpan) value;
                    writer.Write(span.Ticks);
                    return;
                }
                case ResourceTypeCode.ByteArray:
                {
                    byte[] buffer = (byte[]) value;
                    writer.Write(buffer.Length);
                    writer.Write(buffer, 0, buffer.Length);
                    return;
                }
                case ResourceTypeCode.Stream:
                {
                    int num2;
                    int num3;
                    MemoryStream stream = (MemoryStream) value;
                    if (stream.Length > 0x7fffffffL)
                    {
                        throw new ArgumentException(Environment.GetResourceString("ArgumentOutOfRange_MemStreamLength"));
                    }
                    stream.InternalGetOriginAndLength(out num2, out num3);
                    byte[] buffer2 = stream.InternalGetBuffer();
                    writer.Write(num3);
                    writer.Write(buffer2, num2, num3);
                    return;
                }
            }
            objFormatter.Serialize(writer.BaseStream, value);
        }

        private class PrecannedResource
        {
            internal byte[] Data;
            internal string TypeName;

            internal PrecannedResource(string typeName, byte[] data)
            {
                this.TypeName = typeName;
                this.Data = data;
            }
        }
    }
}

