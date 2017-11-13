namespace System.Data.Linq.Mapping
{
    using System;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple=true, Inherited=false)]
    public sealed class InheritanceMappingAttribute : Attribute
    {
        private object code;
        private bool isDefault;
        private System.Type type;

        public object Code
        {
            get => 
                this.code;
            set
            {
                this.code = value;
            }
        }

        public bool IsDefault
        {
            get => 
                this.isDefault;
            set
            {
                this.isDefault = value;
            }
        }

        public System.Type Type
        {
            get => 
                this.type;
            set
            {
                this.type = value;
            }
        }
    }
}

