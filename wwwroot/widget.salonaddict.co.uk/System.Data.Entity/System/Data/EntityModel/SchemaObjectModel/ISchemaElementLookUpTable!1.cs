namespace System.Data.EntityModel.SchemaObjectModel
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    internal interface ISchemaElementLookUpTable<T> where T: SchemaElement
    {
        bool ContainsKey(string key);
        IEnumerator<T> GetEnumerator();
        T LookUpEquivalentKey(string key);

        int Count { get; }

        T this[string key] { get; }
    }
}

