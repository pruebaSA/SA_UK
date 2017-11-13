namespace System.Deployment.Application
{
    using System;
    using System.Reflection;

    internal class AssemblyReference
    {
        private AssemblyName _name;

        public AssemblyReference(AssemblyName name)
        {
            this._name = name;
        }

        public AssemblyName Name =>
            this._name;
    }
}

