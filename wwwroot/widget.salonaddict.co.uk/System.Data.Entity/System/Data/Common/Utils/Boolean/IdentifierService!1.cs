namespace System.Data.Common.Utils.Boolean
{
    using System;

    internal abstract class IdentifierService<T_Identifier>
    {
        internal static readonly IdentifierService<T_Identifier> Instance;

        static IdentifierService()
        {
            IdentifierService<T_Identifier>.Instance = IdentifierService<T_Identifier>.GetIdentifierService();
        }

        private IdentifierService()
        {
        }

        internal abstract ConversionContext<T_Identifier> CreateConversionContext();
        private static IdentifierService<T_Identifier> GetIdentifierService()
        {
            Type type = typeof(T_Identifier);
            if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(DomainConstraint<,>)))
            {
                Type[] genericArguments = type.GetGenericArguments();
                Type type2 = genericArguments[0];
                Type type3 = genericArguments[1];
                return (IdentifierService<T_Identifier>) Activator.CreateInstance(typeof(DomainConstraintIdentifierService).MakeGenericType(new Type[] { type, type2, type3 }));
            }
            return new GenericIdentifierService<T_Identifier>();
        }

        internal abstract BoolExpr<T_Identifier> LocalSimplify(BoolExpr<T_Identifier> expression);
        internal abstract Literal<T_Identifier> NegateLiteral(Literal<T_Identifier> literal);

        private class DomainConstraintIdentifierService<T_Variable, T_Element> : IdentifierService<DomainConstraint<T_Variable, T_Element>>
        {
            internal override ConversionContext<DomainConstraint<T_Variable, T_Element>> CreateConversionContext() => 
                new DomainConstraintConversionContext<T_Variable, T_Element>();

            internal override BoolExpr<DomainConstraint<T_Variable, T_Element>> LocalSimplify(BoolExpr<DomainConstraint<T_Variable, T_Element>> expression)
            {
                expression = NegationPusher.EliminateNot<T_Variable, T_Element>(expression);
                return expression.Accept<BoolExpr<DomainConstraint<T_Variable, T_Element>>>(Simplifier<DomainConstraint<T_Variable, T_Element>>.Instance);
            }

            internal override Literal<DomainConstraint<T_Variable, T_Element>> NegateLiteral(Literal<DomainConstraint<T_Variable, T_Element>> literal) => 
                new Literal<DomainConstraint<T_Variable, T_Element>>(new TermExpr<DomainConstraint<T_Variable, T_Element>>(literal.Term.Identifier.InvertDomainConstraint()), literal.IsTermPositive);
        }

        private class GenericIdentifierService : IdentifierService<T_Identifier>
        {
            internal override ConversionContext<T_Identifier> CreateConversionContext() => 
                new GenericConversionContext<T_Identifier>();

            internal override BoolExpr<T_Identifier> LocalSimplify(BoolExpr<T_Identifier> expression) => 
                expression.Accept<BoolExpr<T_Identifier>>(Simplifier<T_Identifier>.Instance);

            internal override Literal<T_Identifier> NegateLiteral(Literal<T_Identifier> literal) => 
                new Literal<T_Identifier>(literal.Term, !literal.IsTermPositive);
        }
    }
}

