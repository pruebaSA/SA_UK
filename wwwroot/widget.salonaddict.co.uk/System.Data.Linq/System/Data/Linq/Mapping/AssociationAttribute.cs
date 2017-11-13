namespace System.Data.Linq.Mapping
{
    using System;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple=false)]
    public sealed class AssociationAttribute : DataAttribute
    {
        private bool deleteOnNull;
        private string deleteRule;
        private bool isForeignKey;
        private bool isUnique;
        private string otherKey;
        private string thisKey;

        public bool DeleteOnNull
        {
            get => 
                this.deleteOnNull;
            set
            {
                this.deleteOnNull = value;
            }
        }

        public string DeleteRule
        {
            get => 
                this.deleteRule;
            set
            {
                this.deleteRule = value;
            }
        }

        public bool IsForeignKey
        {
            get => 
                this.isForeignKey;
            set
            {
                this.isForeignKey = value;
            }
        }

        public bool IsUnique
        {
            get => 
                this.isUnique;
            set
            {
                this.isUnique = value;
            }
        }

        public string OtherKey
        {
            get => 
                this.otherKey;
            set
            {
                this.otherKey = value;
            }
        }

        public string ThisKey
        {
            get => 
                this.thisKey;
            set
            {
                this.thisKey = value;
            }
        }
    }
}

