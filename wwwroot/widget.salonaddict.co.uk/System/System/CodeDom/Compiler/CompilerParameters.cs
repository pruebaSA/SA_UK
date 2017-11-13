﻿namespace System.CodeDom.Compiler
{
    using Microsoft.Win32.SafeHandles;
    using System;
    using System.Collections.Specialized;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Security.Permissions;
    using System.Security.Policy;

    [Serializable, PermissionSet(SecurityAction.InheritanceDemand, Name="FullTrust"), PermissionSet(SecurityAction.LinkDemand, Name="FullTrust")]
    public class CompilerParameters
    {
        private StringCollection assemblyNames;
        private string compilerOptions;
        [OptionalField]
        private StringCollection embeddedResources;
        private System.Security.Policy.Evidence evidence;
        private bool generateExecutable;
        private bool generateInMemory;
        private bool includeDebugInformation;
        [OptionalField]
        private StringCollection linkedResources;
        private string mainClass;
        private string outputName;
        private TempFileCollection tempFiles;
        private bool treatWarningsAsErrors;
        [NonSerialized]
        private SafeUserTokenHandle userToken;
        private int warningLevel;
        private string win32Resource;

        public CompilerParameters() : this(null, null)
        {
        }

        public CompilerParameters(string[] assemblyNames) : this(assemblyNames, null, false)
        {
        }

        public CompilerParameters(string[] assemblyNames, string outputName) : this(assemblyNames, outputName, false)
        {
        }

        public CompilerParameters(string[] assemblyNames, string outputName, bool includeDebugInformation)
        {
            this.assemblyNames = new StringCollection();
            this.embeddedResources = new StringCollection();
            this.linkedResources = new StringCollection();
            this.warningLevel = -1;
            if (assemblyNames != null)
            {
                this.ReferencedAssemblies.AddRange(assemblyNames);
            }
            this.outputName = outputName;
            this.includeDebugInformation = includeDebugInformation;
        }

        public string CompilerOptions
        {
            get => 
                this.compilerOptions;
            set
            {
                this.compilerOptions = value;
            }
        }

        [ComVisible(false)]
        public StringCollection EmbeddedResources =>
            this.embeddedResources;

        public System.Security.Policy.Evidence Evidence
        {
            get
            {
                System.Security.Policy.Evidence evidence = null;
                if (this.evidence != null)
                {
                    evidence = CompilerResults.CloneEvidence(this.evidence);
                }
                return evidence;
            }
            [SecurityPermission(SecurityAction.Demand, ControlEvidence=true)]
            set
            {
                if (value != null)
                {
                    this.evidence = CompilerResults.CloneEvidence(value);
                }
                else
                {
                    this.evidence = null;
                }
            }
        }

        public bool GenerateExecutable
        {
            get => 
                this.generateExecutable;
            set
            {
                this.generateExecutable = value;
            }
        }

        public bool GenerateInMemory
        {
            get => 
                this.generateInMemory;
            set
            {
                this.generateInMemory = value;
            }
        }

        public bool IncludeDebugInformation
        {
            get => 
                this.includeDebugInformation;
            set
            {
                this.includeDebugInformation = value;
            }
        }

        [ComVisible(false)]
        public StringCollection LinkedResources =>
            this.linkedResources;

        public string MainClass
        {
            get => 
                this.mainClass;
            set
            {
                this.mainClass = value;
            }
        }

        public string OutputAssembly
        {
            get => 
                this.outputName;
            set
            {
                this.outputName = value;
            }
        }

        public StringCollection ReferencedAssemblies =>
            this.assemblyNames;

        internal SafeUserTokenHandle SafeUserToken =>
            this.userToken;

        public TempFileCollection TempFiles
        {
            get
            {
                if (this.tempFiles == null)
                {
                    this.tempFiles = new TempFileCollection();
                }
                return this.tempFiles;
            }
            set
            {
                this.tempFiles = value;
            }
        }

        public bool TreatWarningsAsErrors
        {
            get => 
                this.treatWarningsAsErrors;
            set
            {
                this.treatWarningsAsErrors = value;
            }
        }

        public IntPtr UserToken
        {
            get
            {
                if (this.userToken != null)
                {
                    return this.userToken.DangerousGetHandle();
                }
                return IntPtr.Zero;
            }
            set
            {
                if (this.userToken != null)
                {
                    this.userToken.Close();
                }
                this.userToken = new SafeUserTokenHandle(value, false);
            }
        }

        public int WarningLevel
        {
            get => 
                this.warningLevel;
            set
            {
                this.warningLevel = value;
            }
        }

        public string Win32Resource
        {
            get => 
                this.win32Resource;
            set
            {
                this.win32Resource = value;
            }
        }
    }
}

