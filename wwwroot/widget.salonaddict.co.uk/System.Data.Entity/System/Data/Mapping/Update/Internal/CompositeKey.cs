namespace System.Data.Mapping.Update.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common.Utils;

    internal class CompositeKey
    {
        internal readonly KeyValuePair<PropagatorResult, long>[] KeyComponents;

        internal CompositeKey(PropagatorResult[] constants)
        {
            this.KeyComponents = new KeyValuePair<PropagatorResult, long>[constants.Length];
            for (int i = 0; i < constants.Length; i++)
            {
                PropagatorResult key = constants[i];
                long identifier = key.Identifier;
                this.KeyComponents[i] = new KeyValuePair<PropagatorResult, long>(key, identifier);
            }
        }

        internal static IEqualityComparer<CompositeKey> CreateComparer(KeyManager keyManager) => 
            new CompositeKeyComparer(keyManager);

        private class CompositeKeyComparer : IEqualityComparer<CompositeKey>
        {
            private readonly KeyManager _manager;

            internal CompositeKeyComparer(KeyManager manager)
            {
                this._manager = EntityUtil.CheckArgumentNull<KeyManager>(manager, "manager");
            }

            public bool Equals(CompositeKey left, CompositeKey right)
            {
                if (!object.ReferenceEquals(left, right))
                {
                    if ((left == null) || (right == null))
                    {
                        return false;
                    }
                    if (left.KeyComponents.Length != right.KeyComponents.Length)
                    {
                        return false;
                    }
                    for (int i = 0; i < left.KeyComponents.Length; i++)
                    {
                        object x = this.GetValue(left.KeyComponents[i]);
                        object y = this.GetValue(right.KeyComponents[i]);
                        if (!CdpEqualityComparer.DefaultEqualityComparer.Equals(x, y))
                        {
                            return false;
                        }
                    }
                }
                return true;
            }

            public int GetHashCode(CompositeKey key)
            {
                EntityUtil.CheckArgumentNull<CompositeKey>(key, "key");
                int num = 0;
                foreach (KeyValuePair<PropagatorResult, long> pair in key.KeyComponents)
                {
                    num ^= this.GetValue(pair).GetHashCode();
                }
                return num;
            }

            private object GetValue(KeyValuePair<PropagatorResult, long> keyComponent)
            {
                if (keyComponent.Value == -1L)
                {
                    return keyComponent.Key.GetSimpleValue();
                }
                return this._manager.GetCanonicalIdentifier(keyComponent.Value);
            }
        }
    }
}

