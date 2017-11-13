namespace MS.Internal.IO.Packaging.CompoundFile
{
    using MS.Internal.WindowsBase;
    using System;
    using System.Text;
    using System.Windows;

    [FriendAccessAllowed]
    internal class VersionPair : IComparable
    {
        private short _major;
        private short _minor;

        internal VersionPair(short major, short minor)
        {
            if (major < 0)
            {
                throw new ArgumentOutOfRangeException("major", System.Windows.SR.Get("VersionNumberComponentNegative"));
            }
            if (minor < 0)
            {
                throw new ArgumentOutOfRangeException("minor", System.Windows.SR.Get("VersionNumberComponentNegative"));
            }
            this._major = major;
            this._minor = minor;
        }

        public int CompareTo(object obj)
        {
            if (obj != null)
            {
                if (obj.GetType() != base.GetType())
                {
                    throw new ArgumentException(System.Windows.SR.Get("ExpectedVersionPairObject"));
                }
                VersionPair pair = (VersionPair) obj;
                if (this.Equals(obj))
                {
                    return 0;
                }
                if (this < pair)
                {
                    return -1;
                }
            }
            return 1;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (obj.GetType() != base.GetType())
            {
                return false;
            }
            VersionPair pair = (VersionPair) obj;
            if (this != pair)
            {
                return false;
            }
            return true;
        }

        public override int GetHashCode() => 
            (this._major << (0x10 + this._minor));

        public static bool operator ==(VersionPair v1, VersionPair v2)
        {
            bool flag = false;
            if ((v1 == null) && (v2 == null))
            {
                return true;
            }
            if (((v1 != null) && (v2 != null)) && ((v1.Major == v2.Major) && (v1.Minor == v2.Minor)))
            {
                flag = true;
            }
            return flag;
        }

        public static bool operator >(VersionPair v1, VersionPair v2)
        {
            bool flag = false;
            return (((v1 != null) && (v2 == null)) || ((((v1 != null) && (v2 != null)) && ((v1 >= v2) && (v1 != v2))) || flag));
        }

        public static bool operator >=(VersionPair v1, VersionPair v2) => 
            (v1 >= v2);

        public static bool operator !=(VersionPair v1, VersionPair v2) => 
            !(v1 == v2);

        public static bool operator <(VersionPair v1, VersionPair v2)
        {
            bool flag = false;
            if (((v1 != null) || (v2 == null)) && (((v1 == null) || (v2 == null)) || ((v1.Major >= v2.Major) && ((v1.Major != v2.Major) || (v1.Minor >= v2.Minor)))))
            {
                return flag;
            }
            return true;
        }

        public static bool operator <=(VersionPair v1, VersionPair v2) => 
            (v1 <= v2);

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder("(");
            builder.Append(this._major);
            builder.Append(",");
            builder.Append(this._minor);
            builder.Append(")");
            return builder.ToString();
        }

        public short Major =>
            this._major;

        public short Minor =>
            this._minor;
    }
}

