namespace System.Data.EntityModel.SchemaObjectModel
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Entity;
    using System.Reflection;

    internal sealed class FilteredSchemaElementLookUpTable<T, S> : IEnumerable<T>, IEnumerable, ISchemaElementLookUpTable<T> where T: S where S: SchemaElement
    {
        private SchemaElementLookUpTable<S> _lookUpTable;

        public FilteredSchemaElementLookUpTable(SchemaElementLookUpTable<S> lookUpTable)
        {
            this._lookUpTable = lookUpTable;
        }

        public bool ContainsKey(string key)
        {
            if (!this._lookUpTable.ContainsKey(key))
            {
                return false;
            }
            return (this._lookUpTable[key] is T);
        }

        public IEnumerator<T> GetEnumerator() => 
            this._lookUpTable.GetFilteredEnumerator<T>();

        public T LookUpEquivalentKey(string key) => 
            (this._lookUpTable.LookUpEquivalentKey(key) as T);

        IEnumerator IEnumerable.GetEnumerator() => 
            this._lookUpTable.GetFilteredEnumerator<T>();

        public int Count
        {
            get
            {
                int num = 0;
                foreach (SchemaElement element in this._lookUpTable)
                {
                    if (element is T)
                    {
                        num++;
                    }
                }
                return num;
            }
        }

        public T this[string key]
        {
            get
            {
                S local = this._lookUpTable[key];
                if (local == null)
                {
                    return default(T);
                }
                T local2 = local as T;
                if (local2 == null)
                {
                    throw EntityUtil.InvalidOperation(Strings.UnexpectedTypeInCollection(local.GetType(), key));
                }
                return local2;
            }
        }
    }
}

