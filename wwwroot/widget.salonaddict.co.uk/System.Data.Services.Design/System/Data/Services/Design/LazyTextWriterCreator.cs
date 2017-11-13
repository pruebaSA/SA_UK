namespace System.Data.Services.Design
{
    using System;
    using System.IO;

    internal class LazyTextWriterCreator : IDisposable
    {
        private bool _ownTextWriter;
        private string _targetFilePath;
        private TextWriter _writer;

        internal LazyTextWriterCreator(TextWriter writer)
        {
            this._writer = writer;
        }

        internal LazyTextWriterCreator(string targetFilePath)
        {
            this._ownTextWriter = true;
            this._targetFilePath = targetFilePath;
        }

        public void Dispose()
        {
            if (this._ownTextWriter && (this._writer != null))
            {
                this._writer.Dispose();
            }
            GC.SuppressFinalize(this);
        }

        internal TextWriter GetOrCreateTextWriter()
        {
            if (this._writer == null)
            {
                this._writer = new StreamWriter(this._targetFilePath);
            }
            return this._writer;
        }

        internal string TargetFilePath =>
            this._targetFilePath;
    }
}

