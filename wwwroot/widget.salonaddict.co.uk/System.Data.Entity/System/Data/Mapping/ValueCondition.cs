namespace System.Data.Mapping
{
    using System;

    internal class ValueCondition : IEquatable<ValueCondition>
    {
        internal readonly string Description;
        internal static readonly ValueCondition IsNotNull = new ValueCondition("NOT NULL", true);
        internal const string IsNotNullDescription = "NOT NULL";
        internal static readonly ValueCondition IsNull = new ValueCondition("NULL", true);
        internal const string IsNullDescription = "NULL";
        internal static readonly ValueCondition IsOther = new ValueCondition("OTHER", true);
        internal const string IsOtherDescription = "OTHER";
        internal readonly bool IsSentinel;

        internal ValueCondition(string description) : this(description, false)
        {
        }

        private ValueCondition(string description, bool isSentinel)
        {
            this.Description = description;
            this.IsSentinel = isSentinel;
        }

        public bool Equals(ValueCondition other) => 
            ((other.IsSentinel == this.IsSentinel) && (other.Description == this.Description));

        public override int GetHashCode() => 
            this.Description.GetHashCode();

        public override string ToString() => 
            this.Description;

        internal bool IsNotNullCondition =>
            object.ReferenceEquals(this, IsNotNull);
    }
}

