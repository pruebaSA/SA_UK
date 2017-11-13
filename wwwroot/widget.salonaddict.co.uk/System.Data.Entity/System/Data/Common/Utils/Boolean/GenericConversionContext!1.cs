namespace System.Data.Common.Utils.Boolean
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal sealed class GenericConversionContext<T_Identifier> : ConversionContext<T_Identifier>
    {
        private Dictionary<int, TermExpr<T_Identifier>> _inverseVariableMap;
        private readonly Dictionary<TermExpr<T_Identifier>, int> _variableMap;

        public GenericConversionContext()
        {
            this._variableMap = new Dictionary<TermExpr<T_Identifier>, int>();
        }

        internal override IEnumerable<LiteralVertexPair<T_Identifier>> GetSuccessors(Vertex vertex)
        {
            LiteralVertexPair<T_Identifier>[] pairArray = new LiteralVertexPair<T_Identifier>[2];
            Vertex vertex2 = vertex.Children[0];
            Vertex vertex3 = vertex.Children[1];
            this.InitializeInverseVariableMap();
            TermExpr<T_Identifier> term = this._inverseVariableMap[vertex.Variable];
            Literal<T_Identifier> literal = new Literal<T_Identifier>(term, true);
            pairArray[0] = new LiteralVertexPair<T_Identifier>(vertex2, literal);
            literal = literal.MakeNegated();
            pairArray[1] = new LiteralVertexPair<T_Identifier>(vertex3, literal);
            return pairArray;
        }

        private void InitializeInverseVariableMap()
        {
            if (this._inverseVariableMap == null)
            {
                this._inverseVariableMap = this._variableMap.ToDictionary<KeyValuePair<TermExpr<T_Identifier>, int>, int, TermExpr<T_Identifier>>(kvp => kvp.Value, kvp => kvp.Key);
            }
        }

        internal override Vertex TranslateTermToVertex(TermExpr<T_Identifier> term)
        {
            int num;
            if (!this._variableMap.TryGetValue(term, out num))
            {
                num = base.Solver.CreateVariable();
                this._variableMap.Add(term, num);
            }
            return base.Solver.CreateLeafVertex(num, Solver.BooleanVariableChildren);
        }
    }
}

