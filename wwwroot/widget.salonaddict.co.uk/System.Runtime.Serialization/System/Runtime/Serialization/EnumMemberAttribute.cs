namespace System.Runtime.Serialization
{
    using System;

    [AttributeUsage(AttributeTargets.Field, Inherited=false, AllowMultiple=false)]
    public sealed class EnumMemberAttribute : Attribute
    {
        private bool isValueSetExplicit;
        private string value;

        internal bool IsValueSetExplicit =>
            this.isValueSetExplicit;

        public string Value
        {
            get => 
                this.value;
            set
            {
                this.value = value;
                this.isValueSetExplicit = true;
            }
        }
    }
}

