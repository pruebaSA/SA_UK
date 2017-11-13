namespace System.Data.EntityModel.SchemaObjectModel
{
    using System;

    internal sealed class ReturnValue<T>
    {
        private bool _succeeded;
        private T _value;

        internal ReturnValue()
        {
            this._value = default(T);
        }

        internal bool Succeeded =>
            this._succeeded;

        internal T Value
        {
            get => 
                this._value;
            set
            {
                this._value = value;
                this._succeeded = true;
            }
        }
    }
}

