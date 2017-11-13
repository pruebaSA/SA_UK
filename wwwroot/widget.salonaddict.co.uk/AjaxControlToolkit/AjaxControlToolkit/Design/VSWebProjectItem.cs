namespace AjaxControlToolkit.Design
{
    using System;

    internal class VSWebProjectItem
    {
        private object _reference;
        private static Type _referencedType;

        public VSWebProjectItem() : this(null)
        {
        }

        public VSWebProjectItem(object reference)
        {
            this._reference = ReferencedType.IsInstanceOfType(reference) ? reference : null;
        }

        public void Load()
        {
            ReferencedType.GetMethod("Load").Invoke(this._reference, new object[0]);
        }

        public void Unload()
        {
            ReferencedType.GetMethod("Unload").Invoke(this._reference, new object[0]);
        }

        public AjaxControlToolkit.Design.ProjectItem ProjectItem
        {
            get
            {
                object reference = ReferencedType.GetProperty("ProjectItem").GetValue(this._reference, new object[0]);
                if (reference == null)
                {
                    return null;
                }
                return new AjaxControlToolkit.Design.ProjectItem(reference);
            }
        }

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
                    _referencedType = ReferencedAssemblies.VsWebSite.GetType("VsWebSite.VSWebProjectItem");
                    if (_referencedType == null)
                    {
                        throw new InvalidOperationException("Failed to load type 'VsWebSite.VSWebProjectItem' from assembly 'VsWebSite.Interop, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'.");
                    }
                }
                return _referencedType;
            }
        }
    }
}

