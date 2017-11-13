namespace System.Windows.Markup
{
    using System;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=true)]
    public sealed class MarkupExtensionReturnTypeAttribute : Attribute
    {
        private Type _expressionType;
        private Type _returnType;

        public MarkupExtensionReturnTypeAttribute()
        {
        }

        public MarkupExtensionReturnTypeAttribute(Type returnType)
        {
            this._returnType = returnType;
        }

        public MarkupExtensionReturnTypeAttribute(Type returnType, Type expressionType)
        {
            this._returnType = returnType;
            this._expressionType = expressionType;
        }

        public Type ExpressionType =>
            this._expressionType;

        public Type ReturnType =>
            this._returnType;
    }
}

