namespace System.Data.Mapping.Update.Internal
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Data.Common.Utils;
    using System.Data.Entity;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;

    internal class KeyManager
    {
        private Partitioner _commonValuePartitioner;
        private readonly KeyToListMap<long, IEntityStateEntry> _dependentStateEntries;
        private long _identifier;
        private readonly Dictionary<long, PropagatorResult> _identifierOwners;
        private readonly KeyToListMap<long, long> _identifierRefConstraints;
        private readonly Dictionary<EntityKey, long> _keyIdentifiers;
        private readonly UpdateTranslator _translator;
        private const byte Black = 1;
        private const byte Gray = 2;

        internal KeyManager(UpdateTranslator translator)
        {
            this._translator = EntityUtil.CheckArgumentNull<UpdateTranslator>(translator, "translator");
            this._keyIdentifiers = new Dictionary<EntityKey, long>(EqualityComparer<EntityKey>.Default);
            this._identifierRefConstraints = new KeyToListMap<long, long>(EqualityComparer<long>.Default);
            this._dependentStateEntries = new KeyToListMap<long, IEntityStateEntry>(EqualityComparer<long>.Default);
            this._identifierOwners = new Dictionary<long, PropagatorResult>();
            this._commonValuePartitioner = new Partitioner();
        }

        internal void AddReferentialConstraint(IEntityStateEntry dependentStateEntry, long dependentIdentifier, long principalIdentifier)
        {
            if (dependentIdentifier != principalIdentifier)
            {
                if (this._commonValuePartitioner == null)
                {
                    this._commonValuePartitioner = new Partitioner();
                }
                this._commonValuePartitioner.AssociateNodes(dependentIdentifier, principalIdentifier);
                this._identifierRefConstraints.Add(dependentIdentifier, principalIdentifier);
            }
            this._dependentStateEntries.Add(dependentIdentifier, dependentStateEntry);
        }

        internal long GetCanonicalIdentifier(long identifier) => 
            this._commonValuePartitioner?.GetPartitionId(identifier);

        internal IEnumerable<IEntityStateEntry> GetDependentStateEntries(long identifier) => 
            this._dependentStateEntries.EnumerateValues(identifier);

        internal long GetKeyIdentifierForMemberOffset(EntityKey entityKey, int memberOffset, int keyMemberCount)
        {
            long num;
            if (!this._keyIdentifiers.TryGetValue(entityKey, out num))
            {
                num = this._identifier;
                this._identifier += keyMemberCount;
                this._keyIdentifiers.Add(entityKey, num);
            }
            return (num + memberOffset);
        }

        internal IEnumerable<long> GetPrincipals(long identifier)
        {
            Stack<long> iteratorVariable0 = new Stack<long>();
            iteratorVariable0.Push(identifier);
        Label_PostSwitchInIterator:;
            while (iteratorVariable0.Count > 0)
            {
                ReadOnlyCollection<long> iteratorVariable2;
                long key = iteratorVariable0.Pop();
                if (this._identifierRefConstraints.TryGetListForKey(key, out iteratorVariable2))
                {
                    foreach (long num in iteratorVariable2)
                    {
                        iteratorVariable0.Push(num);
                    }
                }
                else
                {
                    yield return key;
                    goto Label_PostSwitchInIterator;
                }
            }
        }

        internal object GetPrincipalValue(PropagatorResult result)
        {
            long identifier = result.Identifier;
            if (-1L == identifier)
            {
                return result.GetSimpleValue();
            }
            bool flag = true;
            object x = null;
            foreach (long num2 in this.GetPrincipals(identifier))
            {
                PropagatorResult result2;
                if (this._identifierOwners.TryGetValue(num2, out result2))
                {
                    if (flag)
                    {
                        x = result2.GetSimpleValue();
                        flag = false;
                    }
                    else if (!CdpEqualityComparer.DefaultEqualityComparer.Equals(x, result2.GetSimpleValue()))
                    {
                        throw EntityUtil.Constraint(Strings.Update_ReferentialConstraintIntegrityViolation);
                    }
                }
            }
            if (flag)
            {
                x = result.GetSimpleValue();
            }
            return x;
        }

        internal bool HasPrincipals(long identifier) => 
            this._identifierRefConstraints.ContainsKey(identifier);

        internal void RegisterIdentifierOwner(PropagatorResult owner)
        {
            this._identifierOwners[owner.Identifier] = owner;
        }

        internal bool TryGetIdentifierOwner(long identifier, out PropagatorResult owner) => 
            this._identifierOwners.TryGetValue(identifier, out owner);

        internal void ValidateReferentialIntegrityGraphAcyclic()
        {
            Dictionary<long, byte> color = new Dictionary<long, byte>();
            foreach (long num in this._identifierRefConstraints.Keys)
            {
                if (!color.ContainsKey(num))
                {
                    this.ValidateReferentialIntegrityGraphAcyclic(num, color, null);
                }
            }
        }

        private void ValidateReferentialIntegrityGraphAcyclic(long node, Dictionary<long, byte> color, IdentifierPath parent)
        {
            color[node] = 2;
            IdentifierPath path = new IdentifierPath(node, parent);
            foreach (long num in this._identifierRefConstraints.EnumerateValues(node))
            {
                byte num2;
                if (color.TryGetValue(num, out num2))
                {
                    if (num2 != 2)
                    {
                        continue;
                    }
                    List<IEntityStateEntry> stateEntries = new List<IEntityStateEntry>();
                    foreach (long num3 in path.GetPath())
                    {
                        PropagatorResult result;
                        if (this._identifierOwners.TryGetValue(num3, out result))
                        {
                            stateEntries.Add(result.StateEntry);
                        }
                        if (num3 == num)
                        {
                            break;
                        }
                    }
                    throw EntityUtil.Update(Strings.Update_CircularRelationships, null, stateEntries);
                }
                this.ValidateReferentialIntegrityGraphAcyclic(num, color, path);
            }
            color[node] = 1;
        }


        private sealed class IdentifierPath
        {
            private readonly long _identifier;
            private readonly KeyManager.IdentifierPath _parent;

            internal IdentifierPath(long identifier, KeyManager.IdentifierPath parent)
            {
                this._identifier = identifier;
                this._parent = parent;
            }

            internal IEnumerable<long> GetPath()
            {
                KeyManager.IdentifierPath iteratorVariable0 = this;
                while (true)
                {
                    if (iteratorVariable0 == null)
                    {
                        yield break;
                    }
                    yield return iteratorVariable0._identifier;
                    iteratorVariable0 = iteratorVariable0._parent;
                }
            }

        }

        private class Partitioner
        {
            private readonly Dictionary<long, Partition> _nodeIdToPartitionMap = new Dictionary<long, Partition>();

            internal Partitioner()
            {
            }

            internal void AssociateNodes(long firstId, long secondId)
            {
                if (firstId != secondId)
                {
                    Partition partition;
                    if (this._nodeIdToPartitionMap.TryGetValue(firstId, out partition))
                    {
                        Partition partition2;
                        if (this._nodeIdToPartitionMap.TryGetValue(secondId, out partition2))
                        {
                            partition.Merge(this, partition2);
                        }
                        else
                        {
                            partition.AddNode(this, secondId);
                        }
                    }
                    else
                    {
                        Partition partition3;
                        if (this._nodeIdToPartitionMap.TryGetValue(secondId, out partition3))
                        {
                            partition3.AddNode(this, firstId);
                        }
                        else
                        {
                            Partition.CreatePartition(this, firstId, secondId);
                        }
                    }
                }
            }

            internal long GetPartitionId(long id)
            {
                Partition partition;
                if (this._nodeIdToPartitionMap.TryGetValue(id, out partition))
                {
                    return partition.PartitionId;
                }
                return id;
            }

            private class Partition
            {
                private List<long> _nodeIds = new List<long>(2);
                internal readonly long PartitionId;

                private Partition(long partitionId)
                {
                    this.PartitionId = partitionId;
                }

                internal void AddNode(KeyManager.Partitioner partitioner, long nodeId)
                {
                    this._nodeIds.Add(nodeId);
                    partitioner._nodeIdToPartitionMap[nodeId] = this;
                }

                internal static void CreatePartition(KeyManager.Partitioner partitioner, long firstId, long secondId)
                {
                    KeyManager.Partitioner.Partition partition = new KeyManager.Partitioner.Partition(firstId);
                    partition.AddNode(partitioner, firstId);
                    partition.AddNode(partitioner, secondId);
                }

                internal void Merge(KeyManager.Partitioner partitioner, KeyManager.Partitioner.Partition other)
                {
                    if (other.PartitionId != this.PartitionId)
                    {
                        foreach (long num in other._nodeIds)
                        {
                            this.AddNode(partitioner, num);
                        }
                    }
                }
            }
        }
    }
}

