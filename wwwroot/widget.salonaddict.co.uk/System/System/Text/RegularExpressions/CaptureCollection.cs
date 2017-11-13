﻿namespace System.Text.RegularExpressions
{
    using System;
    using System.Collections;
    using System.Reflection;

    [Serializable]
    public class CaptureCollection : ICollection, IEnumerable
    {
        internal int _capcount;
        internal Capture[] _captures;
        internal Group _group;

        internal CaptureCollection(Group group)
        {
            this._group = group;
            this._capcount = this._group._capcount;
        }

        public void CopyTo(Array array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            int index = arrayIndex;
            for (int i = 0; i < this.Count; i++)
            {
                array.SetValue(this[i], index);
                index++;
            }
        }

        internal Capture GetCapture(int i)
        {
            if ((i == (this._capcount - 1)) && (i >= 0))
            {
                return this._group;
            }
            if ((i >= this._capcount) || (i < 0))
            {
                throw new ArgumentOutOfRangeException("i");
            }
            if (this._captures == null)
            {
                this._captures = new Capture[this._capcount];
                for (int j = 0; j < (this._capcount - 1); j++)
                {
                    this._captures[j] = new Capture(this._group._text, this._group._caps[j * 2], this._group._caps[(j * 2) + 1]);
                }
            }
            return this._captures[i];
        }

        public IEnumerator GetEnumerator() => 
            new CaptureEnumerator(this);

        public int Count =>
            this._capcount;

        public bool IsReadOnly =>
            true;

        public bool IsSynchronized =>
            false;

        public Capture this[int i] =>
            this.GetCapture(i);

        public object SyncRoot =>
            this._group;
    }
}

