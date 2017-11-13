﻿namespace System.EnterpriseServices
{
    using System;
    using System.Runtime.InteropServices;

    [ComVisible(false)]
    public sealed class SharedPropertyGroup
    {
        private ISharedPropertyGroup _x;

        internal SharedPropertyGroup(ISharedPropertyGroup grp)
        {
            this._x = grp;
        }

        public SharedProperty CreateProperty(string name, out bool fExists) => 
            new SharedProperty(this._x.CreateProperty(name, out fExists));

        public SharedProperty CreatePropertyByPosition(int position, out bool fExists) => 
            new SharedProperty(this._x.CreatePropertyByPosition(position, out fExists));

        public SharedProperty Property(string name) => 
            new SharedProperty(this._x.Property(name));

        public SharedProperty PropertyByPosition(int position) => 
            new SharedProperty(this._x.PropertyByPosition(position));
    }
}

