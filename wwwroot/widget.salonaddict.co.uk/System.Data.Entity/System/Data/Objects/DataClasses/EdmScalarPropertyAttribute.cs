namespace System.Data.Objects.DataClasses
{
    using System;

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class EdmScalarPropertyAttribute : EdmPropertyAttribute
    {
        private bool _entityKeyProperty;
        private bool _isNullable = true;

        public bool EntityKeyProperty
        {
            get => 
                this._entityKeyProperty;
            set
            {
                this._entityKeyProperty = value;
            }
        }

        public bool IsNullable
        {
            get => 
                this._isNullable;
            set
            {
                this._isNullable = value;
            }
        }
    }
}

