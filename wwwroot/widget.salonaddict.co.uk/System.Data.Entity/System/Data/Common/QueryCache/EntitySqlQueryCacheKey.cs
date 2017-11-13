namespace System.Data.Common.QueryCache
{
    using System;
    using System.Data.Objects;

    internal sealed class EntitySqlQueryCacheKey : QueryCacheKey
    {
        private string _defaultContainer;
        private readonly string _eSqlStatement;
        private readonly int _hashCode;
        private readonly string _includePathsToken;
        private readonly MergeOption _mergeOption;
        private readonly int _parameterCount;
        private readonly string _parametersToken;
        private readonly Type _resultType;

        internal EntitySqlQueryCacheKey(string defaultContainerName, string eSqlStatement, int parameterCount, string parametersToken, string includePathsToken, MergeOption mergeOption, Type resultType)
        {
            this._defaultContainer = defaultContainerName;
            this._eSqlStatement = eSqlStatement;
            this._parameterCount = parameterCount;
            this._parametersToken = parametersToken;
            this._includePathsToken = includePathsToken;
            this._mergeOption = mergeOption;
            this._resultType = resultType;
            int num = this._eSqlStatement.GetHashCode() ^ this._mergeOption.GetHashCode();
            if (this._parametersToken != null)
            {
                num ^= this._parametersToken.GetHashCode();
            }
            if (this._includePathsToken != null)
            {
                num ^= this._includePathsToken.GetHashCode();
            }
            if (this._defaultContainer != null)
            {
                num ^= this._defaultContainer.GetHashCode();
            }
            this._hashCode = num;
        }

        public override bool Equals(object otherObject)
        {
            if (typeof(EntitySqlQueryCacheKey) != otherObject.GetType())
            {
                return false;
            }
            EntitySqlQueryCacheKey key = (EntitySqlQueryCacheKey) otherObject;
            return (((((this._parameterCount == key._parameterCount) && (this._mergeOption == key._mergeOption)) && (this.Equals(key._defaultContainer, this._defaultContainer) && this.Equals(key._eSqlStatement, this._eSqlStatement))) && (this.Equals(key._includePathsToken, this._includePathsToken) && this.Equals(key._parametersToken, this._parametersToken))) && object.Equals(key._resultType, this._resultType));
        }

        public override int GetHashCode() => 
            this._hashCode;

        public override string ToString() => 
            string.Join("|", new string[] { this._defaultContainer, this._eSqlStatement, this._parametersToken, this._includePathsToken, Enum.GetName(typeof(MergeOption), this._mergeOption) });
    }
}

