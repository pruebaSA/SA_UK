namespace System.Deployment.Application
{
    using System;

    internal class DownloadEventArgs : EventArgs
    {
        internal long _bytesCompleted;
        internal long _bytesTotal;
        internal object _cookie;
        internal string _fileLocalPath;
        internal Uri _fileResponseUri;
        internal int _filesCompleted;
        internal Uri _fileSourceUri;
        internal int _filesTotal;
        internal int _progress;

        public long BytesCompleted =>
            this._bytesCompleted;

        public long BytesTotal =>
            this._bytesTotal;

        internal object Cookie
        {
            get => 
                this._cookie;
            set
            {
                this._cookie = value;
            }
        }

        internal string FileLocalPath
        {
            get => 
                this._fileLocalPath;
            set
            {
                this._fileLocalPath = value;
            }
        }

        public Uri FileResponseUri =>
            this._fileResponseUri;

        public Uri FileSourceUri =>
            this._fileSourceUri;

        public int Progress =>
            this._progress;
    }
}

