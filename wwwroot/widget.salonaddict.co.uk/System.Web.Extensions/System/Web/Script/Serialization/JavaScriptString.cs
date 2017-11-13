﻿namespace System.Web.Script.Serialization
{
    using System;
    using System.Globalization;
    using System.Text;

    internal class JavaScriptString
    {
        private int _index;
        private string _s;

        internal JavaScriptString(string s)
        {
            this._s = s;
        }

        private static void AppendCharAsUnicode(StringBuilder builder, char c)
        {
            builder.Append(@"\u");
            builder.AppendFormat(CultureInfo.InvariantCulture, "{0:x4}", new object[] { (int) c });
        }

        private static bool CharRequiresJavaScriptEncoding(char c)
        {
            if ((((c >= ' ') && (c != '"')) && ((c != '\\') && (c != '\''))) && (((c != '<') && (c != '>')) && ((c != '\x0085') && (c != '\u2028'))))
            {
                return (c == '\u2029');
            }
            return true;
        }

        internal string GetDebugString(string message) => 
            string.Concat(new object[] { message, " (", this._index, "): ", this._s });

        internal char? GetNextNonEmptyChar()
        {
            while (this._s.Length > this._index)
            {
                char c = this._s[this._index++];
                if (!char.IsWhiteSpace(c))
                {
                    return new char?(c);
                }
            }
            return null;
        }

        internal char? MoveNext()
        {
            if (this._s.Length > this._index)
            {
                return new char?(this._s[this._index++]);
            }
            return null;
        }

        internal string MoveNext(int count)
        {
            if (this._s.Length >= (this._index + count))
            {
                string str = this._s.Substring(this._index, count);
                this._index += count;
                return str;
            }
            return null;
        }

        internal void MovePrev()
        {
            if (this._index > 0)
            {
                this._index--;
            }
        }

        internal void MovePrev(int count)
        {
            while ((this._index > 0) && (count > 0))
            {
                this._index--;
                count--;
            }
        }

        internal static string QuoteString(string value)
        {
            StringBuilder builder = null;
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }
            int startIndex = 0;
            int count = 0;
            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];
                if (CharRequiresJavaScriptEncoding(c))
                {
                    if (builder == null)
                    {
                        builder = new StringBuilder(value.Length + 5);
                    }
                    if (count > 0)
                    {
                        builder.Append(value, startIndex, count);
                    }
                    startIndex = i + 1;
                    count = 0;
                }
                switch (c)
                {
                    case '\b':
                    {
                        builder.Append(@"\b");
                        continue;
                    }
                    case '\t':
                    {
                        builder.Append(@"\t");
                        continue;
                    }
                    case '\n':
                    {
                        builder.Append(@"\n");
                        continue;
                    }
                    case '\f':
                    {
                        builder.Append(@"\f");
                        continue;
                    }
                    case '\r':
                    {
                        builder.Append(@"\r");
                        continue;
                    }
                    case '"':
                    {
                        builder.Append("\\\"");
                        continue;
                    }
                    case '\\':
                    {
                        builder.Append(@"\\");
                        continue;
                    }
                }
                if (CharRequiresJavaScriptEncoding(c))
                {
                    AppendCharAsUnicode(builder, c);
                }
                else
                {
                    count++;
                }
            }
            if (builder == null)
            {
                return value;
            }
            if (count > 0)
            {
                builder.Append(value, startIndex, count);
            }
            return builder.ToString();
        }

        internal static string QuoteString(string value, bool addQuotes)
        {
            string str = QuoteString(value);
            if (addQuotes)
            {
                str = "\"" + str + "\"";
            }
            return str;
        }

        public override string ToString()
        {
            if (this._s.Length > this._index)
            {
                return this._s.Substring(this._index);
            }
            return string.Empty;
        }
    }
}

