namespace System.Data.Mapping.ViewGeneration.CqlGeneration
{
    using System;
    using System.Data.Common.Utils;
    using System.Data.Mapping.ViewGeneration.Utils;
    using System.Data.Metadata.Edm;
    using System.Text;
    using System.Text.RegularExpressions;

    internal static class CqlWriter
    {
        private static readonly Regex s_wordIdentifierRegex = new Regex(@"^[_A-Za-z]\w*$", RegexOptions.ECMAScript | RegexOptions.Compiled);

        internal static void AppendEscapedName(StringBuilder builder, string name)
        {
            if (s_wordIdentifierRegex.IsMatch(name) && !ExternalCalls.IsReservedKeyword(name))
            {
                builder.Append(name);
            }
            else
            {
                string str = name.Replace("]", "]]");
                builder.Append('[').Append(str).Append(']');
            }
        }

        internal static void AppendEscapedQualifiedName(StringBuilder builder, string namespc, string name)
        {
            AppendEscapedName(builder, namespc);
            builder.Append('.');
            AppendEscapedName(builder, name);
        }

        internal static void AppendEscapedTypeName(StringBuilder builder, EdmType type)
        {
            AppendEscapedQualifiedName(builder, type.NamespaceName, type.Name);
        }

        internal static string GetQualifiedName(string blockName, string field) => 
            StringUtil.FormatInvariant("{0}.{1}", new object[] { blockName, field });
    }
}

