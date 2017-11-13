namespace System.ServiceModel
{
    using System;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited=false)]
    public sealed class MessagePropertyAttribute : Attribute
    {
        private bool isNameSetExplicit;
        private string name;

        internal bool IsNameSetExplicit =>
            this.isNameSetExplicit;

        public string Name
        {
            get => 
                this.name;
            set
            {
                this.isNameSetExplicit = true;
                this.name = value;
            }
        }
    }
}

