namespace AjaxControlToolkit
{
    using System;

    public class AsyncFileUploadEventArgs : EventArgs
    {
        private string _filename;
        private string _filesize;
        private AsyncFileUploadState _state;
        private string _statusMessage;

        public AsyncFileUploadEventArgs()
        {
            this._statusMessage = string.Empty;
            this._filename = string.Empty;
            this._filesize = string.Empty;
            this._state = AsyncFileUploadState.Unknown;
        }

        public AsyncFileUploadEventArgs(AsyncFileUploadState state, string statusMessage, string filename, string filesize)
        {
            this._statusMessage = string.Empty;
            this._filename = string.Empty;
            this._filesize = string.Empty;
            this._state = AsyncFileUploadState.Unknown;
            this._statusMessage = statusMessage;
            this._filename = filename;
            this._filesize = filesize;
            this._state = state;
        }

        public string FileName =>
            this._filename;

        public string FileSize =>
            this._filesize;

        public AsyncFileUploadState State =>
            this._state;

        public string StatusMessage =>
            this._statusMessage;
    }
}

