namespace System.Data.Linq.Mapping
{
    using System;
    using System.Reflection;

    internal sealed class AttributedMetaParameter : MetaParameter
    {
        private ParameterAttribute paramAttrib;
        private ParameterInfo parameterInfo;

        public AttributedMetaParameter(ParameterInfo parameterInfo)
        {
            this.parameterInfo = parameterInfo;
            this.paramAttrib = Attribute.GetCustomAttribute(parameterInfo, typeof(ParameterAttribute), false) as ParameterAttribute;
        }

        public override string DbType
        {
            get
            {
                if ((this.paramAttrib != null) && (this.paramAttrib.DbType != null))
                {
                    return this.paramAttrib.DbType;
                }
                return null;
            }
        }

        public override string MappedName
        {
            get
            {
                if ((this.paramAttrib != null) && (this.paramAttrib.Name != null))
                {
                    return this.paramAttrib.Name;
                }
                return this.parameterInfo.Name;
            }
        }

        public override string Name =>
            this.parameterInfo.Name;

        public override ParameterInfo Parameter =>
            this.parameterInfo;

        public override Type ParameterType =>
            this.parameterInfo.ParameterType;
    }
}

