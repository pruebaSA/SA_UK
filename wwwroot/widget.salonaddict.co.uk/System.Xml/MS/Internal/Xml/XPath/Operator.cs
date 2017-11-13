namespace MS.Internal.Xml.XPath
{
    using System;
    using System.Xml.XPath;

    internal class Operator : AstNode
    {
        private AstNode opnd1;
        private AstNode opnd2;
        private Op opType;

        public Operator(Op op, AstNode opnd1, AstNode opnd2)
        {
            this.opType = op;
            this.opnd1 = opnd1;
            this.opnd2 = opnd2;
        }

        public AstNode Operand1 =>
            this.opnd1;

        public AstNode Operand2 =>
            this.opnd2;

        public Op OperatorType =>
            this.opType;

        public override XPathResultType ReturnType
        {
            get
            {
                if (this.opType < Op.PLUS)
                {
                    return XPathResultType.Boolean;
                }
                if (this.opType < Op.UNION)
                {
                    return XPathResultType.Number;
                }
                return XPathResultType.NodeSet;
            }
        }

        public override AstNode.AstType Type =>
            AstNode.AstType.Operator;

        public enum Op
        {
            LT,
            GT,
            LE,
            GE,
            EQ,
            NE,
            OR,
            AND,
            PLUS,
            MINUS,
            MUL,
            MOD,
            DIV,
            UNION,
            INVALID
        }
    }
}

