namespace System.Xml.Xsl.XPath
{
    using System;
    using System.Xml;
    using System.Xml.Xsl;

    internal sealed class XPathScanner
    {
        private bool canBeFunction;
        private char curChar;
        private int curIndex;
        private LexKind kind;
        private int lexStart;
        private string name;
        private double numberValue;
        private string prefix;
        private int prevLexEnd;
        private string stringValue;
        private XmlCharType xmlCharType;
        private string xpathExpr;

        public XPathScanner(string xpathExpr) : this(xpathExpr, 0)
        {
        }

        public XPathScanner(string xpathExpr, int startFrom)
        {
            this.numberValue = double.NaN;
            this.xmlCharType = XmlCharType.Instance;
            this.xpathExpr = xpathExpr;
            this.SetSourceIndex(startFrom);
            this.NextLex();
        }

        public void CheckToken(LexKind t)
        {
            if (this.kind != t)
            {
                if (t == LexKind.Eof)
                {
                    throw this.CreateException("XPath_EofExpected", new string[] { this.RawValue });
                }
                throw this.CreateException("XPath_TokenExpected", new string[] { this.LexKindToString(t), this.RawValue });
            }
        }

        public XPathCompileException CreateException(string resId, params string[] args) => 
            new XPathCompileException(this.xpathExpr, this.lexStart, this.curIndex, resId, args);

        public bool IsKeyword(string keyword) => 
            (((this.kind == LexKind.Name) && (this.prefix.Length == 0)) && this.name.Equals(keyword));

        public string LexKindToString(LexKind t)
        {
            if (",/@.()[]{}*+-=<>!$|".IndexOf((char) ((ushort) t)) >= 0)
            {
                char ch = (char) ((ushort) t);
                return ch.ToString();
            }
            switch (t)
            {
                case LexKind.DotDot:
                    return "..";

                case LexKind.Eof:
                    return "<eof>";

                case LexKind.Ge:
                    return ">=";

                case LexKind.Le:
                    return "<=";

                case LexKind.Ne:
                    return "!=";

                case LexKind.SlashSlash:
                    return "//";

                case LexKind.Unknown:
                    return "<unknown>";

                case LexKind.Axis:
                    return "<axis>";

                case LexKind.Number:
                    return "<number literal>";

                case LexKind.Name:
                    return "<name>";

                case LexKind.String:
                    return "<string literal>";
            }
            return string.Empty;
        }

        private bool NextChar()
        {
            this.curIndex++;
            if (this.curIndex < this.xpathExpr.Length)
            {
                this.curChar = this.xpathExpr[this.curIndex];
                return true;
            }
            this.curChar = '\0';
            return false;
        }

        public bool NextLex()
        {
            this.prevLexEnd = this.curIndex;
            this.SkipSpace();
            this.lexStart = this.curIndex;
            switch (this.curChar)
            {
                case '!':
                    this.kind = LexKind.Bang;
                    this.NextChar();
                    if (this.curChar == '=')
                    {
                        this.kind = LexKind.Ne;
                        this.NextChar();
                    }
                    break;

                case '"':
                case '\'':
                    this.ScanString();
                    break;

                case '#':
                case '$':
                case '(':
                case ')':
                case '*':
                case '+':
                case ',':
                case '-':
                case '=':
                case '@':
                case '[':
                case ']':
                case '{':
                case '|':
                case '}':
                    this.kind = (LexKind) this.curChar;
                    this.NextChar();
                    break;

                case '.':
                    this.kind = LexKind.Dot;
                    this.NextChar();
                    if (this.curChar != '.')
                    {
                        if (this.xmlCharType.IsDigit(this.curChar))
                        {
                            this.ScanFraction();
                        }
                    }
                    else
                    {
                        this.kind = LexKind.DotDot;
                        this.NextChar();
                    }
                    break;

                case '/':
                    this.kind = LexKind.Slash;
                    this.NextChar();
                    if (this.curChar == '/')
                    {
                        this.kind = LexKind.SlashSlash;
                        this.NextChar();
                    }
                    break;

                case '<':
                    this.kind = LexKind.Lt;
                    this.NextChar();
                    if (this.curChar == '=')
                    {
                        this.kind = LexKind.Le;
                        this.NextChar();
                    }
                    break;

                case '>':
                    this.kind = LexKind.Gt;
                    this.NextChar();
                    if (this.curChar == '=')
                    {
                        this.kind = LexKind.Ge;
                        this.NextChar();
                    }
                    break;

                case '\0':
                    this.kind = LexKind.Eof;
                    return false;

                default:
                    if (this.xmlCharType.IsDigit(this.curChar))
                    {
                        this.ScanNumber();
                    }
                    else if (this.xmlCharType.IsStartNCNameChar(this.curChar))
                    {
                        this.kind = LexKind.Name;
                        this.name = this.ScanNCName();
                        this.prefix = string.Empty;
                        int curIndex = this.curIndex;
                        if (this.curChar == ':')
                        {
                            this.NextChar();
                            if (this.curChar == ':')
                            {
                                this.NextChar();
                                this.kind = LexKind.Axis;
                            }
                            else if (this.curChar == '*')
                            {
                                this.NextChar();
                                this.prefix = this.name;
                                this.name = "*";
                            }
                            else if (this.xmlCharType.IsStartNCNameChar(this.curChar))
                            {
                                this.prefix = this.name;
                                this.name = this.ScanNCName();
                            }
                            else
                            {
                                this.SetSourceIndex(curIndex);
                            }
                        }
                        else
                        {
                            this.SkipSpace();
                            if (this.curChar == ':')
                            {
                                this.NextChar();
                                if (this.curChar == ':')
                                {
                                    this.NextChar();
                                    this.kind = LexKind.Axis;
                                }
                                else
                                {
                                    this.SetSourceIndex(curIndex);
                                }
                            }
                        }
                        curIndex = this.curIndex;
                        this.SkipSpace();
                        this.canBeFunction = this.curChar == '(';
                        this.SetSourceIndex(curIndex);
                    }
                    else
                    {
                        this.kind = LexKind.Unknown;
                        this.NextChar();
                    }
                    break;
            }
            return true;
        }

