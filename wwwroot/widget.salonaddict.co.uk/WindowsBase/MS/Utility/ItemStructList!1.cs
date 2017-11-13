namespace MS.Utility
{
    using MS.Internal.WindowsBase;
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential), FriendAccessAllowed]
    internal struct ItemStructList<T>
    {
        public T[] List;
        public int Count;
        public ItemStructList(int capacity)
        {
            this.List = new T[capacity];
            this.Count = 0;
        }

        public void EnsureIndex(int index)
        {
            int delta = (index + 1) - this.Count;
            if (delta > 0)
            {
                this.Add(delta);
            }
        }

        public bool IsValidIndex(int index) => 
            ((index >= 0) && (index < this.Count));

        public int IndexOf(T value)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (this.List[i].Equals(value))
                {
                    return i;
                }
            }
            return -1;
        }

        public bool Contains(T value) => 
            (this.IndexOf(value) != -1);

        public void Add(T item)
        {
            int index = this.Add(1, false);
            this.List[index] = item;
            this.Count++;
        }

        public void Add(ref T item)
        {
            int index = this.Add(1, false);
            this.List[index] = item;
            this.Count++;
        }

        public int Add() => 
            this.Add(1, true);

        public int Add(int delta) => 
            this.Add(delta, true);

        private int Add(int delta, bool incrCount)
        {
            if (this.List != null)
            {
                if ((this.Count + delta) > this.List.Length)
                {
                    T[] array = new T[Math.Max((int) (this.List.Length * 2), (int) (this.Count + delta))];
                    this.List.CopyTo(array, 0);
                    this.List = array;
                }
            }
            else
            {
                this.List = new T[Math.Max(delta, 2)];
            }
            int count = this.Count;
            if (incrCount)
            {
                this.Count += delta;
            }
            return count;
        }

        public void Sort()
        {
            if (this.List != null)
            {
                Array.Sort<T>(this.List, 0, this.Count);
            }
        }

        public void AppendTo(ref ItemStructList<T> destinationList)
        {
            for (int i = 0; i < this.Count; i++)
            {
                destinationList.Add(ref this.List[i]);
            }
        }

        public T[] ToArray()
        {
            T[] destinationArray = new T[this.Count];
            Array.Copy(this.List, 0, destinationArray, 0, this.Count);
            return destinationArray;
        }

        public void Clear()
        {
            Array.Clear(this.List, 0, this.Count);
            this.Count = 0;
        }

        public void Remove(T value)
        {
            int index = this.IndexOf(value);
            if (index != -1)
            {
                Array.Copy(this.List, index + 1, this.List, index, (this.Count - index) - 1);
                Array.Clear(this.List, this.Count - 1, 1);
                this.Count--;
            }
        }
    }
}

