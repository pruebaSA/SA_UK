namespace PdfSharp.Pdf.Content
{
    using PdfSharp.Pdf.Content.Objects;
    using PdfSharp.Pdf.Internal;
    using System;
    using System.Collections.Generic;
    using System.IO;

    internal class ContentWriter
    {
        protected int indent = 2;
        private CharCat lastCat;
        private List<CObject> stack = new List<CObject>();
        private System.IO.Stream stream;
        protected int writeIndent;

        public ContentWriter(System.IO.Stream contentStream)
        {
            this.stream = contentStream;
        }

        public void Close()
        {
            this.Close(true);
        }

        public void Close(bool closeUnderlyingStream)
        {
            if ((this.stream != null) && closeUnderlyingStream)
            {
                this.stream.Close();
                this.stream = null;
            }
        }

        private void DecreaseIndent()
        {
            this.writeIndent -= this.indent;
        }

        private CharCat GetCategory(char ch) => 
            CharCat.Character;

        private void IncreaseIndent()
        {
            this.writeIndent += this.indent;
        }

        public void NewLine()
        {
            if (this.lastCat != CharCat.NewLine)
            {
                this.WriteRaw('\n');
            }
        }

        public void Write(bool value)
        {
        }

        private void WriteIndent()
        {
            this.WriteRaw(this.IndentBlanks);
        }

        public void WriteLineRaw(string rawString)
        {
            if ((rawString != null) && (rawString.Length != 0))
            {
                byte[] bytes = PdfEncoders.RawEncoding.GetBytes(rawString);
                this.stream.Write(bytes, 0, bytes.Length);
                this.stream.Write(new byte[] { 10 }, 0, 1);
                this.lastCat = this.GetCategory((char) bytes[bytes.Length - 1]);
            }
        }

        public void WriteRaw(char ch)
        {
            this.stream.WriteByte((byte) ch);
            this.lastCat = this.GetCategory(ch);
        }

        public void WriteRaw(string rawString)
        {
            if ((rawString != null) && (rawString.Length != 0))
            {
                byte[] bytes = PdfEncoders.RawEncoding.GetBytes(rawString);
                this.stream.Write(bytes, 0, bytes.Length);
                this.lastCat = this.GetCategory((char) bytes[bytes.Length - 1]);
            }
        }

        private void WriteSeparator(CharCat cat)
        {
            this.WriteSeparator(cat, '\0');
        }

        private void WriteSeparator(CharCat cat, char ch)
        {
            CharCat lastCat = this.lastCat;
        }

        internal int Indent
        {
            get => 
                this.indent;
            set
            {
                this.indent = value;
            }
        }

        private string IndentBlanks =>
            new string(' ', this.writeIndent);

        public int Position =>
            ((int) this.stream.Position);

        internal System.IO.Stream Stream =>
            this.stream;

        private enum CharCat
        {
            NewLine,
            Character,
            Delimiter
        }

        private class StackItem
        {
            public CObject Object;

            public StackItem(CObject value)
            {
                this.Object = value;
            }
        }
    }
}

