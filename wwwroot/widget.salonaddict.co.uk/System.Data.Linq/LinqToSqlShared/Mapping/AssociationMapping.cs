namespace LinqToSqlShared.Mapping
{
    using System;

    internal sealed class AssociationMapping : MemberMapping
    {
        private bool deleteOnNull;
        private string deleteRule;
        private bool isForeignKey;
        private bool isUnique;
        private string otherKey;
        private string thisKey;

        internal AssociationMapping()
        {
        }

        internal bool DeleteOnNull
        {
            get => 
                this.deleteOnNull;
            set
            {
                this.deleteOnNull = value;
            }
        }

        internal string DeleteRule
        {
            get => 
                this.deleteRule;
            set
            {
                this.deleteRule = value;
            }
        }

        internal bool IsForeignKey
        {
            get => 
                this.isForeignKey;
            set
            {
                this.isForeignKey = value;
            }
        }

        internal bool IsUnique
        {
            get => 
                this.isUnique;
            set
            {
                this.isUnique = value;
            }
        }

        internal string OtherKey
        {
            get => 
                this.otherKey;
            set
            {
                this.otherKey = value;
            }
        }

        internal string ThisKey
        {
            get => 
                this.thisKey;
            set
            {
                this.thisKey = value;
            }
        }

        internal string XmlDeleteOnNull
        {
            get
            {
                if (!this.deleteOnNull)
                {
                    return null;
                }
                return "true";
            }
            set
            {
                this.deleteOnNull = (value != null) ? bool.Parse(value) : false;
            }
        }

        internal string XmlIsForeignKey
        {
            get
            {
                if (!this.isForeignKey)
                {
                    return null;
                }
                return "true";
            }
            set
            {
                this.isForeignKey = (value != null) ? bool.Parse(value) : false;
            }
        }

        internal string XmlIsUnique
        {
            get
            {
                if (!this.isUnique)
                {
                    return null;
                }
                return "true";
            }
            set
            {
                this.isUnique = (value != null) ? bool.Parse(value) : false;
            }
        }
    }
}

