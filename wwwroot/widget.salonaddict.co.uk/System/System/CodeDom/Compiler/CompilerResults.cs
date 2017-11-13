namespace System.CodeDom.Compiler
{
    using System;
    using System.Collections.Specialized;
    using System.IO;
    using System.Reflection;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Security;
    using System.Security.Permissions;
    using System.Security.Policy;

    [Serializable, PermissionSet(SecurityAction.InheritanceDemand, Name="FullTrust")]
    public class CompilerResults
    {
        private Assembly compiledAssembly;
        private CompilerErrorCollection errors = new CompilerErrorCollection();
        private System.Security.Policy.Evidence evidence;
        private int nativeCompilerReturnValue;
        private StringCollection output = new StringCollection();
        private string pathToAssembly;
        private TempFileCollection tempFiles;

        [PermissionSet(SecurityAction.LinkDemand, Name="FullTrust")]
        public CompilerResults(TempFileCollection tempFiles)
        {
            this.tempFiles = tempFiles;
        }

        internal static System.Security.Policy.Evidence CloneEvidence(System.Security.Policy.Evidence ev)
        {
            new PermissionSet(PermissionState.Unrestricted).Assert();
            MemoryStream serializationStream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(serializationStream, ev);
            serializationStream.Position = 0L;
            return (System.Security.Policy.Evidence) formatter.Deserialize(serializationStream);
        }

        public Assembly CompiledAssembly
        {
            [SecurityPermission(SecurityAction.Assert, Flags=SecurityPermissionFlag.ControlEvidence)]
            get
            {
                if ((this.compiledAssembly == null) && (this.pathToAssembly != null))
                {
                    AssemblyName assemblyRef = new AssemblyName {
                        CodeBase = this.pathToAssembly
                    };
                    this.compiledAssembly = Assembly.Load(assemblyRef, this.evidence);
                }
                return this.compiledAssembly;
            }
            [PermissionSet(SecurityAction.LinkDemand, Name="FullTrust")]
            set
            {
                this.compiledAssembly = value;
            }
        }

        public CompilerErrorCollection Errors =>
            this.errors;

        public System.Security.Policy.Evidence Evidence
        {
            [PermissionSet(SecurityAction.LinkDemand, Name="FullTrust")]
            get
            {
                System.Security.Policy.Evidence evidence = null;
                if (this.evidence != null)
                {
                    evidence = CloneEvidence(this.evidence);
                }
                return evidence;
            }
            [PermissionSet(SecurityAction.LinkDemand, Name="FullTrust"), SecurityPermission(SecurityAction.Demand, ControlEvidence=true)]
            set
            {
                if (value != null)
                {
                    this.evidence = CloneEvidence(value);
                }
                else
                {
                    this.evidence = null;
                }
            }
        }

        public int NativeCompilerReturnValue
        {
            get => 
                this.nativeCompilerReturnValue;
            [PermissionSet(SecurityAction.LinkDemand, Name="FullTrust")]
            set
            {
                this.nativeCompilerReturnValue = value;
            }
        }

        public StringCollection Output =>
            this.output;

        public string PathToAssembly
        {
            [PermissionSet(SecurityAction.LinkDemand, Name="FullTrust")]
            get => 
                this.pathToAssembly;
            [PermissionSet(SecurityAction.LinkDemand, Name="FullTrust")]
            set
            {
                this.pathToAssembly = value;
            }
        }

        public TempFileCollection TempFiles
        {
            [PermissionSet(SecurityAction.LinkDemand, Name="FullTrust")]
            get => 
                this.tempFiles;
            [PermissionSet(SecurityAction.LinkDemand, Name="FullTrust")]
            set
            {
                this.tempFiles = value;
            }
        }
    }
}

