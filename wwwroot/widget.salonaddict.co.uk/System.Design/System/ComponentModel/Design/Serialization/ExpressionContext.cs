namespace System.ComponentModel.Design.Serialization
{
    using System;
    using System.CodeDom;

    public sealed class ExpressionContext
    {
        private CodeExpression _expression;
        private Type _expressionType;
        private object _owner;
        private object _presetValue;

        public ExpressionContext(CodeExpression expression, Type expressionType, object owner) : this(expression, expressionType, owner, null)
        {
        }

        public ExpressionContext(CodeExpression expression, Type expressionType, object owner, object presetValue)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            if (expressionType == null)
            {
                throw new ArgumentNullException("expressionType");
            }
            if (owner == null)
            {
                throw new ArgumentNullException("owner");
            }
            this._expression = expression;
            this._expressionType = expressionType;
            this._owner = owner;
            this._presetValue = presetValue;
        }

        public CodeExpression Expression =>
            this._expression;

        public Type ExpressionType =>
            this._expressionType;

        public object Owner =>
            this._owner;

        public object PresetValue =>
            this._presetValue;
    }
}

