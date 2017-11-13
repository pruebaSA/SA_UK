namespace System.Runtime.Serialization
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;

    public class ImportOptions
    {
        private CodeDomProvider codeProvider;
        private IDataContractSurrogate dataContractSurrogate;
        private bool enableDataBinding;
        private bool generateInternal;
        private bool generateSerializable;
        private bool importXmlType;
        private IDictionary<string, string> namespaces;
        private ICollection<Type> referencedCollectionTypes;
        private ICollection<Type> referencedTypes;

        public CodeDomProvider CodeProvider
        {
            get => 
                this.codeProvider;
            set
            {
                this.codeProvider = value;
            }
        }

        public IDataContractSurrogate DataContractSurrogate
        {
            get => 
                this.dataContractSurrogate;
            set
            {
                this.dataContractSurrogate = value;
            }
        }

        public bool EnableDataBinding
        {
            get => 
                this.enableDataBinding;
            set
            {
                this.enableDataBinding = value;
            }
        }

        public bool GenerateInternal
        {
            get => 
                this.generateInternal;
            set
            {
                this.generateInternal = value;
            }
        }

        public bool GenerateSerializable
        {
            get => 
                this.generateSerializable;
            set
            {
                this.generateSerializable = value;
            }
        }

        public bool ImportXmlType
        {
            get => 
                this.importXmlType;
            set
            {
                this.importXmlType = value;
            }
        }

        public IDictionary<string, string> Namespaces
        {
            get
            {
                if (this.namespaces == null)
                {
                    this.namespaces = new Dictionary<string, string>();
                }
                return this.namespaces;
            }
        }

        public ICollection<Type> ReferencedCollectionTypes
        {
            get
            {
                if (this.referencedCollectionTypes == null)
                {
                    this.referencedCollectionTypes = new List<Type>();
                }
                return this.referencedCollectionTypes;
            }
        }

        public ICollection<Type> ReferencedTypes
        {
            get
            {
                if (this.referencedTypes == null)
                {
                    this.referencedTypes = new List<Type>();
                }
                return this.referencedTypes;
            }
        }
    }
}

