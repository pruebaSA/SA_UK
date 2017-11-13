namespace System.Data.Common.CommandTrees.Internal
{
    using System;
    using System.Data.Common.CommandTrees;
    using System.Runtime.CompilerServices;

    internal delegate void ExpressionLinkConstraint(ExpressionLink link, DbExpression newValue);
}

