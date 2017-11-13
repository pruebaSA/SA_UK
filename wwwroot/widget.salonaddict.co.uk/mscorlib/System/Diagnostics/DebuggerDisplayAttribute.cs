namespace System.Diagnostics
{
    using System;
    using System.Runtime.InteropServices;

    [ComVisible(true), AttributeUsage(AttributeTargets.Delegate | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Enum | AttributeTargets.Struct | AttributeTargets.Class | AttributeTargets.Assembly, AllowMultiple=true)]
    public sealed class DebuggerDisplayAttribute : Attribute
    {
        private string name;
        private System.Type target;
        private string targetName;
        private string type;
        private string value;

        public DebuggerDisplayAttribute(string value)
        {
            if (value == null)
            {
                this.value = "";
            }
            else
            {
                this.value = value;
            }
            this.name = "";
            this.type = "";
        }

        public string Name
        {
            get => 
                this.name;
            set
            {
                this.name = value;
            }
        }

        public System.Type Target
        {
            get => 
                this.target;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                this.targetName = value.AssemblyQualifiedName;
                this.target = value;
            }
        }

        public string TargetTypeName
        {
            get => 
                this.targetName;
            set
            {
                this.targetName = value;
            }
        }

        public string Type
        {
            get => 
                this.type;
            set
            {
                this.type = value;
            }
        }

        public string Value =>
            this.value;
    }
}

