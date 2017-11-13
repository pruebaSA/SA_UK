namespace MS.Internal.IO.Packaging.CompoundFile
{
    using System;
    using System.Windows;

    internal class CompoundFileStorageReference : CompoundFileReference, IComparable
    {
        private string _fullName;

        public CompoundFileStorageReference(string fullName)
        {
            this.SetFullName(fullName);
        }

        public override bool Equals(object o)
        {
            if (o == null)
            {
                return false;
            }
            if (o.GetType() != base.GetType())
            {
                return false;
            }
            CompoundFileStorageReference reference = (CompoundFileStorageReference) o;
            return (string.CompareOrdinal(this._fullName.ToUpperInvariant(), reference._fullName.ToUpperInvariant()) == 0);
        }

        public override int GetHashCode() => 
            this._fullName.GetHashCode();

        private void SetFullName(string fullName)
        {
            if ((fullName == null) || (fullName.Length == 0))
            {
                this._fullName = string.Empty;
            }
            else
            {
                if (fullName.StartsWith(ContainerUtilities.PathSeparatorAsString, StringComparison.Ordinal))
                {
                    throw new ArgumentException(System.Windows.SR.Get("DelimiterLeading"), "fullName");
                }
                this._fullName = fullName;
                if (ContainerUtilities.ConvertBackSlashPathToStringArrayPath(this._fullName).Length == 0)
                {
                    throw new ArgumentException(System.Windows.SR.Get("CompoundFilePathNullEmpty"), "fullName");
                }
            }
        }

        int IComparable.CompareTo(object o)
        {
            if (o == null)
            {
                return 1;
            }
            if (o.GetType() != base.GetType())
            {
                throw new ArgumentException(System.Windows.SR.Get("CanNotCompareDiffTypes"));
            }
            CompoundFileStorageReference reference = (CompoundFileStorageReference) o;
            return string.CompareOrdinal(this._fullName.ToUpperInvariant(), reference._fullName.ToUpperInvariant());
        }

        public override string FullName =>
            this._fullName;
    }
}

