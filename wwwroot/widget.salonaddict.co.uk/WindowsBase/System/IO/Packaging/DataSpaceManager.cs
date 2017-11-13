namespace System.IO.Packaging
{
    using MS.Internal.IO.Packaging;
    using MS.Internal.IO.Packaging.CompoundFile;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Windows;

    internal class DataSpaceManager
    {
        private StorageRoot _associatedStorage;
        private Hashtable _dataSpaceDefinitions;
        private SortedList _dataSpaceMap;
        private bool _dirtyFlag;
        private FormatVersion _fileFormatVersion;
        private byte[] _mapTableHeaderPreservation;
        private Hashtable _transformDefinitions;
        private ArrayList _transformedStreams;
        private static readonly Hashtable _transformLookupTable = new Hashtable(ContainerUtilities.StringCaseInsensitiveComparer);
        private const int AllowedExtraDataMaximumSize = 0x2000;
        private static readonly VersionPair DataSpaceCurrentReaderVersion = new VersionPair(1, 0);
        private static readonly VersionPair DataSpaceCurrentUpdaterVersion = new VersionPair(1, 0);
        private static readonly VersionPair DataSpaceCurrentWriterVersion = new VersionPair(1, 0);
        private const string DataSpaceDefinitionsStorageName = "DataSpaceInfo";
        private const string DataSpaceMapTableName = "DataSpaceMap";
        private const string DataSpaceStorageName = "\x0006DataSpaces";
        private static readonly string DataSpaceVersionIdentifier = "Microsoft.Container.DataSpaces";
        private const string DataSpaceVersionName = "Version";
        private const int KnownBytesInDataSpaceDefinitionHeader = 8;
        private const int KnownBytesInMapTableHeader = 8;
        private const int KnownBytesInTransformDefinitionHeader = 8;
        private const string TransformDefinitions = "TransformInfo";
        internal const int TransformIdentifierTypes_PredefinedTransformName = 1;
        private const string TransformPrimaryInfo = "\x0006Primary";

        internal event TransformInitializeEventHandler OnTransformInitialization;

        static DataSpaceManager()
        {
            _transformLookupTable[RightsManagementEncryptionTransform.ClassTransformIdentifier] = "System.IO.Packaging.RightsManagementEncryptionTransform";
            _transformLookupTable[CompressionTransform.ClassTransformIdentifier] = "System.IO.Packaging.CompressionTransform";
        }

        internal DataSpaceManager(StorageRoot containerInstance)
        {
            this._associatedStorage = containerInstance;
            StorageInfo info = new StorageInfo(this._associatedStorage, "\x0006DataSpaces");
            this._dataSpaceMap = new SortedList();
            this._mapTableHeaderPreservation = new byte[0];
            this._dataSpaceDefinitions = new Hashtable(ContainerUtilities.StringCaseInsensitiveComparer);
            this._transformDefinitions = new Hashtable(ContainerUtilities.StringCaseInsensitiveComparer);
            this._transformedStreams = new ArrayList();
            if (info.Exists)
            {
                this.ReadDataSpaceMap();
                this.ReadDataSpaceDefinitions();
                this.ReadTransformDefinitions();
            }
        }

        internal void CallTransformInitializers(TransformInitializationEventArgs initArguments)
        {
            if (this.OnTransformInitialization != null)
            {
                this.OnTransformInitialization(this, initArguments);
            }
        }

        internal void CheckDisposedStatus()
        {
            this._associatedStorage.CheckRootDisposedStatus();
            if (this._dataSpaceMap == null)
            {
                throw new ObjectDisposedException(null, System.Windows.SR.Get("DataSpaceManagerDisposed"));
            }
        }

        internal void CreateDataSpaceMapping(CompoundFileReference containerReference, string label)
        {
            this._dataSpaceMap[containerReference] = label;
            this._dirtyFlag = true;
        }

        internal Stream CreateDataSpaceStream(CompoundFileStreamReference containerReference, Stream rawStream)
        {
            Stream encodedDataStream = rawStream;
            IDictionary transformContext = new Hashtable();
            string dataSpaceLabel = this._dataSpaceMap[containerReference] as string;
            foreach (string str2 in this.GetDataSpaceDefinition(dataSpaceLabel).TransformStack)
            {
                TransformInstance transformInstanceOf = this.GetTransformInstanceOf(str2);
                if (transformInstanceOf.transformReference == null)
                {
                    transformInstanceOf.transformEnvironment = new TransformEnvironment(this, str2);
                    transformInstanceOf.transformReference = this.InstantiateDataTransformObject(transformInstanceOf.ClassType, transformInstanceOf.typeName, transformInstanceOf.transformEnvironment);
                }
                IDataTransform transformReference = transformInstanceOf.transformReference;
                if (!transformReference.IsReady)
                {
                    this.CallTransformInitializers(new TransformInitializationEventArgs(transformReference, dataSpaceLabel, containerReference.FullName, str2));
                    if (!transformReference.IsReady)
                    {
                        throw new InvalidOperationException(System.Windows.SR.Get("TransformObjectInitFailed"));
                    }
                }
                encodedDataStream = transformReference.GetTransformedStream(encodedDataStream, transformContext);
            }
            encodedDataStream = new BufferedStream(encodedDataStream);
            encodedDataStream = new StreamWithDictionary(encodedDataStream, transformContext);
            this._transformedStreams.Add(encodedDataStream);
            return encodedDataStream;
        }

        internal bool DataSpaceIsDefined(string dataSpaceLabel)
        {
            ContainerUtilities.CheckStringAgainstNullAndEmpty(dataSpaceLabel, "dataSpaceLabel");
            return this._dataSpaceDefinitions.Contains(dataSpaceLabel);
        }

        internal string DataSpaceOf(CompoundFileReference target)
        {
            if (this._dataSpaceMap.Contains(target))
            {
                return (string) this._dataSpaceMap[target];
            }
            return null;
        }

        internal string DefineDataSpace(string[] transformStack)
        {
            this.CheckDisposedStatus();
            long num = DateTime.Now.ToFileTime();
            string dataSpaceLabel = num.ToString(CultureInfo.InvariantCulture);
            while (this.DataSpaceIsDefined(dataSpaceLabel))
            {
                dataSpaceLabel = (num + 1L).ToString(CultureInfo.InvariantCulture);
            }
            this.DefineDataSpace(transformStack, dataSpaceLabel);
            return dataSpaceLabel;
        }

        internal void DefineDataSpace(string[] transformStack, string newDataSpaceLabel)
        {
            this.CheckDisposedStatus();
            if ((transformStack == null) || (transformStack.Length == 0))
            {
                throw new ArgumentException(System.Windows.SR.Get("TransformStackValid"));
            }
            ContainerUtilities.CheckStringAgainstNullAndEmpty(newDataSpaceLabel, "newDataSpaceLabel");
            ContainerUtilities.CheckStringAgainstReservedName(newDataSpaceLabel, "newDataSpaceLabel");
            if (this.DataSpaceIsDefined(newDataSpaceLabel))
            {
                throw new ArgumentException(System.Windows.SR.Get("DataSpaceLabelInUse"));
            }
            foreach (string str in transformStack)
            {
                ContainerUtilities.CheckStringAgainstNullAndEmpty(str, "Transform label");
                if (!this.TransformLabelIsDefined(str))
                {
                    throw new ArgumentException(System.Windows.SR.Get("TransformLabelUndefined"));
                }
            }
            this.SetDataSpaceDefinition(newDataSpaceLabel, new DataSpaceDefinition(new ArrayList(transformStack), null));
            this._dirtyFlag = true;
        }

        internal string DefineTransform(string transformClassName)
        {
            this.CheckDisposedStatus();
            long num = DateTime.Now.ToFileTime();
            string transformLabel = num.ToString(CultureInfo.InvariantCulture);
            while (this.TransformLabelIsDefined(transformLabel))
            {
                transformLabel = (num + 1L).ToString(CultureInfo.InvariantCulture);
            }
            this.DefineTransform(transformClassName, transformLabel);
            return transformLabel;
        }

        internal void DefineTransform(string transformClassName, string newTransformLabel)
        {
            this.CheckDisposedStatus();
            ContainerUtilities.CheckStringAgainstNullAndEmpty(transformClassName, "Transform identifier name");
            ContainerUtilities.CheckStringAgainstNullAndEmpty(newTransformLabel, "Transform label");
            ContainerUtilities.CheckStringAgainstReservedName(newTransformLabel, "Transform label");
            if (this.TransformLabelIsDefined(newTransformLabel))
            {
                throw new ArgumentException(System.Windows.SR.Get("TransformLabelInUse"));
            }
            TransformEnvironment environment = new TransformEnvironment(this, newTransformLabel);
            TransformInstance definition = new TransformInstance(1, transformClassName, null, environment);
            this.SetTransformDefinition(newTransformLabel, definition);
            IDataTransform instance = this.InstantiateDataTransformObject(1, transformClassName, environment);
            definition.transformReference = instance;
            if (!instance.IsReady)
            {
                this.CallTransformInitializers(new TransformInitializationEventArgs(instance, null, null, newTransformLabel));
            }
            this._dirtyFlag = true;
        }

        public void Dispose()
        {
            this.CheckDisposedStatus();
            foreach (StreamWithDictionary dictionary in this._transformedStreams)
            {
                if (!dictionary.Disposed)
                {
                    dictionary.Flush();
                }
            }
            this._transformedStreams.Clear();
            foreach (object obj2 in this._transformDefinitions.Values)
            {
                IDataTransform transformReference = ((TransformInstance) obj2).transformReference;
                if ((transformReference != null) && (transformReference is IDisposable))
                {
                    ((IDisposable) transformReference).Dispose();
                }
            }
            if ((FileAccess.Read != this._associatedStorage.OpenAccess) && this.DirtyFlag)
            {
                this.WriteDataSpaceMap();
                this.WriteDataSpaceDefinitions();
                this.WriteTransformDefinitions();
            }
            this._dataSpaceMap = null;
            this._dataSpaceDefinitions = null;
            this._transformDefinitions = null;
        }

        private void EnsureDataSpaceVersionInformation()
        {
            if (this._fileFormatVersion == null)
            {
                this._fileFormatVersion = new FormatVersion(DataSpaceVersionIdentifier, DataSpaceCurrentWriterVersion, DataSpaceCurrentReaderVersion, DataSpaceCurrentUpdaterVersion);
            }
        }

        private DataSpaceDefinition GetDataSpaceDefinition(string dataSpaceLabel) => 
            ((DataSpaceDefinition) this._dataSpaceDefinitions[dataSpaceLabel]);

        internal StorageInfo GetInstanceDataStorageOf(string transformLabel)
        {
            TransformInstance transformInstanceOf = this.GetTransformInstanceOf(transformLabel);
            if (transformInstanceOf.transformStorage == null)
            {
                StorageInfo parent = new StorageInfo(this._associatedStorage, "\x0006DataSpaces");
                if (!parent.Exists)
                {
                    parent.Create();
                }
                StorageInfo info2 = new StorageInfo(parent, "TransformInfo");
                if (!info2.Exists)
                {
                    info2.Create();
                }
                transformInstanceOf.transformStorage = new StorageInfo(info2, transformLabel);
            }
            return transformInstanceOf.transformStorage;
        }

        internal Stream GetPrimaryInstanceStreamOf(string transformLabel)
        {
            TransformInstance transformInstanceOf = this.GetTransformInstanceOf(transformLabel);
            if (transformInstanceOf.transformPrimaryStream == null)
            {
                if (this._associatedStorage.OpenAccess == FileAccess.Read)
                {
                    transformInstanceOf.transformPrimaryStream = new DirtyStateTrackingStream(new MemoryStream(new byte[0], false));
                }
                else
                {
                    transformInstanceOf.transformPrimaryStream = new DirtyStateTrackingStream(new MemoryStream());
                }
            }
            return transformInstanceOf.transformPrimaryStream;
        }

        internal IDataTransform GetTransformFromName(string transformLabel)
        {
            TransformInstance instance = this._transformDefinitions[transformLabel] as TransformInstance;
            if (instance == null)
            {
                return null;
            }
            IDataTransform transformReference = instance.transformReference;
            if (transformReference == null)
            {
                TransformEnvironment transformEnvironment = new TransformEnvironment(this, transformLabel);
                transformReference = this.InstantiateDataTransformObject(instance.ClassType, instance.typeName, transformEnvironment);
                instance.transformReference = transformReference;
            }
            return transformReference;
        }

        private TransformInstance GetTransformInstanceOf(string transformLabel) => 
            (this._transformDefinitions[transformLabel] as TransformInstance);

        internal List<IDataTransform> GetTransformsForStreamInfo(StreamInfo streamInfo)
        {
            string dataSpaceLabel = this.DataSpaceOf(streamInfo.StreamReference);
            if (dataSpaceLabel == null)
            {
                return new List<IDataTransform>(0);
            }
            ArrayList transformStack = this.GetDataSpaceDefinition(dataSpaceLabel).TransformStack;
            List<IDataTransform> list2 = new List<IDataTransform>(transformStack.Count);
            for (int i = 0; i < transformStack.Count; i++)
            {
                list2.Add(this.GetTransformFromName(transformStack[i] as string));
            }
            return list2;
        }

        private IDataTransform InstantiateDataTransformObject(int transformClassType, string transformClassName, TransformEnvironment transformEnvironment)
        {
            object obj2 = null;
            if (transformClassType != 1)
            {
                throw new NotSupportedException(System.Windows.SR.Get("TransformTypeUnsupported"));
            }
            if (((IEqualityComparer) ContainerUtilities.StringCaseInsensitiveComparer).Equals(transformClassName, RightsManagementEncryptionTransform.ClassTransformIdentifier))
            {
                obj2 = new RightsManagementEncryptionTransform(transformEnvironment);
            }
            else
            {
                if (!((IEqualityComparer) ContainerUtilities.StringCaseInsensitiveComparer).Equals(transformClassName, CompressionTransform.ClassTransformIdentifier))
                {
                    throw new ArgumentException(System.Windows.SR.Get("TransformLabelUndefined"));
                }
                obj2 = new CompressionTransform(transformEnvironment);
            }
            if (obj2 == null)
            {
                return null;
            }
            if (!(obj2 is IDataTransform))
            {
                throw new ArgumentException(System.Windows.SR.Get("TransformObjectImplementIDataTransform"));
            }
            return (IDataTransform) obj2;
        }

        private void ReadDataSpaceDefinitions()
        {
            this.ThrowIfIncorrectReaderVersion();
            StorageInfo parent = new StorageInfo(this._associatedStorage, "\x0006DataSpaces");
            StorageInfo info2 = new StorageInfo(parent, "DataSpaceInfo");
            if (info2.Exists)
            {
                foreach (StreamInfo info3 in info2.GetStreams())
                {
                    using (Stream stream = info3.GetStream(FileMode.Open))
                    {
                        using (BinaryReader reader = new BinaryReader(stream, Encoding.Unicode))
                        {
                            int num = reader.ReadInt32();
                            int capacity = reader.ReadInt32();
                            if ((num < 8) || (capacity < 0))
                            {
                                throw new FileFormatException(System.Windows.SR.Get("CorruptedData"));
                            }
                            ArrayList transformStack = new ArrayList(capacity);
                            byte[] extraData = null;
                            int count = num - 8;
                            if (count > 0x2000)
                            {
                                throw new FileFormatException(System.Windows.SR.Get("CorruptedData"));
                            }
                            if (count > 0)
                            {
                                extraData = reader.ReadBytes(count);
                                if (extraData.Length != count)
                                {
                                    throw new FileFormatException(System.Windows.SR.Get("CorruptedData"));
                                }
                            }
                            for (int i = 0; i < capacity; i++)
                            {
                                transformStack.Add(ContainerUtilities.ReadByteLengthPrefixedDWordPaddedUnicodeString(reader));
                            }
                            this.SetDataSpaceDefinition(info3.Name, new DataSpaceDefinition(transformStack, extraData));
                        }
                    }
                }
            }
        }

        private void ReadDataSpaceMap()
        {
            StorageInfo parent = new StorageInfo(this._associatedStorage, "\x0006DataSpaces");
            StreamInfo info2 = new StreamInfo(parent, "DataSpaceMap");
            if (parent.StreamExists("DataSpaceMap"))
            {
                this.ReadDataSpaceVersionInformation(parent);
                this.ThrowIfIncorrectReaderVersion();
                using (Stream stream = info2.GetStream(FileMode.Open))
                {
                    using (BinaryReader reader = new BinaryReader(stream, Encoding.Unicode))
                    {
                        int num = reader.ReadInt32();
                        int num2 = reader.ReadInt32();
                        if ((num < 8) || (num2 < 0))
                        {
                            throw new FileFormatException(System.Windows.SR.Get("CorruptedData"));
                        }
                        int count = num - 8;
                        if (0 < count)
                        {
                            if (count > 0x2000)
                            {
                                throw new FileFormatException(System.Windows.SR.Get("CorruptedData"));
                            }
                            this._mapTableHeaderPreservation = reader.ReadBytes(count);
                            if (this._mapTableHeaderPreservation.Length != count)
                            {
                                throw new FileFormatException(System.Windows.SR.Get("CorruptedData"));
                            }
                        }
                        this._dataSpaceMap.Capacity = num2;
                        for (int i = 0; i < num2; i++)
                        {
                            int num5;
                            int num4 = reader.ReadInt32();
                            if (num4 < 0)
                            {
                                throw new FileFormatException(System.Windows.SR.Get("CorruptedData"));
                            }
                            int num6 = 4;
                            CompoundFileReference reference = CompoundFileReference.Load(reader, out num5);
                            num6 += num5;
                            string str = ContainerUtilities.ReadByteLengthPrefixedDWordPaddedUnicodeString(reader, out num5);
                            num6 += num5;
                            this._dataSpaceMap[reference] = str;
                            if (num4 != num6)
                            {
                                throw new IOException(System.Windows.SR.Get("DataSpaceMapEntryInvalid"));
                            }
                        }
                    }
                }
            }
        }

        private void ReadDataSpaceVersionInformation(StorageInfo dataSpaceStorage)
        {
            if ((this._fileFormatVersion == null) && dataSpaceStorage.StreamExists("Version"))
            {
                using (Stream stream = dataSpaceStorage.GetStreamInfo("Version").GetStream(FileMode.Open))
                {
                    this._fileFormatVersion = FormatVersion.LoadFromStream(stream);
                    if (!((IEqualityComparer) ContainerUtilities.StringCaseInsensitiveComparer).Equals(this._fileFormatVersion.FeatureIdentifier, DataSpaceVersionIdentifier))
                    {
                        throw new FileFormatException(System.Windows.SR.Get("InvalidTransformFeatureName", new object[] { this._fileFormatVersion.FeatureIdentifier, DataSpaceVersionIdentifier }));
                    }
                    this._fileFormatVersion.WriterVersion = DataSpaceCurrentWriterVersion;
                }
            }
        }

        private void ReadTransformDefinitions()
        {
            this.ThrowIfIncorrectReaderVersion();
            StorageInfo parent = new StorageInfo(this._associatedStorage, "\x0006DataSpaces");
            StorageInfo info2 = new StorageInfo(parent, "TransformInfo");
            if (info2.Exists)
            {
                foreach (StorageInfo info3 in info2.GetSubStorages())
                {
                    StreamInfo info4 = new StreamInfo(info3, "\x0006Primary");
                    using (Stream stream = info4.GetStream(FileMode.Open))
                    {
                        using (BinaryReader reader = new BinaryReader(stream, Encoding.Unicode))
                        {
                            int num = reader.ReadInt32();
                            int classType = reader.ReadInt32();
                            if (num < 0)
                            {
                                throw new FileFormatException(System.Windows.SR.Get("CorruptedData"));
                            }
                            TransformInstance definition = new TransformInstance(classType, ContainerUtilities.ReadByteLengthPrefixedDWordPaddedUnicodeString(reader));
                            int count = num - ((int) stream.Position);
                            if (count < 0)
                            {
                                throw new FileFormatException(System.Windows.SR.Get("CorruptedData"));
                            }
                            if (count > 0)
                            {
                                if (count > 0x2000)
                                {
                                    throw new FileFormatException(System.Windows.SR.Get("CorruptedData"));
                                }
                                byte[] buffer = reader.ReadBytes(count);
                                if (buffer.Length != count)
                                {
                                    throw new FileFormatException(System.Windows.SR.Get("CorruptedData"));
                                }
                                definition.ExtraData = buffer;
                            }
                            if (stream.Length > stream.Position)
                            {
                                MemoryStream stream2;
                                int num4 = (int) (stream.Length - stream.Position);
                                byte[] buffer2 = new byte[num4];
                                PackagingUtilities.ReliableRead(stream, buffer2, 0, num4);
                                if (this._associatedStorage.OpenAccess == FileAccess.Read)
                                {
                                    stream2 = new MemoryStream(buffer2, false);
                                }
                                else
                                {
                                    stream2 = new MemoryStream();
                                    stream2.Write(buffer2, 0, num4);
                                }
                                stream2.Seek(0L, SeekOrigin.Begin);
                                definition.transformPrimaryStream = new DirtyStateTrackingStream(stream2);
                            }
                            definition.transformStorage = info3;
                            this.SetTransformDefinition(info3.Name, definition);
                        }
                    }
                }
            }
        }

        internal void RemoveContainerFromDataSpaceMap(CompoundFileReference target)
        {
            this.CheckDisposedStatus();
            if (this._dataSpaceMap.Contains(target))
            {
                this._dataSpaceMap.Remove(target);
                this._dirtyFlag = true;
            }
        }

        private void SetDataSpaceDefinition(string dataSpaceLabel, DataSpaceDefinition definition)
        {
            this._dataSpaceDefinitions[dataSpaceLabel] = definition;
        }

        private void SetTransformDefinition(string transformLabel, TransformInstance definition)
        {
            this._transformDefinitions[transformLabel] = definition;
        }

        private void ThrowIfIncorrectReaderVersion()
        {
            this.EnsureDataSpaceVersionInformation();
            if (!this._fileFormatVersion.IsReadableBy(DataSpaceCurrentReaderVersion))
            {
                throw new FileFormatException(System.Windows.SR.Get("ReaderVersionError", new object[] { this._fileFormatVersion.ReaderVersion, DataSpaceCurrentReaderVersion }));
            }
        }

        private void ThrowIfIncorrectUpdaterVersion()
        {
            this.EnsureDataSpaceVersionInformation();
            if (!this._fileFormatVersion.IsUpdatableBy(DataSpaceCurrentUpdaterVersion))
            {
                throw new FileFormatException(System.Windows.SR.Get("UpdaterVersionError", new object[] { this._fileFormatVersion.UpdaterVersion, DataSpaceCurrentUpdaterVersion }));
            }
        }

        internal bool TransformLabelIsDefined(string transformLabel) => 
            this._transformDefinitions.Contains(transformLabel);

        private void WriteDataSpaceDefinitions()
        {
            this.ThrowIfIncorrectUpdaterVersion();
            StorageInfo parent = new StorageInfo(this._associatedStorage, "\x0006DataSpaces");
            if (0 < this._dataSpaceDefinitions.Count)
            {
                StorageInfo info2 = new StorageInfo(parent, "DataSpaceInfo");
                info2.Create();
                foreach (string str in this._dataSpaceDefinitions.Keys)
                {
                    StreamInfo info3 = new StreamInfo(info2, str);
                    using (Stream stream = info3.GetStream())
                    {
                        using (BinaryWriter writer = new BinaryWriter(stream, Encoding.Unicode))
                        {
                            DataSpaceDefinition definition = (DataSpaceDefinition) this._dataSpaceDefinitions[str];
                            int num = 8;
                            if (definition.ExtraData != null)
                            {
                                num += definition.ExtraData.Length;
                            }
                            writer.Write(num);
                            writer.Write(definition.TransformStack.Count);
                            if (definition.ExtraData != null)
                            {
                                writer.Write(definition.ExtraData);
                            }
                            foreach (object obj2 in definition.TransformStack)
                            {
                                ContainerUtilities.WriteByteLengthPrefixedDWordPaddedUnicodeString(writer, (string) obj2);
                            }
                        }
                    }
                }
            }
        }

        private void WriteDataSpaceMap()
        {
            this.ThrowIfIncorrectUpdaterVersion();
            StorageInfo parent = new StorageInfo(this._associatedStorage, "\x0006DataSpaces");
            StreamInfo info2 = new StreamInfo(parent, "DataSpaceMap");
            if (0 < this._dataSpaceMap.Count)
            {
                StreamInfo streamInfo = null;
                if (parent.StreamExists("Version"))
                {
                    streamInfo = parent.GetStreamInfo("Version");
                }
                else
                {
                    streamInfo = parent.CreateStream("Version");
                }
                Stream stream = streamInfo.GetStream();
                this._fileFormatVersion.SaveToStream(stream);
                stream.Close();
                using (Stream stream2 = info2.GetStream(FileMode.Create))
                {
                    using (BinaryWriter writer = new BinaryWriter(stream2, Encoding.Unicode))
                    {
                        writer.Write((int) (8 + this._mapTableHeaderPreservation.Length));
                        writer.Write(this._dataSpaceMap.Count);
                        writer.Write(this._mapTableHeaderPreservation);
                        foreach (CompoundFileReference reference in this._dataSpaceMap.Keys)
                        {
                            string outputString = (string) this._dataSpaceMap[reference];
                            int num = CompoundFileReference.Save(reference, null) + ContainerUtilities.WriteByteLengthPrefixedDWordPaddedUnicodeString(null, outputString);
                            num += 4;
                            writer.Write(num);
                            CompoundFileReference.Save(reference, writer);
                            ContainerUtilities.WriteByteLengthPrefixedDWordPaddedUnicodeString(writer, outputString);
                        }
                    }
                    return;
                }
            }
            if (parent.StreamExists("DataSpaceMap"))
            {
                parent.DeleteStream("DataSpaceMap");
            }
        }

        private void WriteTransformDefinitions()
        {
            this.ThrowIfIncorrectUpdaterVersion();
            StorageInfo parent = new StorageInfo(this._associatedStorage, "\x0006DataSpaces");
            StorageInfo info2 = new StorageInfo(parent, "TransformInfo");
            if (0 < this._transformDefinitions.Count)
            {
                foreach (string str in this._transformDefinitions.Keys)
                {
                    string fileName = null;
                    TransformInstance transformInstanceOf = this.GetTransformInstanceOf(str);
                    if (transformInstanceOf.transformEnvironment != null)
                    {
                        fileName = transformInstanceOf.transformEnvironment.TransformLabel;
                    }
                    else
                    {
                        fileName = str;
                    }
                    StorageInfo info3 = new StorageInfo(info2, fileName);
                    StreamInfo info4 = new StreamInfo(info3, "\x0006Primary");
                    using (Stream stream = info4.GetStream())
                    {
                        using (BinaryWriter writer = new BinaryWriter(stream, Encoding.Unicode))
                        {
                            int num = 8 + ContainerUtilities.WriteByteLengthPrefixedDWordPaddedUnicodeString(null, transformInstanceOf.typeName);
                            if (transformInstanceOf.ExtraData != null)
                            {
                                num += transformInstanceOf.ExtraData.Length;
                            }
                            writer.Write(num);
                            writer.Write(1);
                            ContainerUtilities.WriteByteLengthPrefixedDWordPaddedUnicodeString(writer, transformInstanceOf.typeName);
                            if (transformInstanceOf.ExtraData != null)
                            {
                                writer.Write(transformInstanceOf.ExtraData);
                            }
                            if (transformInstanceOf.transformPrimaryStream != null)
                            {
                                byte[] buffer = ((MemoryStream) ((DirtyStateTrackingStream) transformInstanceOf.transformPrimaryStream).BaseStream).GetBuffer();
                                stream.Write(buffer, 0, buffer.Length);
                            }
                        }
                    }
                }
            }
            else if (info2.Exists)
            {
                parent.Delete(true, "TransformInfo");
            }
        }

        internal int Count
        {
            get
            {
                this.CheckDisposedStatus();
                return this._dataSpaceMap.Count;
            }
        }

        private bool DirtyFlag
        {
            get
            {
                if (this._dirtyFlag)
                {
                    return true;
                }
                foreach (string str in this._transformDefinitions.Keys)
                {
                    if (((DirtyStateTrackingStream) this.GetTransformInstanceOf(str).transformPrimaryStream).DirtyFlag)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DataSpaceDefinition
        {
            private ArrayList _transformStack;
            private byte[] _extraData;
            internal DataSpaceDefinition(ArrayList transformStack, byte[] extraData)
            {
                this._transformStack = transformStack;
                this._extraData = extraData;
            }

            internal ArrayList TransformStack =>
                this._transformStack;
            internal byte[] ExtraData =>
                this._extraData;
        }

        private class DirtyStateTrackingStream : Stream
        {
            private Stream _baseStream;
            private bool _dirty;

            internal DirtyStateTrackingStream(Stream baseStream)
            {
                this._baseStream = baseStream;
            }

            private void CheckDisposed()
            {
                if (this._baseStream == null)
                {
                    throw new ObjectDisposedException(null, System.Windows.SR.Get("StreamObjectDisposed"));
                }
            }

            protected override void Dispose(bool disposing)
            {
                try
                {
                    if (disposing && (this._baseStream != null))
                    {
                        this._baseStream.Close();
                    }
                }
                finally
                {
                    this._baseStream = null;
                    base.Dispose(disposing);
                }
            }

            public override void Flush()
            {
                this.CheckDisposed();
                this._baseStream.Flush();
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                this.CheckDisposed();
                return this._baseStream.Read(buffer, offset, count);
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                this.CheckDisposed();
                return this._baseStream.Seek(offset, origin);
            }

            public override void SetLength(long newLength)
            {
                this.CheckDisposed();
                if (newLength != this._baseStream.Length)
                {
                    this._dirty = true;
                }
                this._baseStream.SetLength(newLength);
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                this.CheckDisposed();
                this._baseStream.Write(buffer, offset, count);
                this._dirty = true;
            }

            internal Stream BaseStream =>
                this._baseStream;

            public override bool CanRead =>
                ((this._baseStream != null) && this._baseStream.CanRead);

            public override bool CanSeek =>
                ((this._baseStream != null) && this._baseStream.CanSeek);

            public override bool CanWrite =>
                ((this._baseStream != null) && this._baseStream.CanWrite);

            internal bool DirtyFlag =>
                ((this._baseStream != null) && this._dirty);

            public override long Length
            {
                get
                {
                    this.CheckDisposed();
                    return this._baseStream.Length;
                }
            }

            public override long Position
            {
                get
                {
                    this.CheckDisposed();
                    return this._baseStream.Position;
                }
                set
                {
                    this.CheckDisposed();
                    this._baseStream.Position = value;
                }
            }
        }

        internal delegate void TransformInitializeEventHandler(object sender, TransformInitializationEventArgs e);

        private class TransformInstance
        {
            private int _classType;
            private byte[] _extraData;
            internal TransformEnvironment transformEnvironment;
            internal Stream transformPrimaryStream;
            internal IDataTransform transformReference;
            internal StorageInfo transformStorage;
            internal string typeName;

            internal TransformInstance(int classType, string name) : this(classType, name, null, null, null, null)
            {
            }

            internal TransformInstance(int classType, string name, IDataTransform instance, TransformEnvironment environment) : this(classType, name, instance, environment, null, null)
            {
            }

            internal TransformInstance(int classType, string name, IDataTransform instance, TransformEnvironment environment, Stream primaryStream, StorageInfo storage)
            {
                this.typeName = name;
                this.transformReference = instance;
                this.transformEnvironment = environment;
                this.transformPrimaryStream = primaryStream;
                this.transformStorage = storage;
                this._classType = classType;
            }

            internal int ClassType =>
                this._classType;

            internal byte[] ExtraData
            {
                get => 
                    this._extraData;
                set
                {
                    this._extraData = value;
                }
            }
        }
    }
}

