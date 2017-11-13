namespace System.Data.Services.Client
{
    using System;
    using System.Diagnostics;
    using System.Linq.Expressions;

    [DebuggerDisplay("InputReferenceExpression -> {Type}")]
    internal sealed class InputReferenceExpression : Expression
    {
        private ResourceExpression target;

        internal InputReferenceExpression(ResourceExpression target) : base((ExpressionType) 0x2717, target.ResourceType)
        {
            this.target = target;
        }

        internal void OverrideTarget(ResourceSetExpression newTarget)
        {
            this.target = newTarget;
        }

        internal ResourceExpression Target =>
            this.target;
    }
}

