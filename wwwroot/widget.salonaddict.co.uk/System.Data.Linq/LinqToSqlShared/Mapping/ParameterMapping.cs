namespace LinqToSqlShared.Mapping
{
    using System;

    internal class ParameterMapping
    {
        private string dbType;
        private MappingParameterDirection direction;
        private string name;
        private string parameterName;

        internal string DbType
        {
            get => 
                this.dbType;
            set
            {
                this.dbType = value;
            }
        }

        public MappingParameterDirection Direction
        {
            get => 
                this.direction;
            set
            {
                this.direction = value;
            }
        }

        internal string Name
        {
            get => 
                this.name;
            set
            {
                this.name = value;
            }
        }

        internal string ParameterName
        {
            get => 
                this.parameterName;
            set
            {
                this.parameterName = value;
            }
        }

        public string XmlDirection
        {
            get
            {
                if (this.direction != MappingParameterDirection.In)
                {
                    return this.direction.ToString();
                }
                return null;
            }
            set
            {
                this.direction = (value == null) ? MappingParameterDirection.In : ((MappingParameterDirection) Enum.Parse(typeof(MappingParameterDirection), value, true));
            }
        }
    }
}

