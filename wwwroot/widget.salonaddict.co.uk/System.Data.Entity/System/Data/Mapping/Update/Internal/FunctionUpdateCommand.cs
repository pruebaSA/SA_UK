namespace System.Data.Mapping.Update.Internal
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.Common.Utils;
    using System.Data.Entity;
    using System.Data.EntityClient;
    using System.Data.Mapping;
    using System.Data.Metadata.Edm;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;

    internal sealed class FunctionUpdateCommand : UpdateCommand
    {
        private readonly DbCommand m_dbCommand;
        private List<KeyValuePair<long, DbParameter>> m_inputIdentifiers;
        private Dictionary<long, string> m_outputIdentifiers;
        private List<KeyValuePair<string, PropagatorResult>> m_resultColumns;
        private DbParameter m_rowsAffectedParameter;
        private readonly List<IEntityStateEntry> m_stateEntries;

        internal FunctionUpdateCommand(StorageFunctionMapping functionMapping, UpdateTranslator translator, IEntityStateEntry stateEntry)
        {
            EntityUtil.CheckArgumentNull<StorageFunctionMapping>(functionMapping, "functionMapping");
            EntityUtil.CheckArgumentNull<UpdateTranslator>(translator, "translator");
            EntityUtil.CheckArgumentNull<IEntityStateEntry>(stateEntry, "stateEntry");
            this.m_stateEntries = new List<IEntityStateEntry>(1);
            this.m_stateEntries.Add(stateEntry);
            this.m_dbCommand = translator.GenerateCommandDefinition(functionMapping).CreateCommand();
        }

        private void AddOutputIdentifier(string columnName, long identifier)
        {
            if (this.m_outputIdentifiers == null)
            {
                this.m_outputIdentifiers = new Dictionary<long, string>(2);
            }
            this.m_outputIdentifiers[identifier] = columnName;
        }

        internal void AddResultColumn(UpdateTranslator translator, string columnName, PropagatorResult result)
        {
            if (this.m_resultColumns == null)
            {
                this.m_resultColumns = new List<KeyValuePair<string, PropagatorResult>>(2);
            }
            this.m_resultColumns.Add(new KeyValuePair<string, PropagatorResult>(columnName, result));
            long identifier = result.Identifier;
            if (-1L != identifier)
            {
                if (translator.KeyManager.HasPrincipals(identifier))
                {
                    throw EntityUtil.InvalidOperation(System.Data.Entity.Strings.Update_GeneratedDependent(columnName));
                }
                this.AddOutputIdentifier(columnName, identifier);
            }
        }

        internal override int CompareToType(UpdateCommand otherCommand)
        {
            FunctionUpdateCommand command = (FunctionUpdateCommand) otherCommand;
            IEntityStateEntry entry = this.m_stateEntries[0];
            IEntityStateEntry entry2 = command.m_stateEntries[0];
            int num = (int) (entry.State - entry2.State);
            if (num == 0)
            {
                num = StringComparer.Ordinal.Compare(entry.EntitySet.Name, entry2.EntitySet.Name);
                if (num != 0)
                {
                    return num;
                }
                num = StringComparer.Ordinal.Compare(entry.EntitySet.EntityContainer.Name, entry2.EntitySet.EntityContainer.Name);
                if (num != 0)
                {
                    return num;
                }
                int num2 = (this.m_inputIdentifiers == null) ? 0 : this.m_inputIdentifiers.Count;
                int num3 = (command.m_inputIdentifiers == null) ? 0 : command.m_inputIdentifiers.Count;
                num = num2 - num3;
                if (num != 0)
                {
                    return num;
                }
                for (int i = 0; i < num2; i++)
                {
                    KeyValuePair<long, DbParameter> pair = this.m_inputIdentifiers[i];
                    DbParameter parameter = pair.Value;
                    KeyValuePair<long, DbParameter> pair2 = command.m_inputIdentifiers[i];
                    DbParameter parameter2 = pair2.Value;
                    num = Comparer<object>.Default.Compare(parameter.Value, parameter2.Value);
                    if (num != 0)
                    {
                        return num;
                    }
                }
            }
            return num;
        }

        internal override int Execute(UpdateTranslator translator, EntityConnection connection, Dictionary<long, object> identifierValues, List<KeyValuePair<PropagatorResult, object>> generatedValues)
        {
            int num;
            this.m_dbCommand.Transaction = connection.CurrentTransaction?.StoreTransaction;
            this.m_dbCommand.Connection = connection.StoreConnection;
            if (translator.CommandTimeout.HasValue)
            {
                this.m_dbCommand.CommandTimeout = translator.CommandTimeout.Value;
            }
            if (this.m_inputIdentifiers != null)
            {
                foreach (KeyValuePair<long, DbParameter> pair in this.m_inputIdentifiers)
                {
                    object obj2;
                    if (identifierValues.TryGetValue(pair.Key, out obj2))
                    {
                        pair.Value.Value = obj2;
                    }
                }
            }
            if (this.m_resultColumns != null)
            {
                num = 0;
                Func<KeyValuePair<string, PropagatorResult>, KeyValuePair<int, PropagatorResult>> selector = null;
                using (DbDataReader reader = this.m_dbCommand.ExecuteReader(CommandBehavior.SequentialAccess))
                {
                    if (reader.Read())
                    {
                        num++;
                        if (selector == null)
                        {
                            selector = r => new KeyValuePair<int, PropagatorResult>(this.GetColumnOrdinal(translator, reader, r.Key), r.Value);
                        }
                        foreach (KeyValuePair<int, PropagatorResult> pair2 in from r in this.m_resultColumns.Select<KeyValuePair<string, PropagatorResult>, KeyValuePair<int, PropagatorResult>>(selector)
                            orderby r.Key
                            select r)
                        {
                            int key = pair2.Key;
                            object obj3 = reader.GetValue(key);
                            PropagatorResult result = pair2.Value;
                            generatedValues.Add(new KeyValuePair<PropagatorResult, object>(result, obj3));
                            long identifier = result.Identifier;
                            if (-1L != identifier)
                            {
                                identifierValues.Add(identifier, obj3);
                            }
                        }
                    }
                    CommandHelper.ConsumeReader(reader);
                }
            }
            else
            {
                num = this.m_dbCommand.ExecuteNonQuery();
            }
            if (this.m_rowsAffectedParameter != null)
            {
                if (DBNull.Value.Equals(this.m_rowsAffectedParameter.Value))
                {
                    return 0;
                }
                try
                {
                    num = Convert.ToInt32(this.m_rowsAffectedParameter.Value, CultureInfo.InvariantCulture);
                }
                catch (Exception exception)
                {
                    if (UpdateTranslator.RequiresContext(exception))
                    {
                        throw EntityUtil.Update(System.Data.Entity.Strings.Update_UnableToConvertRowsAffectedParameterToInt32(this.m_rowsAffectedParameter.ParameterName, typeof(int).FullName), exception, this.GetStateEntries(translator));
                    }
                    throw;
                }
            }
            return num;
        }

        private int GetColumnOrdinal(UpdateTranslator translator, DbDataReader reader, string columnName)
        {
            int ordinal;
            try
            {
                ordinal = reader.GetOrdinal(columnName);
            }
            catch (IndexOutOfRangeException)
            {
                throw EntityUtil.Update(System.Data.Entity.Strings.Update_MissingResultColumn(columnName), null, this.GetStateEntries(translator));
            }
            return ordinal;
        }

        internal override List<IEntityStateEntry> GetStateEntries(UpdateTranslator translator) => 
            this.m_stateEntries;

        internal void RegisterRowsAffectedParameter(FunctionParameter rowsAffectedParameter)
        {
            if (rowsAffectedParameter != null)
            {
                this.m_rowsAffectedParameter = this.m_dbCommand.Parameters[rowsAffectedParameter.Name];
            }
        }

        internal void RegisterStateEntry(IEntityStateEntry stateEntry)
        {
            this.m_stateEntries.Add(EntityUtil.CheckArgumentNull<IEntityStateEntry>(stateEntry, "stateEntry"));
        }

        internal void SetParameterValue(PropagatorResult result, StorageFunctionParameterBinding parameterBinding, UpdateTranslator translator)
        {
            DbParameter parameter = this.m_dbCommand.Parameters[parameterBinding.Parameter.Name];
            parameter.Value = translator.KeyManager.GetPrincipalValue(result);
            long identifier = result.Identifier;
            if (-1L != identifier)
            {
                if (this.m_inputIdentifiers == null)
                {
                    this.m_inputIdentifiers = new List<KeyValuePair<long, DbParameter>>(2);
                }
                foreach (long num2 in translator.KeyManager.GetPrincipals(identifier))
                {
                    this.m_inputIdentifiers.Add(new KeyValuePair<long, DbParameter>(num2, parameter));
                }
                foreach (IEntityStateEntry entry in translator.KeyManager.GetDependentStateEntries(identifier))
                {
                    this.m_stateEntries.Add(entry);
                }
            }
        }

        internal override IEnumerable<long> InputIdentifiers
        {
            get
            {
                if (this.m_inputIdentifiers != null)
                {
                    foreach (KeyValuePair<long, DbParameter> iteratorVariable0 in this.m_inputIdentifiers)
                    {
                        yield return iteratorVariable0.Key;
                    }
                }
            }
        }

        internal override UpdateCommandKind Kind =>
            UpdateCommandKind.Function;

        internal override IEnumerable<long> OutputIdentifiers =>
            this.m_outputIdentifiers?.Keys;

    }
}

