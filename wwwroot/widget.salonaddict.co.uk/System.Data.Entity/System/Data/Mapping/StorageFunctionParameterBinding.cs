namespace System.Data.Mapping
{
    using System;
    using System.Data;
    using System.Data.Metadata.Edm;
    using System.Globalization;

    internal sealed class StorageFunctionParameterBinding
    {
        internal readonly bool IsCurrent;
        internal readonly StorageFunctionMemberPath MemberPath;
        internal readonly FunctionParameter Parameter;

        internal StorageFunctionParameterBinding(FunctionParameter parameter, StorageFunctionMemberPath memberPath, bool isCurrent)
        {
            this.Parameter = EntityUtil.CheckArgumentNull<FunctionParameter>(parameter, "parameter");
            this.MemberPath = EntityUtil.CheckArgumentNull<StorageFunctionMemberPath>(memberPath, "memberPath");
            this.IsCurrent = isCurrent;
        }

        public override string ToString() => 
            string.Format(CultureInfo.InvariantCulture, "@{0}->{1}{2}", new object[] { this.Parameter, this.IsCurrent ? "+" : "-", this.MemberPath });
    }
}

