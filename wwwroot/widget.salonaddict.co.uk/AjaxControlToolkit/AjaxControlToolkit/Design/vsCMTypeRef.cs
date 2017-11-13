namespace AjaxControlToolkit.Design
{
    using System;

    internal class vsCMTypeRef
    {
        private object _reference;
        private static Type _referencedType;
        public static readonly vsCMTypeRef vsCMTypeRefArray = new vsCMTypeRef(2);
        public static readonly vsCMTypeRef vsCMTypeRefPointer = new vsCMTypeRef(4);

        public vsCMTypeRef() : this(null)
        {
        }

        public vsCMTypeRef(object reference)
        {
            this._reference = (ReferencedType.IsInstanceOfType(reference) || (reference is int)) ? reference : null;
        }

        public override bool Equals(object obj)
        {
            vsCMTypeRef ref2 = obj as vsCMTypeRef;
            if (ref2 == null)
            {
                return false;
            }
            if (this._reference == null)
            {
                return (ref2._reference == null);
            }
            if (ref2._reference == null)
            {
                return false;
            }
            return (this.Value == ref2.Value);
        }

        public override int GetHashCode()
        {
            if (this._reference != null)
            {
                return this.Value.GetHashCode();
            }
            return 0;
        }

        public static bool operator ==(vsCMTypeRef left, vsCMTypeRef right)
        {
            if (object.ReferenceEquals(left, null))
            {
                return object.ReferenceEquals(right, null);
            }
            return left.Equals(right);
        }

        public static bool operator !=(vsCMTypeRef left, vsCMTypeRef right) => 
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
                    _referencedType = ReferencedAssemblies.EnvDTE.GetType("EnvDTE.vsCMTypeRef");
                    if (_referencedType == null)
                    {
                        throw new InvalidOperationException("Failed to load type 'EnvDTE.vsCMTypeRef' from assembly 'EnvDTE, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'.");
                    }
                }
                return _referencedType;
            }
        }

        public int Value =>
            ((int) this._reference);
    }
}

