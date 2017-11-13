namespace System.Data.Mapping.Update.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Data.Metadata.Edm;

    internal class SourceInterpreter
    {
        private readonly EntitySet m_sourceTable;
        private readonly List<IEntityStateEntry> m_stateEntries = new List<IEntityStateEntry>();
        private readonly UpdateTranslator m_translator;

        private SourceInterpreter(UpdateTranslator translator, EntitySet sourceTable)
        {
            this.m_translator = translator;
            this.m_sourceTable = sourceTable;
        }

        private bool ExtentInScope(EntitySetBase extent)
        {
            if (extent == null)
            {
                return false;
            }
            return this.m_translator.ViewLoader.GetAffectedTables(extent).Contains(this.m_sourceTable);
        }

        internal static ReadOnlyCollection<IEntityStateEntry> GetAllStateEntries(PropagatorResult source, UpdateTranslator translator, EntitySet sourceTable)
        {
            SourceInterpreter interpreter = new SourceInterpreter(translator, sourceTable);
            interpreter.RetrieveResultMarkup(source);
            return new ReadOnlyCollection<IEntityStateEntry>(interpreter.m_stateEntries);
        }

        private void RetrieveResultMarkup(PropagatorResult source)
        {
            if (source.StateEntry != null)
            {
                this.m_stateEntries.Add(source.StateEntry);
            }
            long identifier = source.Identifier;
            if (-1L != identifier)
            {
                PropagatorResult result;
                if ((this.m_translator.KeyManager.TryGetIdentifierOwner(identifier, out result) && (result.StateEntry != null)) && this.ExtentInScope(result.StateEntry.EntitySet))
                {
                    this.m_stateEntries.Add(result.StateEntry);
                }
                foreach (IEntityStateEntry entry in this.m_translator.KeyManager.GetDependentStateEntries(identifier))
                {
                    this.m_stateEntries.Add(entry);
                }
            }
            if (!source.IsSimple && !source.IsNull)
            {
                foreach (PropagatorResult result2 in source.GetMemberValues())
                {
                    this.RetrieveResultMarkup(result2);
                }
            }
        }
    }
}

