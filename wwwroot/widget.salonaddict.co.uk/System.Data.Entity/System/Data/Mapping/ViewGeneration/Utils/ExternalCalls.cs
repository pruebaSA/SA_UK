namespace System.Data.Mapping.ViewGeneration.Utils
{
    using System;
    using System.Data.Common.CommandTrees;
    using System.Data.Common.EntitySql;
    using System.Data.Metadata.Edm;

    internal static class ExternalCalls
    {
        internal static DbCommandTree CompileView(string viewDef, MetadataWorkspace metadataWorkspace, ParserOptions.CompilationMode compilationMode)
        {
            Perspective perspective = new TargetPerspective(metadataWorkspace);
            ParserOptions parserOptions = new ParserOptions {
                ParserCompilationMode = compilationMode
            };
            return CqlQuery.Compile(viewDef, perspective, parserOptions, null, null, true);
        }

        internal static ItemCollection GetItemCollection(MetadataWorkspace workspace, DataSpace space) => 
            workspace.GetItemCollection(space);

        internal static bool IsReservedKeyword(string name) => 
            CqlLexer.IsReservedKeyword(name);
    }
}

