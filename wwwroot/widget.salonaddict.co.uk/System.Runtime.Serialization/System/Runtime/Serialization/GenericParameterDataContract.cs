namespace System.Runtime.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Security;

    internal sealed class GenericParameterDataContract : DataContract
    {
        [SecurityCritical]
        private GenericParameterDataContractCriticalHelper helper;

        [SecurityTreatAsSafe, SecurityCritical]
        internal GenericParameterDataContract(Type type) : base(new GenericParameterDataContractCriticalHelper(type))
        {
            this.helper = base.Helper as GenericParameterDataContractCriticalHelper;
        }

        internal override DataContract BindGenericParameters(DataContract[] paramContracts, Dictionary<DataContract, DataContract> boundContracts) => 
            paramContracts[this.ParameterPosition];

        internal override bool IsBuiltInDataContract =>
            true;

        internal int ParameterPosition =>
            this.helper.ParameterPosition;

        [SecurityCritical(SecurityCriticalScope.Everything)]
        private class GenericParameterDataContractCriticalHelper : DataContract.DataContractCriticalHelper
        {
            private int parameterPosition;

            internal GenericParameterDataContractCriticalHelper(Type type) : base(type)
            {
                base.SetDataContractName(DataContract.GetStableName(type));
                this.parameterPosition = type.GenericParameterPosition;
            }

            internal int ParameterPosition =>
                this.parameterPosition;
        }
    }
}

