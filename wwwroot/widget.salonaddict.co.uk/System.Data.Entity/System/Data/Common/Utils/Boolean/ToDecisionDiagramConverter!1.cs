namespace System.Data.Common.Utils.Boolean
{
    using System;

    internal class ToDecisionDiagramConverter<T_Identifier> : Visitor<T_Identifier, Vertex>
    {
        private readonly ConversionContext<T_Identifier> _context;

        private ToDecisionDiagramConverter(ConversionContext<T_Identifier> context)
        {
            this._context = context;
        }

        internal static Vertex TranslateToRobdd(BoolExpr<T_Identifier> expr, ConversionContext<T_Identifier> context)
        {
            ToDecisionDiagramConverter<T_Identifier> visitor = new ToDecisionDiagramConverter<T_Identifier>(context);
            return expr.Accept<Vertex>(visitor);
        }

        internal override Vertex VisitAnd(AndExpr<T_Identifier> expression) => 
            this._context.Solver.And(from child in expression.Children select child.Accept<Vertex>(this));

        internal override Vertex VisitFalse(FalseExpr<T_Identifier> expression) => 
            Vertex.Zero;

        internal override Vertex VisitNot(NotExpr<T_Identifier> expression) => 
            this._context.Solver.Not(expression.Child.Accept<Vertex>(this));

        internal override Vertex VisitOr(OrExpr<T_Identifier> expression) => 
            this._context.Solver.Or(from child in expression.Children select child.Accept<Vertex>(this));

        internal override Vertex VisitTerm(TermExpr<T_Identifier> expression) => 
            this._context.TranslateTermToVertex(expression);

        internal override Vertex VisitTrue(TrueExpr<T_Identifier> expression) => 
            Vertex.One;
    }
}

