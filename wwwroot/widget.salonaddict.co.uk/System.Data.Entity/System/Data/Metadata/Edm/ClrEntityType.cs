namespace System.Data.Metadata.Edm
{
    using System;
    using System.Threading;

    internal sealed class ClrEntityType : EntityType
    {
        private Delegate _constructor;
        private readonly string _cspaceTypeName;
        private readonly RuntimeTypeHandle _type;

        internal ClrEntityType(Type type, string cspaceNamespaceName, string cspaceTypeName) : base(EntityUtil.GenericCheckArgumentNull<Type>(type, "type").Name, type.Namespace ?? string.Empty, DataSpace.OSpace)
        {
            this._type = type.TypeHandle;
            this._cspaceTypeName = cspaceNamespaceName + "." + cspaceTypeName;
            base.Abstract = type.IsAbstract;
        }

        internal override Type ClrType =>
            Type.GetTypeFromHandle(this._type);

        internal Delegate Constructor
        {
            get => 
                this._constructor;
            set
            {
                Interlocked.CompareExchange<Delegate>(ref this._constructor, value, null);
            }
        }

        internal string CSpaceTypeName =>
            this._cspaceTypeName;
    }
}

