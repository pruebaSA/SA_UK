namespace System.Data.Objects.DataClasses
{
    using System;
    using System.Data;
    using System.Globalization;

    [Serializable]
    internal class RelationshipNavigation
    {
        private readonly string _from;
        private readonly string _relationshipName;
        [NonSerialized]
        private RelationshipNavigation _reverse;
        private readonly string _to;

        internal RelationshipNavigation(string relationshipName, string from, string to)
        {
            EntityUtil.CheckStringArgument(relationshipName, "relationshipName");
            EntityUtil.CheckStringArgument(from, "from");
            EntityUtil.CheckStringArgument(to, "to");
            this._relationshipName = relationshipName;
            this._from = from;
            this._to = to;
        }

        public override bool Equals(object obj)
        {
            RelationshipNavigation navigation = obj as RelationshipNavigation;
            return ((this == navigation) || ((((this != null) && (navigation != null)) && ((this.RelationshipName == navigation.RelationshipName) && (this.From == navigation.From))) && (this.To == navigation.To)));
        }

        public override int GetHashCode() => 
            this.RelationshipName.GetHashCode();

        public override string ToString() => 
            string.Format(CultureInfo.InvariantCulture, "RelationshipNavigation: ({0},{1},{2})", new object[] { this._relationshipName, this._from, this._to });

        internal string From =>
            this._from;

        internal string RelationshipName =>
            this._relationshipName;

        internal RelationshipNavigation Reverse
        {
            get
            {
                if (this._reverse == null)
                {
                    this._reverse = new RelationshipNavigation(this._relationshipName, this._to, this._from);
                }
                return this._reverse;
            }
        }

        internal string To =>
            this._to;
    }
}

