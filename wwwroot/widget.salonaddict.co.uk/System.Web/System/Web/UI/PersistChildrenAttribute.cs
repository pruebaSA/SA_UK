namespace System.Web.UI
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AttributeUsage(AttributeTargets.Class), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class PersistChildrenAttribute : Attribute
    {
        private bool _persist;
        private bool _usesCustomPersistence;
        public static readonly PersistChildrenAttribute Default = Yes;
        public static readonly PersistChildrenAttribute No = new PersistChildrenAttribute(false);
        public static readonly PersistChildrenAttribute Yes = new PersistChildrenAttribute(true);

        public PersistChildrenAttribute(bool persist)
        {
            this._persist = persist;
        }

        public PersistChildrenAttribute(bool persist, bool usesCustomPersistence) : this(persist)
        {
            this._usesCustomPersistence = usesCustomPersistence;
        }

        public override bool Equals(object obj) => 
            ((obj == this) || (((obj != null) && (obj is PersistChildrenAttribute)) && (((PersistChildrenAttribute) obj).Persist == this._persist)));

        public override int GetHashCode() => 
            this.Persist.GetHashCode();

        public override bool IsDefaultAttribute() => 
            this.Equals(Default);

        public bool Persist =>
            this._persist;

        public bool UsesCustomPersistence =>
            (!this._persist && this._usesCustomPersistence);
    }
}

