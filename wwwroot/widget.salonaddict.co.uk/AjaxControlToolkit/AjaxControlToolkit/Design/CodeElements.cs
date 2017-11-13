namespace AjaxControlToolkit.Design
{
    using System;
    using System.Collections;

    internal class CodeElements
    {
        private object _reference;
        private static Type _referencedType;

        public CodeElements() : this(null)
        {
        }

        public CodeElements(object reference)
        {
            this._reference = ReferencedType.IsInstanceOfType(reference) ? reference : null;
        }

        public IEnumerator GetEnumerator() => 
            ((IEnumerator) ReferencedType.GetMethod("GetEnumerator").Invoke(this._reference, new object[0]));

        public int Count =>
            ((int) ReferencedType.GetProperty("Count").GetValue(this._reference, new object[0]));

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
                    _referencedType = ReferencedAssemblies.EnvDTE.GetType("EnvDTE.CodeElements");
                    if (_referencedType == null)
                    {
                        throw new InvalidOperationException("Failed to load type 'EnvDTE.CodeElements' from assembly 'EnvDTE, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'.");
                    }
                }
                return _referencedType;
            }
        }
    }
}

