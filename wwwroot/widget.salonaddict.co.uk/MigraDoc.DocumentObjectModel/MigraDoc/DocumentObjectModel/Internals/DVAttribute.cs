namespace MigraDoc.DocumentObjectModel.Internals
{
    using System;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    internal class DVAttribute : Attribute
    {
        public System.Type ItemType = null;
        public bool RefOnly = false;
        internal System.Type type;

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

