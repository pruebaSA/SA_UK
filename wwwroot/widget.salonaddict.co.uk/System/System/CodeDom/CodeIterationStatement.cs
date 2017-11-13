namespace System.CodeDom
{
    using System;
    using System.Runtime.InteropServices;

    [Serializable, ComVisible(true), ClassInterface(ClassInterfaceType.AutoDispatch)]
    public class CodeIterationStatement : CodeStatement
    {
        private CodeStatement incrementStatement;
        private CodeStatement initStatement;
        private CodeStatementCollection statements;
        private CodeExpression testExpression;

        public CodeIterationStatement()
        {
            this.statements = new CodeStatementCollection();
        }

        public CodeIterationStatement(CodeStatement initStatement, CodeExpression testExpression, CodeStatement incrementStatement, params CodeStatement[] statements)
        {
            this.statements = new CodeStatementCollection();
            this.InitStatement = initStatement;
            this.TestExpression = testExpression;
            this.IncrementStatement = incrementStatement;
            this.Statements.AddRange(statements);
        }

        public CodeStatement IncrementStatement
        {
            get => 
                this.incrementStatement;
            set
            {
                this.incrementStatement = value;
            }
        }

        public CodeStatement InitStatement
        {
            get => 
                this.initStatement;
            set
            {
                this.initStatement = value;
            }
        }

        public CodeStatementCollection Statements =>
            this.statements;

        public CodeExpression TestExpression
        {
            get => 
                this.testExpression;
            set
            {
                this.testExpression = value;
            }
        }
    }
}

