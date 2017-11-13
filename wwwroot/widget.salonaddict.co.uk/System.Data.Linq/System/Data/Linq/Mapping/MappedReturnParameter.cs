namespace System.Data.Linq.Mapping
{
    using LinqToSqlShared.Mapping;
    using System;
    using System.Reflection;

    internal sealed class MappedReturnParameter : MetaParameter
    {
        private ReturnMapping map;
        private ParameterInfo parameterInfo;

        public MappedReturnParameter(ParameterInfo parameterInfo, ReturnMapping map)
        {
            this.parameterInfo = parameterInfo;
            this.map = map;
        }

        public override string DbType =>
            this.map.DbType;

        public override string MappedName =>
            null;

        public override string Name =>
            null;

        public override ParameterInfo Parameter =>
            this.parameterInfo;

        public override Type ParameterType =>
            this.parameterInfo.ParameterType;
    }
}

