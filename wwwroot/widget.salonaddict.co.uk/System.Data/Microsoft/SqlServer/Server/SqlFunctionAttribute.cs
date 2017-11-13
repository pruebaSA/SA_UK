namespace Microsoft.SqlServer.Server
{
    using System;

    [Serializable, AttributeUsage(AttributeTargets.Method, AllowMultiple=false, Inherited=false)]
    public class SqlFunctionAttribute : Attribute
    {
        private DataAccessKind m_eDataAccess = DataAccessKind.None;
        private SystemDataAccessKind m_eSystemDataAccess = SystemDataAccessKind.None;
        private bool m_fDeterministic = false;
        private string m_FillRowMethodName = null;
        private string m_fName = null;
        private bool m_fPrecise = false;
        private string m_fTableDefinition = null;

        public DataAccessKind DataAccess
        {
            get => 
                this.m_eDataAccess;
            set
            {
                this.m_eDataAccess = value;
            }
        }

        public string FillRowMethodName
        {
            get => 
                this.m_FillRowMethodName;
            set
            {
                this.m_FillRowMethodName = value;
            }
        }

        public bool IsDeterministic
        {
            get => 
                this.m_fDeterministic;
            set
            {
                this.m_fDeterministic = value;
            }
        }

        public bool IsPrecise
        {
            get => 
                this.m_fPrecise;
            set
            {
                this.m_fPrecise = value;
            }
        }

        public string Name
        {
            get => 
                this.m_fName;
            set
            {
                this.m_fName = value;
            }
        }

        public SystemDataAccessKind SystemDataAccess
        {
            get => 
                this.m_eSystemDataAccess;
            set
            {
                this.m_eSystemDataAccess = value;
            }
        }

        public string TableDefinition
        {
            get => 
                this.m_fTableDefinition;
            set
            {
                this.m_fTableDefinition = value;
            }
        }
    }
}

