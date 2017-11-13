namespace System.Data.EntityModel.SchemaObjectModel
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    internal sealed class SchemaElementLookUpTableEnumerator<T, S> : IEnumerator<T>, IDisposable, IEnumerator where T: S where S: SchemaElement
    {
        private Dictionary<string, S> _data;
        private List<string>.Enumerator _enumerator;

        public SchemaElementLookUpTableEnumerator(Dictionary<string, S> data, List<string> keysInOrder)
        {
            this._data = data;
            this._enumerator = keysInOrder.GetEnumerator();
        }

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            while (this._enumerator.MoveNext())
            {
                if (this.Current != null)
                {
                    return true;
                }
            }
            return false;
        }

        public void Reset()
        {
            this._enumerator.Reset();
        }

        public T Current
        {
            get
            {
                string current = this._enumerator.Current;
                return (this._data[current] as T);
            }
        }

        object IEnumerator.Current
        {
            get
            {
                string current = this._enumerator.Current;
                return (this._data[current] as T);
            }
        }
    }
}

