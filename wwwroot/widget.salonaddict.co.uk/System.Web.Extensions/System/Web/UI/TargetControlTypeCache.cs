namespace System.Web.UI
{
    using System;
    using System.Collections;

    internal static class TargetControlTypeCache
    {
        private static readonly Hashtable _targetControlTypeCache = Hashtable.Synchronized(new Hashtable());

        public static Type[] GetTargetControlTypes(Type extenderControlType)
        {
            Type[] targetControlTypesInternal = (Type[]) _targetControlTypeCache[extenderControlType];
            if (targetControlTypesInternal == null)
            {
                targetControlTypesInternal = GetTargetControlTypesInternal(extenderControlType);
                _targetControlTypeCache[extenderControlType] = targetControlTypesInternal;
            }
            return targetControlTypesInternal;
        }

        private static Type[] GetTargetControlTypesInternal(Type extenderControlType)
        {
            object[] customAttributes = extenderControlType.GetCustomAttributes(typeof(TargetControlTypeAttribute), true);
            Type[] typeArray = new Type[customAttributes.Length];
            for (int i = 0; i < customAttributes.Length; i++)
            {
                typeArray[i] = ((TargetControlTypeAttribute) customAttributes[i]).TargetControlType;
            }
            return typeArray;
        }
    }
}

