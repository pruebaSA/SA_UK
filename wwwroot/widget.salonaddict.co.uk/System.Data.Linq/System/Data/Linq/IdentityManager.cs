namespace System.Data.Linq
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Linq.Mapping;
    using System.Data.Linq.SqlClient;
    using System.Reflection;
    using System.Runtime.InteropServices;

    internal abstract class IdentityManager
    {
        protected IdentityManager()
        {
        }

        internal static IdentityManager CreateIdentityManager(bool asReadOnly)
        {
            if (asReadOnly)
            {
                return new ReadOnlyIdentityManager();
            }
            return new StandardIdentityManager();
        }

        internal abstract object Find(MetaType type, object[] keyValues);
        internal abstract object FindLike(MetaType type, object instance);
        internal abstract object InsertLookup(MetaType type, object instance);
        internal abstract bool RemoveLike(MetaType type, object instance);

        private class ReadOnlyIdentityManager : IdentityManager
        {
            internal ReadOnlyIdentityManager()
            {
            }

            internal override object Find(MetaType type, object[] keyValues) => 
                null;

            internal override object FindLike(MetaType type, object instance) => 
                null;

            internal override object InsertLookup(MetaType type, object instance) => 
                instance;

            internal override bool RemoveLike(MetaType type, object instance) => 
                false;
        }

        private class StandardIdentityManager : IdentityManager
        {
            private Dictionary<MetaType, IdentityCache> caches = new Dictionary<MetaType, IdentityCache>();
            private IdentityCache currentCache;
            private MetaType currentType;

            internal StandardIdentityManager()
            {
            }

            internal override object Find(MetaType type, object[] keyValues)
            {
                this.SetCurrent(type);
                return this.currentCache.Find(keyValues);
            }

            internal override object FindLike(MetaType type, object instance)
            {
                this.SetCurrent(type);
                return this.currentCache.FindLike(instance);
            }

            private static KeyManager GetKeyManager(MetaType type)
            {
                int count = type.IdentityMembers.Count;
                MetaDataMember member = type.IdentityMembers[0];
                KeyManager manager = (KeyManager) Activator.CreateInstance(typeof(SingleKeyManager).MakeGenericType(new Type[] { type.Type, member.Type }), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, new object[] { member.StorageAccessor, 0 }, null);
                for (int i = 1; i < count; i++)
                {
                    member = type.IdentityMembers[i];
                    manager = (KeyManager) Activator.CreateInstance(typeof(MultiKeyManager).MakeGenericType(new Type[] { type.Type, member.Type, manager.KeyType }), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, new object[] { member.StorageAccessor, i, manager }, null);
                }
                return manager;
            }

            internal override object InsertLookup(MetaType type, object instance)
            {
                this.SetCurrent(type);
                return this.currentCache.InsertLookup(instance);
            }

            internal override bool RemoveLike(MetaType type, object instance)
            {
                this.SetCurrent(type);
                return this.currentCache.RemoveLike(instance);
            }

            private void SetCurrent(MetaType type)
            {
                type = type.InheritanceRoot;
                if (this.currentType != type)
                {
                    if (!this.caches.TryGetValue(type, out this.currentCache))
                    {
                        KeyManager keyManager = GetKeyManager(type);
                        this.currentCache = (IdentityCache) Activator.CreateInstance(typeof(IdentityCache).MakeGenericType(new Type[] { type.Type, keyManager.KeyType }), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, new object[] { keyManager }, null);
                        this.caches.Add(type, this.currentCache);
                    }
                    this.currentType = type;
                }
            }

            internal abstract class IdentityCache
            {
                protected IdentityCache()
                {
                }

                internal abstract object Find(object[] keyValues);
                internal abstract object FindLike(object instance);
                internal abstract object InsertLookup(object instance);
                internal abstract bool RemoveLike(object instance);
            }

            internal class IdentityCache<T, K> : IdentityManager.StandardIdentityManager.IdentityCache
            {
                private int[] buckets;
                private IEqualityComparer<K> comparer;
                private int count;
                private int freeList;
                private IdentityManager.StandardIdentityManager.KeyManager<T, K> keyManager;
                private Slot<T, K>[] slots;

                public IdentityCache(IdentityManager.StandardIdentityManager.KeyManager<T, K> keyManager)
                {
                    this.keyManager = keyManager;
                    this.comparer = keyManager.Comparer;
                    this.buckets = new int[7];
                    this.slots = new Slot<T, K>[7];
                    this.freeList = -1;
                }

                internal override object Find(object[] keyValues)
                {
                    K local;
                    if (this.keyManager.TryCreateKeyFromValues(keyValues, out local))
                    {
                        T local2 = default(T);
                        if (this.Find(local, ref local2, false))
                        {
                            return local2;
                        }
                    }
                    return null;
                }

                private bool Find(K key, ref T value, bool add)
                {
                    int num = this.comparer.GetHashCode(key) & 0x7fffffff;
                    for (int i = this.buckets[num % this.buckets.Length] - 1; i >= 0; i = this.slots[i].next)
                    {
                        if ((this.slots[i].hashCode == num) && this.comparer.Equals(this.slots[i].key, key))
                        {
                            value = this.slots[i].value;
                            return true;
                        }
                    }
                    if (add)
                    {
                        int freeList;
                        if (this.freeList >= 0)
                        {
                            freeList = this.freeList;
                            this.freeList = this.slots[freeList].next;
                        }
                        else
                        {
                            if (this.count == this.slots.Length)
                            {
                                this.Resize();
                            }
                            freeList = this.count;
                            this.count++;
                        }
                        int index = num % this.buckets.Length;
                        this.slots[freeList].hashCode = num;
                        this.slots[freeList].key = key;
                        this.slots[freeList].value = value;
                        this.slots[freeList].next = this.buckets[index] - 1;
                        this.buckets[index] = freeList + 1;
                    }
                    return false;
                }

                internal override object FindLike(object instance)
                {
                    T local = (T) instance;
                    K key = this.keyManager.CreateKeyFromInstance(local);
                    if (this.Find(key, ref local, false))
                    {
                        return local;
                    }
                    return null;
                }

                internal override object InsertLookup(object instance)
                {
                    T local = (T) instance;
                    K key = this.keyManager.CreateKeyFromInstance(local);
                    this.Find(key, ref local, true);
                    return local;
                }

                internal override bool RemoveLike(object instance)
                {
                    T local = (T) instance;
                    K local2 = this.keyManager.CreateKeyFromInstance(local);
                    int num = this.comparer.GetHashCode(local2) & 0x7fffffff;
                    int index = num % this.buckets.Length;
                    int num3 = -1;
                    for (int i = this.buckets[index] - 1; i >= 0; i = this.slots[i].next)
                    {
                        if ((this.slots[i].hashCode == num) && this.comparer.Equals(this.slots[i].key, local2))
                        {
                            if (num3 < 0)
                            {
                                this.buckets[index] = this.slots[i].next + 1;
                            }
                            else
                            {
                                this.slots[num3].next = this.slots[i].next;
                            }
                            this.slots[i].hashCode = -1;
                            this.slots[i].value = default(T);
                            this.slots[i].next = this.freeList;
                            this.freeList = i;
                            return true;
                        }
                        num3 = i;
                    }
                    return false;
                }

                private void Resize()
                {
                    int num = (this.count * 2) + 1;
                    int[] numArray = new int[num];
                    Slot<T, K>[] destinationArray = new Slot<T, K>[num];
                    Array.Copy(this.slots, 0, destinationArray, 0, this.count);
                    for (int i = 0; i < this.count; i++)
                    {
                        int index = destinationArray[i].hashCode % num;
                        destinationArray[i].next = numArray[index] - 1;
                        numArray[index] = i + 1;
                    }
                    this.buckets = numArray;
                    this.slots = destinationArray;
                }

                [StructLayout(LayoutKind.Sequential)]
                internal struct Slot
                {
                    internal int hashCode;
                    internal K key;
                    internal T value;
                    internal int next;
                }
            }

            internal abstract class KeyManager
            {
                protected KeyManager()
                {
                }

                internal abstract Type KeyType { get; }
            }

            internal abstract class KeyManager<T, K> : IdentityManager.StandardIdentityManager.KeyManager
            {
                protected KeyManager()
                {
                }

                internal abstract K CreateKeyFromInstance(T instance);
                internal abstract bool TryCreateKeyFromValues(object[] values, out K k);

                internal abstract IEqualityComparer<K> Comparer { get; }
            }

            [StructLayout(LayoutKind.Sequential)]
            internal struct MultiKey<T1, T2>
            {
                private T1 value1;
                private T2 value2;
                internal MultiKey(T1 value1, T2 value2)
                {
                    this.value1 = value1;
                    this.value2 = value2;
                }
                internal class Comparer : IEqualityComparer<IdentityManager.StandardIdentityManager.MultiKey<T1, T2>>, IEqualityComparer
                {
                    private IEqualityComparer<T1> comparer1;
                    private IEqualityComparer<T2> comparer2;

                    internal Comparer(IEqualityComparer<T1> comparer1, IEqualityComparer<T2> comparer2)
                    {
                        this.comparer1 = comparer1;
                        this.comparer2 = comparer2;
                    }

                    public bool Equals(IdentityManager.StandardIdentityManager.MultiKey<T1, T2> x, IdentityManager.StandardIdentityManager.MultiKey<T1, T2> y) => 
                        (this.comparer1.Equals(x.value1, y.value1) && this.comparer2.Equals(x.value2, y.value2));

                    public int GetHashCode(IdentityManager.StandardIdentityManager.MultiKey<T1, T2> x) => 
                        (this.comparer1.GetHashCode(x.value1) ^ this.comparer2.GetHashCode(x.value2));

                    bool IEqualityComparer.Equals(object x, object y) => 
                        this.Equals((IdentityManager.StandardIdentityManager.MultiKey<T1, T2>) x, (IdentityManager.StandardIdentityManager.MultiKey<T1, T2>) y);

                    int IEqualityComparer.GetHashCode(object x) => 
                        this.GetHashCode((IdentityManager.StandardIdentityManager.MultiKey<T1, T2>) x);
                }
            }

            internal class MultiKeyManager<T, V1, V2> : IdentityManager.StandardIdentityManager.KeyManager<T, IdentityManager.StandardIdentityManager.MultiKey<V1, V2>>
            {
                private MetaAccessor<T, V1> accessor;
                private IEqualityComparer<IdentityManager.StandardIdentityManager.MultiKey<V1, V2>> comparer;
                private IdentityManager.StandardIdentityManager.KeyManager<T, V2> next;
                private int offset;

                internal MultiKeyManager(MetaAccessor<T, V1> accessor, int offset, IdentityManager.StandardIdentityManager.KeyManager<T, V2> next)
                {
                    this.accessor = accessor;
                    this.next = next;
                    this.offset = offset;
                }

                internal override IdentityManager.StandardIdentityManager.MultiKey<V1, V2> CreateKeyFromInstance(T instance) => 
                    new IdentityManager.StandardIdentityManager.MultiKey<V1, V2>(this.accessor.GetValue(instance), this.next.CreateKeyFromInstance(instance));

                internal override bool TryCreateKeyFromValues(object[] values, out IdentityManager.StandardIdentityManager.MultiKey<V1, V2> k)
                {
                    V2 local;
                    object obj2 = values[this.offset];
                    if ((obj2 == null) && typeof(V1).IsValueType)
                    {
                        k = new IdentityManager.StandardIdentityManager.MultiKey<V1, V2>();
                        return false;
                    }
                    if (!this.next.TryCreateKeyFromValues(values, out local))
                    {
                        k = new IdentityManager.StandardIdentityManager.MultiKey<V1, V2>();
                        return false;
                    }
                    k = new IdentityManager.StandardIdentityManager.MultiKey<V1, V2>((V1) obj2, local);
                    return true;
                }

                internal override IEqualityComparer<IdentityManager.StandardIdentityManager.MultiKey<V1, V2>> Comparer
                {
                    get
                    {
                        if (this.comparer == null)
                        {
                            this.comparer = new IdentityManager.StandardIdentityManager.MultiKey<V1, V2>.Comparer(EqualityComparer<V1>.Default, this.next.Comparer);
                        }
                        return this.comparer;
                    }
                }

                internal override Type KeyType =>
                    typeof(IdentityManager.StandardIdentityManager.MultiKey<V1, V2>);
            }

            internal class SingleKeyManager<T, V> : IdentityManager.StandardIdentityManager.KeyManager<T, V>
            {
                private MetaAccessor<T, V> accessor;
                private IEqualityComparer<V> comparer;
                private bool isKeyNullAssignable;
                private int offset;

                internal SingleKeyManager(MetaAccessor<T, V> accessor, int offset)
                {
                    this.accessor = accessor;
                    this.offset = offset;
                    this.isKeyNullAssignable = TypeSystem.IsNullAssignable(typeof(V));
                }

                internal override V CreateKeyFromInstance(T instance) => 
                    this.accessor.GetValue(instance);

                internal override bool TryCreateKeyFromValues(object[] values, out V v)
                {
                    object obj2 = values[this.offset];
                    if ((obj2 == null) && !this.isKeyNullAssignable)
                    {
                        v = default(V);
                        return false;
                    }
                    v = (V) obj2;
                    return true;
                }

                internal override IEqualityComparer<V> Comparer
                {
                    get
                    {
                        if (this.comparer == null)
                        {
                            this.comparer = EqualityComparer<V>.Default;
                        }
                        return this.comparer;
                    }
                }

                internal override Type KeyType =>
                    typeof(V);
            }
        }
    }
}

