namespace System.Runtime.InteropServices
{
    using System;
    using System.Reflection;

    [ComVisible(true), AttributeUsage(AttributeTargets.ReturnValue | AttributeTargets.Parameter | AttributeTargets.Field, Inherited=false)]
    public sealed class MarshalAsAttribute : Attribute
    {
        internal UnmanagedType _val;
        public UnmanagedType ArraySubType;
        public int IidParameterIndex;
        public string MarshalCookie;
        [ComVisible(true)]
        public string MarshalType;
        [ComVisible(true)]
        public Type MarshalTypeRef;
        public VarEnum SafeArraySubType;
        public Type SafeArrayUserDefinedSubType;
        public int SizeConst;
        public short SizeParamIndex;

        public MarshalAsAttribute(short unmanagedType)
        {
            this._val = (UnmanagedType) unmanagedType;
        }

        public MarshalAsAttribute(UnmanagedType unmanagedType)
        {
            this._val = unmanagedType;
        }

        internal MarshalAsAttribute(UnmanagedType val, VarEnum safeArraySubType, Type safeArrayUserDefinedSubType, UnmanagedType arraySubType, short sizeParamIndex, int sizeConst, string marshalType, Type marshalTypeRef, string marshalCookie, int iidParamIndex)
        {
            this._val = val;
            this.SafeArraySubType = safeArraySubType;
            this.SafeArrayUserDefinedSubType = safeArrayUserDefinedSubType;
            this.IidParameterIndex = iidParamIndex;
            this.ArraySubType = arraySubType;
            this.SizeParamIndex = sizeParamIndex;
            this.SizeConst = sizeConst;
            this.MarshalType = marshalType;
            this.MarshalTypeRef = marshalTypeRef;
            this.MarshalCookie = marshalCookie;
        }

        internal static Attribute GetCustomAttribute(ParameterInfo parameter) => 
            GetCustomAttribute(parameter.MetadataToken, parameter.Member.Module);

        internal static Attribute GetCustomAttribute(RuntimeFieldInfo field) => 
            GetCustomAttribute(field.MetadataToken, field.Module);

        internal static Attribute GetCustomAttribute(int token, Module scope)
        {
            UnmanagedType type;
            UnmanagedType type2;
            VarEnum enum2;
            int sizeParamIndex = 0;
            int sizeConst = 0;
            string marshalType = null;
            string marshalCookie = null;
            string safeArrayUserDefinedSubType = null;
            int iidParamIndex = 0;
            ConstArray fieldMarshal = scope.ModuleHandle.GetMetadataImport().GetFieldMarshal(token);
            if (fieldMarshal.Length == 0)
            {
                return null;
            }
            MetadataImport.GetMarshalAs(fieldMarshal, out type, out enum2, out safeArrayUserDefinedSubType, out type2, out sizeParamIndex, out sizeConst, out marshalType, out marshalCookie, out iidParamIndex);
            Type type3 = ((safeArrayUserDefinedSubType == null) || (safeArrayUserDefinedSubType.Length == 0)) ? null : RuntimeTypeHandle.GetTypeByNameUsingCARules(safeArrayUserDefinedSubType, scope);
            Type marshalTypeRef = null;
            try
            {
                marshalTypeRef = (marshalType == null) ? null : RuntimeTypeHandle.GetTypeByNameUsingCARules(marshalType, scope);
            }
            catch (TypeLoadException)
            {
            }
            return new MarshalAsAttribute(type, enum2, type3, type2, (short) sizeParamIndex, sizeConst, marshalType, marshalTypeRef, marshalCookie, iidParamIndex);
        }

        internal static bool IsDefined(ParameterInfo parameter) => 
            (GetCustomAttribute(parameter) != null);

        internal static bool IsDefined(RuntimeFieldInfo field) => 
            (GetCustomAttribute(field) != null);

        public UnmanagedType Value =>
            this._val;
    }
}

