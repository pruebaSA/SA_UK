namespace System.Runtime.CompilerServices
{
    using System;

    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple=true, Inherited=false)]
    public sealed class InternalsVisibleToAttribute : Attribute
    {
        private bool _allInternalsVisible = true;
        private string _assemblyName;

        public InternalsVisibleToAttribute(string assemblyName)
        {
            this._assemblyName = assemblyName;
        }

        public bool AllInternalsVisible
        {
            get => 
                this._allInternalsVisible;
            set
            {
                this._allInternalsVisible = value;
            }
        }

        public string AssemblyName =>
            this._assemblyName;
    }
}

