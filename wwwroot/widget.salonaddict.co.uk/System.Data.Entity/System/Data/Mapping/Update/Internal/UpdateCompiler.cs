namespace System.Data.Mapping.Update.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common.CommandTrees;
    using System.Data.Common.Utils;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;
    using System.Runtime.InteropServices;

    internal sealed class UpdateCompiler
    {
        internal readonly UpdateTranslator m_translator;
        private const string s_targetVarName = "target";

        internal UpdateCompiler(UpdateTranslator translator)
        {
            this.m_translator = translator;
        }

        internal UpdateCommand BuildDeleteCommand(PropagatorResult oldRow, TableChangeProcessor processor)
        {
            bool rowMustBeTouched = true;
            DbDeleteCommandTree commandTree = new DbDeleteCommandTree(this.m_translator.MetadataWorkspace, DataSpace.CSpace);
            SetTarget(processor, commandTree);
            commandTree.Predicate = this.BuildPredicate(commandTree, oldRow, null, processor, ref rowMustBeTouched);
            return new DynamicUpdateCommand(processor, this.m_translator, ModificationOperator.Delete, oldRow, null, commandTree, null);
        }

        internal UpdateCommand BuildInsertCommand(PropagatorResult newRow, TableChangeProcessor processor)
        {
            Dictionary<long, string> dictionary;
            DbExpression expression;
            DbInsertCommandTree commandTree = new DbInsertCommandTree(this.m_translator.MetadataWorkspace, DataSpace.CSpace);
            SetTarget(processor, commandTree);
            bool rowMustBeTouched = true;
            List<DbModificationClause> setClauses = new List<DbModificationClause>();
            foreach (DbModificationClause clause in this.BuildSetClauses(commandTree, newRow, processor, true, out dictionary, out expression, ref rowMustBeTouched))
            {
                setClauses.Add(clause);
            }
            commandTree.InitializeSetClauses(setClauses);
            if (expression != null)
            {
                commandTree.Returning = expression;
            }
            return new DynamicUpdateCommand(processor, this.m_translator, ModificationOperator.Insert, null, newRow, commandTree, dictionary);
        }

        private DbExpression BuildPredicate(DbModificationCommandTree commandTree, PropagatorResult referenceRow, PropagatorResult current, TableChangeProcessor processor, ref bool rowMustBeTouched)
        {
            Dictionary<EdmProperty, PropagatorResult> dictionary = new Dictionary<EdmProperty, PropagatorResult>();
            int ordinal = 0;
            foreach (EdmProperty property in processor.Table.ElementType.Members)
            {
                PropagatorResult memberValue = referenceRow.GetMemberValue(ordinal);
                PropagatorResult input = current?.GetMemberValue(ordinal);
                if (!rowMustBeTouched && (HasFlag(memberValue, PropagatorFlags.ConcurrencyValue) || HasFlag(input, PropagatorFlags.ConcurrencyValue)))
                {
                    rowMustBeTouched = true;
                }
                if (!dictionary.ContainsKey(property) && (HasFlag(memberValue, PropagatorFlags.ConcurrencyValue | PropagatorFlags.Key) || HasFlag(input, PropagatorFlags.ConcurrencyValue | PropagatorFlags.Key)))
                {
                    dictionary.Add(property, memberValue);
                }
                ordinal++;
            }
            DbExpression left = null;
            foreach (KeyValuePair<EdmProperty, PropagatorResult> pair in dictionary)
            {
                DbExpression right = this.GenerateEqualityExpression(commandTree, pair.Key, pair.Value);
                if (left == null)
                {
                    left = right;
                }
                else
                {
                    left = commandTree.CreateAndExpression(left, right);
                }
            }
            return left;
        }

        private IEnumerable<DbModificationClause> BuildSetClauses(DbModificationCommandTree commandTree, PropagatorResult row, TableChangeProcessor processor, bool insertMode, out Dictionary<long, string> outputIdentifiers, out DbExpression returning, ref bool rowMustBeTouched)
        {
            Dictionary<EdmProperty, PropagatorResult> dictionary = new Dictionary<EdmProperty, PropagatorResult>();
            List<KeyValuePair<string, DbExpression>> recordColumns = new List<KeyValuePair<string, DbExpression>>();
            outputIdentifiers = new Dictionary<long, string>();
            PropagatorFlags flags = insertMode ? PropagatorFlags.NoFlags : (PropagatorFlags.NoFlags | PropagatorFlags.Preserve | PropagatorFlags.Unknown);
            for (int i = 0; i < processor.Table.ElementType.Properties.Count; i++)
            {
                EdmProperty member = processor.Table.ElementType.Properties[i];
                PropagatorResult memberValue = row.GetMemberValue(i);
                if (-1L != memberValue.Identifier)
                {
                    memberValue = memberValue.ReplicateResultWithNewValue(this.m_translator.KeyManager.GetPrincipalValue(memberValue));
                }
                bool flag = false;
                bool flag2 = false;
                for (int j = 0; j < processor.KeyOrdinals.Length; j++)
                {
                    if (processor.KeyOrdinals[j] == i)
                    {
                        flag2 = true;
                        break;
                    }
                }
                PropagatorFlags noFlags = PropagatorFlags.NoFlags;
                if (!insertMode && flag2)
                {
                    flag = true;
                }
                else
                {
                    noFlags = (PropagatorFlags) ((byte) (noFlags | memberValue.PropagatorFlags));
                }
                StoreGeneratedPattern storeGeneratedPattern = MetadataHelper.GetStoreGeneratedPattern(member);
                bool flag3 = (storeGeneratedPattern == StoreGeneratedPattern.Computed) || (insertMode && (storeGeneratedPattern == StoreGeneratedPattern.Identity));
                if (flag3)
                {
                    DbPropertyExpression expression = commandTree.CreatePropertyExpression(member, commandTree.Target.Variable);
                    recordColumns.Add(new KeyValuePair<string, DbExpression>(member.Name, expression));
                    long identifier = memberValue.Identifier;
                    if (-1L != identifier)
                    {
                        if (this.m_translator.KeyManager.HasPrincipals(identifier))
                        {
                            throw EntityUtil.InvalidOperation(Strings.Update_GeneratedDependent(member.Name));
                        }
                        outputIdentifiers.Add(identifier, member.Name);
                        if ((storeGeneratedPattern != StoreGeneratedPattern.Identity) && processor.IsKeyProperty(i))
                        {
                            throw EntityUtil.NotSupported(Strings.Update_NotSupportedComputedKeyColumn("StoreGeneratedPattern", "Computed", "Identity", member.Name, member.DeclaringType.FullName));
                        }
                    }
                }
                if (((byte) (noFlags & flags)) != 0)
                {
                    flag = true;
                }
                else if (flag3)
                {
                    flag = true;
                    rowMustBeTouched = true;
                }
                if ((!flag && !insertMode) && (storeGeneratedPattern == StoreGeneratedPattern.Identity))
                {
                    throw EntityUtil.InvalidOperation(Strings.Update_ModifyingIdentityColumn("Identity", member.Name, member.DeclaringType.FullName));
                }
                if (!flag)
                {
                    dictionary.Add(member, memberValue);
                }
            }
            if (0 < recordColumns.Count)
            {
                returning = commandTree.CreateNewRowExpression(recordColumns);
            }
            else
            {
                returning = null;
            }
            List<DbModificationClause> list2 = new List<DbModificationClause>(dictionary.Count);
            foreach (KeyValuePair<EdmProperty, PropagatorResult> pair in dictionary)
            {
                EdmProperty key = pair.Key;
                list2.Add(new DbSetClause(commandTree, GeneratePropertyExpression(commandTree, pair.Key), this.GenerateValueExpression(commandTree, pair.Key, pair.Value)));
            }
            return list2;
        }

        internal UpdateCommand BuildUpdateCommand(PropagatorResult oldRow, PropagatorResult newRow, TableChangeProcessor processor)
        {
            Dictionary<long, string> dictionary;
            DbExpression expression;
            bool rowMustBeTouched = false;
            DbUpdateCommandTree commandTree = new DbUpdateCommandTree(this.m_translator.MetadataWorkspace, DataSpace.CSpace);
            SetTarget(processor, commandTree);
            commandTree.Predicate = this.BuildPredicate(commandTree, oldRow, newRow, processor, ref rowMustBeTouched);
            List<DbModificationClause> setClauses = new List<DbModificationClause>();
            foreach (DbModificationClause clause in this.BuildSetClauses(commandTree, newRow, processor, false, out dictionary, out expression, ref rowMustBeTouched))
            {
                setClauses.Add(clause);
            }
            commandTree.InitializeSetClauses(setClauses);
            commandTree.Predicate = this.BuildPredicate(commandTree, oldRow, newRow, processor, ref rowMustBeTouched);
            if (!rowMustBeTouched && (commandTree.SetClauses.Count == 0))
            {
                return null;
            }
            if (expression != null)
            {
                commandTree.Returning = expression;
            }
            return new DynamicUpdateCommand(processor, this.m_translator, ModificationOperator.Update, oldRow, newRow, commandTree, dictionary);
        }

        private DbExpression GenerateEqualityExpression(DbModificationCommandTree commandTree, EdmProperty property, PropagatorResult value)
        {
            DbExpression argument = GeneratePropertyExpression(commandTree, property);
            if (value.IsNull)
            {
                return commandTree.CreateIsNullExpression(argument);
            }
            if (!TypeSemantics.IsEqualComparable(property.TypeUsage))
            {
                throw EntityUtil.InvalidOperation(Strings.Update_NonEquatableColumnTypeInClause(property.Name, property.DeclaringType.Name));
            }
            return commandTree.CreateEqualsExpression(argument, this.GenerateValueExpression(commandTree, property, value));
        }

        private static DbExpression GeneratePropertyExpression(DbModificationCommandTree commandTree, EdmProperty property) => 
            commandTree.CreatePropertyExpression(property, commandTree.Target.Variable);

        private DbExpression GenerateValueExpression(DbCommandTree commandTree, EdmProperty property, PropagatorResult value)
        {
            if (value.IsNull)
            {
                return commandTree.CreateNullExpression(Helper.GetModelTypeUsage(property));
            }
            object principalValue = this.m_translator.KeyManager.GetPrincipalValue(value);
            return commandTree.CreateConstantExpression(principalValue, Helper.GetModelTypeUsage(property));
        }

        private static bool HasFlag(PropagatorResult input, PropagatorFlags flags)
        {
            if (input == null)
            {
                return false;
            }
            return (0 != ((byte) (flags & input.PropagatorFlags)));
        }

        private static void SetTarget(TableChangeProcessor processor, DbModificationCommandTree commandTree)
        {
            commandTree.Target = commandTree.CreateExpressionBinding(commandTree.CreateScanExpression(processor.Table), "target");
        }
    }
}

