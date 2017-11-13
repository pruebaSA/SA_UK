namespace System.Data.Mapping.ViewGeneration
{
    using System;
    using System.Data.Common.Utils;
    using System.Data.Mapping.ViewGeneration.Structures;
    using System.Data.Metadata.Edm;
    using System.Text;

    internal class SchemaContext : InternalBase
    {
        private System.Data.Mapping.ViewGeneration.Structures.ViewTarget m_viewTarget;
        private System.Data.Metadata.Edm.MetadataWorkspace m_workspace;

        public SchemaContext(System.Data.Mapping.ViewGeneration.Structures.ViewTarget viewTarget, System.Data.Metadata.Edm.MetadataWorkspace workspace)
        {
            this.m_viewTarget = viewTarget;
            this.m_workspace = workspace;
        }

        internal override void ToCompactString(StringBuilder builder)
        {
            StringUtil.FormatStringBuilder(builder, "ViewTarget = {0}", new object[] { this.m_viewTarget });
        }

        internal System.Data.Metadata.Edm.MetadataWorkspace MetadataWorkspace =>
            this.m_workspace;

        internal System.Data.Mapping.ViewGeneration.Structures.ViewTarget ViewTarget =>
            this.m_viewTarget;
    }
}

