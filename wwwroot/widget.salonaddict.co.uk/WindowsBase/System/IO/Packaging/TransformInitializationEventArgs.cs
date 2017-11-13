namespace System.IO.Packaging
{
    using System;

    internal class TransformInitializationEventArgs : EventArgs
    {
        private IDataTransform dataInstance;
        private string dataSpaceLabel;
        private string streamPath;
        private string transformLabel;

        internal TransformInitializationEventArgs(IDataTransform instance, string dataSpaceInstanceLabel, string transformedStreamPath, string transformInstanceLabel)
        {
            this.dataInstance = instance;
            this.dataSpaceLabel = dataSpaceInstanceLabel;
            this.streamPath = transformedStreamPath;
            this.transformLabel = transformInstanceLabel;
        }

        internal string DataSpaceLabel =>
            this.dataSpaceLabel;

        internal IDataTransform DataTransform =>
            this.dataInstance;

        internal string Path =>
            this.streamPath;

        internal string TransformInstanceLabel =>
            this.transformLabel;
    }
}