        public void PassToken(LexKind t)
        {
            this.CheckToken(t);
            this.NextLex();
        }

        private void ScanFraction()
        {
            int startIndex = this.curIndex - 1;
            while (this.xmlCharType.IsDigit(this.curChar))
            {
                this.NextChar();
            }
            this.kind = LexKind.Number;
            this.numberValue = XPathConvert.StringToDouble(this.xpathExpr.Substring(startIndex, this.curIndex - startIndex));
        }

        private string ScanNCName()
        {
            int curIndex = this.curIndex;
            while (this.xmlCharType.IsNCNameChar(this.curChar))
            {
                this.NextChar();
            }
            return this.xpathExpr.Substring(curIndex, this.curIndex - curIndex);
        }

        private void ScanNumber()
        {
            int curIndex = this.curIndex;
            while (this.xmlCharType.IsDigit(this.curChar))
            {
                this.NextChar();
            }
            if (this.curChar == '.')
            {
                this.NextChar();
                while (this.xmlCharType.IsDigit(this.curChar))
                {
                    this.NextChar();
                }
            }
            if ((this.curChar & '￟') == 0x45)
            {
                this.NextChar();
                if ((this.curChar == '+') || (this.curChar == '-'))
                {
                    this.NextChar();
                }
                while (this.xmlCharType.IsDigit(this.curChar))
                {
                    this.NextChar();
                }
                throw this.CreateException("XPath_ScientificNotation", new string[0]);
            }
            this.kind = LexKind.Number;
            this.numberValue = XPathConvert.StringToDouble(this.xpathExpr.Substring(curIndex, this.curIndex - curIndex));
        }

        private void ScanString()
        {
            char curChar = this.curChar;
            int startIndex = this.curIndex + 1;
            do
            {
                if (!this.NextChar())
                {
                    throw this.CreateException("XPath_UnclosedString", new string[0]);
                }
            }
            while (this.curChar != curChar);
            this.kind = LexKind.String;
            this.stringValue = this.xpathExpr.Substring(startIndex, this.curIndex - startIndex);
            this.NextChar();
        }

        private void SetSourceIndex(int index)
        {
            this.curIndex = index - 1;
            this.NextChar();
        }

        private void SkipSpace()
        {
            while (this.xmlCharType.IsWhiteSpace(this.curChar) && this.NextChar())
            {
            }
        }

        public bool CanBeFunction =>
            this.canBeFunction;

        public LexKind Kind =>
            this.kind;

        public int LexSize =>
            (this.curIndex - this.lexStart);

        public int LexStart =>
            this.lexStart;

        public string Name =>
            this.name;

        public double NumberValue =>
            this.numberValue;

        public string Prefix =>
            this.prefix;

        public int PrevLexEnd =>
            this.prevLexEnd;

        public string RawValue
        {
            get
            {
                if (this.kind == LexKind.Eof)
                {
                    return this.LexKindToString(this.kind);
                }
                return this.xpathExpr.Substring(this.lexStart, this.curIndex - this.lexStart);
            }
        }

        public string Source =>
            this.xpathExpr;

        public string StringValue =>
            this.stringValue;
    }
}

