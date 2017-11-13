namespace AjaxControlToolkit.Design
{
    using System;

    internal class vsCMFunction
    {
        private object _reference;
        private static Type _referencedType;
        public static readonly vsCMFunction vsCMFunctionFunction = new vsCMFunction(0x80);

        public vsCMFunction() : this(null)
        {
        }

        public vsCMFunction(object reference)
        {
            this._reference = (ReferencedType.IsInstanceOfType(reference) || (reference is int)) ? reference : null;
        }

        public override bool Equals(object obj)
        {
            vsCMFunction function = obj as vsCMFunction;
            if (function == null)
            {
                return false;
            }
            if (this._reference == null)
            {
                return (function._reference == null);
            }
            if (function._reference == null)
            {
                return false;
            }
            return (this.Value == function.Value);
        }

        public override int GetHashCode()
        {
            if (this._reference != null)
            {
                return this.Value.GetHashCode();
            }
            return 0;
        }

        public static bool operator ==(vsCMFunction left, vsCMFunction right)
        {
            if (object.ReferenceEquals(left, null))
            {
                return object.ReferenceEquals(right, null);
            }
            return left.Equals(right);
        }

        public static bool operator !=(vsCMFunction left, vsCMFunction right) => 
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
                    _referencedType = ReferencedAssemblies.EnvDTE.GetType("EnvDTE.vsCMFunction");
                    if (_referencedType == null)
                    {
                        throw new InvalidOperationException("Failed to load type 'EnvDTE.vsCMFunction' from assembly 'EnvDTE, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'.");
                    }
                }
                return _referencedType;
            }
        }

        public int Value =>
            ((int) this._reference);
    }
}

