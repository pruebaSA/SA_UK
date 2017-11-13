namespace System.Data.Mapping.Update.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.EntityClient;
    using System.Data.Metadata.Edm;
    using System.Runtime.InteropServices;

    internal abstract class UpdateCommand : IComparable<UpdateCommand>, IEquatable<UpdateCommand>
    {
        protected UpdateCommand()
        {
        }

        public int CompareTo(UpdateCommand other)
        {
            if (this.Equals(other))
            {
                return 0;
            }
            int num = (int) (this.Kind - other.Kind);
            if (num != 0)
            {
                return num;
            }
            return this.CompareToType(other);
        }

        internal abstract int CompareToType(UpdateCommand other);
        public bool Equals(UpdateCommand other) => 
            base.Equals(other);

        public override bool Equals(object obj) => 
            base.Equals(obj);

        internal abstract int Execute(UpdateTranslator translator, EntityConnection connection, Dictionary<long, object> identifierValues, List<KeyValuePair<PropagatorResult, object>> generatedValues);
        public override int GetHashCode() => 
            base.GetHashCode();

        internal void GetRequiredAndProducedEntities(UpdateTranslator translator, out List<EntityKey> required, out List<EntityKey> produced)
        {
            List<IEntityStateEntry> stateEntries = this.GetStateEntries(translator);
            required = new List<EntityKey>();
            produced = new List<EntityKey>();
            EntityKey item = null;
            foreach (IEntityStateEntry entry in stateEntries)
            {
                if (!entry.IsRelationship)
                {
                    item = entry.EntityKey;
                    if (entry.State == EntityState.Added)
                    {
                        produced.Add(item);
                    }
                    else if (entry.State == EntityState.Deleted)
                    {
                        required.Add(item);
                    }
                    break;
                }
            }
            foreach (IEntityStateEntry entry2 in stateEntries)
            {
                if (entry2.IsRelationship)
                {
                    bool flag = entry2.State == EntityState.Added;
                    if (flag || (entry2.State == EntityState.Deleted))
                    {
                        DbDataRecord record = flag ? entry2.CurrentValues : entry2.OriginalValues;
                        EntityKey key2 = (EntityKey) record[0];
                        EntityKey key3 = (EntityKey) record[1];
                        List<EntityKey> list2 = flag ? required : produced;
                        if (item == null)
                        {
                            list2.Add(key2);
                            list2.Add(key3);
                        }
                        else if (item.Equals(key2))
                        {
                            list2.Add(key3);
                        }
                        else
                        {
                            list2.Add(key2);
                        }
                    }
                }
            }
        }

        internal abstract List<IEntityStateEntry> GetStateEntries(UpdateTranslator translator);

        internal abstract IEnumerable<long> InputIdentifiers { get; }

        internal abstract UpdateCommandKind Kind { get; }

        internal abstract IEnumerable<long> OutputIdentifiers { get; }

        internal virtual EntitySet Table =>
            null;
    }
}

