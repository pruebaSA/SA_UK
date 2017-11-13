namespace System.Data.Design
{
    using System;
    using System.CodeDom;
    using System.ComponentModel;

    internal abstract class Source : DataSourceComponent, IDataSourceNamedObject, INamedObject, ICloneable
    {
        private string generatorGetMethodName;
        private string generatorGetMethodNameForPaging;
        private string generatorSourceName;
        private string generatorSourceNameForPaging;
        private MemberAttributes modifier = MemberAttributes.Public;
        protected string name;
        protected DataSourceComponent owner;
        private string userSourceName;
        private bool webMethod;
        private string webMethodDescription;

        internal Source()
        {
        }

        public abstract object Clone();
        internal virtual bool NameExist(string nameToCheck) => 
            StringUtil.EqualValue(this.Name, nameToCheck, true);

        public override void SetCollection(DataSourceCollectionBase collection)
        {
            base.SetCollection(collection);
            if (collection != null)
            {
                this.Owner = collection.CollectionHost;
            }
            else
            {
                this.Owner = null;
            }
        }

        public override string ToString() => 
            (this.PublicTypeName + " " + this.DisplayName);

        internal virtual string DisplayName
        {
            get => 
                this.Name;
            set
            {
            }
        }

        [DefaultValue(false), DataSourceXmlAttribute]
        public bool EnableWebMethods
        {
            get => 
                this.webMethod;
            set
            {
                this.webMethod = value;
            }
        }

        [Browsable(false), DefaultValue((string) null), DataSourceXmlAttribute]
        public string GeneratorGetMethodName
        {
            get => 
                this.generatorGetMethodName;
            set
            {
                this.generatorGetMethodName = value;
            }
        }

        [DefaultValue((string) null), DataSourceXmlAttribute, Browsable(false)]
        public string GeneratorGetMethodNameForPaging
        {
            get => 
                this.generatorGetMethodNameForPaging;
            set
            {
                this.generatorGetMethodNameForPaging = value;
            }
        }

        [Browsable(false)]
        public override string GeneratorName =>
            this.GeneratorSourceName;

        [DataSourceXmlAttribute, Browsable(false), DefaultValue((string) null)]
        public string GeneratorSourceName
        {
            get => 
                this.generatorSourceName;
            set
            {
                this.generatorSourceName = value;
            }
        }

        [DefaultValue((string) null), DataSourceXmlAttribute, Browsable(false)]
        public string GeneratorSourceNameForPaging
        {
            get => 
                this.generatorSourceNameForPaging;
            set
            {
                this.generatorSourceNameForPaging = value;
            }
        }

        internal bool IsMainSource
        {
            get
            {
                DesignTable owner = this.Owner as DesignTable;
                return ((owner != null) && (owner.MainSource == this));
            }
        }

        [DefaultValue(0x6000), DataSourceXmlAttribute]
        public MemberAttributes Modifier
        {
            get => 
                this.modifier;
            set
            {
                this.modifier = value;
            }
        }

        [DefaultValue(""), MergableProperty(false), DataSourceXmlAttribute]
        public virtual string Name
        {
            get => 
                this.name;
            set
            {
                if (this.name != value)
                {
                    if (this.CollectionParent != null)
                    {
                        this.CollectionParent.ValidateUniqueName(this, value);
                    }
                    this.name = value;
                }
            }
        }

        [Browsable(false)]
        internal DataSourceComponent Owner
        {
            get
            {
                if ((this.owner == null) && (this.CollectionParent != null))
                {
                    SourceCollection collectionParent = this.CollectionParent as SourceCollection;
                    if (collectionParent != null)
                    {
                        this.owner = collectionParent.CollectionHost;
                    }
                }
                return this.owner;
            }
            set
            {
                this.owner = value;
            }
        }

        [Browsable(false)]
        public virtual string PublicTypeName =>
            "Function";

        [DefaultValue((string) null), DataSourceXmlAttribute, Browsable(false)]
        public string UserSourceName
        {
            get => 
                this.userSourceName;
            set
            {
                this.userSourceName = value;
            }
        }

        [DefaultValue(""), DataSourceXmlAttribute]
        public string WebMethodDescription
        {
            get => 
                this.webMethodDescription;
            set
            {
                this.webMethodDescription = value;
            }
        }
    }
}

