namespace MigraDoc.DocumentObjectModel.Internals
{
    using System;

    internal interface INullableValue
    {
        object GetValue();
        void SetNull();
        void SetValue(object value);

        bool IsNull { get; }
    }
}

