namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Text.RegularExpressions;
    using System.Threading;

    internal static class SqlIdentifier
    {
        private static SqlCommandBuilder builder = new SqlCommandBuilder();
        private const string ParameterPrefix = "@";
        private const string QuotePrefix = "[";
        private const string QuoteSuffix = "]";
        private const string SchemaSeparator = ".";
        private const char SchemaSeparatorChar = '.';

        internal static IEnumerable<string> GetCompoundIdentifierParts(string s)
        {
            if (s == null)
            {
                throw Error.ArgumentNull("s");
            }
            if (s.StartsWith("@", StringComparison.Ordinal))
            {
                throw Error.ArgumentWrongValue("s");
            }
            string input = QuoteCompoundIdentifier(s);
            string pattern = @"^(?<component>\[([^\]]|\]\])*\])(\.(?<component>\[([^\]]|\]\])*\]))*$";
            Match iteratorVariable2 = Regex.Match(input, pattern);
            if (!iteratorVariable2.Success)
            {
                throw Error.ArgumentWrongValue("s");
            }
            IEnumerator enumerator = iteratorVariable2.Groups["component"].Captures.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Capture current = (Capture) enumerator.Current;
                yield return current.Value;
            }
        }

        private static bool IsQuoted(string s)
        {
            if (s == null)
            {
                throw Error.ArgumentNull("s");
            }
            if (s.Length < 2)
            {
                return false;
            }
            return (s.StartsWith("[", StringComparison.Ordinal) && s.EndsWith("]", StringComparison.Ordinal));
        }

        internal static string QuoteCompoundIdentifier(string s)
        {
            if (s == null)
            {
                throw Error.ArgumentNull("s");
            }
            if (s.StartsWith("@", StringComparison.Ordinal))
            {
                return s;
            }
            if (IsQuoted(s))
            {
                return s;
            }
            if (!s.StartsWith("[", StringComparison.Ordinal) && s.EndsWith("]", StringComparison.Ordinal))
            {
                int length = s.IndexOf('.');
                if (length < 0)
                {
                    return builder.QuoteIdentifier(s);
                }
                string str = s.Substring(0, length);
                string str2 = s.Substring(length + 1, (s.Length - length) - 1);
                if (!IsQuoted(str2))
                {
                    str2 = builder.QuoteIdentifier(str2);
                }
                return (QuoteCompoundIdentifier(str) + ('.' + str2));
            }
            if (s.StartsWith("[", StringComparison.Ordinal) && !s.EndsWith("]", StringComparison.Ordinal))
            {
                int num2 = s.LastIndexOf('.');
                if (num2 < 0)
                {
                    return builder.QuoteIdentifier(s);
                }
                string str3 = s.Substring(0, num2);
                if (!IsQuoted(str3))
                {
                    str3 = builder.QuoteIdentifier(str3);
                }
                string str4 = s.Substring(num2 + 1, (s.Length - num2) - 1);
                return (str3 + ((string) '.') + QuoteCompoundIdentifier(str4));
            }
            int index = s.IndexOf('.');
            if (index < 0)
            {
                return builder.QuoteIdentifier(s);
            }
            string str5 = s.Substring(0, index);
            string str6 = s.Substring(index + 1, (s.Length - index) - 1);
            return (QuoteCompoundIdentifier(str5) + ((string) '.') + QuoteCompoundIdentifier(str6));
        }

        internal static string QuoteIdentifier(string s)
        {
            if (s == null)
            {
                throw Error.ArgumentNull("s");
            }
            if (s.StartsWith("@", StringComparison.Ordinal))
            {
                return s;
            }
            if (IsQuoted(s))
            {
                return s;
            }
            return builder.QuoteIdentifier(s);
        }

    }
}

