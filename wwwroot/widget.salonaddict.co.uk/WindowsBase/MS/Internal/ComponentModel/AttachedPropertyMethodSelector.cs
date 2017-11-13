namespace MS.Internal.ComponentModel
{
    using System;
    using System.Globalization;
    using System.Reflection;
    using System.Runtime.InteropServices;

    internal class AttachedPropertyMethodSelector : Binder
    {
        public override FieldInfo BindToField(BindingFlags bindingAttr, FieldInfo[] match, object value, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override MethodBase BindToMethod(BindingFlags bindingAttr, MethodBase[] match, ref object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] names, out object state)
        {
            throw new NotImplementedException();
        }

        public override object ChangeType(object value, Type type, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static bool ParametersMatch(ParameterInfo[] parameters, Type[] types)
        {
            if (parameters.Length != types.Length)
            {
                return false;
            }
            bool flag = true;
            for (int i = 0; i < parameters.Length; i++)
            {
                ParameterInfo info = parameters[i];
                Type type = types[i];
                if (info.ParameterType != type)
                {
                    flag = false;
                    break;
                }
            }
            if (flag)
            {
                return true;
            }
            flag = true;
            for (int j = 0; j < parameters.Length; j++)
            {
                ParameterInfo info2 = parameters[j];
                Type type2 = types[j];
                if (!type2.IsAssignableFrom(info2.ParameterType))
                {
                    return false;
                }
            }
            return flag;
        }

        public override void ReorderArgumentArray(ref object[] args, object state)
        {
            throw new NotImplementedException();
        }

        public override MethodBase SelectMethod(BindingFlags bindingAttr, MethodBase[] match, Type[] types, ParameterModifier[] modifiers)
        {
            if (types == null)
            {
                if (match.Length > 1)
                {
                    throw new AmbiguousMatchException();
                }
                return match[0];
            }
            for (int i = 0; i < match.Length; i++)
            {
                MethodBase base2 = match[i];
                if (ParametersMatch(base2.GetParameters(), types))
                {
                    return base2;
                }
            }
            return null;
        }

        public override PropertyInfo SelectProperty(BindingFlags bindingAttr, PropertyInfo[] match, Type returnType, Type[] indexes, ParameterModifier[] modifiers)
        {
            throw new NotImplementedException();
        }
    }
}

