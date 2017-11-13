namespace MS.Internal.IO.Packaging.CompoundFile
{
    using System;
    using System.Collections.Specialized;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Windows;

    internal abstract class CompoundFileReference : IComparable
    {
        protected CompoundFileReference()
        {
        }

        public override bool Equals(object o) => 
            false;

        public override int GetHashCode() => 
            0;

        internal static CompoundFileReference Load(BinaryReader reader, out int bytesRead)
        {
            ContainerUtilities.CheckAgainstNull(reader, "reader");
            bytesRead = 0;
            int num = reader.ReadInt32();
            bytesRead += ContainerUtilities.Int32Size;
            if (num < 0)
            {
                throw new FileFormatException(System.Windows.SR.Get("CFRCorrupt"));
            }
            StringCollection arrayPath = null;
            string streamName = null;
            while (num > 0)
            {
                int num2;
                RefComponentType type = (RefComponentType) reader.ReadInt32();
                bytesRead += ContainerUtilities.Int32Size;
                switch (type)
                {
                    case RefComponentType.Stream:
                        if (streamName != null)
                        {
                            throw new FileFormatException(System.Windows.SR.Get("CFRCorruptMultiStream"));
                        }
                        goto Label_00AD;

                    case RefComponentType.Storage:
                        if (streamName != null)
                        {
                            throw new FileFormatException(System.Windows.SR.Get("CFRCorruptStgFollowStm"));
                        }
                        break;

                    default:
                        throw new FileFormatException(System.Windows.SR.Get("UnknownReferenceComponentType"));
                }
                if (arrayPath == null)
                {
                    arrayPath = new StringCollection();
                }
                string str2 = ContainerUtilities.ReadByteLengthPrefixedDWordPaddedUnicodeString(reader, out num2);
                bytesRead += num2;
                arrayPath.Add(str2);
                goto Label_00CE;
            Label_00AD:
                streamName = ContainerUtilities.ReadByteLengthPrefixedDWordPaddedUnicodeString(reader, out num2);
                bytesRead += num2;
            Label_00CE:
                num--;
            }
            if (streamName == null)
            {
                return new CompoundFileStorageReference(ContainerUtilities.ConvertStringArrayPathToBackSlashPath(arrayPath));
            }
            return new CompoundFileStreamReference(ContainerUtilities.ConvertStringArrayPathToBackSlashPath(arrayPath, streamName));
        }

        internal static int Save(CompoundFileReference reference, BinaryWriter writer)
        {
            int num = 0;
            bool flag = writer == null;
            CompoundFileStreamReference reference2 = reference as CompoundFileStreamReference;
            if ((reference2 == null) && !(reference is CompoundFileStorageReference))
            {
                throw new ArgumentException(System.Windows.SR.Get("UnknownReferenceSerialize"), "reference");
            }
            string[] strArray = ContainerUtilities.ConvertBackSlashPathToStringArrayPath(reference.FullName);
            int length = strArray.Length;
            if (!flag)
            {
                writer.Write(length);
            }
            num += ContainerUtilities.Int32Size;
            for (int i = 0; i < (strArray.Length - ((reference2 == null) ? 0 : 1)); i++)
            {
                if (!flag)
                {
                    writer.Write(1);
                }
                num += ContainerUtilities.Int32Size;
                num += ContainerUtilities.WriteByteLengthPrefixedDWordPaddedUnicodeString(writer, strArray[i]);
            }
            if (reference2 == null)
            {
                return num;
            }
            if (!flag)
            {
                writer.Write(0);
            }
            num += ContainerUtilities.Int32Size;
            return (num + ContainerUtilities.WriteByteLengthPrefixedDWordPaddedUnicodeString(writer, strArray[strArray.Length - 1]));
        }

        int IComparable.CompareTo(object ob) => 
            0;

        public abstract string FullName { get; }

        private enum RefComponentType
        {
            Stream,
            Storage
        }
    }
}

