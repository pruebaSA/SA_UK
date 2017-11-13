namespace System.Web.Compilation
{
    using System;
    using System.Reflection;
    using System.Web.Util;

    internal class BuildResultCompiledType : BuildResultCompiledAssemblyBase, ITypedWebObjectFactory, IWebObjectFactory
    {
        private Type _builtType;
        private InstantiateObject _instObj;
        private bool _triedToGetInstObj;

        internal BuildResultCompiledType()
        {
        }

        internal BuildResultCompiledType(Type t)
        {
            this._builtType = t;
        }

        protected override void ComputeHashCode(HashCodeCombiner hashCodeCombiner)
        {
            base.ComputeHashCode(hashCodeCombiner);
            if (base.VirtualPath != null)
            {
                Assembly localResourcesAssembly = BuildManager.GetLocalResourcesAssembly(base.VirtualPath.Parent);
                if (localResourcesAssembly != null)
                {
                    hashCodeCombiner.AddFile(localResourcesAssembly.Location);
                }
            }
        }

        public object CreateInstance()
        {
            if (!this._triedToGetInstObj)
            {
                this._instObj = ObjectFactoryCodeDomTreeGenerator.GetFastObjectCreationDelegate(this.ResultType);
                this._triedToGetInstObj = true;
            }
            return this._instObj?.Invoke();
        }

        internal override BuildResultTypeCode GetCode() => 
            BuildResultTypeCode.BuildResultCompiledType;

        internal override void GetPreservedAttributes(PreservationFileReader pfr)
        {
            base.GetPreservedAttributes(pfr);
            Assembly preservedAssembly = BuildResultCompiledAssemblyBase.GetPreservedAssembly(pfr);
            string attribute = pfr.GetAttribute("type");
            this.ResultType = preservedAssembly.GetType(attribute, true);
        }

        internal override void SetPreservedAttributes(PreservationFileWriter pfw)
        {
            base.SetPreservedAttributes(pfw);
            pfw.SetAttribute("type", this.ResultType.FullName);
        }

        public virtual Type InstantiatedType =>
            this.ResultType;

        internal override Assembly ResultAssembly
        {
            get => 
                this._builtType.Assembly;
            set
            {
            }
        }

        internal Type ResultType
        {
            get => 
                this._builtType;
            set
            {
                this._builtType = value;
            }
        }
    }
}

