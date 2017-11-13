namespace System.Runtime.Remoting.Contexts
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;

    [ComVisible(true), SecurityPermission(SecurityAction.InheritanceDemand, Flags=SecurityPermissionFlag.Infrastructure), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure)]
    public class ContextProperty
    {
        internal string _name;
        internal object _property;

        internal ContextProperty(string name, object prop)
        {
            this._name = name;
            this._property = prop;
        }

        public virtual string Name =>
            this._name;

        public virtual object Property =>
            this._property;
    }
}

