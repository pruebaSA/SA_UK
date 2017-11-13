namespace AjaxControlToolkit.Design
{
    using System;

    internal class UndoContext
    {
        private object _reference;
        private static Type _referencedType;

        public UndoContext() : this(null)
        {
        }

        public UndoContext(object reference)
        {
            this._reference = ReferencedType.IsInstanceOfType(reference) ? reference : null;
        }

        public void Close()
        {
            ReferencedType.GetMethod("Close").Invoke(this._reference, new object[0]);
        }

        public void Open(string Name, bool Strict)
        {
            ReferencedType.GetMethod("Open").Invoke(this._reference, new object[] { Name, Strict });
        }

        public bool IsOpen =>
            ((bool) ReferencedType.GetProperty("IsOpen").GetValue(this._reference, new object[0]));

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
                    _referencedType = ReferencedAssemblies.EnvDTE.GetType("EnvDTE.UndoContext");
                    if (_referencedType == null)
                    {
                        throw new InvalidOperationException("Failed to load type 'EnvDTE.UndoContext' from assembly 'EnvDTE, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'.");
                    }
                }
                return _referencedType;
            }
        }
    }
}

