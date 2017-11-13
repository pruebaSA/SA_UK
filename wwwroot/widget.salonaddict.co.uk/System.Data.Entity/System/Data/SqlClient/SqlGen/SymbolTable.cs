namespace System.Data.SqlClient.SqlGen
{
    using System;
    using System.Collections.Generic;

    internal sealed class SymbolTable
    {
        private List<Dictionary<string, Symbol>> symbols = new List<Dictionary<string, Symbol>>();

        internal void Add(string name, Symbol value)
        {
            this.symbols[this.symbols.Count - 1][name] = value;
        }

        internal void EnterScope()
        {
            this.symbols.Add(new Dictionary<string, Symbol>(StringComparer.OrdinalIgnoreCase));
        }

        internal void ExitScope()
        {
            this.symbols.RemoveAt(this.symbols.Count - 1);
        }

        internal Symbol Lookup(string name)
        {
            for (int i = this.symbols.Count - 1; i >= 0; i--)
            {
                if (this.symbols[i].ContainsKey(name))
                {
                    return this.symbols[i][name];
                }
            }
            return null;
        }
    }
}

