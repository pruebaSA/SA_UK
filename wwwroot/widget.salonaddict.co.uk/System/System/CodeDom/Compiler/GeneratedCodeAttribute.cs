namespace System.CodeDom.Compiler
{
    using System;

    [AttributeUsage(AttributeTargets.All, Inherited=false, AllowMultiple=false)]
    public sealed class GeneratedCodeAttribute : Attribute
    {
        private readonly string tool;
        private readonly string version;

        public GeneratedCodeAttribute(string tool, string version)
        {
            this.tool = tool;
            this.version = version;
        }

        public string Tool =>
            this.tool;

        public string Version =>
            this.version;
    }
}

