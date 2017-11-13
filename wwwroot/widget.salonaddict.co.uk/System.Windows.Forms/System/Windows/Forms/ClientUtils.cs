namespace System.Windows.Forms
{
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Security;
    using System.Threading;

    internal static class ClientUtils
    {
        public static int GetBitCount(uint x)
        {
            int num = 0;
            while (x > 0)
            {
                x &= x - 1;
                num++;
            }
            return num;
        }

        public static bool IsCriticalException(Exception ex) => 
            (((((ex is NullReferenceException) || (ex is StackOverflowException)) || ((ex is OutOfMemoryException) || (ex is ThreadAbortException))) || ((ex is ExecutionEngineException) || (ex is IndexOutOfRangeException))) || (ex is AccessViolationException));

        public static bool IsEnumValid(Enum enumValue, int value, int minValue, int maxValue) => 
            ((value >= minValue) && (value <= maxValue));

        public static bool IsEnumValid(Enum enumValue, int value, int minValue, int maxValue, int maxNumberOfBitsOn) => 
            (((value >= minValue) && (value <= maxValue)) && (GetBitCount((uint) value) <= maxNumberOfBitsOn));

        public static bool IsEnumValid_Masked(Enum enumValue, int value, uint mask) => 
            ((value & mask) == value);

        public static bool IsEnumValid_NotSequential(Enum enumValue, int value, params int[] enumValues)
        {
            for (int i = 0; i < enumValues.Length; i++)
            {
                if (enumValues[i] == value)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsSecurityOrCriticalException(Exception ex) => 
            ((ex is SecurityException) || IsCriticalException(ex));

        internal class WeakRefCollection : IList, ICollection, IEnumerable
        {
            private ArrayList _innerList;
            private int refCheckThreshold;

            internal WeakRefCollection()
            {
                this.refCheckThreshold = 0x7fffffff;
                this._innerList = new ArrayList(4);
            }

            internal WeakRefCollection(int size)
            {
                this.refCheckThreshold = 0x7fffffff;
                this._innerList = new ArrayList(size);
            }

            public int Add(object value)
            {
                if (this.Count > this.RefCheckThreshold)
                {
                    this.ScavengeReferences();
                }
                return this.InnerList.Add(this.CreateWeakRefObject(value));
            }

            public void Clear()
            {
                this.InnerList.Clear();
            }

            public bool Contains(object value) => 
                this.InnerList.Contains(this.CreateWeakRefObject(value));

            private static void Copy(System.Windows.Forms.ClientUtils.WeakRefCollection sourceList, int sourceIndex, System.Windows.Forms.ClientUtils.WeakRefCollection destinationList, int destinationIndex, int length)
            {
                if (sourceIndex < destinationIndex)
                {
                    sourceIndex += length;
                    destinationIndex += length;
                    while (length > 0)
                    {
                        destinationList.InnerList[--destinationIndex] = sourceList.InnerList[--sourceIndex];
                        length--;
                    }
                }
                else
                {
                    while (length > 0)
                    {
                        destinationList.InnerList[destinationIndex++] = sourceList.InnerList[sourceIndex++];
                        length--;
                    }
                }
            }

            public void CopyTo(Array array, int index)
            {
                this.InnerList.CopyTo(array, index);
            }

            private WeakRefObject CreateWeakRefObject(object value)
            {
                if (value == null)
                {
                    return null;
                }
                return new WeakRefObject(value);
            }

            public override bool Equals(object obj)
            {
                System.Windows.Forms.ClientUtils.WeakRefCollection refs = obj as System.Windows.Forms.ClientUtils.WeakRefCollection;
                if (refs != this)
                {
                    if ((refs == null) || (this.Count != refs.Count))
                    {
                        return false;
                    }
                    for (int i = 0; i < this.Count; i++)
                    {
                        if ((this.InnerList[i] != refs.InnerList[i]) && ((this.InnerList[i] == null) || !this.InnerList[i].Equals(refs.InnerList[i])))
                        {
                            return false;
                        }
                    }
                }
                return true;
            }

            public IEnumerator GetEnumerator() => 
                this.InnerList.GetEnumerator();

            public override int GetHashCode() => 
                base.GetHashCode();

            public int IndexOf(object value) => 
                this.InnerList.IndexOf(this.CreateWeakRefObject(value));

            public void Insert(int index, object value)
            {
                this.InnerList.Insert(index, this.CreateWeakRefObject(value));
            }

            public void Remove(object value)
            {
                this.InnerList.Remove(this.CreateWeakRefObject(value));
            }

            public void RemoveAt(int index)
            {
                this.InnerList.RemoveAt(index);
            }

            public void RemoveByHashCode(object value)
            {
                if (value != null)
                {
                    int hashCode = value.GetHashCode();
                    for (int i = 0; i < this.InnerList.Count; i++)
                    {
                        if ((this.InnerList[i] != null) && (this.InnerList[i].GetHashCode() == hashCode))
                        {
                            this.RemoveAt(i);
                            return;
                        }
                    }
                }
            }

            public void ScavengeReferences()
            {
                int index = 0;
                int count = this.Count;
                for (int i = 0; i < count; i++)
                {
                    if (this[index] == null)
                    {
                        this.InnerList.RemoveAt(index);
                    }
                    else
                    {
                        index++;
                    }
                }
            }

            public int Count =>
                this.InnerList.Count;

            internal ArrayList InnerList =>
                this._innerList;

            public bool IsFixedSize =>
                this.InnerList.IsFixedSize;

            public bool IsReadOnly =>
                this.InnerList.IsReadOnly;

            public object this[int index]
            {
                get
                {
                    WeakRefObject obj2 = this.InnerList[index] as WeakRefObject;
                    if ((obj2 != null) && obj2.IsAlive)
                    {
                        return obj2.Target;
                    }
                    return null;
                }
                set
                {
                    this.InnerList[index] = this.CreateWeakRefObject(value);
                }
            }

            public int RefCheckThreshold
            {
                get => 
                    this.refCheckThreshold;
                set
                {
                    this.refCheckThreshold = value;
                }
            }

            bool ICollection.IsSynchronized =>
                this.InnerList.IsSynchronized;

            object ICollection.SyncRoot =>
                this.InnerList.SyncRoot;

            internal class WeakRefObject
            {
                private int hash;
                private WeakReference weakHolder;

                internal WeakRefObject(object obj)
                {
                    this.weakHolder = new WeakReference(obj);
                    this.hash = obj.GetHashCode();
                }

                public override bool Equals(object obj)
                {
                    System.Windows.Forms.ClientUtils.WeakRefCollection.WeakRefObject obj2 = obj as System.Windows.Forms.ClientUtils.WeakRefCollection.WeakRefObject;
                    return ((obj2 == this) || ((obj2?.Target == this.Target) || ((this.Target != null) && this.Target.Equals(obj2?.Target))));
                }

                public override int GetHashCode() => 
                    this.hash;

                internal bool IsAlive =>
                    this.weakHolder.IsAlive;

                internal object Target =>
                    this.weakHolder.Target;
            }
        }
    }
}

