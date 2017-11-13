namespace Microsoft.Practices.Unity.Utility
{
    using System;
    using System.Reflection;

    public class ReflectionHelper
    {
        private readonly System.Type t;

        public ReflectionHelper(System.Type typeToReflect)
        {
            this.t = typeToReflect;
        }

        public System.Type GetClosedParameterType(System.Type[] genericArguments)
        {
            Guard.ArgumentNotNull(genericArguments, "genericArguments");
            if (this.IsOpenGeneric)
            {
                System.Type[] typeArguments = this.Type.GetGenericArguments();
                for (int i = 0; i < typeArguments.Length; i++)
                {
                    typeArguments[i] = genericArguments[typeArguments[i].GenericParameterPosition];
                }
                return this.Type.GetGenericTypeDefinition().MakeGenericType(typeArguments);
            }
            if (this.Type.IsGenericParameter)
            {
                return genericArguments[this.Type.GenericParameterPosition];
            }
            if (this.IsArray && this.ArrayElementType.IsGenericParameter)
            {
                int arrayRank = this.Type.GetArrayRank();
                if (arrayRank == 1)
                {
                    return genericArguments[this.Type.GetElementType().GenericParameterPosition].MakeArrayType();
                }
                return genericArguments[this.Type.GetElementType().GenericParameterPosition].MakeArrayType(arrayRank);
            }
            return this.Type;
        }

        public System.Type GetNamedGenericParameter(string parameterName)
        {
            System.Type genericTypeDefinition = this.Type.GetGenericTypeDefinition();
            System.Type type2 = null;
            int index = -1;
            foreach (System.Type type3 in genericTypeDefinition.GetGenericArguments())
            {
                if (type3.Name == parameterName)
                {
                    index = type3.GenericParameterPosition;
                    break;
                }
            }
            if (index != -1)
            {
                type2 = this.Type.GetGenericArguments()[index];
            }
            return type2;
        }

        public static bool MethodHasOpenGenericParameters(MethodBase method)
        {
            Guard.ArgumentNotNull(method, "method");
            foreach (ParameterInfo info in method.GetParameters())
            {
                ReflectionHelper helper = new ReflectionHelper(info.ParameterType);
                if (helper.IsOpenGeneric)
                {
                    return true;
                }
            }
            return false;
        }

        public System.Type ArrayElementType =>
            this.t.GetElementType();

        public bool IsArray =>
            this.t.IsArray;

        public bool IsGenericArray =>
            (this.IsArray && this.ArrayElementType.IsGenericParameter);

        public bool IsGenericType =>
            this.t.IsGenericType;

        public bool IsOpenGeneric =>
            (this.t.IsGenericType && this.t.ContainsGenericParameters);

        public System.Type Type =>
            this.t;
    }
}

