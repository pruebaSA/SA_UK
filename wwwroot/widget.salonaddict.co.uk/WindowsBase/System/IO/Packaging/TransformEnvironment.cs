namespace System.IO.Packaging
{
    using System;
    using System.IO;
    using System.Windows;

    internal class TransformEnvironment
    {
        private DataSpaceManager transformHost;
        private string transformLabel;

        internal TransformEnvironment(DataSpaceManager host, string instanceLabel)
        {
            this.transformHost = host;
            this.transformLabel = instanceLabel;
        }

        internal StorageInfo GetInstanceDataStorage()
        {
            this.transformHost.CheckDisposedStatus();
            StorageInfo instanceDataStorageOf = this.transformHost.GetInstanceDataStorageOf(this.transformLabel);
            if (!instanceDataStorageOf.Exists)
            {
                instanceDataStorageOf.Create();
            }
            return instanceDataStorageOf;
        }

        internal Stream GetPrimaryInstanceData()
        {
            this.transformHost.CheckDisposedStatus();
            return this.transformHost.GetPrimaryInstanceStreamOf(this.transformLabel);
        }

        internal bool DefaultInstanceDataTransform
        {
            get => 
                false;
            set
            {
                this.transformHost.CheckDisposedStatus();
                throw new NotSupportedException(System.Windows.SR.Get("NYIDefault"));
            }
        }

        internal bool RequireInstanceDataUnaltered
        {
            get => 
                false;
            set
            {
                this.transformHost.CheckDisposedStatus();
                throw new NotSupportedException(System.Windows.SR.Get("NYIDefault"));
            }
        }

        internal bool RequireOtherInstanceData
        {
            get => 
                false;
            set
            {
                this.transformHost.CheckDisposedStatus();
                throw new NotSupportedException(System.Windows.SR.Get("NYIDefault"));
            }
        }

        internal string TransformLabel =>
            this.transformLabel;
    }
}

