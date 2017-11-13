namespace System.Runtime.Serialization
{
    using System;

    internal class TypeHandleRef
    {
        private RuntimeTypeHandle value;

        public TypeHandleRef()
        {
        }

        public TypeHandleRef(RuntimeTypeHandle value)
        {
            this.value = value;
        }

        public RuntimeTypeHandle Value
        {
            get => 
                this.value;
            set
            {
                this.value = value;
            }
        }
    }
}

