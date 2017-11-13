namespace MS.Internal
{
    using MS.Internal.WindowsBase;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Windows;

    [FriendAccessAllowed]
    internal sealed class ContentType
    {
        private static readonly char[] _allowedCharacters = new char[] { '!', '#', '$', '%', '&', '\'', '*', '+', '-', '.', '^', '_', '`', '|', '~' };
        private string _contentType;
        private static readonly ContentType _emptyContentType = new ContentType("");
        private const char _equalSeparator = '=';
        private static readonly char[] _forwardSlashSeparator = new char[] { '/' };
        private bool _isInitialized;
        private static readonly char[] _LinearWhiteSpaceChars = new char[] { ' ', '\n', '\r', '\t' };
        private string _originalString;
        private Dictionary<string, string> _parameterDictionary;
        private const string _quote = "\"";
        private const char _semicolonSeparator = ';';
        private string _subType = string.Empty;
        private string _type = string.Empty;

        internal ContentType(string contentType)
        {
            if (contentType == null)
            {
                throw new ArgumentNullException("contentType");
            }
            if (string.CompareOrdinal(contentType, string.Empty) == 0)
            {
                this._contentType = string.Empty;
            }
            else
            {
                if (IsLinearWhiteSpaceChar(contentType[0]) || IsLinearWhiteSpaceChar(contentType[contentType.Length - 1]))
                {
                    throw new ArgumentException(System.Windows.SR.Get("ContentTypeCannotHaveLeadingTrailingLWS"));
                }
                ValidateCarriageReturns(contentType);
                int index = contentType.IndexOf(';');
                if (index == -1)
                {
                    this.ParseTypeAndSubType(contentType);
                }
                else
                {
                    this.ParseTypeAndSubType(contentType.Substring(0, index));
                    this.ParseParameterAndValue(contentType.Substring(index));
                }
            }
            this._originalString = contentType;
            this._isInitialized = true;
        }

        internal bool AreTypeAndSubTypeEqual(ContentType contentType) => 
            this.AreTypeAndSubTypeEqual(contentType, false);

        internal bool AreTypeAndSubTypeEqual(ContentType contentType, bool allowParameterValuePairs)
        {
            bool flag = false;
            if (contentType == null)
            {
                return flag;
            }
            if (!allowParameterValuePairs)
            {
                if ((this._parameterDictionary != null) && (this._parameterDictionary.Count > 0))
                {
                    return false;
                }
                Dictionary<string, string>.Enumerator parameterValuePairs = contentType.ParameterValuePairs;
                parameterValuePairs.MoveNext();
                KeyValuePair<string, string> current = parameterValuePairs.Current;
                if (current.Key != null)
                {
                    return false;
                }
            }
            return ((string.Compare(this._type, contentType.TypeComponent, StringComparison.OrdinalIgnoreCase) == 0) && (string.Compare(this._subType, contentType.SubTypeComponent, StringComparison.OrdinalIgnoreCase) == 0));
        }

        private void EnsureParameterDictionary()
        {
            if (this._parameterDictionary == null)
            {
                this._parameterDictionary = new Dictionary<string, string>();
            }
        }

        private static int GetLengthOfParameterValue(string s, int startIndex)
        {
            int length = 0;
            if (s[startIndex] != '"')
            {
                int index = s.IndexOf(';', startIndex);
                if (index != -1)
                {
                    int num3 = s.IndexOfAny(_LinearWhiteSpaceChars, startIndex);
                    if ((num3 != -1) && (num3 < index))
                    {
                        length = num3;
                    }
                    else
                    {
                        length = index;
                    }
                }
                else
                {
                    length = index;
                }
                if (length == -1)
                {
                    length = s.Length;
                }
            }
            else
            {
                bool flag = false;
                length = startIndex;
                while (!flag)
                {
                    length = s.IndexOf('"', ++length);
                    if (length == -1)
                    {
                        throw new ArgumentException(System.Windows.SR.Get("InvalidParameterValue"));
                    }
                    if (s[length - 1] != '\\')
                    {
                        flag = true;
                        length++;
                    }
                }
            }
            return (length - startIndex);
        }

        private static bool IsAllowedCharacter(char character)
        {
            foreach (char ch in _allowedCharacters)
            {
                if (ch == character)
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IsAsciiLetter(char character) => 
            (((character >= 'a') && (character <= 'z')) || ((character >= 'A') && (character <= 'Z')));

        private static bool IsAsciiLetterOrDigit(char character) => 
            (IsAsciiLetter(character) || ((character >= '0') && (character <= '9')));

        private static bool IsLinearWhiteSpaceChar(char ch)
        {
            if (ch <= ' ')
            {
                foreach (char ch2 in _LinearWhiteSpaceChars)
                {
                    if (ch == ch2)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void ParseParameterAndValue(string parameterAndValue)
        {
            while (string.CompareOrdinal(parameterAndValue, string.Empty) != 0)
            {
                if (parameterAndValue[0] != ';')
                {
                    throw new ArgumentException(System.Windows.SR.Get("ExpectingSemicolon"));
                }
                if (parameterAndValue.Length == 1)
                {
                    throw new ArgumentException(System.Windows.SR.Get("ExpectingParameterValuePairs"));
                }
                parameterAndValue = parameterAndValue.Substring(1);
                parameterAndValue = parameterAndValue.TrimStart(_LinearWhiteSpaceChars);
                int index = parameterAndValue.IndexOf('=');
                if ((index <= 0) || (index == (parameterAndValue.Length - 1)))
                {
                    throw new ArgumentException(System.Windows.SR.Get("InvalidParameterValuePair"));
                }
                int startIndex = index + 1;
                int lengthOfParameterValue = GetLengthOfParameterValue(parameterAndValue, startIndex);
                this.EnsureParameterDictionary();
                this._parameterDictionary.Add(ValidateToken(parameterAndValue.Substring(0, index)), ValidateQuotedStringOrToken(parameterAndValue.Substring(startIndex, lengthOfParameterValue)));
                parameterAndValue = parameterAndValue.Substring(startIndex + lengthOfParameterValue).TrimStart(_LinearWhiteSpaceChars);
            }
        }

        private void ParseTypeAndSubType(string typeAndSubType)
        {
            typeAndSubType = typeAndSubType.TrimEnd(_LinearWhiteSpaceChars);
            string[] strArray = typeAndSubType.Split(_forwardSlashSeparator);
            if (strArray.Length != 2)
            {
                throw new ArgumentException(System.Windows.SR.Get("InvalidTypeSubType"));
            }
            this._type = ValidateToken(strArray[0]);
            this._subType = ValidateToken(strArray[1]);
        }

        public override string ToString()
        {
            if (this._contentType == null)
            {
                if (!this._isInitialized)
                {
                    return string.Empty;
                }
                StringBuilder builder = new StringBuilder(this._type);
                builder.Append(_forwardSlashSeparator[0]);
                builder.Append(this._subType);
                if ((this._parameterDictionary != null) && (this._parameterDictionary.Count > 0))
                {
                    foreach (string str in this._parameterDictionary.Keys)
                    {
                        builder.Append(_LinearWhiteSpaceChars[0]);
                        builder.Append(';');
                        builder.Append(_LinearWhiteSpaceChars[0]);
                        builder.Append(str);
                        builder.Append('=');
                        builder.Append(this._parameterDictionary[str]);
                    }
                }
                this._contentType = builder.ToString();
            }
            return this._contentType;
        }

        private static void ValidateCarriageReturns(string contentType)
        {
            for (int i = contentType.IndexOf(_LinearWhiteSpaceChars[2]); i != -1; i = contentType.IndexOf(_LinearWhiteSpaceChars[2], ++i))
            {
                if ((contentType[i - 1] != _LinearWhiteSpaceChars[1]) && (contentType[i + 1] != _LinearWhiteSpaceChars[1]))
                {
                    throw new ArgumentException(System.Windows.SR.Get("InvalidLinearWhiteSpaceCharacter"));
                }
            }
        }

        private static string ValidateQuotedStringOrToken(string parameterValue)
        {
            if (string.CompareOrdinal(parameterValue, string.Empty) == 0)
            {
                throw new ArgumentException(System.Windows.SR.Get("InvalidParameterValue"));
            }
            if (((parameterValue.Length >= 2) && parameterValue.StartsWith("\"", StringComparison.Ordinal)) && parameterValue.EndsWith("\"", StringComparison.Ordinal))
            {
                ValidateQuotedText(parameterValue.Substring(1, parameterValue.Length - 2));
                return parameterValue;
            }
            ValidateToken(parameterValue);
            return parameterValue;
        }

        private static void ValidateQuotedText(string quotedText)
        {
            for (int i = 0; i < quotedText.Length; i++)
            {
                if (!IsLinearWhiteSpaceChar(quotedText[i]))
                {
                    if ((quotedText[i] <= ' ') || (quotedText[i] >= '\x00ff'))
                    {
                        throw new ArgumentException(System.Windows.SR.Get("InvalidParameterValue"));
                    }
                    if ((quotedText[i] == '"') && ((i == 0) || (quotedText[i - 1] != '\\')))
                    {
                        throw new ArgumentException(System.Windows.SR.Get("InvalidParameterValue"));
                    }
                }
            }
        }

        private static string ValidateToken(string token)
        {
            if (string.CompareOrdinal(token, string.Empty) == 0)
            {
                throw new ArgumentException(System.Windows.SR.Get("InvalidToken"));
            }
            for (int i = 0; i < token.Length; i++)
            {
                if (!IsAsciiLetterOrDigit(token[i]) && !IsAllowedCharacter(token[i]))
                {
                    throw new ArgumentException(System.Windows.SR.Get("InvalidToken"));
                }
            }
            return token;
        }

        internal static ContentType Empty =>
            _emptyContentType;

        internal string OriginalString =>
            this._originalString;

        internal Dictionary<string, string>.Enumerator ParameterValuePairs
        {
            get
            {
                this.EnsureParameterDictionary();
                return this._parameterDictionary.GetEnumerator();
            }
        }

        internal string SubTypeComponent =>
            this._subType;

        internal string TypeComponent =>
            this._type;

        internal class StrongComparer : IEqualityComparer<ContentType>
        {
            public bool Equals(ContentType x, ContentType y) => 
                x?.AreTypeAndSubTypeEqual(y);

            public int GetHashCode(ContentType obj) => 
                obj.ToString().ToUpperInvariant().GetHashCode();
        }

        internal class WeakComparer : IEqualityComparer<ContentType>
        {
            public bool Equals(ContentType x, ContentType y) => 
                x?.AreTypeAndSubTypeEqual(y, true);

            public int GetHashCode(ContentType obj) => 
                (obj._type.ToUpperInvariant().GetHashCode() ^ obj._subType.ToUpperInvariant().GetHashCode());
        }
    }
}

