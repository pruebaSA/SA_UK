namespace System.Xml.Xsl.XsltOld
{
    using System;

    internal abstract class Event
    {
        protected Event()
        {
        }

        internal void OnInstructionExecute(Processor processor)
        {
            processor.OnInstructionExecute();
        }

        public abstract bool Output(Processor processor, ActionFrame frame);
        public virtual void ReplaceNamespaceAlias(Compiler compiler)
        {
        }

        internal virtual System.Xml.Xsl.XsltOld.DbgData DbgData =>
            System.Xml.Xsl.XsltOld.DbgData.Empty;
    }
}

