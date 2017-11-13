namespace System.Xml.Xsl.XsltOld
{
    using System;
    using System.Xml.XPath;

    internal class DbgData
    {
        private static DbgData s_nullDbgData = new DbgData();
        private XPathNavigator styleSheet;
        private VariableAction[] variables;

        private DbgData()
        {
            this.styleSheet = null;
            this.variables = new VariableAction[0];
        }

        public DbgData(Compiler compiler)
        {
            DbgCompiler compiler2 = (DbgCompiler) compiler;
            this.styleSheet = compiler2.Input.Navigator.Clone();
            this.variables = compiler2.LocalVariables;
            compiler2.Debugger.OnInstructionCompile(this.StyleSheet);
        }

        internal void ReplaceVariables(VariableAction[] vars)
        {
            this.variables = vars;
        }

        public static DbgData Empty =>
            s_nullDbgData;

        public XPathNavigator StyleSheet =>
            this.styleSheet;

        public VariableAction[] Variables =>
            this.variables;
    }
}

