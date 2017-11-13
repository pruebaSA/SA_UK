namespace System.Data.Mapping.ViewGeneration.Structures
{
    using System;
    using System.Data.Common.Utils;
    using System.Globalization;
    using System.Text;

    internal class CqlIdentifiers : InternalBase
    {
        private Set<string> m_identifiers = new Set<string>(StringComparer.Ordinal);

        internal CqlIdentifiers()
        {
        }

        internal void AddIdentifier(string identifier)
        {
            this.m_identifiers.Add(identifier.ToLower(CultureInfo.InvariantCulture));
        }

        internal string GetBlockAlias() => 
            this.GetNonConflictingName("T", -1);

        internal string GetBlockAlias(int num) => 
            this.GetNonConflictingName("T", num);

        internal string GetFromVariable(int num) => 
            this.GetNonConflictingName("_from", num);

        private string GetNonConflictingName(string prefix, int number)
        {
            string str = (number < 0) ? prefix : StringUtil.FormatInvariant("{0}{1}", new object[] { prefix, number });
            if (!this.m_identifiers.Contains(str.ToLower(CultureInfo.InvariantCulture)))
            {
                return str;
            }
            for (int i = 0; i < 0x7fffffff; i++)
            {
                if (number < 0)
                {
                    str = StringUtil.FormatInvariant("{0}_{1}", new object[] { prefix, i });
                }
                else
                {
                    str = StringUtil.FormatInvariant("{0}_{1}_{2}", new object[] { prefix, i, number });
                }
                if (!this.m_identifiers.Contains(str.ToLower(CultureInfo.InvariantCulture)))
                {
                    return str;
                }
            }
            return null;
        }

        internal override void ToCompactString(StringBuilder builder)
        {
            this.m_identifiers.ToCompactString(builder);
        }
    }
}

