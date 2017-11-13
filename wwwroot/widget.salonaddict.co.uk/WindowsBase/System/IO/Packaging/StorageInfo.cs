namespace System.IO.Packaging
{
    using MS.Internal;
    using MS.Internal.IO.Packaging.CompoundFile;
    using System;
    using System.Collections;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Runtime.InteropServices.ComTypes;
    using System.Windows;

    public class StorageInfo
    {
        internal StorageInfoCore core;
        private StorageInfo parentStorage;
        private StorageRoot rootStorage;
        private static readonly string sc_compressionTransformName = "CompressionTransform";
        private static readonly string sc_dataspaceLabelNoEncryptionNormalCompression = "NoEncryptionNormalCompression";
        private static readonly string sc_dataspaceLabelRMEncryptionNormalCompression = "RMEncryptionNormalCompression";

        internal StorageInfo(IStorage safeIStorage)
        {
            this.core = new StorageInfoCore(null, safeIStorage);
        }

        internal StorageInfo(StorageInfo parent, string fileName)
        {
            ContainerUtilities.CheckAgainstNull(parent, "parent");
            ContainerUtilities.CheckAgainstNull(fileName, "fileName");
            this.BuildStorageInfoRelativeToStorage(parent, fileName);
        }

        internal ArrayList BuildFullNameFromParentName()
        {
            if (this.parentStorage == null)
            {
                return new ArrayList();
            }
            ArrayList list = this.parentStorage.BuildFullNameFromParentName();
            list.Add(this.core.storageName);
            return list;
        }

        internal ArrayList BuildFullNameInternalFromParentNameInternal()
        {
            if (this.parentStorage == null)
            {
                return new ArrayList();
            }
            ArrayList list = this.parentStorage.BuildFullNameInternalFromParentNameInternal();
            list.Add(this.core.storageName);
            return list;
        }

        private void BuildStorageInfoRelativeToStorage(StorageInfo parent, string fileName)
        {
            this.parentStorage = parent;
            this.core = parent.CoreForChildStorage(fileName);
            this.rootStorage = parent.Root;
        }

        private bool CanOpenStorage(string nameInternal)
        {
            StorageInfoCore core = this.core.elementInfoCores[nameInternal] as StorageInfoCore;
            int errorCode = 0;
            errorCode = this.core.safeIStorage.OpenStorage(nameInternal, null, (this.GetStat().grfMode & 3) | 0x10, IntPtr.Zero, 0, out core.safeIStorage);
            if (errorCode == 0)
            {
                return true;
            }
            if (-2147287038 != errorCode)
            {
                throw new IOException(System.Windows.SR.Get("CanNotOpenStorage"), new COMException(System.Windows.SR.Get("NamedAPIFailure", new object[] { "IStorage::OpenStorage" }), errorCode));
            }
            return false;
        }

        internal void CheckDisposedStatus()
        {
            if (this.StorageDisposed)
            {
                throw new ObjectDisposedException(null, System.Windows.SR.Get("StorageInfoDisposed"));
            }
        }

        private DateTime ConvertFILETIMEToDateTime(System.Runtime.InteropServices.ComTypes.FILETIME time)
        {
            if ((time.dwHighDateTime == 0) && (time.dwLowDateTime == 0))
            {
                throw new NotSupportedException(System.Windows.SR.Get("TimeStampNotAvailable"));
            }
            return DateTime.FromFileTime((time.dwHighDateTime << 0x20) + ((long) ((ulong) time.dwLowDateTime)));
        }

        private StorageInfoCore CoreForChildStorage(string storageNname)
        {
            this.CheckDisposedStatus();
            object obj2 = this.core.elementInfoCores[storageNname];
            if ((obj2 != null) && !(obj2 is StorageInfoCore))
            {
                throw new InvalidOperationException(System.Windows.SR.Get("NameAlreadyInUse", new object[] { storageNname }));
            }
            if (obj2 == null)
            {
                obj2 = new StorageInfoCore(storageNname);
                this.core.elementInfoCores[storageNname] = obj2;
            }
            return (obj2 as StorageInfoCore);
        }

        internal StreamInfoCore CoreForChildStream(string streamName)
        {
            this.CheckDisposedStatus();
            object obj2 = this.core.elementInfoCores[streamName];
            if ((obj2 != null) && !(obj2 is StreamInfoCore))
            {
                throw new InvalidOperationException(System.Windows.SR.Get("NameAlreadyInUse", new object[] { streamName }));
            }
            if (obj2 == null)
            {
                DataSpaceManager dataSpaceManager = this.Root.GetDataSpaceManager();
                if (dataSpaceManager != null)
                {
                    obj2 = new StreamInfoCore(streamName, dataSpaceManager.DataSpaceOf(new CompoundFileStreamReference(this.FullNameInternal, streamName)));
                }
                else
                {
                    obj2 = new StreamInfoCore(streamName, null, null);
                }
                this.core.elementInfoCores[streamName] = obj2;
            }
            return (obj2 as StreamInfoCore);
        }

        internal void Create()
        {
            this.CheckDisposedStatus();
            if (this.parentStorage != null)
            {
                if (!this.parentStorage.Exists)
                {
                    this.parentStorage.Create();
                }
                if (!this.InternalExists())
                {
                    this.parentStorage.CreateStorage(this.core.storageName);
                }
            }
        }

        private StorageInfo CreateStorage(string name)
        {
            StorageInfo info = new StorageInfo(this, name);
            if (info.InternalExists(name))
            {
                throw new IOException(System.Windows.SR.Get("StorageAlreadyExist"));
            }
            StorageInfoCore core = this.core.elementInfoCores[name] as StorageInfoCore;
            Invariant.Assert(null != core);
            int errorCode = this.core.safeIStorage.CreateStorage(name, (this.GetStat().grfMode & 3) | 0x10, 0, 0, out core.safeIStorage);
            switch (errorCode)
            {
                case 0:
                    this.InvalidateEnumerators();
                    return info;

                case -2147287035:
                    throw new UnauthorizedAccessException(System.Windows.SR.Get("CanNotCreateAccessDenied"), new COMException(System.Windows.SR.Get("NamedAPIFailure", new object[] { "IStorage.CreateStorage" }), errorCode));
            }
            throw new IOException(System.Windows.SR.Get("UnableToCreateStorage"), new COMException(System.Windows.SR.Get("NamedAPIFailure", new object[] { "IStorage.CreateStorage" }), errorCode));
        }

        public StreamInfo CreateStream(string name) => 
            this.CreateStream(name, CompressionOption.NotCompressed, EncryptionOption.None);

        public StreamInfo CreateStream(string name, CompressionOption compressionOption, EncryptionOption encryptionOption)
        {
            this.CheckDisposedStatus();
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (((IEqualityComparer) ContainerUtilities.StringCaseInsensitiveComparer).Equals(name, EncryptedPackageEnvelope.PackageStreamName))
            {
                throw new ArgumentException(System.Windows.SR.Get("StreamNameNotValid", new object[] { name }));
            }
            StreamInfo info = new StreamInfo(this, name, compressionOption, encryptionOption);
            if (info.InternalExists())
            {
                throw new IOException(System.Windows.SR.Get("StreamAlreadyExist"));
            }
            DataSpaceManager dataSpaceManager = this.Root.GetDataSpaceManager();
            string dataSpaceLabel = null;
            if (dataSpaceManager != null)
            {
                if ((compressionOption != CompressionOption.NotCompressed) && !dataSpaceManager.TransformLabelIsDefined(sc_compressionTransformName))
                {
                    dataSpaceManager.DefineTransform(CompressionTransform.ClassTransformIdentifier, sc_compressionTransformName);
                }
                if ((encryptionOption == EncryptionOption.RightsManagement) && !dataSpaceManager.TransformLabelIsDefined(EncryptedPackageEnvelope.EncryptionTransformName))
                {
                    throw new SystemException(System.Windows.SR.Get("RightsManagementEncryptionTransformNotFound"));
                }
                if ((compressionOption != CompressionOption.NotCompressed) && (encryptionOption == EncryptionOption.RightsManagement))
                {
                    dataSpaceLabel = sc_dataspaceLabelRMEncryptionNormalCompression;
                    if (!dataSpaceManager.DataSpaceIsDefined(dataSpaceLabel))
                    {
                        string[] transformStack = new string[] { EncryptedPackageEnvelope.EncryptionTransformName, sc_compressionTransformName };
                        dataSpaceManager.DefineDataSpace(transformStack, dataSpaceLabel);
                    }
                }
                else if ((compressionOption != CompressionOption.NotCompressed) && (encryptionOption == EncryptionOption.None))
                {
                    dataSpaceLabel = sc_dataspaceLabelNoEncryptionNormalCompression;
                    if (!dataSpaceManager.DataSpaceIsDefined(dataSpaceLabel))
                    {
                        string[] strArray2 = new string[] { sc_compressionTransformName };
                        dataSpaceManager.DefineDataSpace(strArray2, dataSpaceLabel);
                    }
                }
                else if (encryptionOption == EncryptionOption.RightsManagement)
                {
                    dataSpaceLabel = EncryptedPackageEnvelope.DataspaceLabelRMEncryptionNoCompression;
                    if (!dataSpaceManager.DataSpaceIsDefined(dataSpaceLabel))
                    {
                        string[] strArray3 = new string[] { EncryptedPackageEnvelope.EncryptionTransformName };
                        dataSpaceManager.DefineDataSpace(strArray3, dataSpaceLabel);
                    }
                }
            }
            if (dataSpaceLabel == null)
            {
                info.Create();
                return info;
            }
            info.Create(dataSpaceLabel);
            return info;
        }

        public StorageInfo CreateSubStorage(string name)
        {
            this.CheckDisposedStatus();
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            return this.CreateStorage(name);
        }

        internal bool Delete(bool recursive, string name)
        {
            bool flag = false;
            this.CheckDisposedStatus();
            if (this.parentStorage == null)
            {
                throw new InvalidOperationException(System.Windows.SR.Get("CanNotDeleteRoot"));
            }
            if (!this.InternalExists(name))
            {
                return flag;
            }
            if (!recursive && !this.StorageIsEmpty())
            {
                throw new IOException(System.Windows.SR.Get("CanNotDeleteNonEmptyStorage"));
            }
            this.InvalidateEnumerators();
            this.parentStorage.DestroyElement(name);
            return true;
        }

        public void DeleteStream(string name)
        {
            this.CheckDisposedStatus();
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            StreamInfo info = new StreamInfo(this, name);
            if (info.InternalExists())
            {
                info.Delete();
            }
        }

        public void DeleteSubStorage(string name)
        {
            this.CheckDisposedStatus();
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            StorageInfo info = new StorageInfo(this, name);
            if (info.InternalExists(name))
            {
                this.InvalidateEnumerators();
                this.DestroyElement(name);
            }
        }

        internal void DestroyElement(string elementNameInternal)
        {
            object obj2 = this.core.elementInfoCores[elementNameInternal];
            if (FileAccess.Read == this.Root.OpenAccess)
            {
                throw new UnauthorizedAccessException(System.Windows.SR.Get("CanNotDeleteInReadOnly"));
            }
            DataSpaceManager dataSpaceManager = this.Root.GetDataSpaceManager();
            if (dataSpaceManager != null)
            {
                if (obj2 is StorageInfoCore)
                {
                    string storageName = ((StorageInfoCore) obj2).storageName;
                    StorageInfo storageInfo = new StorageInfo(this, storageName);
                    this.RemoveSubStorageEntryFromDataSpaceMap(storageInfo);
                }
                else if (obj2 is StreamInfoCore)
                {
                    dataSpaceManager.RemoveContainerFromDataSpaceMap(new CompoundFileStreamReference(this.FullNameInternal, elementNameInternal));
                }
            }
            try
            {
                this.core.safeIStorage.DestroyElement(elementNameInternal);
            }
            catch (COMException exception)
            {
                if (exception.ErrorCode == -2147287035)
                {
                    throw new UnauthorizedAccessException(System.Windows.SR.Get("CanNotDeleteAccessDenied"), exception);
                }
                throw new IOException(System.Windows.SR.Get("CanNotDelete"), exception);
            }
            this.InvalidateEnumerators();
            if (obj2 is StorageInfoCore)
            {
                StorageInfoCore core = (StorageInfoCore) obj2;
                core.storageName = null;
                if (core.safeIStorage != null)
                {
                    ((IDisposable) core.safeIStorage).Dispose();
                    core.safeIStorage = null;
                }
            }
            else if (obj2 is StreamInfoCore)
            {
                StreamInfoCore core2 = (StreamInfoCore) obj2;
                core2.streamName = null;
                try
                {
                    if (core2.exposedStream != null)
                    {
                        ((Stream) core2.exposedStream).Close();
                    }
                }
                catch (Exception exception2)
                {
                    if (CriticalExceptions.IsCriticalException(exception2))
                    {
                        throw;
                    }
                }
                core2.exposedStream = null;
                if (core2.safeIStream != null)
                {
                    ((IDisposable) core2.safeIStream).Dispose();
                    core2.safeIStream = null;
                }
            }
            this.core.elementInfoCores.Remove(elementNameInternal);
        }

        private void EnsureArrayForEnumeration(EnumeratorTypes desiredArrayType)
        {
            if (this.core.validEnumerators[desiredArrayType] == null)
            {
                System.Runtime.InteropServices.ComTypes.STATSTG statstg;
                uint num;
                ArrayList list = new ArrayList();
                string nameString = null;
                IEnumSTATSTG ppEnum = null;
                this.core.safeIStorage.EnumElements(0, IntPtr.Zero, 0, out ppEnum);
                ppEnum.Reset();
                ppEnum.Next(1, out statstg, out num);
                while (0 < num)
                {
                    nameString = statstg.pwcsName;
                    if (!ContainerUtilities.IsReservedName(nameString))
                    {
                        if (1 == statstg.type)
                        {
                            if ((desiredArrayType == EnumeratorTypes.Everything) || (desiredArrayType == EnumeratorTypes.OnlyStorages))
                            {
                                list.Add(new StorageInfo(this, nameString));
                            }
                        }
                        else
                        {
                            if (2 != statstg.type)
                            {
                                throw new NotSupportedException(System.Windows.SR.Get("UnsupportedTypeEncounteredWhenBuildingStgEnum"));
                            }
                            if ((desiredArrayType == EnumeratorTypes.Everything) || (desiredArrayType == EnumeratorTypes.OnlyStreams))
                            {
                                list.Add(new StreamInfo(this, nameString));
                            }
                        }
                    }
                    ppEnum.Next(1, out statstg, out num);
                }
                this.core.validEnumerators[desiredArrayType] = list;
                ((IDisposable) ppEnum).Dispose();
                ppEnum = null;
            }
        }

        internal bool FindStatStgOfName(string streamName, out System.Runtime.InteropServices.ComTypes.STATSTG statStg)
        {
            uint num;
            bool flag = false;
            IEnumSTATSTG ppEnum = null;
            this.core.safeIStorage.EnumElements(0, IntPtr.Zero, 0, out ppEnum);
            ppEnum.Reset();
            ppEnum.Next(1, out statStg, out num);
            while ((0 < num) && !flag)
            {
                if (((IEqualityComparer) ContainerUtilities.StringCaseInsensitiveComparer).Equals(streamName, statStg.pwcsName))
                {
                    flag = true;
                }
                else
                {
                    ppEnum.Next(1, out statStg, out num);
                }
            }
            ((IDisposable) ppEnum).Dispose();
            ppEnum = null;
            return flag;
        }

        private System.Runtime.InteropServices.ComTypes.STATSTG GetStat()
        {
            System.Runtime.InteropServices.ComTypes.STATSTG statstg;
            this.VerifyExists();
            this.core.safeIStorage.Stat(out statstg, 0);
            return statstg;
        }

        public StreamInfo GetStreamInfo(string name)
        {
            this.CheckDisposedStatus();
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            StreamInfo info = new StreamInfo(this, name);
            if (!info.InternalExists())
            {
                throw new IOException(System.Windows.SR.Get("StreamNotExist"));
            }
            return info;
        }

        public StreamInfo[] GetStreams()
        {
            this.CheckDisposedStatus();
            this.VerifyExists();
            this.EnsureArrayForEnumeration(EnumeratorTypes.OnlyStreams);
            ArrayList list = (ArrayList) this.core.validEnumerators[EnumeratorTypes.OnlyStreams];
            Invariant.Assert(list != null);
            return (StreamInfo[]) list.ToArray(typeof(StreamInfo));
        }

        public StorageInfo GetSubStorageInfo(string name)
        {
            StorageInfo info = new StorageInfo(this, name);
            if (!info.InternalExists(name))
            {
                throw new IOException(System.Windows.SR.Get("StorageNotExist"));
            }
            return info;
        }

        public StorageInfo[] GetSubStorages()
        {
            this.CheckDisposedStatus();
            this.VerifyExists();
            this.EnsureArrayForEnumeration(EnumeratorTypes.OnlyStorages);
            ArrayList list = (ArrayList) this.core.validEnumerators[EnumeratorTypes.OnlyStorages];
            Invariant.Assert(list != null);
            return (StorageInfo[]) list.ToArray(typeof(StorageInfo));
        }

        private bool InternalExists() => 
            this.InternalExists(this.core.storageName);

        private bool InternalExists(string name)
        {
            if (this.core.safeIStorage != null)
            {
                return true;
            }
            if (this.parentStorage == null)
            {
                return true;
            }
            if (!this.parentStorage.Exists)
            {
                return false;
            }
            return this.parentStorage.CanOpenStorage(name);
        }

        internal void InvalidateEnumerators()
        {
            InvalidateEnumerators(this.core);
        }

        private static void InvalidateEnumerators(StorageInfoCore invalidateCore)
        {
            foreach (object obj2 in invalidateCore.validEnumerators.Values)
            {
                ((ArrayList) obj2).Clear();
            }
            invalidateCore.validEnumerators.Clear();
        }

        internal static void RecursiveStorageInfoCoreRelease(StorageInfoCore startCore)
        {
            if (startCore.safeIStorage != null)
            {
                try
                {
                    foreach (object obj2 in startCore.elementInfoCores.Values)
                    {
                        if (obj2 is StorageInfoCore)
                        {
                            RecursiveStorageInfoCoreRelease((StorageInfoCore) obj2);
                        }
                        else if (obj2 is StreamInfoCore)
                        {
                            StreamInfoCore core = (StreamInfoCore) obj2;
                            try
                            {
                                if (core.exposedStream != null)
                                {
                                    ((Stream) core.exposedStream).Close();
                                }
                                core.exposedStream = null;
                            }
                            finally
                            {
                                if (core.safeIStream != null)
                                {
                                    ((IDisposable) core.safeIStream).Dispose();
                                    core.safeIStream = null;
                                }
                                ((StreamInfoCore) obj2).streamName = null;
                            }
                        }
                    }
                    InvalidateEnumerators(startCore);
                }
                finally
                {
                    if (startCore.safeIStorage != null)
                    {
                        ((IDisposable) startCore.safeIStorage).Dispose();
                        startCore.safeIStorage = null;
                    }
                    startCore.storageName = null;
                }
            }
        }

        internal void RemoveSubStorageEntryFromDataSpaceMap(StorageInfo storageInfo)
        {
            foreach (StorageInfo info in storageInfo.GetSubStorages())
            {
                this.RemoveSubStorageEntryFromDataSpaceMap(info);
            }
            StreamInfo[] streams = storageInfo.GetStreams();
            DataSpaceManager dataSpaceManager = this.Root.GetDataSpaceManager();
            foreach (StreamInfo info2 in streams)
            {
                dataSpaceManager.RemoveContainerFromDataSpaceMap(new CompoundFileStreamReference(storageInfo.FullNameInternal, info2.Name));
            }
        }

        internal bool StorageIsEmpty()
        {
            uint num;
            IEnumSTATSTG ppEnum = null;
            System.Runtime.InteropServices.ComTypes.STATSTG statstg;
            this.core.safeIStorage.EnumElements(0, IntPtr.Zero, 0, out ppEnum);
            ppEnum.Reset();
            ppEnum.Next(1, out statstg, out num);
            ((IDisposable) ppEnum).Dispose();
            ppEnum = null;
            return (0 == num);
        }

        public bool StreamExists(string name)
        {
            this.CheckDisposedStatus();
            StreamInfo info = new StreamInfo(this, name);
            return info.InternalExists();
        }

        public bool SubStorageExists(string name)
        {
            StorageInfo info = new StorageInfo(this, name);
            return info.InternalExists(name);
        }

        private void VerifyExists()
        {
            if (!this.InternalExists())
            {
                throw new DirectoryNotFoundException(System.Windows.SR.Get("CanNotOnNonExistStorage"));
            }
        }

        internal bool Exists
        {
            get
            {
                this.CheckDisposedStatus();
                return this.InternalExists();
            }
        }

        internal string FullNameInternal
        {
            get
            {
                this.CheckDisposedStatus();
                return ContainerUtilities.ConvertStringArrayPathToBackSlashPath(this.BuildFullNameInternalFromParentNameInternal());
            }
        }

        public string Name
        {
            get
            {
                this.CheckDisposedStatus();
                return this.core.storageName;
            }
        }

        internal StorageRoot Root
        {
            get
            {
                this.CheckDisposedStatus();
                if (this.rootStorage == null)
                {
                    return (StorageRoot) this;
                }
                return this.rootStorage;
            }
        }

        internal IStorage SafeIStorage
        {
            get
            {
                this.VerifyExists();
                return this.core.safeIStorage;
            }
        }

        internal bool StorageDisposed
        {
            get
            {
                if (this.parentStorage != null)
                {
                    return ((this.core.storageName == null) || this.parentStorage.StorageDisposed);
                }
                if (this is StorageRoot)
                {
                    return ((StorageRoot) this).RootDisposed;
                }
                return this.rootStorage.RootDisposed;
            }
        }

        private enum EnumeratorTypes
        {
            Everything,
            OnlyStorages,
            OnlyStreams
        }
    }
}

