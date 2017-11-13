namespace System.Data.Mapping.Update.Internal
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.Common.CommandTrees;
    using System.Data.Common.Utils;
    using System.Data.EntityClient;
    using System.Data.Metadata.Edm;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;

    internal sealed class DynamicUpdateCommand : UpdateCommand
    {
        internal readonly PropagatorResult CurrentValues;
        private readonly List<KeyValuePair<long, DbSetClause>> m_inputIdentifiers;
        private readonly DbModificationCommandTree m_modificationCommandTree;
        private readonly ModificationOperator m_operator;
        private readonly Dictionary<long, string> m_outputIdentifiers;
        private readonly TableChangeProcessor m_processor;
        internal readonly PropagatorResult OriginalValues;

        internal DynamicUpdateCommand(TableChangeProcessor processor, UpdateTranslator translator, ModificationOperator op, PropagatorResult originalValues, PropagatorResult currentValues, DbModificationCommandTree tree, Dictionary<long, string> outputIdentifiers)
        {
            this.m_processor = EntityUtil.CheckArgumentNull<TableChangeProcessor>(processor, "processor");
            this.m_operator = op;
            this.OriginalValues = originalValues;
            this.CurrentValues = currentValues;
            this.m_modificationCommandTree = EntityUtil.CheckArgumentNull<DbModificationCommandTree>(tree, "commandTree");
            this.m_outputIdentifiers = outputIdentifiers;
            if ((ModificationOperator.Insert == op) || (op == ModificationOperator.Update))
            {
                this.m_inputIdentifiers = new List<KeyValuePair<long, DbSetClause>>(2);
                foreach (KeyValuePair<EdmMember, PropagatorResult> pair in Helper.PairEnumerations<EdmMember, PropagatorResult>(TypeHelpers.GetAllStructuralMembers(this.CurrentValues.StructuralType), this.CurrentValues.GetMemberValues()))
                {
                    DbSetClause clause;
                    long identifier = pair.Value.Identifier;
                    if ((-1L != identifier) && TryGetSetterExpression(tree, pair.Key, op, out clause))
                    {
                        foreach (long num2 in translator.KeyManager.GetPrincipals(identifier))
                        {
                            this.m_inputIdentifiers.Add(new KeyValuePair<long, DbSetClause>(num2, clause));
                        }
                    }
                }
            }
        }

        internal override int CompareToType(UpdateCommand otherCommand)
        {
            DynamicUpdateCommand command = (DynamicUpdateCommand) otherCommand;
            int num = (int) (this.Operator - command.Operator);
            if (num == 0)
            {
                num = StringComparer.Ordinal.Compare(this.m_processor.Table.Name, command.m_processor.Table.Name);
                if (num != 0)
                {
                    return num;
                }
                num = StringComparer.Ordinal.Compare(this.m_processor.Table.EntityContainer.Name, command.m_processor.Table.EntityContainer.Name);
                if (num != 0)
                {
                    return num;
                }
                PropagatorResult result = (this.Operator == ModificationOperator.Delete) ? this.OriginalValues : this.CurrentValues;
                PropagatorResult result2 = (command.Operator == ModificationOperator.Delete) ? command.OriginalValues : command.CurrentValues;
                for (int i = 0; i < this.m_processor.KeyOrdinals.Length; i++)
                {
                    int ordinal = this.m_processor.KeyOrdinals[i];
                    object simpleValue = result.GetMemberValue(ordinal).GetSimpleValue();
                    object y = result2.GetMemberValue(ordinal).GetSimpleValue();
                    num = Comparer<object>.Default.Compare(simpleValue, y);
                    if (num != 0)
                    {
                        return num;
                    }
                }
            }
            return num;
        }

        private DbCommand CreateCommand(UpdateTranslator translator, Dictionary<long, object> identifierValues)
        {
            if (this.m_inputIdentifiers != null)
            {
                foreach (KeyValuePair<long, DbSetClause> pair in this.m_inputIdentifiers)
                {
                    object obj2;
                    if (identifierValues.TryGetValue(pair.Key, out obj2))
                    {
                        pair.Value.Value = this.m_modificationCommandTree.CreateConstantExpression(obj2);
                    }
                }
            }
            return translator.CreateCommand(this.m_modificationCommandTree);
        }

        internal override int Execute(UpdateTranslator translator, EntityConnection connection, Dictionary<long, object> identifierValues, List<KeyValuePair<PropagatorResult, object>> generatedValues)
        {
            DbCommand command = this.CreateCommand(translator, identifierValues);
            command.Transaction = connection.CurrentTransaction?.StoreTransaction;
            command.Connection = connection.StoreConnection;
            if (translator.CommandTimeout.HasValue)
            {
                command.CommandTimeout = translator.CommandTimeout.Value;
            }
            if (this.m_modificationCommandTree.HasReader)
            {
                int num = 0;
                using (DbDataReader reader = command.ExecuteReader(CommandBehavior.SequentialAccess))
                {
                    if (reader.Read())
                    {
                        num++;
                        IBaseList<EdmMember> allStructuralMembers = TypeHelpers.GetAllStructuralMembers(this.CurrentValues.StructuralType);
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            string name = reader.GetName(i);
                            object obj2 = reader.GetValue(i);
                            int index = allStructuralMembers.IndexOf(allStructuralMembers[name]);
                            PropagatorResult memberValue = this.CurrentValues.GetMemberValue(index);
                            generatedValues.Add(new KeyValuePair<PropagatorResult, object>(memberValue, obj2));
                            long identifier = memberValue.Identifier;
                            if (-1L != identifier)
                            {
                                identifierValues.Add(identifier, obj2);
                            }
                        }
                    }
                    CommandHelper.ConsumeReader(reader);
                    return num;
                }
            }
            return command.ExecuteNonQuery();
        }

        internal override List<IEntityStateEntry> GetStateEntries(UpdateTranslator translator)
        {
            List<IEntityStateEntry> list = new List<IEntityStateEntry>(2);
            if (this.OriginalValues != null)
            {
                foreach (IEntityStateEntry entry in SourceInterpreter.GetAllStateEntries(this.OriginalValues, translator, this.Table))
                {
                    list.Add(entry);
                }
            }
            if (this.CurrentValues != null)
            {
                foreach (IEntityStateEntry entry2 in SourceInterpreter.GetAllStateEntries(this.CurrentValues, translator, this.Table))
                {
                    list.Add(entry2);
                }
            }
            return list;
        }

        private static bool TryGetSetterExpression(DbModificationCommandTree tree, EdmMember member, ModificationOperator op, out DbSetClause setter)
        {
            IEnumerable<DbModificationClause> setClauses;
            if (ModificationOperator.Insert == op)
            {
                setClauses = ((DbInsertCommandTree) tree).SetClauses;
            }
            else
            {
                setClauses = ((DbUpdateCommandTree) tree).SetClauses;
            }
            foreach (DbSetClause clause in setClauses)
            {
                if (((DbPropertyExpression) clause.Property).Property.EdmEquals(member))
                {
                    setter = clause;
                    return true;
                }
            }
            setter = null;
            return false;
        }

        internal override IEnumerable<long> InputIdentifiers
        {
            get
            {
                if (this.m_inputIdentifiers != null)
                {
                    foreach (KeyValuePair<long, DbSetClause> iteratorVariable0 in this.m_inputIdentifiers)
                    {
                        yield return iteratorVariable0.Key;
                    }
                }
            }
        }

        internal override UpdateCommandKind Kind =>
            UpdateCommandKind.Dynamic;

        internal ModificationOperator Operator =>
            this.m_operator;

        internal override IEnumerable<long> OutputIdentifiers =>
            this.m_outputIdentifiers?.Keys;

        internal override EntitySet Table =>
            this.m_processor.Table;

    }
}

