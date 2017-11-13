namespace System.Linq.Expressions
{
    using System;
    using System.Data;
    using System.Data.Entity;

    internal static class Error
    {
        internal static Exception UnhandledBindingType(MemberBindingType memberBindingType) => 
            EntityUtil.NotSupported(System.Data.Entity.Strings.ELinq_UnhandledBindingType(memberBindingType));

        internal static Exception UnhandledExpressionType(ExpressionType expressionType) => 
            EntityUtil.NotSupported(System.Data.Entity.Strings.ELinq_UnhandledExpressionType(expressionType));
    }
}

