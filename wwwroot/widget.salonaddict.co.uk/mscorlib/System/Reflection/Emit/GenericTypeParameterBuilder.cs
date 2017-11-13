﻿namespace System.Reflection.Emit
{
    using System;
    using System.Collections;
    using System.Globalization;
    using System.Reflection;
    using System.Runtime.InteropServices;

    [ComVisible(true)]
    public sealed class GenericTypeParameterBuilder : Type
    {
        internal TypeBuilder m_type;

        internal GenericTypeParameterBuilder(TypeBuilder type)
        {
            this.m_type = type;
        }

        public override bool Equals(object o)
        {
            GenericTypeParameterBuilder builder = o as GenericTypeParameterBuilder;
            if (builder == null)
            {
                return false;
            }
            return (builder.m_type == this.m_type);
        }

        protected override TypeAttributes GetAttributeFlagsImpl()
        {
            throw new NotSupportedException();
        }

        protected override ConstructorInfo GetConstructorImpl(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
        {
            throw new NotSupportedException();
        }

        [ComVisible(true)]
        public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr)
        {
            throw new NotSupportedException();
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            throw new NotSupportedException();
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            throw new NotSupportedException();
        }

        public override Type GetElementType()
        {
            throw new NotSupportedException();
        }

        public override EventInfo GetEvent(string name, BindingFlags bindingAttr)
        {
            throw new NotSupportedException();
        }

        public override EventInfo[] GetEvents()
        {
            throw new NotSupportedException();
        }

        public override EventInfo[] GetEvents(BindingFlags bindingAttr)
        {
            throw new NotSupportedException();
        }

        public override FieldInfo GetField(string name, BindingFlags bindingAttr)
        {
            throw new NotSupportedException();
        }

        public override FieldInfo[] GetFields(BindingFlags bindingAttr)
        {
            throw new NotSupportedException();
        }

        public override Type[] GetGenericArguments()
        {
            throw new InvalidOperationException();
        }

        public override Type GetGenericTypeDefinition()
        {
            throw new InvalidOperationException();
        }

        public override int GetHashCode() => 
            this.m_type.GetHashCode();

        public override Type GetInterface(string name, bool ignoreCase)
        {
            throw new NotSupportedException();
        }

        [ComVisible(true)]
        public override InterfaceMapping GetInterfaceMap(Type interfaceType)
        {
            throw new NotSupportedException();
        }

        public override Type[] GetInterfaces()
        {
            throw new NotSupportedException();
        }

        public override MemberInfo[] GetMember(string name, MemberTypes type, BindingFlags bindingAttr)
        {
            throw new NotSupportedException();
        }

        public override MemberInfo[] GetMembers(BindingFlags bindingAttr)
        {
            throw new NotSupportedException();
        }

        protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
        {
            throw new NotSupportedException();
        }

        public override MethodInfo[] GetMethods(BindingFlags bindingAttr)
        {
            throw new NotSupportedException();
        }

        public override Type GetNestedType(string name, BindingFlags bindingAttr)
        {
            throw new NotSupportedException();
        }

        public override Type[] GetNestedTypes(BindingFlags bindingAttr)
        {
            throw new NotSupportedException();
        }

        public override PropertyInfo[] GetProperties(BindingFlags bindingAttr)
        {
            throw new NotSupportedException();
        }

        protected override PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
        {
            throw new NotSupportedException();
        }

        protected override bool HasElementTypeImpl() => 
            false;

        public override object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters)
        {
            throw new NotSupportedException();
        }

        protected override bool IsArrayImpl() => 
            false;

        public override bool IsAssignableFrom(Type c)
        {
            throw new NotSupportedException();
        }

        protected override bool IsByRefImpl() => 
            false;

        protected override bool IsCOMObjectImpl() => 
            false;

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            throw new NotSupportedException();
        }

        protected override bool IsPointerImpl() => 
            false;

        protected override bool IsPrimitiveImpl() => 
            false;

        [ComVisible(true)]
        public override bool IsSubclassOf(Type c)
        {
            throw new NotSupportedException();
        }

        protected override bool IsValueTypeImpl() => 
            false;

        public override Type MakeArrayType() => 
            SymbolType.FormCompoundType("[]".ToCharArray(), this, 0);

        public override Type MakeArrayType(int rank)
        {
            if (rank <= 0)
            {
                throw new IndexOutOfRangeException();
            }
            string str = "";
            if (rank == 1)
            {
                str = "*";
            }
            else
            {
                for (int i = 1; i < rank; i++)
                {
                    str = str + ",";
                }
            }
            return (SymbolType.FormCompoundType(string.Format(CultureInfo.InvariantCulture, "[{0}]", new object[] { str }).ToCharArray(), this, 0) as SymbolType);
        }

        public override Type MakeByRefType() => 
            SymbolType.FormCompoundType("&".ToCharArray(), this, 0);

        public override Type MakeGenericType(params Type[] typeArguments)
        {
            throw new InvalidOperationException(Environment.GetResourceString("Arg_NotGenericTypeDefinition"));
        }

        public override Type MakePointerType() => 
            SymbolType.FormCompoundType("*".ToCharArray(), this, 0);

        public void SetBaseTypeConstraint(Type baseTypeConstraint)
        {
            this.m_type.CheckContext(new Type[] { baseTypeConstraint });
            this.m_type.SetParent(baseTypeConstraint);
        }

        public void SetCustomAttribute(CustomAttributeBuilder customBuilder)
        {
            if (this.m_type.m_ca == null)
            {
                this.m_type.m_ca = new ArrayList();
            }
            this.m_type.m_ca.Add(new TypeBuilder.CustAttr(customBuilder));
        }

        public void SetCustomAttribute(ConstructorInfo con, byte[] binaryAttribute)
        {
            if (this.m_type.m_ca == null)
            {
                this.m_type.m_ca = new ArrayList();
            }
            this.m_type.m_ca.Add(new TypeBuilder.CustAttr(con, binaryAttribute));
        }

        public void SetGenericParameterAttributes(GenericParameterAttributes genericParameterAttributes)
        {
            this.m_type.m_genParamAttributes = genericParameterAttributes;
        }

        [ComVisible(true)]
        public void SetInterfaceConstraints(params Type[] interfaceConstraints)
        {
            this.m_type.CheckContext(interfaceConstraints);
            this.m_type.SetInterfaces(interfaceConstraints);
        }

        public override string ToString() => 
            this.m_type.Name;

        public override System.Reflection.Assembly Assembly =>
            this.m_type.Assembly;

        public override string AssemblyQualifiedName =>
            null;

        public override Type BaseType =>
            this.m_type.BaseType;

        public override bool ContainsGenericParameters =>
            this.m_type.ContainsGenericParameters;

        public override MethodBase DeclaringMethod =>
            this.m_type.DeclaringMethod;

        public override Type DeclaringType =>
            this.m_type.DeclaringType;

        public override string FullName =>
            null;

        public override int GenericParameterPosition =>
            this.m_type.GenericParameterPosition;

        public override Guid GUID
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        public override bool IsGenericParameter =>
            true;

        public override bool IsGenericType =>
            false;

        public override bool IsGenericTypeDefinition =>
            false;

        internal override int MetadataTokenInternal =>
            this.m_type.MetadataTokenInternal;

        public override System.Reflection.Module Module =>
            this.m_type.Module;

        public override string Name =>
            this.m_type.Name;

        public override string Namespace =>
            null;

        public override Type ReflectedType =>
            this.m_type.ReflectedType;

        public override RuntimeTypeHandle TypeHandle
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        public override Type UnderlyingSystemType =>
            this;
    }
}

