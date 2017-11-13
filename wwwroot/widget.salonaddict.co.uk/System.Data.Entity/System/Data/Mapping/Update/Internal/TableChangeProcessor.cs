namespace System.Data.Mapping.Update.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.Common.Utils;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;

    internal class TableChangeProcessor
    {
        private readonly int[] m_keyOrdinals;
        private readonly EntitySet m_table;

        internal TableChangeProcessor(EntitySet table)
        {
            EntityUtil.CheckArgumentNull<EntitySet>(table, "table");
            this.m_table = table;
            this.m_keyOrdinals = InitializeKeyOrdinals(table);
        }

        internal List<UpdateCommand> CompileCommands(ChangeNode changeNode, UpdateCompiler compiler)
        {
            Set<CompositeKey> keys = new Set<CompositeKey>(compiler.m_translator.KeyComparer);
            Dictionary<CompositeKey, PropagatorResult> dictionary = this.ProcessKeys(compiler, changeNode.Deleted, keys);
            Dictionary<CompositeKey, PropagatorResult> dictionary2 = this.ProcessKeys(compiler, changeNode.Inserted, keys);
            List<UpdateCommand> list = new List<UpdateCommand>(dictionary.Count + dictionary2.Count);
            foreach (CompositeKey key in keys)
            {
                PropagatorResult result;
                PropagatorResult result2;
                bool flag = dictionary.TryGetValue(key, out result);
                bool flag2 = dictionary2.TryGetValue(key, out result2);
                try
                {
                    if (!flag)
                    {
                        list.Add(compiler.BuildInsertCommand(result2, this));
                    }
                    else if (!flag2)
                    {
                        list.Add(compiler.BuildDeleteCommand(result, this));
                    }
                    else
                    {
                        UpdateCommand item = compiler.BuildUpdateCommand(result, result2, this);
                        if (item != null)
                        {
                            list.Add(item);
                        }
                    }
                }
                catch (Exception exception)
                {
                    if (!UpdateTranslator.RequiresContext(exception))
                    {
                        throw;
                    }
                    List<IEntityStateEntry> stateEntries = new List<IEntityStateEntry>();
                    if (result != null)
                    {
                        stateEntries.AddRange(SourceInterpreter.GetAllStateEntries(result, compiler.m_translator, this.m_table));
                    }
                    if (result2 != null)
                    {
                        stateEntries.AddRange(SourceInterpreter.GetAllStateEntries(result2, compiler.m_translator, this.m_table));
                    }
                    throw EntityUtil.Update(Strings.Update_GeneralExecutionException, exception, stateEntries);
                }
            }
            return list;
        }

        private PropagatorResult[] GetKeyConstants(PropagatorResult row)
        {
            PropagatorResult[] resultArray = new PropagatorResult[this.m_keyOrdinals.Length];
            for (int i = 0; i < this.m_keyOrdinals.Length; i++)
            {
                resultArray[i] = row.GetMemberValue(this.m_keyOrdinals[i]);
            }
            return resultArray;
        }

        private static int[] InitializeKeyOrdinals(EntitySet table)
        {
            EntityType elementType = table.ElementType;
            IList<EdmMember> keyMembers = elementType.KeyMembers;
            IBaseList<EdmMember> allStructuralMembers = TypeHelpers.GetAllStructuralMembers(elementType);
            int[] numArray = new int[keyMembers.Count];
            for (int i = 0; i < keyMembers.Count; i++)
            {
                EdmMember item = keyMembers[i];
                numArray[i] = allStructuralMembers.IndexOf(item);
            }
            return numArray;
        }

        internal bool IsKeyProperty(int propertyOrdinal)
        {
            foreach (int num in this.m_keyOrdinals)
            {
                if (propertyOrdinal == num)
                {
                    return true;
                }
            }
            return false;
        }

        private Dictionary<CompositeKey, PropagatorResult> ProcessKeys(UpdateCompiler compiler, List<PropagatorResult> changes, Set<CompositeKey> keys)
        {
            Dictionary<CompositeKey, PropagatorResult> dictionary = new Dictionary<CompositeKey, PropagatorResult>(compiler.m_translator.KeyComparer);
            foreach (PropagatorResult result in changes)
            {
                PropagatorResult row = result;
                CompositeKey key = new CompositeKey(this.GetKeyConstants(row));
                dictionary.Add(key, row);
                keys.Add(key);
            }
            return dictionary;
        }

        internal int[] KeyOrdinals =>
            this.m_keyOrdinals;

        internal EntitySet Table =>
            this.m_table;
    }
}

