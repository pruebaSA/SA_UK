namespace System.Runtime.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Security;

    internal class DataMember
    {
        [SecurityCritical]
        private CriticalHelper helper;

        [SecurityCritical, SecurityTreatAsSafe]
        internal DataMember()
        {
            this.helper = new CriticalHelper();
        }

        [SecurityTreatAsSafe, SecurityCritical]
        internal DataMember(System.Reflection.MemberInfo memberInfo)
        {
            this.helper = new CriticalHelper(memberInfo);
        }

        [SecurityCritical, SecurityTreatAsSafe]
        internal DataMember(string name)
        {
            this.helper = new CriticalHelper(name);
        }

        [SecurityTreatAsSafe, SecurityCritical]
        internal DataMember(DataContract memberTypeContract, string name, bool isNullable, bool isRequired, bool emitDefaultValue, int order)
        {
            this.helper = new CriticalHelper(memberTypeContract, name, isNullable, isRequired, emitDefaultValue, order);
        }

        internal DataMember BindGenericParameters(DataContract[] paramContracts, Dictionary<DataContract, DataContract> boundContracts)
        {
            DataContract memberTypeContract = this.MemberTypeContract.BindGenericParameters(paramContracts, boundContracts);
            return new DataMember(memberTypeContract, this.Name, !memberTypeContract.IsValueType, this.IsRequired, this.EmitDefaultValue, this.Order);
        }

        internal bool Equals(object other, Dictionary<DataContractPairKey, object> checkedContracts)
        {
            if (this == other)
            {
                return true;
            }
            DataMember member = other as DataMember;
            if (member == null)
            {
                return false;
            }
            bool flag = (this.MemberTypeContract != null) && !this.MemberTypeContract.IsValueType;
            bool flag2 = (member.MemberTypeContract != null) && !member.MemberTypeContract.IsValueType;
            return ((((this.Name == member.Name) && ((this.IsNullable || flag) == (member.IsNullable || flag2))) && ((this.IsRequired == member.IsRequired) && (this.EmitDefaultValue == member.EmitDefaultValue))) && this.MemberTypeContract.Equals(member.MemberTypeContract, checkedContracts));
        }

        public override int GetHashCode() => 
            base.GetHashCode();

        internal bool RequiresMemberAccessForGet()
        {
            System.Reflection.MemberInfo memberInfo = this.MemberInfo;
            FieldInfo field = memberInfo as FieldInfo;
            if (field != null)
            {
                return DataContract.FieldRequiresMemberAccess(field);
            }
            MethodInfo getMethod = ((PropertyInfo) memberInfo).GetGetMethod(true);
            return ((getMethod != null) && DataContract.MethodRequiresMemberAccess(getMethod));
        }

        internal bool RequiresMemberAccessForSet()
        {
            System.Reflection.MemberInfo memberInfo = this.MemberInfo;
            FieldInfo field = memberInfo as FieldInfo;
            if (field != null)
            {
                return DataContract.FieldRequiresMemberAccess(field);
            }
            MethodInfo setMethod = ((PropertyInfo) memberInfo).GetSetMethod(true);
            return ((setMethod != null) && DataContract.MethodRequiresMemberAccess(setMethod));
        }

        internal DataMember ConflictingMember
        {
            [SecurityTreatAsSafe, SecurityCritical]
            get => 
                this.helper.ConflictingMember;
            [SecurityCritical]
            set
            {
                this.helper.ConflictingMember = value;
            }
        }

        internal bool EmitDefaultValue
        {
            [SecurityCritical, SecurityTreatAsSafe]
            get => 
                this.helper.EmitDefaultValue;
            [SecurityCritical]
            set
            {
                this.helper.EmitDefaultValue = value;
            }
        }

        internal bool HasConflictingNameAndType
        {
            [SecurityCritical, SecurityTreatAsSafe]
            get => 
                this.helper.HasConflictingNameAndType;
            [SecurityCritical]
            set
            {
                this.helper.HasConflictingNameAndType = value;
            }
        }

        internal bool IsGetOnlyCollection
        {
            [SecurityCritical, SecurityTreatAsSafe]
            get => 
                this.helper.IsGetOnlyCollection;
            [SecurityCritical]
            set
            {
                this.helper.IsGetOnlyCollection = value;
            }
        }

        internal bool IsNullable
        {
            [SecurityTreatAsSafe, SecurityCritical]
            get => 
                this.helper.IsNullable;
            [SecurityCritical]
            set
            {
                this.helper.IsNullable = value;
            }
        }

        internal bool IsRequired
        {
            [SecurityCritical, SecurityTreatAsSafe]
            get => 
                this.helper.IsRequired;
            [SecurityCritical]
            set
            {
                this.helper.IsRequired = value;
            }
        }

        internal System.Reflection.MemberInfo MemberInfo =>
            this.helper.MemberInfo;

        internal Type MemberType =>
            this.helper.MemberType;

        internal DataContract MemberTypeContract
        {
            [SecurityCritical, SecurityTreatAsSafe]
            get => 
                this.helper.MemberTypeContract;
            [SecurityCritical]
            set
            {
                this.helper.MemberTypeContract = value;
            }
        }

        internal string Name
        {
            [SecurityTreatAsSafe, SecurityCritical]
            get => 
                this.helper.Name;
            [SecurityCritical]
            set
            {
                this.helper.Name = value;
            }
        }

        internal int Order
        {
            [SecurityCritical, SecurityTreatAsSafe]
            get => 
                this.helper.Order;
            [SecurityCritical]
            set
            {
                this.helper.Order = value;
            }
        }

        [SecurityCritical(SecurityCriticalScope.Everything)]
        private class CriticalHelper
        {
            private DataMember conflictingMember;
            private bool emitDefaultValue;
            private bool hasConflictingNameAndType;
            private bool isGetOnlyCollection;
            private bool isNullable;
            private bool isRequired;
            private System.Reflection.MemberInfo memberInfo;
            private DataContract memberTypeContract;
            private string name;
            private int order;

            internal CriticalHelper()
            {
                this.emitDefaultValue = true;
            }

            internal CriticalHelper(System.Reflection.MemberInfo memberInfo)
            {
                this.emitDefaultValue = true;
                this.memberInfo = memberInfo;
            }

            internal CriticalHelper(string name)
            {
                this.Name = name;
            }

            internal CriticalHelper(DataContract memberTypeContract, string name, bool isNullable, bool isRequired, bool emitDefaultValue, int order)
            {
                this.MemberTypeContract = memberTypeContract;
                this.Name = name;
                this.IsNullable = isNullable;
                this.IsRequired = isRequired;
                this.EmitDefaultValue = emitDefaultValue;
                this.Order = order;
            }

            internal DataMember ConflictingMember
            {
                get => 
                    this.conflictingMember;
                set
                {
                    this.conflictingMember = value;
                }
            }

            internal bool EmitDefaultValue
            {
                get => 
                    this.emitDefaultValue;
                set
                {
                    this.emitDefaultValue = value;
                }
            }

            internal bool HasConflictingNameAndType
            {
                get => 
                    this.hasConflictingNameAndType;
                set
                {
                    this.hasConflictingNameAndType = value;
                }
            }

            internal bool IsGetOnlyCollection
            {
                get => 
                    this.isGetOnlyCollection;
                set
                {
                    this.isGetOnlyCollection = value;
                }
            }

            internal bool IsNullable
            {
                get => 
                    this.isNullable;
                set
                {
                    this.isNullable = value;
                }
            }

            internal bool IsRequired
            {
                get => 
                    this.isRequired;
                set
                {
                    this.isRequired = value;
                }
            }

            internal System.Reflection.MemberInfo MemberInfo =>
                this.memberInfo;

            internal Type MemberType
            {
                get
                {
                    FieldInfo memberInfo = this.MemberInfo as FieldInfo;
                    if (memberInfo != null)
                    {
                        return memberInfo.FieldType;
                    }
                    return ((PropertyInfo) this.MemberInfo).PropertyType;
                }
            }

            internal DataContract MemberTypeContract
            {
                get
                {
                    if ((this.memberTypeContract == null) && (this.MemberInfo != null))
                    {
                        if (this.IsGetOnlyCollection)
                        {
                            this.memberTypeContract = DataContract.GetGetOnlyCollectionDataContract(DataContract.GetId(this.MemberType.TypeHandle), this.MemberType.TypeHandle, this.MemberType, SerializationMode.SharedContract);
                        }
                        else
                        {
                            this.memberTypeContract = DataContract.GetDataContract(this.MemberType);
                        }
                    }
                    return this.memberTypeContract;
                }
                set
                {
                    this.memberTypeContract = value;
                }
            }

            internal string Name
            {
                get => 
                    this.name;
                set
                {
                    this.name = value;
                }
            }

            internal int Order
            {
                get => 
                    this.order;
                set
                {
                    this.order = value;
                }
            }
        }
    }
}

