namespace MS.Internal.IO.Packaging.CompoundFile
{
    using MS.Internal.WindowsBase;
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Windows;

    [FriendAccessAllowed]
    internal class FormatVersion
    {
        private string _featureIdentifier;
        private VersionPair _reader;
        private VersionPair _updater;
        private VersionPair _writer;

        private FormatVersion()
        {
        }

        public FormatVersion(string featureId, VersionPair version) : this(featureId, version, version, version)
        {
        }

        public FormatVersion(string featureId, VersionPair writerVersion, VersionPair readerVersion, VersionPair updaterVersion)
        {
            if (featureId == null)
            {
                throw new ArgumentNullException("featureId");
            }
            if (writerVersion == null)
            {
                throw new ArgumentNullException("writerVersion");
            }
            if (readerVersion == null)
            {
                throw new ArgumentNullException("readerVersion");
            }
            if (updaterVersion == null)
            {
                throw new ArgumentNullException("updaterVersion");
            }
            if (featureId.Length == 0)
            {
                throw new ArgumentException(System.Windows.SR.Get("ZeroLengthFeatureID"));
            }
            this._featureIdentifier = featureId;
            this._reader = readerVersion;
            this._updater = updaterVersion;
            this._writer = writerVersion;
        }

        public bool IsReadableBy(VersionPair version)
        {
            if (version == null)
            {
                throw new ArgumentNullException("version");
            }
            return (this._reader <= version);
        }

        public bool IsUpdatableBy(VersionPair version)
        {
            if (version == null)
            {
                throw new ArgumentNullException("version");
            }
            return (this._updater <= version);
        }

        private static FormatVersion LoadFromBinaryReader(BinaryReader reader, out int bytesRead)
        {
            int num;
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            FormatVersion version = new FormatVersion();
            bytesRead = 0;
            version._featureIdentifier = ContainerUtilities.ReadByteLengthPrefixedDWordPaddedUnicodeString(reader, out num);
            bytesRead += num;
            short major = reader.ReadInt16();
            bytesRead += ContainerUtilities.Int16Size;
            short minor = reader.ReadInt16();
            bytesRead += ContainerUtilities.Int16Size;
            version.ReaderVersion = new VersionPair(major, minor);
            major = reader.ReadInt16();
            bytesRead += ContainerUtilities.Int16Size;
            minor = reader.ReadInt16();
            bytesRead += ContainerUtilities.Int16Size;
            version.UpdaterVersion = new VersionPair(major, minor);
            major = reader.ReadInt16();
            bytesRead += ContainerUtilities.Int16Size;
            minor = reader.ReadInt16();
            bytesRead += ContainerUtilities.Int16Size;
            version.WriterVersion = new VersionPair(major, minor);
            return version;
        }

        public static FormatVersion LoadFromStream(Stream stream)
        {
            int num;
            return LoadFromStream(stream, out num);
        }

        internal static FormatVersion LoadFromStream(Stream stream, out int bytesRead)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            BinaryReader reader = new BinaryReader(stream, Encoding.Unicode);
            return LoadFromBinaryReader(reader, out bytesRead);
        }

        public int SaveToStream(Stream stream)
        {
            int num = 0;
            BinaryWriter writer = null;
            if (stream != null)
            {
                writer = new BinaryWriter(stream, Encoding.Unicode);
            }
            num += ContainerUtilities.WriteByteLengthPrefixedDWordPaddedUnicodeString(writer, this._featureIdentifier);
            if (stream != null)
            {
                writer.Write(this._reader.Major);
                writer.Write(this._reader.Minor);
            }
            num += ContainerUtilities.Int16Size;
            num += ContainerUtilities.Int16Size;
            if (stream != null)
            {
                writer.Write(this._updater.Major);
                writer.Write(this._updater.Minor);
            }
            num += ContainerUtilities.Int16Size;
            num += ContainerUtilities.Int16Size;
            if (stream != null)
            {
                writer.Write(this._writer.Major);
                writer.Write(this._writer.Minor);
            }
            num += ContainerUtilities.Int16Size;
            return (num + ContainerUtilities.Int16Size);
        }

        public string FeatureIdentifier =>
            this._featureIdentifier;

        public VersionPair ReaderVersion
        {
            get => 
                this._reader;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                this._reader = value;
            }
        }

        public VersionPair UpdaterVersion
        {
            get => 
                this._updater;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                this._updater = value;
            }
        }

        public VersionPair WriterVersion
        {
            get => 
                this._writer;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                this._writer = value;
            }
        }
    }
}

