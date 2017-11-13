namespace System.Data.EntityModel.SchemaObjectModel
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;
    using System.Reflection;
    using System.Runtime.InteropServices;

    internal sealed class RelationshipEndCollection : IList<IRelationshipEnd>, ICollection<IRelationshipEnd>, IEnumerable<IRelationshipEnd>, IEnumerable
    {
        private Dictionary<string, IRelationshipEnd> _endLookup;
        private List<string> _keysInDefOrder;

        public void Add(IRelationshipEnd end)
        {
            SchemaElement element = end as SchemaElement;
            if (IsEndValid(end) && this.ValidateUniqueName(element, end.Name))
            {
                this.EndLookup.Add(end.Name, end);
                this.KeysInDefOrder.Add(end.Name);
            }
        }

        public void Clear()
        {
            this.EndLookup.Clear();
            this.KeysInDefOrder.Clear();
        }

        public bool Contains(IRelationshipEnd end) => 
            this.Contains(end.Name);

        public bool Contains(string name) => 
            this.EndLookup.ContainsKey(name);

        public void CopyTo(IRelationshipEnd[] ends, int index)
        {
            foreach (IRelationshipEnd end in this)
            {
                ends[index++] = end;
            }
        }

        public IEnumerator<IRelationshipEnd> GetEnumerator() => 
            new Enumerator(this.EndLookup, this.KeysInDefOrder);

        private static bool IsEndValid(IRelationshipEnd end) => 
            !string.IsNullOrEmpty(end.Name);

        public bool Remove(IRelationshipEnd end)
        {
            if (!IsEndValid(end))
            {
                return false;
            }
            this.KeysInDefOrder.Remove(end.Name);
            return this.EndLookup.Remove(end.Name);
        }

        int IList<IRelationshipEnd>.IndexOf(IRelationshipEnd end)
        {
            throw EntityUtil.NotSupported();
        }

        void IList<IRelationshipEnd>.Insert(int index, IRelationshipEnd end)
        {
            throw EntityUtil.NotSupported();
        }

        void IList<IRelationshipEnd>.RemoveAt(int index)
        {
            throw EntityUtil.NotSupported();
        }

        IEnumerator IEnumerable.GetEnumerator() => 
            new Enumerator(this.EndLookup, this.KeysInDefOrder);

        public bool TryGetEnd(string name, out IRelationshipEnd end) => 
            this.EndLookup.TryGetValue(name, out end);

        private bool ValidateUniqueName(SchemaElement end, string name)
        {
            if (this.EndLookup.ContainsKey(name))
            {
                end.AddError(ErrorCode.AlreadyDefined, EdmSchemaErrorSeverity.Error, Strings.EndNameAlreadyDefinedDuplicate(name));
                return false;
            }
            return true;
        }

        public int Count =>
            this.KeysInDefOrder.Count;

        private Dictionary<string, IRelationshipEnd> EndLookup
        {
            get
            {
                if (this._endLookup == null)
                {
                    this._endLookup = new Dictionary<string, IRelationshipEnd>(StringComparer.Ordinal);
                }
                return this._endLookup;
            }
        }

        public bool IsReadOnly =>
            false;

        public IRelationshipEnd this[int index]
        {
            get => 
                this.EndLookup[this.KeysInDefOrder[index]];
            set
            {
                throw EntityUtil.NotSupported();
            }
        }

        private List<string> KeysInDefOrder
        {
            get
            {
                if (this._keysInDefOrder == null)
                {
                    this._keysInDefOrder = new List<string>();
                }
                return this._keysInDefOrder;
            }
        }

        private sealed class Enumerator : IEnumerator<IRelationshipEnd>, IDisposable, IEnumerator
        {
            private Dictionary<string, IRelationshipEnd> _Data;
            private List<string>.Enumerator _Enumerator;

            public Enumerator(Dictionary<string, IRelationshipEnd> data, List<string> keysInDefOrder)
            {
                this._Enumerator = keysInDefOrder.GetEnumerator();
                this._Data = data;
            }

            public void Dispose()
            {
            }

            public bool MoveNext() => 
                this._Enumerator.MoveNext();

            public void Reset()
            {
                this._Enumerator.Reset();
            }

            public IRelationshipEnd Current =>
                this._Data[this._Enumerator.Current];

            object IEnumerator.Current =>
                this._Data[this._Enumerator.Current];
        }
    }
}

