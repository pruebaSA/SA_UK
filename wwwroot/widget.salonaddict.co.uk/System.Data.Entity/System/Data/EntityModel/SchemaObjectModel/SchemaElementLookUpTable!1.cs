namespace System.Data.EntityModel.SchemaObjectModel
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;
    using System.Reflection;

    internal sealed class SchemaElementLookUpTable<T> : IEnumerable<T>, IEnumerable, ISchemaElementLookUpTable<T> where T: SchemaElement
    {
        private List<string> _keysInDefOrder;
        private Dictionary<string, T> _keyToType;

        public SchemaElementLookUpTable()
        {
            this._keysInDefOrder = new List<string>();
        }

        public void Add(T type, bool doNotAddErrorForEmptyName, Func<object, string> duplicateKeyErrorFormat)
        {
            switch (this.TryAdd(type))
            {
                case AddErrorKind.MissingNameError:
                    if (!doNotAddErrorForEmptyName)
                    {
                        type.AddError(ErrorCode.InvalidName, EdmSchemaErrorSeverity.Error, Strings.MissingName);
                    }
                    return;

                case AddErrorKind.DuplicateNameError:
                    type.AddError(ErrorCode.AlreadyDefined, EdmSchemaErrorSeverity.Error, duplicateKeyErrorFormat(type.FQName));
                    break;
            }
        }

        public bool ContainsKey(string key) => 
            this.KeyToType.ContainsKey(SchemaElementLookUpTable<T>.KeyFromName(key));

        public T GetElementAt(int index) => 
            this.KeyToType[this._keysInDefOrder[index]];

        public IEnumerator<T> GetEnumerator() => 
            new SchemaElementLookUpTableEnumerator<T, T>(this.KeyToType, this._keysInDefOrder);

        public IEnumerator<S> GetFilteredEnumerator<S>() where S: T => 
            new SchemaElementLookUpTableEnumerator<S, T>(this.KeyToType, this._keysInDefOrder);

        private static string KeyFromElement(T type) => 
            SchemaElementLookUpTable<T>.KeyFromName(type.Identity);

        private static string KeyFromName(string unnormalizedKey) => 
            unnormalizedKey;

        public T LookUpEquivalentKey(string key)
        {
            T local;
            key = SchemaElementLookUpTable<T>.KeyFromName(key);
            if (this.KeyToType.TryGetValue(key, out local))
            {
                return local;
            }
            return default(T);
        }

        IEnumerator IEnumerable.GetEnumerator() => 
            new SchemaElementLookUpTableEnumerator<T, T>(this.KeyToType, this._keysInDefOrder);

        public AddErrorKind TryAdd(T type)
        {
            T local;
            if (string.IsNullOrEmpty(type.Identity))
            {
                return AddErrorKind.MissingNameError;
            }
            string key = SchemaElementLookUpTable<T>.KeyFromElement(type);
            if (this.KeyToType.TryGetValue(key, out local))
            {
                return AddErrorKind.DuplicateNameError;
            }
            this.KeyToType.Add(key, type);
            this._keysInDefOrder.Add(key);
            return AddErrorKind.Succeeded;
        }

        public int Count =>
            this.KeyToType.Count;

        public T this[string key] =>
            this.KeyToType[SchemaElementLookUpTable<T>.KeyFromName(key)];

        private Dictionary<string, T> KeyToType
        {
            get
            {
                if (this._keyToType == null)
                {
                    this._keyToType = new Dictionary<string, T>(StringComparer.Ordinal);
                }
                return this._keyToType;
            }
        }
    }
}

