namespace System.ServiceModel.Description
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Net.Security;
    using System.Reflection;
    using System.ServiceModel;
    using System.ServiceModel.Security;

    [DebuggerDisplay("Name={name}, IsInitiating={isInitiating}, IsTerminating={isTerminating}")]
    public class OperationDescription
    {
        private MethodInfo beginMethod;
        private KeyedByTypeCollection<IOperationBehavior> behaviors;
        private ContractDescription declaringContract;
        private MethodInfo endMethod;
        private FaultDescriptionCollection faults;
        private bool hasNoDisposableParameters;
        private bool hasProtectionLevel;
        private bool isInitiating;
        private bool isTerminating;
        private Collection<Type> knownTypes;
        private MessageDescriptionCollection messages;
        private System.ServiceModel.Description.XmlName name;
        private System.Net.Security.ProtectionLevel protectionLevel;
        private MethodInfo syncMethod;
        private bool validateRpcWrapperName;

        public OperationDescription(string name, ContractDescription declaringContract)
        {
            this.validateRpcWrapperName = true;
            if (name == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("name");
            }
            if (name.Length == 0)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("name", System.ServiceModel.SR.GetString("SFxOperationDescriptionNameCannotBeEmpty")));
            }
            this.name = new System.ServiceModel.Description.XmlName(name, true);
            if (declaringContract == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("declaringContract");
            }
            this.declaringContract = declaringContract;
            this.isInitiating = true;
            this.isTerminating = false;
            this.faults = new FaultDescriptionCollection();
            this.messages = new MessageDescriptionCollection();
            this.behaviors = new KeyedByTypeCollection<IOperationBehavior>();
            this.knownTypes = new Collection<Type>();
        }

        internal OperationDescription(string name, ContractDescription declaringContract, bool validateRpcWrapperName) : this(name, declaringContract)
        {
            this.validateRpcWrapperName = validateRpcWrapperName;
        }

        internal void EnsureInvariants()
        {
            if ((this.Messages.Count != 1) && (this.Messages.Count != 2))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("SFxOperationMustHaveOneOrTwoMessages", new object[] { this.Name })));
            }
        }

        internal bool IsServerInitiated()
        {
            this.EnsureInvariants();
            return (this.Messages[0].Direction == MessageDirection.Output);
        }

        internal void ResetProtectionLevel()
        {
            this.protectionLevel = System.Net.Security.ProtectionLevel.None;
            this.hasProtectionLevel = false;
        }

        public MethodInfo BeginMethod
        {
            get => 
                this.beginMethod;
            set
            {
                this.beginMethod = value;
            }
        }

        public KeyedByTypeCollection<IOperationBehavior> Behaviors =>
            this.behaviors;

        internal string CodeName =>
            this.name.DecodedName;

        public ContractDescription DeclaringContract
        {
            get => 
                this.declaringContract;
            set
            {
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("DeclaringContract");
                }
                this.declaringContract = value;
            }
        }

        public MethodInfo EndMethod
        {
            get => 
                this.endMethod;
            set
            {
                this.endMethod = value;
            }
        }

        public FaultDescriptionCollection Faults =>
            this.faults;

        internal bool HasNoDisposableParameters
        {
            get => 
                this.hasNoDisposableParameters;
            set
            {
                this.hasNoDisposableParameters = value;
            }
        }

        public bool HasProtectionLevel =>
            this.hasProtectionLevel;

        public bool IsInitiating
        {
            get => 
                this.isInitiating;
            set
            {
                this.isInitiating = value;
            }
        }

        public bool IsOneWay =>
            (this.Messages.Count == 1);

        public bool IsTerminating
        {
            get => 
                this.isTerminating;
            set
            {
                this.isTerminating = value;
            }
        }

        internal bool IsValidateRpcWrapperName =>
            this.validateRpcWrapperName;

        public Collection<Type> KnownTypes =>
            this.knownTypes;

        public MessageDescriptionCollection Messages =>
            this.messages;

        public string Name =>
            this.name.EncodedName;

        internal MethodInfo OperationMethod =>
            (this.SyncMethod ?? this.BeginMethod);

        public System.Net.Security.ProtectionLevel ProtectionLevel
        {
            get => 
                this.protectionLevel;
            set
            {
                if (!ProtectionLevelHelper.IsDefined(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value"));
                }
                this.protectionLevel = value;
                this.hasProtectionLevel = true;
            }
        }

        public MethodInfo SyncMethod
        {
            get => 
                this.syncMethod;
            set
            {
                this.syncMethod = value;
            }
        }

        internal System.ServiceModel.Description.XmlName XmlName =>
            this.name;
    }
}

