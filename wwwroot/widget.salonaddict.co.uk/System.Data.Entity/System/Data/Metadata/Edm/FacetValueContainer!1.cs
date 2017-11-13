namespace System.Data.Metadata.Edm
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct FacetValueContainer<T>
    {
        private T _value;
        private bool _hasValue;
        private bool _isUnbounded;
        internal T Value
        {
            set
            {
                this._isUnbounded = false;
                this._hasValue = true;
                this._value = value;
            }
        }
        private void SetUnbounded()
        {
            this._isUnbounded = true;
            this._hasValue = true;
        }

        public static implicit operator FacetValueContainer<T>(EdmConstants.Unbounded unbounded)
        {
            FacetValueContainer<T> container = new FacetValueContainer<T>();
            container.SetUnbounded();
            return container;
        }

        public static implicit operator FacetValueContainer<T>(T value) => 
            new FacetValueContainer<T> { Value = value };

        internal object GetValueAsObject()
        {
            if (this._isUnbounded)
            {
                return EdmConstants.UnboundedValue;
            }
            return this._value;
        }

        internal bool HasValue =>
            this._hasValue;
    }
}

