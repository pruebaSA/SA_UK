namespace System.Xml.Xsl.Xslt
{
    using System;
    using System.CodeDom;
    using System.CodeDom.Compiler;
    using System.Collections.Specialized;
    using System.Threading;
    using System.Xml.Xsl;

    internal class ScriptClass
    {
        public CompilerInfo compilerInfo;
        public string endFileName;
        public int endLine;
        public int endPos;
        public string ns;
        public StringCollection nsImports = new StringCollection();
        public StringCollection refAssemblies = new StringCollection();
        public bool refAssembliesByHref;
        private static long scriptClassCounter;
        public StringCollection scriptFiles = new StringCollection();
        public CodeTypeDeclaration typeDecl;

        public ScriptClass(string ns, CompilerInfo compilerInfo)
        {
            this.ns = ns;
            this.compilerInfo = compilerInfo;
            this.typeDecl = new CodeTypeDeclaration(GenerateUniqueClassName());
        }

        public void AddScriptBlock(string source, string uriString, int lineNumber, int endLine, int endPos)
        {
            CodeSnippetTypeMember member = new CodeSnippetTypeMember(source);
            string fileName = SourceLineInfo.GetFileName(uriString);
            if (lineNumber > 0)
            {
                member.LinePragma = new CodeLinePragma(fileName, lineNumber);
                this.scriptFiles.Add(fileName);
            }
            this.typeDecl.Members.Add(member);
            this.endFileName = fileName;
            this.endLine = endLine;
            this.endPos = endPos;
        }

        public CompilerError CreateCompileExceptionError(Exception e) => 
            new CompilerError(this.endFileName, this.endLine, this.endPos, string.Empty, XslTransformException.CreateMessage("Xslt_ScriptCompileException", new string[] { e.Message }));

        private static string GenerateUniqueClassName() => 
            ("Script" + Interlocked.Increment(ref scriptClassCounter));
    }
}

