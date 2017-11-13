namespace MS.Internal.IO.Packaging.CompoundFile
{
    using MS.Internal.IO.Packaging;
    using System;
    using System.Collections;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Windows;

    internal static class ContainerUtilities
    {
        private static readonly int _int16Size = SecurityHelper.SizeOf(typeof(short));
        private static readonly int _int32Size = SecurityHelper.SizeOf(typeof(int));
        private static readonly int _int64Size = SecurityHelper.SizeOf(typeof(long));
        private static readonly byte[] _paddingBuf = new byte[4];
        private static readonly char[] _PathSeparatorArray = new char[] { PathSeparator };
        private static readonly CaseInsensitiveOrdinalStringComparer _stringCaseInsensitiveComparer = new CaseInsensitiveOrdinalStringComparer();
        internal static readonly char PathSeparator = '\\';
        internal static readonly string PathSeparatorAsString = new string(PathSeparator, 1);

        internal static int CalculateDWordPadBytesLength(int length)
        {
            int num = length & 3;
            if (num > 0)
            {
                num = 4 - num;
            }
            return num;
        }

        internal static void CheckAgainstNull(object paramRef, string testStringIdentifier)
        {
            if (paramRef == null)
            {
                throw new ArgumentNullException(testStringIdentifier);
            }
        }

        internal static void CheckStringAgainstNullAndEmpty(string testString, string testStringIdentifier)
        {
            if (testString == null)
            {
                throw new ArgumentNullException(testStringIdentifier);
            }
            if (testString.Length == 0)
            {
                throw new ArgumentException(System.Windows.SR.Get("StringEmpty"), testStringIdentifier);
            }
        }

        internal static void CheckStringAgainstReservedName(string nameString, string nameStringIdentifier)
        {
            if (IsReservedName(nameString))
            {
                throw new ArgumentException(System.Windows.SR.Get("StringCanNotBeReservedName", new object[] { nameStringIdentifier }));
            }
        }

        internal static void CheckStringForEmbeddedPathSeparator(string testString, string testStringIdentifier)
        {
            CheckStringAgainstNullAndEmpty(testString, testStringIdentifier);
            if (testString.IndexOf(PathSeparator) != -1)
            {
                throw new ArgumentException(System.Windows.SR.Get("NameCanNotHaveDelimiter", new object[] { testStringIdentifier, PathSeparator }), "testString");
            }
        }

        internal static string[] ConvertBackSlashPathToStringArrayPath(string backSlashPath)
        {
            if ((backSlashPath == null) || (backSlashPath.Length == 0))
            {
                return new string[0];
            }
            if (char.IsWhiteSpace(backSlashPath[0]) || char.IsWhiteSpace(backSlashPath[backSlashPath.Length - 1]))
            {
                throw new ArgumentException(System.Windows.SR.Get("MalformedCompoundFilePath"));
            }
            string[] strArray = backSlashPath.Split(_PathSeparatorArray);
            foreach (string str in strArray)
            {
                if (str.Length == 0)
                {
                    throw new ArgumentException(System.Windows.SR.Get("PathHasEmptyElement"), "backSlashPath");
                }
            }
            return strArray;
        }

        internal static string ConvertStringArrayPathToBackSlashPath(IList arrayPath)
        {
            if ((arrayPath == null) || (1 > arrayPath.Count))
            {
                return string.Empty;
            }
            if (1 == arrayPath.Count)
            {
                return (string) arrayPath[0];
            }
            CheckStringForEmbeddedPathSeparator((string) arrayPath[0], "Path array element");
            StringBuilder builder = new StringBuilder((string) arrayPath[0]);
            for (int i = 1; i < arrayPath.Count; i++)
            {
                CheckStringForEmbeddedPathSeparator((string) arrayPath[i], "Path array element");
                builder.Append(PathSeparator);
                builder.Append((string) arrayPath[i]);
            }
            return builder.ToString();
        }

        internal static string ConvertStringArrayPathToBackSlashPath(IList storages, string streamName)
        {
            string str = ConvertStringArrayPathToBackSlashPath(storages);
            if (str.Length > 0)
            {
                return (str + PathSeparator + streamName);
            }
            return streamName;
        }

        internal static bool IsReservedName(string nameString)
        {
            CheckStringAgainstNullAndEmpty(nameString, "nameString");
            return ((nameString[0] >= '\x0001') && (nameString[0] <= '\x001f'));
        }

        internal static string ReadByteLengthPrefixedDWordPaddedUnicodeString(BinaryReader reader)
        {
            int num;
            return ReadByteLengthPrefixedDWordPaddedUnicodeString(reader, out num);
        }

        internal static string ReadByteLengthPrefixedDWordPaddedUnicodeString(BinaryReader reader, out int bytesRead)
        {
            bytesRead = 0;
            CheckAgainstNull(reader, "reader");
            bytesRead = reader.ReadInt32();
            string str = null;
            if (bytesRead > 0)
            {
                try
                {
                    if (reader.BaseStream.Length < ((long) (bytesRead / 2)))
                    {
                        throw new FileFormatException(System.Windows.SR.Get("InvalidStringFormat"));
                    }
                }
                catch (NotSupportedException)
                {
                }
                str = new string(reader.ReadChars(bytesRead / 2));
                if (str.Length != (bytesRead / 2))
                {
                    throw new FileFormatException(System.Windows.SR.Get("InvalidStringFormat"));
                }
            }
            else
            {
                if (bytesRead != 0)
                {
                    throw new FileFormatException(System.Windows.SR.Get("InvalidStringFormat"));
                }
                str = string.Empty;
            }
            int count = CalculateDWordPadBytesLength(bytesRead);
            if (count > 0)
            {
                if (reader.ReadBytes(count).Length != count)
                {
                    throw new FileFormatException(System.Windows.SR.Get("InvalidStringFormat"));
                }
                bytesRead += count;
            }
            bytesRead += _int32Size;
            return str;
        }

        internal static int WriteByteLengthPrefixedDWordPaddedUnicodeString(BinaryWriter writer, string outputString)
        {
            int num = 0;
            if (outputString != null)
            {
                num = outputString.Length * 2;
            }
            if (writer != null)
            {
                writer.Write(num);
                if (num != 0)
                {
                    writer.Write(outputString.ToCharArray());
                }
            }
            if (num != 0)
            {
                int count = CalculateDWordPadBytesLength(num);
                if (count != 0)
                {
                    num += count;
                    if (writer != null)
                    {
                        writer.Write(_paddingBuf, 0, count);
                    }
                }
            }
            return (num + _int32Size);
        }

        internal static int Int16Size =>
            _int16Size;

        internal static int Int32Size =>
            _int32Size;

        internal static int Int64Size =>
            _int64Size;

        internal static CaseInsensitiveOrdinalStringComparer StringCaseInsensitiveComparer =>
            _stringCaseInsensitiveComparer;
    }
}

