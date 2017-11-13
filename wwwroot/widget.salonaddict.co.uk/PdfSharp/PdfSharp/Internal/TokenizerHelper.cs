namespace PdfSharp.Internal
{
    using System;
    using System.Globalization;

    internal class TokenizerHelper
    {
        private char argSeparator;
        private int charIndex;
        internal int currentTokenIndex;
        internal int currentTokenLength;
        private bool foundSeparator;
        private static readonly IFormatProvider NeutralCulture = CultureInfo.InvariantCulture;
        private char quoteChar;
        private string str;
        private int strLen;

        public TokenizerHelper(string str) : this(str, NeutralCulture)
        {
        }

        public TokenizerHelper(string str, IFormatProvider formatProvider)
        {
            char numericListSeparator = GetNumericListSeparator(formatProvider);
            this.Initialize(str, '\'', numericListSeparator);
        }

        public TokenizerHelper(string str, char quoteChar, char separator)
        {
            this.Initialize(str, quoteChar, separator);
        }

        internal string GetCurrentToken()
        {
            if (this.currentTokenIndex < 0)
            {
                return null;
            }
            return this.str.Substring(this.currentTokenIndex, this.currentTokenLength);
        }

        internal static char GetNumericListSeparator(IFormatProvider provider)
        {
            char ch = ',';
            NumberFormatInfo instance = NumberFormatInfo.GetInstance(provider);
            if ((instance.NumberDecimalSeparator.Length > 0) && (ch == instance.NumberDecimalSeparator[0]))
            {
                ch = ';';
            }
            return ch;
        }

        private void Initialize(string str, char quoteChar, char separator)
        {
            this.str = str;
            this.strLen = (str == null) ? 0 : str.Length;
            this.currentTokenIndex = -1;
            this.quoteChar = quoteChar;
            this.argSeparator = separator;
            while (this.charIndex < this.strLen)
            {
                if (!char.IsWhiteSpace(this.str, this.charIndex))
                {
                    return;
                }
                this.charIndex++;
            }
        }

        internal void LastTokenRequired()
        {
            if (this.charIndex != this.strLen)
            {
                throw new InvalidOperationException("Extra data encountered");
            }
        }

        internal bool NextToken() => 
            this.NextToken(false);

        internal bool NextToken(bool allowQuotedToken) => 
            this.NextToken(allowQuotedToken, this.argSeparator);

        internal bool NextToken(bool allowQuotedToken, char separator)
        {
            this.currentTokenIndex = -1;
            this.foundSeparator = false;
            if (this.charIndex >= this.strLen)
            {
                return false;
            }
            char c = this.str[this.charIndex];
            int num = 0;
            if (allowQuotedToken && (c == this.quoteChar))
            {
                num++;
                this.charIndex++;
            }
            int charIndex = this.charIndex;
            int num3 = 0;
            while (this.charIndex < this.strLen)
            {
                c = this.str[this.charIndex];
                if (num > 0)
                {
                    if (c != this.quoteChar)
                    {
                        goto Label_00AA;
                    }
                    num--;
                    if (num != 0)
                    {
                        goto Label_00AA;
                    }
                    this.charIndex++;
                    break;
                }
                if (char.IsWhiteSpace(c) || (c == separator))
                {
                    if (c == separator)
                    {
                        this.foundSeparator = true;
                    }
                    break;
                }
            Label_00AA:
                this.charIndex++;
                num3++;
            }
            if (num > 0)
            {
                throw new InvalidOperationException("Missing end quote");
            }
            this.ScanToNextToken(separator);
            this.currentTokenIndex = charIndex;
            this.currentTokenLength = num3;
            if (this.currentTokenLength < 1)
            {
                throw new InvalidOperationException("Empty token");
            }
            return true;
        }

        public string NextTokenRequired()
        {
            if (!this.NextToken(false))
            {
                throw new InvalidOperationException("PrematureStringTermination");
            }
            return this.GetCurrentToken();
        }

        public string NextTokenRequired(bool allowQuotedToken)
        {
            if (!this.NextToken(allowQuotedToken))
            {
                throw new InvalidOperationException("PrematureStringTermination");
            }
            return this.GetCurrentToken();
        }

        internal char PeekNextCharacter()
        {
            if (this.charIndex >= this.strLen)
            {
                return 'X';
            }
            return this.str[this.charIndex];
        }

        private void ScanToNextToken(char separator)
        {
            if (this.charIndex < this.strLen)
            {
                char c = this.str[this.charIndex];
                if ((c != separator) && !char.IsWhiteSpace(c))
                {
                    throw new InvalidOperationException("ExtraDataEncountered");
                }
                int num = 0;
                while (this.charIndex < this.strLen)
                {
                    c = this.str[this.charIndex];
                    if (c == separator)
                    {
                        this.foundSeparator = true;
                        num++;
                        this.charIndex++;
                        if (num > 1)
                        {
                            throw new InvalidOperationException("EmptyToken");
                        }
                    }
                    else
                    {
                        if (!char.IsWhiteSpace(c))
                        {
                            break;
                        }
                        this.charIndex++;
                    }
                }
                if ((num > 0) && (this.charIndex >= this.strLen))
                {
                    throw new InvalidOperationException("EmptyToken");
                }
            }
        }

        internal bool FoundSeparator =>
            this.foundSeparator;
    }
}

