namespace System.Xml.Xsl.XsltOld
{
    using System;

    internal abstract class Action
    {
        internal const int Finished = -1;
        internal const int Initialized = 0;

        protected Action()
        {
        }

        internal abstract void Execute(Processor processor, ActionFrame frame);
        internal virtual DbgData GetDbgData(ActionFrame frame) => 
            DbgData.Empty;

        internal virtual void ReplaceNamespaceAlias(Compiler compiler)
        {
        }
    }
}

