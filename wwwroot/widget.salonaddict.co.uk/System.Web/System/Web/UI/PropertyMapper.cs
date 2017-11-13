﻿namespace System.Web.UI
{
    using System;
    using System.ComponentModel;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Web;
    using System.Web.Util;

    internal sealed class PropertyMapper
    {
        private const char OM_CHAR = '.';
        private const char PERSIST_CHAR = '-';
        private const string STR_OM_CHAR = ".";

        private PropertyMapper()
        {
        }

        internal static PropertyDescriptor GetMappedPropertyDescriptor(object obj, string mappedName, out object childObject, out string propertyName, bool inDesigner)
        {
            childObject = LocatePropertyObject(obj, mappedName, out propertyName, inDesigner);
            if (childObject == null)
            {
                return null;
            }
            return TypeDescriptor.GetProperties(childObject, inDesigner)[propertyName];
        }

        internal static MemberInfo GetMemberInfo(Type ctrlType, string name, out string nameForCodeGen)
        {
            Type propertyType = ctrlType;
            PropertyInfo property = null;
            FieldInfo field = null;
            string str = MapNameToPropertyName(name);
            nameForCodeGen = null;
            int startIndex = 0;
            while (startIndex < str.Length)
            {
                string str2;
                int index = str.IndexOf('.', startIndex);
                if (index < 0)
                {
                    str2 = str.Substring(startIndex);
                    startIndex = str.Length;
                }
                else
                {
                    str2 = str.Substring(startIndex, index - startIndex);
                    startIndex = index + 1;
                }
                BindingFlags bindingAttr = BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.IgnoreCase;
                try
                {
                    property = propertyType.GetProperty(str2, bindingAttr);
                }
                catch (AmbiguousMatchException)
                {
                    bindingAttr |= BindingFlags.DeclaredOnly;
                    property = propertyType.GetProperty(str2, bindingAttr);
                }
                if (property == null)
                {
                    field = propertyType.GetField(str2, bindingAttr);
                    if (field == null)
                    {
                        nameForCodeGen = null;
                        break;
                    }
                }
                str2 = null;
                if (property != null)
                {
                    propertyType = property.PropertyType;
                    str2 = property.Name;
                }
                else
                {
                    propertyType = field.FieldType;
                    str2 = field.Name;
                }
                if (!IsTypeCLSCompliant(propertyType))
                {
                    throw new HttpException(System.Web.SR.GetString("Property_Not_ClsCompliant", new object[] { name, ctrlType.FullName, propertyType.FullName }));
                }
                if (str2 != null)
                {
                    if (nameForCodeGen == null)
                    {
                        nameForCodeGen = str2;
                    }
                    else
                    {
                        nameForCodeGen = nameForCodeGen + "." + str2;
                    }
                }
            }
            if (property != null)
            {
                return property;
            }
            return field;
        }

        private static bool IsTypeCLSCompliant(Type type) => 
            ((((type != typeof(sbyte)) && (type != typeof(TypedReference))) && ((type != typeof(ushort)) && (type != typeof(uint)))) && ((type != typeof(ulong)) && (type != typeof(UIntPtr))));

        internal static object LocatePropertyObject(object obj, string mappedName, out string propertyName, bool inDesigner)
        {
            object target = obj;
            obj.GetType();
            propertyName = null;
            int startIndex = 0;
            while (startIndex < mappedName.Length)
            {
                int index = mappedName.IndexOf('.', startIndex);
                if (index < 0)
                {
                    break;
                }
                propertyName = mappedName.Substring(startIndex, index - startIndex);
                startIndex = index + 1;
                target = FastPropertyAccessor.GetProperty(target, propertyName, inDesigner);
                if (target == null)
                {
                    return null;
                }
            }
            if (startIndex > 0)
            {
                propertyName = mappedName.Substring(startIndex);
                return target;
            }
            propertyName = mappedName;
            return target;
        }

        internal static string MapNameToPropertyName(string attrName) => 
            attrName.Replace('-', '.');

        internal static void SetMappedPropertyValue(object obj, string mappedName, object value, bool inDesigner)
        {
            string str;
            object target = LocatePropertyObject(obj, mappedName, out str, inDesigner);
            if (target != null)
            {
                FastPropertyAccessor.SetProperty(target, str, value, inDesigner);
            }
        }
    }
}

