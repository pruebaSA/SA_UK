namespace System.Data.Services.Design.Common
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    internal sealed class UniqueIdentifierService
    {
        private readonly Dictionary<object, string> _identifierToAdjustedIdentifier;
        private readonly HashSet<string> _knownIdentifiers;

        internal UniqueIdentifierService(bool caseSensitive)
        {
            this._knownIdentifiers = new HashSet<string>(caseSensitive ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase);
            this._identifierToAdjustedIdentifier = new Dictionary<object, string>();
        }

        internal string AdjustIdentifier(string identifier) => 
            this.AdjustIdentifier(identifier, null);

        internal string AdjustIdentifier(string identifier, object value)
        {
            int num = 0;
            string item = identifier;
            while (this._knownIdentifiers.Contains(item))
            {
                num++;
                item = identifier + num.ToString(CultureInfo.InvariantCulture);
            }
            this._knownIdentifiers.Add(item);
            if (value != null)
            {
                this._identifierToAdjustedIdentifier.Add(value, item);
            }
            return item;
        }
    }
}

