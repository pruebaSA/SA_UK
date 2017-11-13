namespace System.Net.Mail
{
    using System;

    internal class SmtpReplyReader
    {
        private SmtpReplyReaderFactory reader;

        internal SmtpReplyReader(SmtpReplyReaderFactory reader)
        {
            this.reader = reader;
        }

        internal IAsyncResult BeginReadLine(AsyncCallback callback, object state) => 
            this.reader.BeginReadLine(this, callback, state);

        internal IAsyncResult BeginReadLines(AsyncCallback callback, object state) => 
            this.reader.BeginReadLines(this, callback, state);

        public void Close()
        {
            this.reader.Close(this);
        }

        internal LineInfo EndReadLine(IAsyncResult result) => 
            this.reader.EndReadLine(result);

        internal LineInfo[] EndReadLines(IAsyncResult result) => 
            this.reader.EndReadLines(result);

        internal LineInfo ReadLine() => 
            this.reader.ReadLine(this);

        internal LineInfo[] ReadLines() => 
            this.reader.ReadLines(this);
    }
}

