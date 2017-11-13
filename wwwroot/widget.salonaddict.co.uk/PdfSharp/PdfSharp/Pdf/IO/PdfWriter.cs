namespace PdfSharp.Pdf.IO
{
    using PdfSharp.Pdf;
    using PdfSharp.Pdf.Advanced;
    using PdfSharp.Pdf.Internal;
    using PdfSharp.Pdf.Security;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Text;

    internal class PdfWriter
    {
        private int commentPosition;
        protected int indent = 2;
        private CharCat lastCat;
        private PdfWriterLayout layout;
        private PdfWriterOptions options;
        private PdfStandardSecurityHandler securityHandler;
        private List<StackItem> stack = new List<StackItem>();
        private System.IO.Stream stream;
        protected int writeIndent;

        public PdfWriter(System.IO.Stream pdfStream, PdfStandardSecurityHandler securityHandler)
        {
            this.stream = pdfStream;
            this.securityHandler = securityHandler;
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

        private CharCat GetCategory(char ch)
        {
            if (Lexer.IsDelimiter(ch))
            {
                return CharCat.Delimiter;
            }
            if (ch == '\n')
            {
                return CharCat.NewLine;
            }
            return CharCat.Character;
        }

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

        public void Write(PdfReference iref)
        {
            this.WriteSeparator(CharCat.Character);
            this.WriteRaw(iref.ToString());
            this.lastCat = CharCat.Character;
        }

        public void Write(PdfBoolean value)
        {
            this.WriteSeparator(CharCat.Character);
            this.WriteRaw(value.Value ? "true" : "false");
            this.lastCat = CharCat.Character;
        }

        public void Write(PdfInteger value)
        {
            this.WriteSeparator(CharCat.Character);
            this.lastCat = CharCat.Character;
            this.WriteRaw(value.Value.ToString(CultureInfo.InvariantCulture));
        }

        public void Write(PdfLiteral value)
        {
            this.WriteSeparator(CharCat.Character);
            this.WriteRaw(value.Value);
            this.lastCat = CharCat.Character;
        }

        public void Write(PdfName value)
        {
            this.WriteSeparator(CharCat.Delimiter, '/');
            string str = value.Value;
            StringBuilder builder = new StringBuilder("/");
            for (int i = 1; i < str.Length; i++)
            {
                char ch = str[i];
                if (ch > ' ')
                {
                    switch (ch)
                    {
                        case '#':
                        case '%':
                        case '(':
                        case ')':
                        case '/':
                        case '<':
                        case '>':
                            goto Label_0081;
                    }
                    builder.Append(str[i]);
                    continue;
                }
            Label_0081:
                builder.AppendFormat("#{0:X2}", (int) str[i]);
            }
            this.WriteRaw(builder.ToString());
            this.lastCat = CharCat.Character;
        }

        public void Write(PdfReal value)
        {
            this.WriteSeparator(CharCat.Character);
            this.WriteRaw(value.Value.ToString("0.###", CultureInfo.InvariantCulture));
            this.lastCat = CharCat.Character;
        }

        public void Write(PdfRectangle rect)
        {
            this.WriteSeparator(CharCat.Delimiter, '/');
            this.WriteRaw(PdfEncoders.Format("[{0:0.###} {1:0.###} {2:0.###} {3:0.###}]", new object[] { rect.X1, rect.Y1, rect.X2, rect.Y2 }));
            this.lastCat = CharCat.Delimiter;
        }

        public void Write(PdfString value)
        {
            string str;
            this.WriteSeparator(CharCat.Delimiter);
            PdfStringEncoding encoding = ((PdfStringEncoding) value.Flags) & ((PdfStringEncoding) 15);
            if ((value.Flags & PdfStringFlags.HexLiteral) == PdfStringFlags.RawEncoding)
            {
                str = PdfEncoders.ToStringLiteral(value.Value, encoding, this.SecurityHandler);
            }
            else
            {
                str = PdfEncoders.ToHexStringLiteral(value.Value, encoding, this.SecurityHandler);
            }
            this.WriteRaw(str);
            this.lastCat = CharCat.Delimiter;
        }

        public void Write(PdfUInteger value)
        {
            this.WriteSeparator(CharCat.Character);
            this.lastCat = CharCat.Character;
            this.WriteRaw(value.Value.ToString(CultureInfo.InvariantCulture));
        }

        public void Write(bool value)
        {
            this.WriteSeparator(CharCat.Character);
            this.WriteRaw(value ? bool.TrueString : bool.FalseString);
            this.lastCat = CharCat.Character;
        }

        public void Write(double value)
        {
            this.WriteSeparator(CharCat.Character);
            this.WriteRaw(value.ToString("0.###", CultureInfo.InvariantCulture));
            this.lastCat = CharCat.Character;
        }

        public void Write(int value)
        {
            this.WriteSeparator(CharCat.Character);
            this.WriteRaw(value.ToString(CultureInfo.InvariantCulture));
            this.lastCat = CharCat.Character;
        }

        public void Write(uint value)
        {
            this.WriteSeparator(CharCat.Character);
            this.WriteRaw(value.ToString(CultureInfo.InvariantCulture));
            this.lastCat = CharCat.Character;
        }

        public void Write(byte[] bytes)
        {
            if ((bytes != null) && (bytes.Length != 0))
            {
                this.stream.Write(bytes, 0, bytes.Length);
                this.lastCat = this.GetCategory((char) bytes[bytes.Length - 1]);
            }
        }

        public void WriteBeginObject(PdfObject value)
        {
            bool isIndirect = value.IsIndirect;
            if (isIndirect)
            {
                this.WriteObjectAddress(value);
                if (this.securityHandler != null)
                {
                    this.securityHandler.SetHashKey(value.ObjectID);
                }
            }
            this.stack.Add(new StackItem(value));
            if (isIndirect)
            {
                if (value is PdfArray)
                {
                    this.WriteRaw("[\n");
                }
                else if (value is PdfDictionary)
                {
                    this.WriteRaw("<<\n");
                }
                this.lastCat = CharCat.NewLine;
            }
            else if (value is PdfArray)
            {
                this.WriteSeparator(CharCat.Delimiter);
                this.WriteRaw('[');
                this.lastCat = CharCat.Delimiter;
            }
            else if (value is PdfDictionary)
            {
                this.NewLine();
                this.WriteSeparator(CharCat.Delimiter);
                this.WriteRaw("<<\n");
                this.lastCat = CharCat.NewLine;
            }
            if (this.layout == PdfWriterLayout.Verbose)
            {
                this.IncreaseIndent();
            }
        }

        public void WriteDocString(string text)
        {
            this.WriteSeparator(CharCat.Delimiter);
            byte[] bytes = PdfEncoders.FormatStringLiteral(PdfEncoders.DocEncoding.GetBytes(text), false, false, false, this.securityHandler);
            this.Write(bytes);
            this.lastCat = CharCat.Delimiter;
        }

        public void WriteDocString(string text, bool unicode)
        {
            byte[] bytes;
            this.WriteSeparator(CharCat.Delimiter);
            if (!unicode)
            {
                bytes = PdfEncoders.DocEncoding.GetBytes(text);
            }
            else
            {
                bytes = PdfEncoders.UnicodeEncoding.GetBytes(text);
            }
            bytes = PdfEncoders.FormatStringLiteral(bytes, unicode, true, false, this.securityHandler);
            this.Write(bytes);
            this.lastCat = CharCat.Delimiter;
        }

        public void WriteDocStringHex(string text)
        {
            this.WriteSeparator(CharCat.Delimiter);
            byte[] buffer = PdfEncoders.FormatStringLiteral(PdfEncoders.DocEncoding.GetBytes(text), false, false, true, this.securityHandler);
            this.stream.Write(buffer, 0, buffer.Length);
            this.lastCat = CharCat.Delimiter;
        }

        public void WriteEndObject()
        {
            int count = this.stack.Count;
            StackItem item = this.stack[count - 1];
            this.stack.RemoveAt(count - 1);
            PdfObject obj2 = item.Object;
            bool isIndirect = obj2.IsIndirect;
            if (this.layout == PdfWriterLayout.Verbose)
            {
                this.DecreaseIndent();
            }
            if (obj2 is PdfArray)
            {
                if (isIndirect)
                {
                    this.WriteRaw("\n]\n");
                    this.lastCat = CharCat.NewLine;
                }
                else
                {
                    this.WriteRaw("]");
                    this.lastCat = CharCat.Delimiter;
                }
            }
            else if (obj2 is PdfDictionary)
            {
                if (isIndirect)
                {
                    if (!item.HasStream)
                    {
                        if (this.lastCat == CharCat.NewLine)
                        {
                            this.WriteRaw(">>\n");
                        }
                        else
                        {
                            this.WriteRaw(" >>\n");
                        }
                    }
                }
                else
                {
                    this.WriteSeparator(CharCat.NewLine);
                    this.WriteRaw(">>\n");
                    this.lastCat = CharCat.NewLine;
                }
            }
            if (isIndirect)
            {
                this.NewLine();
                this.WriteRaw("endobj\n");
                if (this.layout == PdfWriterLayout.Verbose)
                {
                    this.WriteRaw("%--------------------------------------------------------------------------------------------------\n");
                }
            }
        }

        public void WriteEof(PdfDocument document, int startxref)
        {
            this.WriteRaw("startxref\n");
            this.WriteRaw(startxref.ToString(CultureInfo.InvariantCulture));
            this.WriteRaw("\n%%EOF\n");
            int position = (int) this.stream.Position;
            if (this.layout == PdfWriterLayout.Verbose)
            {
                TimeSpan span = (TimeSpan) (DateTime.Now - document.creation);
                this.stream.Position = this.commentPosition;
                this.WriteRaw("Creation date: " + document.creation.ToString("G"));
                this.stream.Position = this.commentPosition + 50;
                this.WriteRaw("Creation time: " + span.TotalSeconds.ToString("0.000", CultureInfo.InvariantCulture) + " seconds");
                this.stream.Position = this.commentPosition + 100;
                this.WriteRaw("File size: " + position.ToString(CultureInfo.InvariantCulture) + " bytes");
                this.stream.Position = this.commentPosition + 150;
                this.WriteRaw("Pages: " + document.Pages.Count.ToString(CultureInfo.InvariantCulture));
                this.stream.Position = this.commentPosition + 200;
                this.WriteRaw("Objects: " + document.irefTable.objectTable.Count.ToString(CultureInfo.InvariantCulture));
            }
        }

        public void WriteFileHeader(PdfDocument document)
        {
            StringBuilder builder = new StringBuilder("%PDF-");
            int version = document.version;
            builder.Append(((version / 10)).ToString(CultureInfo.InvariantCulture) + "." + ((version % 10)).ToString(CultureInfo.InvariantCulture) + "\n%\x00d3\x00f4\x00cc\x00e1\n");
            this.WriteRaw(builder.ToString());
            if (this.layout == PdfWriterLayout.Verbose)
            {
                this.WriteRaw($"% PDFsharp Version {"1.32.2608.0"} (verbose mode)
");
                this.commentPosition = ((int) this.stream.Position) + 2;
                this.WriteRaw("%                                                \n");
                this.WriteRaw("%                                                \n");
                this.WriteRaw("%                                                \n");
                this.WriteRaw("%                                                \n");
                this.WriteRaw("%                                                \n");
                this.WriteRaw("%--------------------------------------------------------------------------------------------------\n");
            }
        }

        private void WriteIndent()
        {
            this.WriteRaw(this.IndentBlanks);
        }

        private void WriteObjectAddress(PdfObject value)
        {
            if (this.layout == PdfWriterLayout.Verbose)
            {
                this.WriteRaw($"{value.ObjectID.ObjectNumber} {value.ObjectID.GenerationNumber} obj   % {value.GetType().FullName}
");
            }
            else
            {
                this.WriteRaw($"{value.ObjectID.ObjectNumber} {value.ObjectID.GenerationNumber} obj
");
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
            switch (this.lastCat)
            {
                case CharCat.NewLine:
                    if (this.layout != PdfWriterLayout.Verbose)
                    {
                        break;
                    }
                    this.WriteIndent();
                    return;

                case CharCat.Character:
                    if (this.layout != PdfWriterLayout.Verbose)
                    {
                        if (cat == CharCat.Character)
                        {
                            this.stream.WriteByte(0x20);
                        }
                        break;
                    }
                    this.stream.WriteByte(0x20);
                    return;

                case CharCat.Delimiter:
                    break;

                default:
                    return;
            }
        }

        public void WriteStream(PdfDictionary value, bool omitStream)
        {
            StackItem item = this.stack[this.stack.Count - 1];
            item.HasStream = true;
            if (this.lastCat == CharCat.NewLine)
            {
                this.WriteRaw(">>\nstream\n");
            }
            else
            {
                this.WriteRaw(" >>\nstream\n");
            }
            if (omitStream)
            {
                this.WriteRaw("  \x00ab...stream content omitted...\x00bb\n");
            }
            else
            {
                byte[] bytes = value.Stream.Value;
                if (bytes.Length != 0)
                {
                    if (this.securityHandler != null)
                    {
                        bytes = (byte[]) bytes.Clone();
                        bytes = this.securityHandler.EncryptBytes(bytes);
                    }
                    this.Write(bytes);
                    if (this.lastCat != CharCat.NewLine)
                    {
                        this.WriteRaw('\n');
                    }
                }
            }
            this.WriteRaw("endstream\n");
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

        public PdfWriterLayout Layout
        {
            get => 
                this.layout;
            set
            {
                this.layout = value;
            }
        }

        public PdfWriterOptions Options
        {
            get => 
                this.options;
            set
            {
                this.options = value;
            }
        }

        public int Position =>
            ((int) this.stream.Position);

        internal PdfStandardSecurityHandler SecurityHandler
        {
            get => 
                this.securityHandler;
            set
            {
                this.securityHandler = value;
            }
        }

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
            public bool HasStream;
            public PdfObject Object;

            public StackItem(PdfObject value)
            {
                this.Object = value;
            }
        }
    }
}

