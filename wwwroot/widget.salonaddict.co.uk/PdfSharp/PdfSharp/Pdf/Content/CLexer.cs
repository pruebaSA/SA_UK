namespace PdfSharp.Pdf.Content
{
    using System;
    using System.Globalization;
    using System.Text;

    internal class CLexer
    {
        private int charIndex;
        private readonly byte[] content;
        private char currChar;
        private char nextChar;
        private static readonly double[] PowersOf10 = new double[] { 1.0, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, 10000000.0, 100000000.0, 1000000000.0 };
        private CSymbol symbol;
        private readonly StringBuilder token = new StringBuilder();
        private long tokenAsLong;
        private double tokenAsReal;

        public CLexer(byte[] content)
        {
            this.content = content;
            this.charIndex = 0;
        }

        internal char AppendAndScanNextChar()
        {
            this.token.Append(this.currChar);
            return this.ScanNextChar();
        }

        private void ClearToken()
        {
            this.token.Length = 0;
            this.tokenAsLong = 0L;
            this.tokenAsReal = 0.0;
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
                    return true;
            }
            return false;
        }

        internal static bool IsOperatorChar(char ch)
        {
            if (!char.IsLetter(ch))
            {
                char ch2 = ch;
                if (((ch2 != '"') && (ch2 != '\'')) && (ch2 != '*'))
                {
                    return false;
                }
            }
            return true;
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

        public CSymbol ScanComment()
        {
            char ch;
            this.ClearToken();
            while (((ch = this.AppendAndScanNextChar()) != '\n') && (ch != 0xffff))
            {
            }
            return (this.symbol = CSymbol.Comment);
        }

        public CSymbol ScanHexadecimalString()
        {
            this.ClearToken();
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
            return (this.symbol = CSymbol.HexString);
        }

        public CSymbol ScanInlineImage()
        {
            while ((this.currChar != 'E') && (this.nextChar != 'I'))
            {
                this.ScanNextChar();
            }
            return CSymbol.None;
        }

        public CSymbol ScanLiteralString()
        {
            this.ClearToken();
            int num = 0;
            char c = this.ScanNextChar();
            if ((c != '\x00fe') || (this.nextChar != '\x00ff'))
            {
                goto Label_01D1;
            }
            this.ScanNextChar();
            char ch2 = this.ScanNextChar();
            if (ch2 == ')')
            {
                this.ScanNextChar();
                return (this.symbol = CSymbol.String);
            }
            char ch3 = this.ScanNextChar();
            c = (char) ((ch2 * 'Ā') + ch3);
        Label_0063:
            switch (c)
            {
                case '(':
                    num++;
                    goto Label_018D;

                case ')':
                    if (num != 0)
                    {
                        num--;
                        goto Label_018D;
                    }
                    this.ScanNextChar();
                    return (this.symbol = CSymbol.String);

                case '\\':
                    c = this.ScanNextChar();
                    switch (c)
                    {
                        case '(':
                            c = '(';
                            goto Label_018D;

                        case ')':
                            c = ')';
                            goto Label_018D;

                        case '\\':
                            c = '\\';
                            goto Label_018D;

                        case '\n':
                            c = this.ScanNextChar();
                            goto Label_0063;

                        case 'r':
                            c = '\r';
                            goto Label_018D;

                        case 't':
                            c = '\t';
                            goto Label_018D;

                        case 'n':
                            c = '\n';
                            goto Label_018D;

                        case 'b':
                            c = '\b';
                            goto Label_018D;

                        case 'f':
                            c = '\f';
                            goto Label_018D;
                    }
                    break;

                default:
                    goto Label_018D;
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
        Label_018D:
            this.token.Append(c);
            ch2 = this.ScanNextChar();
            if (ch2 == ')')
            {
                this.ScanNextChar();
                return (this.symbol = CSymbol.String);
            }
            ch3 = this.ScanNextChar();
            c = (char) ((ch2 * 'Ā') + ch3);
            goto Label_0063;
        Label_01D1:
            switch (c)
            {
                case '(':
                    num++;
                    goto Label_02FB;

                case ')':
                    if (num != 0)
                    {
                        num--;
                        goto Label_02FB;
                    }
                    this.ScanNextChar();
                    return (this.symbol = CSymbol.String);

                case '\\':
                    c = this.ScanNextChar();
                    switch (c)
                    {
                        case '(':
                            c = '(';
                            goto Label_02FB;

                        case ')':
                            c = ')';
                            goto Label_02FB;

                        case '\\':
                            c = '\\';
                            goto Label_02FB;

                        case '\n':
                            c = this.ScanNextChar();
                            goto Label_01D1;

                        case 'r':
                            c = '\r';
                            goto Label_02FB;

                        case 't':
                            c = '\t';
                            goto Label_02FB;

                        case 'n':
                            c = '\n';
                            goto Label_02FB;

                        case 'b':
                            c = '\b';
                            goto Label_02FB;

                        case 'f':
                            c = '\f';
                            goto Label_02FB;
                    }
                    break;

                default:
                    goto Label_02FB;
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
        Label_02FB:
            this.token.Append(c);
            c = this.ScanNextChar();
            goto Label_01D1;
        }

        public CSymbol ScanName()
        {
            this.ClearToken();
            while (true)
            {
                char ch = this.AppendAndScanNextChar();
                if (IsWhiteSpace(ch) || IsDelimiter(ch))
                {
                    return (this.symbol = CSymbol.Name);
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
            if (this.ContLength <= this.charIndex)
            {
                this.currChar = 0xffff;
                this.nextChar = 0xffff;
            }
            else
            {
                this.currChar = this.nextChar;
                this.nextChar = (char) this.content[this.charIndex++];
                if (this.currChar == '\r')
                {
                    if (this.nextChar == '\n')
                    {
                        this.currChar = this.nextChar;
                        if (this.ContLength <= this.charIndex)
                        {
                            this.nextChar = 0xffff;
                        }
                        else
                        {
                            this.nextChar = (char) this.content[this.charIndex++];
                        }
                    }
                    else
                    {
                        this.currChar = '\n';
                    }
                }
            }
            return this.currChar;
        }

        public CSymbol ScanNextToken()
        {
        Label_0000:
            this.ClearToken();
            char c = this.MoveToNonWhiteSpace();
            switch (c)
            {
                case '"':
                case '\'':
                    return (this.symbol = this.ScanOperator());

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
                    return (this.symbol = this.ScanHexadecimalString());

                case '[':
                    this.ScanNextChar();
                    return (this.symbol = CSymbol.BeginArray);

                case ']':
                    this.ScanNextChar();
                    return (this.symbol = CSymbol.EndArray);
            }
            if (char.IsDigit(c))
            {
                return (this.symbol = this.ScanNumber());
            }
            if (char.IsLetter(c))
            {
                return (this.symbol = this.ScanOperator());
            }
            if (c == 0xffff)
            {
                return (this.symbol = CSymbol.Eof);
            }
            return (this.symbol = CSymbol.None);
        }

        public CSymbol ScanNumber()
        {
            long num = 0L;
            int index = 0;
            bool flag = false;
            bool flag2 = false;
            this.ClearToken();
            char currChar = this.currChar;
            switch (currChar)
            {
                case '+':
                case '-':
                    if (currChar == '-')
                    {
                        flag2 = true;
                    }
                    this.token.Append(currChar);
                    currChar = this.ScanNextChar();
                    break;
            }
            while (true)
            {
                if (char.IsDigit(currChar))
                {
                    this.token.Append(currChar);
                    if (index < 10)
                    {
                        num = ((10L * num) + ((long) currChar)) - 0x30L;
                        if (flag)
                        {
                            index++;
                        }
                    }
                }
                else
                {
                    if (currChar != '.')
                    {
                        break;
                    }
                    if (flag)
                    {
                        throw new ContentReaderException("More than one period in number.");
                    }
                    flag = true;
                    this.token.Append(currChar);
                }
                currChar = this.ScanNextChar();
            }
            if (flag2)
            {
                num = -num;
            }
            if (flag)
            {
                if (index > 0)
                {
                    this.tokenAsReal = ((double) num) / PowersOf10[index];
                }
                else
                {
                    this.tokenAsReal = num;
                    this.tokenAsLong = num;
                }
                return CSymbol.Real;
            }
            this.tokenAsLong = num;
            this.tokenAsReal = Convert.ToDouble(num);
            if ((num < -2147483648L) || (num > 0x7fffffffL))
            {
                throw new ContentReaderException("Number exceeds integer range.");
            }
            return CSymbol.Integer;
        }

        public CSymbol ScanOperator()
        {
            this.ClearToken();
            for (char ch = this.currChar; IsOperatorChar(ch); ch = this.AppendAndScanNextChar())
            {
            }
            return (this.symbol = CSymbol.Operator);
        }

        public int ContLength =>
            this.content.Length;

        public CSymbol Symbol
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

        internal int TokenToInteger =>
            ((int) this.tokenAsLong);

        internal double TokenToReal =>
            this.tokenAsReal;
    }
}

