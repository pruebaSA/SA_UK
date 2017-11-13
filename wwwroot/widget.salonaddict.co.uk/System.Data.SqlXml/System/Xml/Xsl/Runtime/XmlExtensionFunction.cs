namespace System.Xml.Xsl.Runtime
{
    using System;
    using System.Globalization;
    using System.Reflection;
    using System.Xml;
    using System.Xml.Xsl;

    internal class XmlExtensionFunction
    {
        private Type[] argClrTypes;
        private XmlQueryType[] argXmlTypes;
        private BindingFlags flags;
        private int hashCode;
        private MethodInfo meth;
        private string name;
        private string namespaceUri;
        private int numArgs;
        private Type objectType;
        private Type retClrType;
        private XmlQueryType retXmlType;

        public XmlExtensionFunction()
        {
        }

        public XmlExtensionFunction(string name, string namespaceUri, MethodInfo meth)
        {
            this.name = name;
            this.namespaceUri = namespaceUri;
            this.Bind(meth);
        }

        public XmlExtensionFunction(string name, string namespaceUri, int numArgs, Type objectType, BindingFlags flags)
        {
            this.Init(name, namespaceUri, numArgs, objectType, flags);
        }

        public void Bind()
        {
            MethodInfo[] methods = this.objectType.GetMethods(this.flags);
            MethodInfo meth = null;
            StringComparison comparisonType = ((this.flags & BindingFlags.IgnoreCase) != BindingFlags.Default) ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            foreach (MethodInfo info2 in methods)
            {
                if (info2.Name.Equals(this.name, comparisonType) && ((this.numArgs == -1) || (info2.GetParameters().Length == this.numArgs)))
                {
                    if (meth != null)
                    {
                        throw new XslTransformException("XmlIl_AmbiguousExtensionMethod", new string[] { this.namespaceUri, this.name, this.numArgs.ToString(CultureInfo.InvariantCulture) });
                    }
                    meth = info2;
                }
            }
            if (meth == null)
            {
                foreach (MethodInfo info3 in this.objectType.GetMethods(this.flags | BindingFlags.NonPublic))
                {
                    if (info3.Name.Equals(this.name, comparisonType) && (info3.GetParameters().Length == this.numArgs))
                    {
                        throw new XslTransformException("XmlIl_NonPublicExtensionMethod", new string[] { this.namespaceUri, this.name });
                    }
                }
                throw new XslTransformException("XmlIl_NoExtensionMethod", new string[] { this.namespaceUri, this.name, this.numArgs.ToString(CultureInfo.InvariantCulture) });
            }
            if (meth.IsGenericMethodDefinition)
            {
                throw new XslTransformException("XmlIl_GenericExtensionMethod", new string[] { this.namespaceUri, this.name });
            }
            this.Bind(meth);
        }

        private void Bind(MethodInfo meth)
        {
            int num;
            ParameterInfo[] parameters = meth.GetParameters();
            this.meth = meth;
            this.argClrTypes = new Type[parameters.Length];
            for (num = 0; num < parameters.Length; num++)
            {
                this.argClrTypes[num] = this.GetClrType(parameters[num].ParameterType);
            }
            this.retClrType = this.GetClrType(this.meth.ReturnType);
            this.argXmlTypes = new XmlQueryType[parameters.Length];
            for (num = 0; num < parameters.Length; num++)
            {
                this.argXmlTypes[num] = this.InferXmlType(this.argClrTypes[num]);
                if (this.namespaceUri.Length == 0)
                {
                    if (object.Equals(this.argXmlTypes[num], XmlQueryTypeFactory.NodeNotRtf))
                    {
                        this.argXmlTypes[num] = XmlQueryTypeFactory.Node;
                    }
                    else if (object.Equals(this.argXmlTypes[num], XmlQueryTypeFactory.NodeDodS))
                    {
                        this.argXmlTypes[num] = XmlQueryTypeFactory.NodeS;
                    }
                }
                else if (object.Equals(this.argXmlTypes[num], XmlQueryTypeFactory.NodeDodS))
                {
                    this.argXmlTypes[num] = XmlQueryTypeFactory.NodeNotRtfS;
                }
            }
            this.retXmlType = this.InferXmlType(this.retClrType);
        }

        public bool CanBind()
        {
            MethodInfo[] methods = this.objectType.GetMethods(this.flags);
            StringComparison comparisonType = ((this.flags & BindingFlags.IgnoreCase) != BindingFlags.Default) ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            foreach (MethodInfo info in methods)
            {
                if ((info.Name.Equals(this.name, comparisonType) && ((this.numArgs == -1) || (info.GetParameters().Length == this.numArgs))) && !info.IsGenericMethodDefinition)
                {
                    return true;
                }
            }
            return false;
        }

        public override bool Equals(object other)
        {
            XmlExtensionFunction function = other as XmlExtensionFunction;
            return (((((this.hashCode == function.hashCode) && (this.name == function.name)) && ((this.namespaceUri == function.namespaceUri) && (this.numArgs == function.numArgs))) && (this.objectType == function.objectType)) && (this.flags == function.flags));
        }

        public Type GetClrArgumentType(int index) => 
            this.argClrTypes[index];

        private Type GetClrType(Type clrType)
        {
            if (clrType.IsEnum)
            {
                return Enum.GetUnderlyingType(clrType);
            }
            if (clrType.IsByRef)
            {
                throw new XslTransformException("XmlIl_ByRefType", new string[] { this.namespaceUri, this.name });
            }
            return clrType;
        }

        public override int GetHashCode() => 
            this.hashCode;

        public XmlQueryType GetXmlArgumentType(int index) => 
            this.argXmlTypes[index];

        private XmlQueryType InferXmlType(Type clrType) => 
            XsltConvert.InferXsltType(clrType);

        public void Init(string name, string namespaceUri, int numArgs, Type objectType, BindingFlags flags)
        {
            this.name = name;
            this.namespaceUri = namespaceUri;
            this.numArgs = numArgs;
            this.objectType = objectType;
            this.flags = flags;
            this.meth = null;
            this.argClrTypes = null;
            this.retClrType = null;
            this.argXmlTypes = null;
            this.retXmlType = null;
            this.hashCode = ((namespaceUri.GetHashCode() ^ name.GetHashCode()) ^ (((int) flags) << 0x10)) ^ numArgs;
        }

        public object Invoke(object extObj, object[] args)
        {
            object obj2;
            try
            {
                obj2 = this.meth.Invoke(extObj, this.flags, null, args, CultureInfo.InvariantCulture);
            }
            catch (TargetInvocationException exception)
            {
                throw new XslTransformException(exception.InnerException, "XmlIl_ExtensionError", new string[] { this.name });
            }
            catch (Exception exception2)
            {
                if (!XmlException.IsCatchableException(exception2))
                {
                    throw;
                }
                throw new XslTransformException(exception2, "XmlIl_ExtensionError", new string[] { this.name });
            }
            return obj2;
        }

        public Type ClrReturnType =>
            this.retClrType;

        public MethodInfo Method =>
            this.meth;

        public XmlQueryType XmlReturnType =>
            this.retXmlType;
    }
}

