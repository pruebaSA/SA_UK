namespace PdfSharp.Pdf.IO
{
    using PdfSharp.Pdf.Internal;
    using System;
    using System.Globalization;
    using System.IO;
    using System.Text;

    internal class Lexer
    {
        private char currChar;
        private int idxChar;
        private char nextChar;
        private Stream pdf;
        private int pdfLength;
        private PdfSharp.Pdf.IO.Symbol symbol;
        private StringBuilder token;

        public Lexer(Stream pdfInputStream)
        {
            this.pdf = pdfInputStream;
            this.pdfLength = (int) this.pdf.Length;
            this.idxChar = 0;
            this.Position = 0;
        }

        internal char AppendAndScanNextChar()
        {
            this.token.Append(this.currChar);
            return this.ScanNextChar();
        }

        private void ClearToken()
        {
            this.token.Length = 0;
        }

        private void Initialize()
        {
            this.currChar = (char) this.pdf.ReadByte();
            this.nextChar = (char) this.pdf.ReadByte();
            this.token = new StringBuilder();
        }

        internal static bool IsDelimiter(char ch)
        {
            switch (ch)
            {
                case '%':
                case '(':
                case ')':
                case '/':
                case '<':
                case '>':
                case '[':
                case ']':
                case '{':
                case '}':
                    return true;
            }
            return false;
        }

        internal static bool IsWhiteSpace(char ch)
        {
            switch (ch)
            {
                case '\t':
                case '\n':
                case '\f':
                case '\r':
                case ' ':
                case '\0':
                    return true;
            }
            return false;
        }

        public char MoveToNonWhiteSpace()
        {
            while (this.currChar != 0xffff)
            {
                switch (this.currChar)
                {
                    case '\t':
                    case '\n':
                    case '\f':
                    case '\r':
                    case ' ':
                    case '\0':
                    {
                        this.ScanNextChar();
                        continue;
                    }
                }
                return this.currChar;
            }
            return this.currChar;
        }

        public string ReadRawString(int position, int length)
        {
            this.pdf.Position = position;
            byte[] buffer = new byte[length];
            this.pdf.Read(buffer, 0, length);
            return PdfEncoders.RawEncoding.GetString(buffer, 0, buffer.Length);
        }

        public byte[] ReadStream(int length)
        {
            int num = 0;
            if (this.currChar == '\r')
            {
                if (this.nextChar == '\n')
                {
                    num = this.idxChar + 2;
                }
                else
                {
                    num = this.idxChar + 1;
                }
            }
            else
            {
                num = this.idxChar + 1;
            }
            this.pdf.Position = num;
            byte[] buffer = new byte[length];
            this.pdf.Read(buffer, 0, length);
            this.Position = num + length;
            return buffer;
        }

        public PdfSharp.Pdf.IO.Symbol ScanComment()
        {
            this.token = new StringBuilder();
            while (this.AppendAndScanNextChar() != '\n')
            {
            }
            if (this.token.ToString().StartsWith("%%EOF"))
            {
                return PdfSharp.Pdf.IO.Symbol.Eof;
            }
            return (this.symbol = PdfSharp.Pdf.IO.Symbol.Comment);
        }

        public PdfSharp.Pdf.IO.Symbol ScanHexadecimalString()
        {
            this.token = new StringBuilder();
            char[] chArray = new char[2];
            this.ScanNextChar();
            while (true)
            {
                this.MoveToNonWhiteSpace();
                if (this.currChar == '>')
                {
                    this.ScanNextChar();
                    break;
                }
                if (char.IsLetterOrDigit(this.currChar))
                {
                    chArray[0] = char.ToUpper(this.currChar);
                    chArray[1] = char.ToUpper(this.nextChar);
                    int num = int.Parse(new string(chArray), NumberStyles.AllowHexSpecifier);
                    this.token.Append(Convert.ToChar(num));
                    this.ScanNextChar();
                    this.ScanNextChar();
                }
            }
            string str = this.token.ToString();
            int length = str.Length;
            if (((length > 2) && (str[0] == '\x00fe')) && (str[1] == '\x00ff'))
            {
                this.token.Length = 0;
                for (int i = 2; i < length; i += 2)
                {
                    this.token.Append((char) ((str[i] * 'Ā') + str[i + 1]));
                }
            }
            return (this.symbol = PdfSharp.Pdf.IO.Symbol.HexString);
        }

        public PdfSharp.Pdf.IO.Symbol ScanKeyword()
        {
            this.token = new StringBuilder();
            char currChar = this.currChar;
            while (true)
            {
                if (!char.IsLetter(currChar))
                {
                    break;
                }
                this.token.Append(currChar);
                currChar = this.ScanNextChar();
            }
            switch (this.token.ToString())
            {
                case "obj":
                    return (this.symbol = PdfSharp.Pdf.IO.Symbol.Obj);

                case "endobj":
                    return (this.symbol = PdfSharp.Pdf.IO.Symbol.EndObj);

                case "null":
                    return (this.symbol = PdfSharp.Pdf.IO.Symbol.Null);

                case "true":
                case "false":
                    return (this.symbol = PdfSharp.Pdf.IO.Symbol.Boolean);

                case "R":
                    return (this.symbol = PdfSharp.Pdf.IO.Symbol.R);

                case "stream":
                    return (this.symbol = PdfSharp.Pdf.IO.Symbol.BeginStream);

                case "endstream":
                    return (this.symbol = PdfSharp.Pdf.IO.Symbol.EndStream);

                case "xref":
                    return (this.symbol = PdfSharp.Pdf.IO.Symbol.XRef);

                case "trailer":
                    return (this.symbol = PdfSharp.Pdf.IO.Symbol.Trailer);

                case "startxref":
                    return (this.symbol = PdfSharp.Pdf.IO.Symbol.StartXRef);
            }
            return (this.symbol = PdfSharp.Pdf.IO.Symbol.Keyword);
        }

        public PdfSharp.Pdf.IO.Symbol ScanLiteralString()
        {
            this.token = new StringBuilder();
            int num = 0;
            char c = this.ScanNextChar();
            if ((c != '\x00fe') || (this.nextChar != '\x00ff'))
            {
                goto Label_016C;
            }
        Label_002F:
            switch (c)
            {
                case '(':
                    num++;
                    goto Label_0153;

                case ')':
                    if (num != 0)
                    {
                        num--;
                        goto Label_0153;
                    }
                    this.ScanNextChar();
                    return (this.symbol = PdfSharp.Pdf.IO.Symbol.String);

                case '\\':
                    c = this.ScanNextChar();
                    switch (c)
                    {
                        case '(':
                            c = '(';
                            goto Label_0153;

                        case ')':
                            c = ')';
                            goto Label_0153;

                        case '\\':
                            c = '\\';
                            goto Label_0153;

                        case '\n':
                            c = this.ScanNextChar();
                            goto Label_002F;

                        case 'r':
                            c = '\r';
                            goto Label_0153;

                        case 't':
                            c = '\t';
                            goto Label_0153;

                        case 'n':
                            c = '\n';
                            goto Label_0153;

                        case 'b':
                            c = '\b';
                            goto Label_0153;

                        case 'f':
                            c = '\f';
                            goto Label_0153;
                    }
                    break;

                default:
                    goto Label_0153;
            }
            if (char.IsDigit(c))
            {
                int num2 = c - '0';
                if (char.IsDigit(this.nextChar))
                {
                    num2 = ((num2 * 8) + this.ScanNextChar()) - 0x30;
                    if (char.IsDigit(this.nextChar))
                    {
                        num2 = ((num2 * 8) + this.ScanNextChar()) - 0x30;
                    }
                }
                c = (char) num2;
            }
        Label_0153:
            this.token.Append(c);
            c = this.ScanNextChar();
            goto Label_002F;
        Label_016C:
            switch (c)
            {
                case '(':
                    num++;
                    goto Label_0290;

                case ')':
                    if (num != 0)
                    {
                        num--;
                        goto Label_0290;
                    }
                    this.ScanNextChar();
                    return (this.symbol = PdfSharp.Pdf.IO.Symbol.String);

                case '\\':
                    c = this.ScanNextChar();
                    switch (c)
                    {
                        case '(':
                            c = '(';
                            goto Label_0290;

                        case ')':
                            c = ')';
                            goto Label_0290;

                        case '\\':
                            c = '\\';
                            goto Label_0290;

                        case '\n':
                            c = this.ScanNextChar();
                            goto Label_016C;

                        case 'r':
                            c = '\r';
                            goto Label_0290;

                        case 't':
                            c = '\t';
                            goto Label_0290;

                        case 'n':
                            c = '\n';
                            goto Label_0290;

                        case 'b':
                            c = '\b';
                            goto Label_0290;

                        case 'f':
                            c = '\f';
                            goto Label_0290;
                    }
                    break;

                default:
                    goto Label_0290;
            }
            if (char.IsDigit(c))
            {
                int num3 = c - '0';
                if (char.IsDigit(this.nextChar))
                {
                    num3 = ((num3 * 8) + this.ScanNextChar()) - 0x30;
                    if (char.IsDigit(this.nextChar))
                    {
                        num3 = ((num3 * 8) + this.ScanNextChar()) - 0x30;
                    }
                }
                c = (char) num3;
            }
        Label_0290:
            this.token.Append(c);
            c = this.ScanNextChar();
            goto Label_016C;
        }

        public PdfSharp.Pdf.IO.Symbol ScanName()
        {
            this.token = new StringBuilder();
            while (true)
            {
                char ch = this.AppendAndScanNextChar();
                if (IsWhiteSpace(ch) || IsDelimiter(ch))
                {
                    return (this.symbol = PdfSharp.Pdf.IO.Symbol.Name);
                }
                if (ch == '#')
                {
                    this.ScanNextChar();
                    char[] chArray = new char[] { this.currChar, this.nextChar };
                    this.ScanNextChar();
                    ch = (char) int.Parse(new string(chArray), NumberStyles.AllowHexSpecifier);
                    this.currChar = ch;
                }
            }
        }

        internal char ScanNextChar()
        {
            if (this.pdfLength <= this.idxChar)
            {
                this.currChar = 0xffff;
                this.nextChar = 0xffff;
            }
            else
            {
                this.currChar = this.nextChar;
                this.nextChar = (char) this.pdf.ReadByte();
                this.idxChar++;
                if (this.currChar == '\r')
                {
                    if (this.nextChar == '\n')
                    {
                        this.currChar = this.nextChar;
                        this.nextChar = (char) this.pdf.ReadByte();
                        this.idxChar++;
                    }
                    else
                    {
                        this.currChar = '\n';
                    }
                }
            }
            return this.currChar;
        }

        public PdfSharp.Pdf.IO.Symbol ScanNextToken()
        {
        Label_0000:
            this.token = new StringBuilder();
            char c = this.MoveToNonWhiteSpace();
            switch (c)
            {
                case '%':
                    this.ScanComment();
                    goto Label_0000;

                case '(':
                    return (this.symbol = this.ScanLiteralString());

                case '+':
                case '-':
                    return (this.symbol = this.ScanNumber());

                case '.':
                    return (this.symbol = this.ScanNumber());

                case '/':
                    return (this.symbol = this.ScanName());

                case '<':
                    if (this.nextChar != '<')
                    {
                        return (this.symbol = this.ScanHexadecimalString());
                    }
                    this.ScanNextChar();
                    this.ScanNextChar();
                    return (this.symbol = PdfSharp.Pdf.IO.Symbol.BeginDictionary);

                case '>':
                    if (this.nextChar != '>')
                    {
                        break;
                    }
                    this.ScanNextChar();
                    this.ScanNextChar();
                    return (this.symbol = PdfSharp.Pdf.IO.Symbol.EndDictionary);

                case '[':
                    this.ScanNextChar();
                    return (this.symbol = PdfSharp.Pdf.IO.Symbol.BeginArray);

                case ']':
                    this.ScanNextChar();
                    return (this.symbol = PdfSharp.Pdf.IO.Symbol.EndArray);
            }
            if (char.IsDigit(c))
            {
                return (this.symbol = this.ScanNumber());
            }
            if (char.IsLetter(c))
            {
                return (this.symbol = this.ScanKeyword());
            }
            if (c == 0xffff)
            {
                return (this.symbol = PdfSharp.Pdf.IO.Symbol.Eof);
            }
            return (this.symbol = PdfSharp.Pdf.IO.Symbol.None);
        }

        public PdfSharp.Pdf.IO.Symbol ScanNumber()
        {
            bool flag = false;
            bool flag2 = false;
            flag.GetType();
            flag2.GetType();
            this.token = new StringBuilder();
            char currChar = this.currChar;
            switch (currChar)
            {
                case '+':
                case '-':
                    flag2 = true;
                    this.token.Append(currChar);
                    currChar = this.ScanNextChar();
                    break;
            }
            while (true)
            {
                if (char.IsDigit(currChar))
                {
                    this.token.Append(currChar);
                }
                else
                {
                    if (currChar != '.')
                    {
                        break;
                    }
                    if (flag)
                    {
                        throw new PdfReaderException("More than one period in number.");
                    }
                    flag = true;
                    this.token.Append(currChar);
                }
                currChar = this.ScanNextChar();
            }
            if (flag)
            {
                return PdfSharp.Pdf.IO.Symbol.Real;
            }
            long num = long.Parse(this.token.ToString(), CultureInfo.InvariantCulture);
            if ((num >= -2147483648L) && (num <= 0x7fffffffL))
            {
                return PdfSharp.Pdf.IO.Symbol.Integer;
            }
            if ((num <= 0L) || (num > 0xffffffffL))
            {
                throw new PdfReaderException("Number exceeds integer range.");
            }
            return PdfSharp.Pdf.IO.Symbol.UInteger;
        }

        public int PdfLength =>
            this.pdfLength;

        public int Position
        {
            get => 
                this.idxChar;
            set
            {
                this.idxChar = value;
                this.pdf.Position = value;
                this.Initialize();
            }
        }

        public PdfSharp.Pdf.IO.Symbol Symbol
        {
            get => 
                this.symbol;
            set
            {
                this.symbol = value;
            }
        }

        internal string Token =>
            this.token.ToString();

        internal bool TokenToBoolean =>
            (this.token.ToString()[0] == 't');

        internal int TokenToInteger =>
            int.Parse(this.token.ToString(), CultureInfo.InvariantCulture);

        internal double TokenToReal =>
            double.Parse(this.token.ToString(), CultureInfo.InvariantCulture);

        internal uint TokenToUInteger =>
            uint.Parse(this.token.ToString(), CultureInfo.InvariantCulture);
    }
}

