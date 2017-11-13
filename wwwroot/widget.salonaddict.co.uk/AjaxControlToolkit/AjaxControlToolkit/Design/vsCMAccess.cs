namespace AjaxControlToolkit.Design
{
    using System;

    internal class vsCMAccess
    {
        private object _reference;
        private static Type _referencedType;
        public static readonly vsCMAccess vsCMAccessPublic = new vsCMAccess(1);

        public vsCMAccess() : this(null)
        {
        }

        public vsCMAccess(object reference)
        {
            this._reference = (ReferencedType.IsInstanceOfType(reference) || (reference is int)) ? reference : null;
        }

        public override bool Equals(object obj)
        {
            vsCMAccess access = obj as vsCMAccess;
            if (access == null)
            {
                return false;
            }
            if (this._reference == null)
            {
                return (access._reference == null);
            }
            if (access._reference == null)
            {
                return false;
            }
            return (this.Value == access.Value);
        }

        public override int GetHashCode()
        {
            if (this._reference != null)
            {
                return this.Value.GetHashCode();
            }
            return 0;
        }

        public static bool operator ==(vsCMAccess left, vsCMAccess right)
        {
            if (object.ReferenceEquals(left, null))
            {
                return object.ReferenceEquals(right, null);
            }
            return left.Equals(right);
        }

        public static bool operator !=(vsCMAccess left, vsCMAccess right) => 
            !(left == right);

        public object Reference
        {
            get => 
                this._reference;
            set
            {
                this._reference = value;
            }
        }

        public static Type ReferencedType
        {
            get
            {
                if (_referencedType == null)
                {
                    _referencedType = ReferencedAssemblies.EnvDTE.GetType("EnvDTE.vsCMAccess");
                    if (_referencedType == null)
                    {
                        throw new InvalidOperationException("Failed to load type 'EnvDTE.vsCMAccess' from assembly 'EnvDTE, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'.");
                    }
                }
                return _referencedType;
            }
        }

        public int Value =>
            ((int) this._reference);
    }
}

