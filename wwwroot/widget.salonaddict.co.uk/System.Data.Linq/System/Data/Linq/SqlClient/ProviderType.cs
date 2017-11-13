namespace System.Data.Linq.SqlClient
{
    using System;

    internal abstract class ProviderType
    {
        protected ProviderType()
        {
        }

        internal abstract bool AreValuesEqual(object o1, object o2);
        internal abstract int ComparePrecedenceTo(ProviderType type);
        public override bool Equals(object obj) => 
            base.Equals(obj);

        internal abstract Type GetClosestRuntimeType();
        public override int GetHashCode() => 
            base.GetHashCode();

        internal abstract ProviderType GetNonUnicodeEquivalent();
        internal abstract bool IsApplicationTypeOf(int index);
        internal abstract bool IsSameTypeFamily(ProviderType type);
        public static bool operator ==(ProviderType typeA, ProviderType typeB) => 
            ((typeA == typeB) || ((typeA != null) && typeA.Equals(typeB)));

        public static bool operator !=(ProviderType typeA, ProviderType typeB)
        {
            if (typeA == typeB)
            {
                return false;
            }
            if (typeA != null)
            {
                return !typeA.Equals(typeB);
            }
            return true;
        }

        internal abstract string ToQueryString();
        internal abstract string ToQueryString(QueryFormatOptions formatOptions);

        internal abstract bool CanBeColumn { get; }

        internal abstract bool CanBeParameter { get; }

        internal abstract bool CanSuppressSizeForConversionToString { get; }

        internal abstract bool HasPrecisionAndScale { get; }

        internal abstract bool HasSizeOrIsLarge { get; }

        internal abstract bool IsApplicationType { get; }

        internal abstract bool IsChar { get; }

        internal abstract bool IsFixedSize { get; }

        internal abstract bool IsGroupable { get; }

        internal abstract bool IsLargeType { get; }

        internal abstract bool IsNumeric { get; }

        internal abstract bool IsOrderable { get; }

        internal abstract bool IsRuntimeOnlyType { get; }

        internal abstract bool IsString { get; }

        internal abstract bool IsUnicodeType { get; }

        internal abstract int? Size { get; }

        internal abstract bool SupportsComparison { get; }

        internal abstract bool SupportsLength { get; }
    }
}

