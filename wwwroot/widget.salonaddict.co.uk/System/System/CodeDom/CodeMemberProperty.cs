namespace System.CodeDom
{
    using System;
    using System.Runtime.InteropServices;

    [Serializable, ClassInterface(ClassInterfaceType.AutoDispatch), ComVisible(true)]
    public class CodeMemberProperty : CodeTypeMember
    {
        private CodeStatementCollection getStatements = new CodeStatementCollection();
        private bool hasGet;
        private bool hasSet;
        private CodeTypeReferenceCollection implementationTypes;
        private CodeParameterDeclarationExpressionCollection parameters = new CodeParameterDeclarationExpressionCollection();
        private CodeTypeReference privateImplements;
        private CodeStatementCollection setStatements = new CodeStatementCollection();
        private CodeTypeReference type;

        public CodeStatementCollection GetStatements =>
            this.getStatements;

        public bool HasGet
        {
            get
            {
                if (!this.hasGet)
                {
                    return (this.getStatements.Count > 0);
                }
                return true;
            }
            set
            {
                this.hasGet = value;
                if (!value)
                {
                    this.getStatements.Clear();
                }
            }
        }

        public bool HasSet
        {
            get
            {
                if (!this.hasSet)
                {
                    return (this.setStatements.Count > 0);
                }
                return true;
            }
            set
            {
                this.hasSet = value;
                if (!value)
                {
                    this.setStatements.Clear();
                }
            }
        }

        public CodeTypeReferenceCollection ImplementationTypes
        {
            get
            {
                if (this.implementationTypes == null)
                {
                    this.implementationTypes = new CodeTypeReferenceCollection();
                }
                return this.implementationTypes;
            }
        }

        public CodeParameterDeclarationExpressionCollection Parameters =>
            this.parameters;

        public CodeTypeReference PrivateImplementationType
        {
            get => 
                this.privateImplements;
            set
            {
                this.privateImplements = value;
            }
        }

        public CodeStatementCollection SetStatements =>
            this.setStatements;

        public CodeTypeReference Type
        {
            get
            {
                if (this.type == null)
                {
                    this.type = new CodeTypeReference("");
                }
                return this.type;
            }
            set
            {
                this.type = value;
            }
        }
    }
}

