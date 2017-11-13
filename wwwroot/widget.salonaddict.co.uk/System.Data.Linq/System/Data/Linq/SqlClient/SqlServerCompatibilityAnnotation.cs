namespace System.Data.Linq.SqlClient
{
    using System;

    internal class SqlServerCompatibilityAnnotation : SqlNodeAnnotation
    {
        private SqlProvider.ProviderMode[] providers;

        internal SqlServerCompatibilityAnnotation(string message, params SqlProvider.ProviderMode[] providers) : base(message)
        {
            this.providers = providers;
        }

        internal bool AppliesTo(SqlProvider.ProviderMode provider)
        {
            foreach (SqlProvider.ProviderMode mode in this.providers)
            {
                if (mode == provider)
                {
                    return true;
                }
            }
            return false;
        }
    }
}

