namespace System.Data.Linq.SqlClient
{
    using System;

    internal class SqlParameterInfo
    {
        private Delegate accessor;
        private SqlParameter parameter;
        private object value;

        internal SqlParameterInfo(SqlParameter parameter)
        {
            this.parameter = parameter;
        }

        internal SqlParameterInfo(SqlParameter parameter, Delegate accessor)
        {
            this.parameter = parameter;
            this.accessor = accessor;
        }

        internal SqlParameterInfo(SqlParameter parameter, object value)
        {
            this.parameter = parameter;
            this.value = value;
        }

        internal Delegate Accessor =>
            this.accessor;

        internal SqlParameter Parameter =>
            this.parameter;

        internal SqlParameterType Type
        {
            get
            {
                if (this.accessor != null)
                {
                    return SqlParameterType.UserArgument;
                }
                if (this.parameter.Name == "@ROWCOUNT")
                {
                    return SqlParameterType.PreviousResult;
                }
                return SqlParameterType.Value;
            }
        }

        internal object Value =>
            this.value;
    }
}

