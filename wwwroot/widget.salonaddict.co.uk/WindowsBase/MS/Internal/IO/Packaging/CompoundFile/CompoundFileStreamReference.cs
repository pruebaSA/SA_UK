namespace MS.Internal.IO.Packaging.CompoundFile
{
    using System;
    using System.Text;
    using System.Windows;

    internal class CompoundFileStreamReference : CompoundFileReference, IComparable
    {
        private string _fullName;

        public CompoundFileStreamReference(string fullName)
        {
            this.SetFullName(fullName);
        }

        public CompoundFileStreamReference(string storageName, string streamName)
        {
            ContainerUtilities.CheckStringAgainstNullAndEmpty(streamName, "streamName");
            if ((storageName == null) || (storageName.Length == 0))
            {
                this._fullName = streamName;
            }
            else
            {
                StringBuilder builder = new StringBuilder(storageName, (storageName.Length + 1) + streamName.Length);
                builder.Append(ContainerUtilities.PathSeparator);
                builder.Append(streamName);
                this._fullName = builder.ToString();
            }
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
            CompoundFileStreamReference reference = (CompoundFileStreamReference) o;
            return (string.CompareOrdinal(this._fullName.ToUpperInvariant(), reference._fullName.ToUpperInvariant()) == 0);
        }

        public override int GetHashCode() => 
            this._fullName.GetHashCode();

        private void SetFullName(string fullName)
        {
            ContainerUtilities.CheckStringAgainstNullAndEmpty(fullName, "fullName");
            if (fullName.StartsWith(ContainerUtilities.PathSeparatorAsString, StringComparison.Ordinal))
            {
                throw new ArgumentException(System.Windows.SR.Get("DelimiterLeading"), "fullName");
            }
            this._fullName = fullName;
            if (ContainerUtilities.ConvertBackSlashPathToStringArrayPath(fullName).Length == 0)
            {
                throw new ArgumentException(System.Windows.SR.Get("CompoundFilePathNullEmpty"), "fullName");
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
            CompoundFileStreamReference reference = (CompoundFileStreamReference) o;
            return string.CompareOrdinal(this._fullName.ToUpperInvariant(), reference._fullName.ToUpperInvariant());
        }

        public override string FullName =>
            this._fullName;
    }
}

