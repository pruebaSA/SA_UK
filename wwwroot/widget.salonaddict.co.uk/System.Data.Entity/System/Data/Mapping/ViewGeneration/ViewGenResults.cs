namespace System.Data.Mapping.ViewGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common.Utils;
    using System.Data.Mapping.ViewGeneration.Structures;
    using System.Text;

    internal class ViewGenResults : InternalBase
    {
        private ErrorLog m_errorLog = new ErrorLog();
        private KeyToListMap<EntitySetBase, GeneratedView> m_views = new KeyToListMap<EntitySetBase, GeneratedView>(EqualityComparer<EntitySetBase>.Default);

        internal ViewGenResults()
        {
        }

        internal void AddErrors(ErrorLog errorLog)
        {
            this.m_errorLog.Merge(errorLog);
        }

        internal string ErrorsToString() => 
            this.m_errorLog.ToString();

        internal override void ToCompactString(StringBuilder builder)
        {
            builder.Append(this.m_errorLog.Count);
            builder.Append(" ");
            this.m_errorLog.ToCompactString(builder);
        }

        internal IEnumerable<EdmSchemaError> Errors =>
            this.m_errorLog.Errors;

        internal bool HasErrors =>
            (this.m_errorLog.Count > 0);

        internal KeyToListMap<EntitySetBase, GeneratedView> Views =>
            this.m_views;
    }
}

