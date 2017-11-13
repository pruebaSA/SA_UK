namespace System.Runtime.Remoting
{
    using System;
    using System.Runtime.InteropServices;

    [Serializable]
    internal class TypeInfo : IRemotingTypeInfo
    {
        private string[] interfacesImplemented;
        private string[] serverHierarchy;
        private string serverType;

        internal TypeInfo(Type typeOfObj)
        {
            this.ServerType = GetQualifiedTypeName(typeOfObj);
            Type baseType = typeOfObj.BaseType;
            int num = 0;
            while ((baseType != typeof(MarshalByRefObject)) && (baseType != null))
            {
                baseType = baseType.BaseType;
                num++;
            }
            string[] strArray = null;
            if (num > 0)
            {
                strArray = new string[num];
                baseType = typeOfObj.BaseType;
                for (int i = 0; i < num; i++)
                {
                    strArray[i] = GetQualifiedTypeName(baseType);
                    baseType = baseType.BaseType;
                }
            }
            this.ServerHierarchy = strArray;
            Type[] interfaces = typeOfObj.GetInterfaces();
            string[] strArray2 = null;
            bool isInterface = typeOfObj.IsInterface;
            if ((interfaces.Length > 0) || isInterface)
            {
                strArray2 = new string[interfaces.Length + (isInterface ? 1 : 0)];
                for (int j = 0; j < interfaces.Length; j++)
                {
                    strArray2[j] = GetQualifiedTypeName(interfaces[j]);
                }
                if (isInterface)
                {
                    strArray2[strArray2.Length - 1] = GetQualifiedTypeName(typeOfObj);
                }
            }
            this.InterfacesImplemented = strArray2;
        }

        public virtual bool CanCastTo(Type castType, object o)
        {
            if (castType != null)
            {
                if ((castType == typeof(MarshalByRefObject)) || (castType == typeof(object)))
                {
                    return true;
                }
                if (castType.IsInterface)
                {
                    return ((this.interfacesImplemented != null) && this.CanCastTo(castType, this.InterfacesImplemented));
                }
                if (castType.IsMarshalByRef)
                {
                    if (this.CompareTypes(castType, this.serverType))
                    {
                        return true;
                    }
                    if ((this.serverHierarchy != null) && this.CanCastTo(castType, this.ServerHierarchy))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool CanCastTo(Type castType, string[] types)
        {
            if (castType != null)
            {
                for (int i = 0; i < types.Length; i++)
                {
                    if (this.CompareTypes(castType, types[i]))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool CompareTypes(Type type1, string type2)
        {
            Type typeFromQualifiedTypeName = RemotingServices.InternalGetTypeFromQualifiedTypeName(type2);
            return (type1 == typeFromQualifiedTypeName);
        }

        internal static string GetQualifiedTypeName(Type type)
        {
            if (type == null)
            {
                return null;
            }
            return RemotingServices.GetDefaultQualifiedTypeName(type);
        }

        internal static bool ParseTypeAndAssembly(string typeAndAssembly, out string typeName, out string assemName)
        {
            if (typeAndAssembly == null)
            {
                typeName = null;
                assemName = null;
                return false;
            }
            int index = typeAndAssembly.IndexOf(',');
            if (index == -1)
            {
                typeName = typeAndAssembly;
                assemName = null;
                return true;
            }
            typeName = typeAndAssembly.Substring(0, index);
            assemName = typeAndAssembly.Substring(index + 1).Trim();
            return true;
        }

        private string[] InterfacesImplemented
        {
            get => 
                this.interfacesImplemented;
            set
            {
                this.interfacesImplemented = value;
            }
        }

        private string[] ServerHierarchy
        {
            get => 
                this.serverHierarchy;
            set
            {
                this.serverHierarchy = value;
            }
        }

        internal string ServerType
        {
            get => 
                this.serverType;
            set
            {
                this.serverType = value;
            }
        }

        public virtual string TypeName
        {
            get => 
                this.serverType;
            set
            {
                this.serverType = value;
            }
        }
    }
}

