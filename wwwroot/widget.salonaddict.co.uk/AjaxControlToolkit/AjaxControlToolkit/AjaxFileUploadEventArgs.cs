namespace AjaxControlToolkit
{
    using System;

    public class AjaxFileUploadEventArgs : EventArgs
    {
        private byte[] _contents;
        private string _contentType = string.Empty;
        private string _fileId = string.Empty;
        private string _fileName = string.Empty;
        private int _fileSize;
        private string _postedUrl = string.Empty;
        private AjaxFileUploadState _state = AjaxFileUploadState.Unknown;
        private string _statusMessage = string.Empty;

        public AjaxFileUploadEventArgs(string fileId, AjaxFileUploadState state, string statusMessage, string fileName, int fileSize, string contentType, byte[] contents)
        {
            this._fileId = fileId;
            this._state = state;
            this._statusMessage = statusMessage;
            this._fileName = fileName;
            this._fileSize = fileSize;
            this._contentType = contentType;
            this._contents = contents;
        }

        public byte[] GetContents() => 
            this._contents;

        public string ContentType =>
            this._contentType;

        public string FileId =>
            this._fileId;

        public string FileName =>
            this._fileName;

        public int FileSize =>
            this._fileSize;

        public string PostedUrl
        {
            get => 
                this._postedUrl;
            set
            {
                this._postedUrl = value;
            }
        }

        public AjaxFileUploadState State =>
            this._state;

        public string StatusMessage =>
            this._statusMessage;
    }
}

