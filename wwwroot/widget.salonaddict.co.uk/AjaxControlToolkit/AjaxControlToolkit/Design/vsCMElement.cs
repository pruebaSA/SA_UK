namespace AjaxControlToolkit.Design
{
    using System;

    internal class vsCMElement
    {
        private object _reference;
        private static Type _referencedType;
        public static readonly vsCMElement vsCMElementClass = new vsCMElement(1);
        public static readonly vsCMElement vsCMElementFunction = new vsCMElement(2);
        public static readonly vsCMElement vsCMElementNamespace = new vsCMElement(5);

        public vsCMElement() : this(null)
        {
        }

        public vsCMElement(object reference)
        {
            this._reference = (ReferencedType.IsInstanceOfType(reference) || (reference is int)) ? reference : null;
        }

        public override bool Equals(object obj)
        {
            vsCMElement element = obj as vsCMElement;
            if (element == null)
            {
                return false;
            }
            if (this._reference == null)
            {
                return (element._reference == null);
            }
            if (element._reference == null)
            {
                return false;
            }
            return (this.Value == element.Value);
        }

        public override int GetHashCode()
        {
            if (this._reference != null)
            {
                return this.Value.GetHashCode();
            }
            return 0;
        }

        public static bool operator ==(vsCMElement left, vsCMElement right)
        {
            if (object.ReferenceEquals(left, null))
            {
                return object.ReferenceEquals(right, null);
            }
            return left.Equals(right);
        }

        public static bool operator !=(vsCMElement left, vsCMElement right) => 
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
                    _referencedType = ReferencedAssemblies.EnvDTE.GetType("EnvDTE.vsCMElement");
                    if (_referencedType == null)
                    {
                        throw new InvalidOperationException("Failed to load type 'EnvDTE.vsCMElement' from assembly 'EnvDTE, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'.");
                    }
                }
                return _referencedType;
            }
        }

        public int Value =>
            ((int) this._reference);
    }
}

