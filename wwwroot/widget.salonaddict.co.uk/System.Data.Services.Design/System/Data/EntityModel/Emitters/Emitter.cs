namespace System.Data.EntityModel.Emitters
{
    using System;
    using System.CodeDom;
    using System.Data.EntityModel;

    internal abstract class Emitter
    {
        private ClientApiGenerator _generator;
        private static CodeExpression _nullExpression;
        private static CodeExpression _thisRef;
        private const string EntityGetContextPropertyName = "Context";
        protected const string SearcherGetContextPropertyName = "Context";

        protected Emitter(ClientApiGenerator generator)
        {
            this.Generator = generator;
        }

        protected static CodeBinaryOperatorExpression EmitExpressionDoesNotEqualNull(CodeExpression expression) => 
            new CodeBinaryOperatorExpression(expression, CodeBinaryOperatorType.IdentityInequality, NullExpression);

        protected static CodeBinaryOperatorExpression EmitExpressionEqualsNull(CodeExpression expression) => 
            new CodeBinaryOperatorExpression(expression, CodeBinaryOperatorType.IdentityEquality, NullExpression);

        protected System.Data.EntityModel.Emitters.AttributeEmitter AttributeEmitter =>
            this._generator.AttributeEmitter;

        internal ClientApiGenerator Generator
        {
            get => 
                this._generator;
            private set
            {
                this._generator = value;
            }
        }

        protected static CodeExpression NullExpression
        {
            get
            {
                if (_nullExpression == null)
                {
                    _nullExpression = new CodePrimitiveExpression(null);
                }
                return _nullExpression;
            }
        }

        protected static CodeExpression ThisRef
        {
            get
            {
                if (_thisRef == null)
                {
                    _thisRef = new CodeThisReferenceExpression();
                }
                return _thisRef;
            }
        }

        protected System.Data.EntityModel.Emitters.TypeReference TypeReference =>
            this._generator.TypeReference;
    }
}

