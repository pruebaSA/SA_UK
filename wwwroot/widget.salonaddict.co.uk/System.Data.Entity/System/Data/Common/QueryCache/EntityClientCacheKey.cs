namespace System.Data.Common.QueryCache
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.EntityClient;
    using System.Text;

    internal sealed class EntityClientCacheKey : QueryCacheKey
    {
        private readonly CommandType _commandType;
        private readonly string _eSqlStatement;
        private readonly int _hashCode;
        private readonly int _parameterCount;
        private readonly string _parametersToken;

        internal EntityClientCacheKey(EntityCommand entityCommand)
        {
            this._commandType = entityCommand.CommandType;
            this._eSqlStatement = entityCommand.CommandText;
            this._parametersToken = GetParametersToken(entityCommand);
            this._parameterCount = entityCommand.Parameters.Count;
            this._hashCode = (this._commandType.GetHashCode() ^ this._eSqlStatement.GetHashCode()) ^ this._parametersToken.GetHashCode();
        }

        public override bool Equals(object otherObject)
        {
            if (typeof(EntityClientCacheKey) != otherObject.GetType())
            {
                return false;
            }
            EntityClientCacheKey key = (EntityClientCacheKey) otherObject;
            return ((((this._commandType == key._commandType) && (this._parameterCount == key._parameterCount)) && this.Equals(key._eSqlStatement, this._eSqlStatement)) && this.Equals(key._parametersToken, this._parametersToken));
        }

        public override int GetHashCode() => 
            this._hashCode;

        private static string GetParametersToken(EntityCommand entityCommand)
        {
            if ((entityCommand.Parameters == null) || (entityCommand.Parameters.Count == 0))
            {
                return "@@0";
            }
            if (1 == entityCommand.Parameters.Count)
            {
                Dictionary<string, TypeUsage> dictionary = entityCommand.GetParameterTypeUsage();
                return ("@@1:" + entityCommand.Parameters[0].ParameterName + ":" + dictionary[entityCommand.Parameters[0].ParameterName].EdmType.FullName);
            }
            StringBuilder builder = new StringBuilder(entityCommand.Parameters.Count * 20);
            Dictionary<string, TypeUsage> parameterTypeUsage = entityCommand.GetParameterTypeUsage();
            builder.Append("@@");
            builder.Append(entityCommand.Parameters.Count);
            builder.Append(":");
            string str = "";
            foreach (KeyValuePair<string, TypeUsage> pair in parameterTypeUsage)
            {
                builder.Append(str);
                builder.Append(pair.Key);
                builder.Append(":");
                builder.Append(pair.Value.EdmType.FullName);
                str = ";";
            }
            return builder.ToString();
        }

        public override string ToString() => 
            string.Join("|", new string[] { Enum.GetName(typeof(CommandType), this._commandType), this._eSqlStatement, this._parametersToken });
    }
}

