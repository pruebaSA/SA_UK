namespace System.Data.Linq.Mapping
{
    using LinqToSqlShared.Mapping;
    using System;
    using System.Reflection;

    internal sealed class MappedParameter : MetaParameter
    {
        private ParameterMapping map;
        private ParameterInfo parameterInfo;

        public MappedParameter(ParameterInfo parameterInfo, ParameterMapping map)
        {
            this.parameterInfo = parameterInfo;
            this.map = map;
        }

        public override string DbType =>
            this.map.DbType;

        public override string MappedName =>
            this.map.Name;

        public override string Name =>
            this.parameterInfo.Name;

        public override ParameterInfo Parameter =>
            this.parameterInfo;

        public override Type ParameterType =>
            this.parameterInfo.ParameterType;
    }
}

