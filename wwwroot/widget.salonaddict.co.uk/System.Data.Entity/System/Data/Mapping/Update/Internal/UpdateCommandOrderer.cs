namespace System.Data.Mapping.Update.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common.Utils;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;
    using System.Linq;
    using System.Runtime.InteropServices;

    internal class UpdateCommandOrderer : Graph<UpdateCommand>
    {
        private readonly bool _hasFunctionCommands;
        private readonly ForeignKeyValueComparer _keyComparer;
        private readonly KeyToListMap<EntitySetBase, ReferentialConstraint> _sourceMap;
        private readonly KeyToListMap<EntitySetBase, ReferentialConstraint> _targetMap;
        private readonly UpdateTranslator _translator;

        internal UpdateCommandOrderer(IEnumerable<UpdateCommand> commands, UpdateTranslator translator) : base(EqualityComparer<UpdateCommand>.Default)
        {
            this._translator = translator;
            this._keyComparer = new ForeignKeyValueComparer(this._translator.KeyComparer);
            HashSet<EntitySet> tables = new HashSet<EntitySet>();
            HashSet<EntityContainer> containers = new HashSet<EntityContainer>();
            foreach (UpdateCommand command in commands)
            {
                if (command.Table != null)
                {
                    tables.Add(command.Table);
                    containers.Add(command.Table.EntityContainer);
                }
                base.AddVertex(command);
                if (command.Kind == UpdateCommandKind.Function)
                {
                    this._hasFunctionCommands = true;
                }
            }
            InitializeForeignKeyMaps(containers, tables, out this._sourceMap, out this._targetMap);
            this.AddServerGenDependencies();
            this.AddForeignKeyDependencies();
            if (this._hasFunctionCommands)
            {
                this.AddModelDependencies();
            }
        }

        private void AddForeignKeyDependencies()
        {
            KeyToListMap<ForeignKeyValue, UpdateCommand> predecessors = this.DetermineForeignKeyPredecessors();
            this.AddForeignKeyEdges(predecessors);
        }

        private void AddForeignKeyEdges(KeyToListMap<ForeignKeyValue, UpdateCommand> predecessors)
        {
            foreach (DynamicUpdateCommand command in base.Vertices.OfType<DynamicUpdateCommand>())
            {
                if ((command.Operator == ModificationOperator.Update) || (ModificationOperator.Insert == command.Operator))
                {
                    foreach (ReferentialConstraint constraint in this._sourceMap.EnumerateValues(command.Table))
                    {
                        ForeignKeyValue value2;
                        ForeignKeyValue value3;
                        if (ForeignKeyValue.TryCreateSourceKey(constraint, command.CurrentValues, true, out value2) && (((command.Operator != ModificationOperator.Update) || !ForeignKeyValue.TryCreateSourceKey(constraint, command.OriginalValues, true, out value3)) || !this._keyComparer.Equals(value3, value2)))
                        {
                            foreach (UpdateCommand command2 in predecessors.EnumerateValues(value2))
                            {
                                if (command2 != command)
                                {
                                    base.AddEdge(command2, command);
                                }
                            }
                        }
                    }
                }
                if ((command.Operator == ModificationOperator.Update) || (ModificationOperator.Delete == command.Operator))
                {
                    foreach (ReferentialConstraint constraint2 in this._targetMap.EnumerateValues(command.Table))
                    {
                        ForeignKeyValue value4;
                        ForeignKeyValue value5;
                        if (ForeignKeyValue.TryCreateTargetKey(constraint2, command.OriginalValues, false, out value4) && (((command.Operator != ModificationOperator.Update) || !ForeignKeyValue.TryCreateTargetKey(constraint2, command.CurrentValues, false, out value5)) || !this._keyComparer.Equals(value5, value4)))
                        {
                            foreach (UpdateCommand command3 in predecessors.EnumerateValues(value4))
                            {
                                if (command3 != command)
                                {
                                    base.AddEdge(command3, command);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void AddModelDependencies()
        {
            KeyToListMap<EntityKey, UpdateCommand> map = new KeyToListMap<EntityKey, UpdateCommand>(EqualityComparer<EntityKey>.Default);
            KeyToListMap<EntityKey, UpdateCommand> map2 = new KeyToListMap<EntityKey, UpdateCommand>(EqualityComparer<EntityKey>.Default);
            foreach (UpdateCommand command in base.Vertices)
            {
                List<EntityKey> list;
                List<EntityKey> list2;
                command.GetRequiredAndProducedEntities(this._translator, out list, out list2);
                foreach (EntityKey key in list)
                {
                    map.Add(key, command);
                }
                foreach (EntityKey key2 in list2)
                {
                    map2.Add(key2, command);
                }
            }
            foreach (KeyValuePair<EntityKey, List<UpdateCommand>> pair in map.KeyValuePairs)
            {
                EntityKey key3 = pair.Key;
                List<UpdateCommand> list3 = pair.Value;
                foreach (UpdateCommand command2 in map2.EnumerateValues(key3))
                {
                    foreach (UpdateCommand command3 in list3)
                    {
                        if (!object.ReferenceEquals(command2, command3) && ((command2.Kind == UpdateCommandKind.Function) || (command3.Kind == UpdateCommandKind.Function)))
                        {
                            base.AddEdge(command2, command3);
                        }
                    }
                }
            }
        }

        private void AddServerGenDependencies()
        {
            Dictionary<long, UpdateCommand> dictionary = new Dictionary<long, UpdateCommand>();
            foreach (UpdateCommand command in base.Vertices)
            {
                foreach (long num in command.OutputIdentifiers)
                {
                    try
                    {
                        dictionary.Add(num, command);
                    }
                    catch (ArgumentException exception)
                    {
                        throw EntityUtil.Update(System.Data.Entity.Strings.Update_AmbiguousServerGenIdentifier, exception, command.GetStateEntries(this._translator));
                    }
                }
            }
            foreach (UpdateCommand command2 in base.Vertices)
            {
                foreach (long num2 in command2.InputIdentifiers)
                {
                    UpdateCommand command3;
                    if (dictionary.TryGetValue(num2, out command3))
                    {
                        base.AddEdge(command3, command2);
                    }
                }
            }
        }

        private KeyToListMap<ForeignKeyValue, UpdateCommand> DetermineForeignKeyPredecessors()
        {
            KeyToListMap<ForeignKeyValue, UpdateCommand> map = new KeyToListMap<ForeignKeyValue, UpdateCommand>(this._keyComparer);
            foreach (DynamicUpdateCommand command in base.Vertices.OfType<DynamicUpdateCommand>())
            {
                if ((command.Operator == ModificationOperator.Update) || (ModificationOperator.Insert == command.Operator))
                {
                    foreach (ReferentialConstraint constraint in this._targetMap.EnumerateValues(command.Table))
                    {
                        ForeignKeyValue value2;
                        ForeignKeyValue value3;
                        if (ForeignKeyValue.TryCreateTargetKey(constraint, command.CurrentValues, true, out value2) && (((command.Operator != ModificationOperator.Update) || !ForeignKeyValue.TryCreateTargetKey(constraint, command.OriginalValues, true, out value3)) || !this._keyComparer.Equals(value3, value2)))
                        {
                            map.Add(value2, command);
                        }
                    }
                }
                if ((command.Operator == ModificationOperator.Update) || (ModificationOperator.Delete == command.Operator))
                {
                    foreach (ReferentialConstraint constraint2 in this._sourceMap.EnumerateValues(command.Table))
                    {
                        ForeignKeyValue value4;
                        ForeignKeyValue value5;
                        if (ForeignKeyValue.TryCreateSourceKey(constraint2, command.OriginalValues, false, out value4) && (((command.Operator != ModificationOperator.Update) || !ForeignKeyValue.TryCreateSourceKey(constraint2, command.CurrentValues, false, out value5)) || !this._keyComparer.Equals(value5, value4)))
                        {
                            map.Add(value4, command);
                        }
                    }
                }
            }
            return map;
        }

        private static void InitializeForeignKeyMaps(HashSet<EntityContainer> containers, HashSet<EntitySet> tables, out KeyToListMap<EntitySetBase, ReferentialConstraint> sourceMap, out KeyToListMap<EntitySetBase, ReferentialConstraint> targetMap)
        {
            sourceMap = new KeyToListMap<EntitySetBase, ReferentialConstraint>(EqualityComparer<EntitySetBase>.Default);
            targetMap = new KeyToListMap<EntitySetBase, ReferentialConstraint>(EqualityComparer<EntitySetBase>.Default);
            foreach (EntityContainer container in containers)
            {
                foreach (EntitySetBase base2 in container.BaseEntitySets)
                {
                    AssociationSet set = base2 as AssociationSet;
                    if (set != null)
                    {
                        AssociationSetEnd end = null;
                        AssociationSetEnd end2 = null;
                        ReadOnlyMetadataCollection<AssociationSetEnd> associationSetEnds = set.AssociationSetEnds;
                        if (2 == associationSetEnds.Count)
                        {
                            AssociationType elementType = set.ElementType;
                            bool flag = false;
                            ReferentialConstraint constraint = null;
                            foreach (ReferentialConstraint constraint2 in elementType.ReferentialConstraints)
                            {
                                if (!flag)
                                {
                                    flag = true;
                                }
                                end = set.AssociationSetEnds[constraint2.ToRole.Name];
                                end2 = set.AssociationSetEnds[constraint2.FromRole.Name];
                                constraint = constraint2;
                            }
                            if (((end2 != null) && (end != null)) && (tables.Contains(end2.EntitySet) && tables.Contains(end.EntitySet)))
                            {
                                sourceMap.Add(end.EntitySet, constraint);
                                targetMap.Add(end2.EntitySet, constraint);
                            }
                        }
                    }
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct ForeignKeyValue
        {
            internal readonly ReferentialConstraint Metadata;
            internal readonly CompositeKey Key;
            internal readonly bool IsInsert;
            private ForeignKeyValue(ReferentialConstraint metadata, PropagatorResult record, bool isTarget, bool isInsert)
            {
                this.Metadata = metadata;
                IList<EdmProperty> list = isTarget ? metadata.FromProperties : metadata.ToProperties;
                PropagatorResult[] constants = new PropagatorResult[list.Count];
                bool flag = false;
                for (int i = 0; i < constants.Length; i++)
                {
                    constants[i] = record.GetMemberValue(list[i]);
                    if (constants[i].IsNull)
                    {
                        flag = true;
                        break;
                    }
                }
                if (flag)
                {
                    this.Key = null;
                }
                else
                {
                    this.Key = new CompositeKey(constants);
                }
                this.IsInsert = isInsert;
            }

            internal static bool TryCreateTargetKey(ReferentialConstraint metadata, PropagatorResult record, bool isInsert, out UpdateCommandOrderer.ForeignKeyValue key)
            {
                key = new UpdateCommandOrderer.ForeignKeyValue(metadata, record, true, isInsert);
                if (key.Key == null)
                {
                    return false;
                }
                return true;
            }

            internal static bool TryCreateSourceKey(ReferentialConstraint metadata, PropagatorResult record, bool isInsert, out UpdateCommandOrderer.ForeignKeyValue key)
            {
                key = new UpdateCommandOrderer.ForeignKeyValue(metadata, record, false, isInsert);
                if (key.Key == null)
                {
                    return false;
                }
                return true;
            }
        }

        private class ForeignKeyValueComparer : IEqualityComparer<UpdateCommandOrderer.ForeignKeyValue>
        {
            private readonly IEqualityComparer<CompositeKey> _baseComparer;

            internal ForeignKeyValueComparer(IEqualityComparer<CompositeKey> baseComparer)
            {
                this._baseComparer = EntityUtil.CheckArgumentNull<IEqualityComparer<CompositeKey>>(baseComparer, "baseComparer");
            }

            public bool Equals(UpdateCommandOrderer.ForeignKeyValue x, UpdateCommandOrderer.ForeignKeyValue y) => 
                (((x.IsInsert == y.IsInsert) && (x.Metadata == y.Metadata)) && this._baseComparer.Equals(x.Key, y.Key));

            public int GetHashCode(UpdateCommandOrderer.ForeignKeyValue obj) => 
                this._baseComparer.GetHashCode(obj.Key);
        }
    }
}

