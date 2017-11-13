namespace System.Data.Metadata.Edm
{
    using System;
    using System.Threading;

    internal sealed class ClrComplexType : ComplexType
    {
        private Delegate _constructor;
        private readonly string _cspaceTypeName;
        private readonly RuntimeTypeHandle _type;

        internal ClrComplexType(Type clrType, string cspaceNamespaceName, string cspaceTypeName) : base(EntityUtil.GenericCheckArgumentNull<Type>(clrType, "clrType").Name, clrType.Namespace ?? string.Empty, DataSpace.OSpace)
        {
            this._type = clrType.TypeHandle;
            this._cspaceTypeName = cspaceNamespaceName + "." + cspaceTypeName;
            base.Abstract = clrType.IsAbstract;
        }

        internal static ClrComplexType CreateReadonlyClrComplexType(Type clrType, string cspaceNamespaceName, string cspaceTypeName)
        {
            ClrComplexType type = new ClrComplexType(clrType, cspaceNamespaceName, cspaceTypeName);
            type.SetReadOnly();
            return type;
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

