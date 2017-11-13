namespace System.Data.Linq
{
    using System;

    internal sealed class ChangeConflictSession
    {
        private DataContext context;
        private DataContext refreshContext;

        internal ChangeConflictSession(DataContext context)
        {
            this.context = context;
        }

        internal DataContext Context =>
            this.context;

        internal DataContext RefreshContext
        {
            get
            {
                if (this.refreshContext == null)
                {
                    this.refreshContext = this.context.CreateRefreshContext();
                }
                return this.refreshContext;
            }
        }
    }
}

