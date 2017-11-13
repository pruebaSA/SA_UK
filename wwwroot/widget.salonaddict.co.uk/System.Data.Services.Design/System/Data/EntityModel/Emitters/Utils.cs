namespace System.Data.EntityModel.Emitters
{
    using System;
    using System.Collections.Generic;
    using System.Data.Metadata.Edm;
    using System.Globalization;
    using System.Reflection;
    using System.Runtime.InteropServices;

    internal static class Utils
    {
        private static string _complexPropertyInitializedSuffix = "Initialized";
        private static string[] _keywords = new string[] { "class", "event" };
        private static string[] _privateMemberPrefixes = new string[] { "_", "_Initialize_", "PropertyInfo", "_pi" };
        private static List<KeyValuePair<string, Type>> _typeReservedNames = InitializeTypeReservedNames();
        public const string AdoEntityClientNamespace = "System.Data.EntityClient";
        public const string AdoFrameworkDataClassesNamespace = "System.Data.Objects.DataClasses";
        public const string AdoFrameworkMetadataEdmNamespace = "System.Data.Metadata.Edm";
        public const string AdoFrameworkNamespace = "System.Data.Objects";
        public const string GetValidValueMethodName = "GetValidValue";
        public const string ReportPropertyChangedMethodName = "ReportPropertyChanged";
        public const string ReportPropertyChangingMethodName = "ReportPropertyChanging";
        public const string SetValidValueMethodName = "SetValidValue";
        public const string VerifyComplexObjectIsNotNullName = "VerifyComplexObjectIsNotNull";
        public const string WebFrameworkCommonNamespace = "System.Data.Services.Common";
        public const string WebFrameworkNamespace = "System.Data.Services.Client";

        public static string CamelCase(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }
            if (text.Length == 1)
            {
                char ch = text[0];
                return ch.ToString(CultureInfo.InvariantCulture).ToLowerInvariant();
            }
            char ch2 = text[0];
            return (ch2.ToString(CultureInfo.InvariantCulture).ToLowerInvariant() + text.Substring(1));
        }

        public static string ComplexPropertyInitializedNameFromPropName(string propName) => 
            (FieldNameFromPropName(propName) + _complexPropertyInitializedSuffix);

        public static bool DoesTypeReserveMemberName(StructuralType type, string name, StringComparison comparison)
        {
            Type applyToSpecificType = null;
            if (!TryGetReservedName(name, comparison, out applyToSpecificType))
            {
                return false;
            }
            return ((applyToSpecificType == null) || (applyToSpecificType == type.GetType()));
        }

        public static string FieldNameFromPropName(string propName) => 
            (PrivateMemberPrefix(PrivateMemberPrefixId.Field) + propName);

        private static string FixKeyword(string name, string prefix)
        {
            foreach (string str in _keywords)
            {
                if (name == str)
                {
                    return (prefix + PascalCase(name));
                }
            }
            return name;
        }

        public static string FixParameterName(string name, string prefix) => 
            CamelCase(FixKeyword(name, prefix));

        public static string GetFullyQualifiedCodeGenerationAttributeName(string attribute) => 
            ("http://schemas.microsoft.com/ado/2006/04/codegeneration:" + attribute);

        private static List<KeyValuePair<string, Type>> InitializeTypeReservedNames()
        {
            Dictionary<string, Type> dictionary = new Dictionary<string, Type>(StringComparer.Ordinal);
            foreach (MemberInfo info in typeof(object).GetMembers(BindingFlags.FlattenHierarchy | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance))
            {
                if (ShouldReserveName(info) && !dictionary.ContainsKey(info.Name))
                {
                    dictionary.Add(info.Name, null);
                }
            }
            List<KeyValuePair<string, Type>> list = new List<KeyValuePair<string, Type>>();
            foreach (KeyValuePair<string, Type> pair in dictionary)
            {
                list.Add(pair);
            }
            return list;
        }

        public static string PascalCase(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }
            if (text.Length == 1)
            {
                char ch = text[0];
                return ch.ToString(CultureInfo.InvariantCulture).ToUpperInvariant();
            }
            char ch2 = text[0];
            return (ch2.ToString(CultureInfo.InvariantCulture).ToUpperInvariant() + text.Substring(1));
        }

        public static string PrivateMemberPrefix(PrivateMemberPrefixId id) => 
            _privateMemberPrefixes[(int) id];

        public static string SetSpecialCaseForFxCopOnPropertyName(string propertyName)
        {
            if (StringComparer.OrdinalIgnoreCase.Equals(propertyName, "id"))
            {
                return "ID";
            }
            return propertyName;
        }

        private static bool ShouldReserveName(EventInfo member)
        {
            bool flag = false;
            MethodInfo addMethod = member.GetAddMethod();
            if (addMethod != null)
            {
                flag |= ShouldReserveName(addMethod, false);
            }
            MethodInfo removeMethod = member.GetRemoveMethod();
            if (removeMethod != null)
            {
                flag |= ShouldReserveName(removeMethod, false);
            }
            return flag;
        }

        private static bool ShouldReserveName(FieldInfo member) => 
            ((!member.IsPrivate && !member.IsAssembly) && !member.IsSpecialName);

        private static bool ShouldReserveName(MemberInfo member)
        {
            if (member is EventInfo)
            {
                return ShouldReserveName((EventInfo) member);
            }
            if (member is FieldInfo)
            {
                return ShouldReserveName((FieldInfo) member);
            }
            if (member is MethodBase)
            {
                return ShouldReserveName((MethodBase) member);
            }
            if (member is PropertyInfo)
            {
                return ShouldReserveName((PropertyInfo) member);
            }
            return ShouldReserveName((Type) member);
        }

        private static bool ShouldReserveName(MethodBase member) => 
            ShouldReserveName(member, true);

        private static bool ShouldReserveName(PropertyInfo member)
        {
            bool flag = false;
            MethodInfo setMethod = member.GetSetMethod();
            if (setMethod != null)
            {
                flag |= ShouldReserveName(setMethod, false);
            }
            MethodInfo getMethod = member.GetGetMethod();
            if (getMethod != null)
            {
                flag |= ShouldReserveName(getMethod, false);
            }
            return flag;
        }

        private static bool ShouldReserveName(Type member) => 
            false;

        private static bool ShouldReserveName(MethodBase member, bool checkForSpecial)
        {
            if (member.IsPrivate || member.IsAssembly)
            {
                return false;
            }
            if (checkForSpecial)
            {
                return !member.IsSpecialName;
            }
            return true;
        }

        public static string[] SplitName(string name)
        {
            if ((name.Length > 0) && (name[0] == '.'))
            {
                return name.Substring(1).Split(new char[] { '.' });
            }
            return name.Split(new char[] { '.' });
        }

        public static bool TryGetPrimitiveTypeKind(EdmType type, out PrimitiveTypeKind modelType)
        {
            if (!System.Data.Metadata.Edm.Helper.IsPrimitiveType(type))
            {
                modelType = PrimitiveTypeKind.Binary;
                return false;
            }
            modelType = ((PrimitiveType) type).PrimitiveTypeKind;
            return true;
        }

        public static bool TryGetReservedName(string name, StringComparison comparison, out Type applyToSpecificType)
        {
            applyToSpecificType = null;
            foreach (KeyValuePair<string, Type> pair in _typeReservedNames)
            {
                if (pair.Key.Equals(name, comparison))
                {
                    applyToSpecificType = pair.Value;
                    return true;
                }
            }
            return false;
        }
    }
}

