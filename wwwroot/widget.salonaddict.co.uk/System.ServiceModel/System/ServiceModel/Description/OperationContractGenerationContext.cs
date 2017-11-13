﻿namespace System.ServiceModel.Description
{
    using System;
    using System.CodeDom;
    using System.ServiceModel;

    public class OperationContractGenerationContext
    {
        private readonly CodeMemberMethod beginMethod;
        private readonly ServiceContractGenerationContext contract;
        private readonly CodeTypeDeclaration declaringType;
        private CodeTypeReference declaringTypeReference;
        private readonly CodeMemberMethod endMethod;
        private readonly OperationDescription operation;
        private readonly System.ServiceModel.Description.ServiceContractGenerator serviceContractGenerator;
        private readonly CodeMemberMethod syncMethod;

        private OperationContractGenerationContext(System.ServiceModel.Description.ServiceContractGenerator serviceContractGenerator, ServiceContractGenerationContext contract, OperationDescription operation, CodeTypeDeclaration declaringType)
        {
            if (serviceContractGenerator == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("serviceContractGenerator"));
            }
            if (contract == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("contract"));
            }
            if (declaringType == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("declaringType"));
            }
            this.serviceContractGenerator = serviceContractGenerator;
            this.contract = contract;
            this.operation = operation;
            this.declaringType = declaringType;
        }

        public OperationContractGenerationContext(System.ServiceModel.Description.ServiceContractGenerator serviceContractGenerator, ServiceContractGenerationContext contract, OperationDescription operation, CodeTypeDeclaration declaringType, CodeMemberMethod method) : this(serviceContractGenerator, contract, operation, declaringType)
        {
            if (method == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("method"));
            }
            this.syncMethod = method;
            this.beginMethod = null;
            this.endMethod = null;
        }

        public OperationContractGenerationContext(System.ServiceModel.Description.ServiceContractGenerator serviceContractGenerator, ServiceContractGenerationContext contract, OperationDescription operation, CodeTypeDeclaration declaringType, CodeMemberMethod syncMethod, CodeMemberMethod beginMethod, CodeMemberMethod endMethod) : this(serviceContractGenerator, contract, operation, declaringType)
        {
            if (syncMethod == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("syncMethod"));
            }
            if (beginMethod == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("beginMethod"));
            }
            if (endMethod == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("endMethod"));
            }
            this.syncMethod = syncMethod;
            this.beginMethod = beginMethod;
            this.endMethod = endMethod;
        }

        public CodeMemberMethod BeginMethod =>
            this.beginMethod;

        public ServiceContractGenerationContext Contract =>
            this.contract;

        public CodeTypeDeclaration DeclaringType =>
            this.declaringType;

        internal CodeTypeReference DeclaringTypeReference
        {
            get => 
                this.declaringTypeReference;
            set
            {
                this.declaringTypeReference = value;
            }
        }

        public CodeMemberMethod EndMethod =>
            this.endMethod;

        public bool IsAsync =>
            (this.beginMethod != null);

        internal bool IsInherited =>
            ((this.declaringType != this.contract.ContractType) && (this.declaringType != this.contract.DuplexCallbackType));

        public OperationDescription Operation =>
            this.operation;

        public System.ServiceModel.Description.ServiceContractGenerator ServiceContractGenerator =>
            this.serviceContractGenerator;

        public CodeMemberMethod SyncMethod =>
            this.syncMethod;
    }
}

