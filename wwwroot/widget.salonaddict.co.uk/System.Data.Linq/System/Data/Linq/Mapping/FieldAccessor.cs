namespace System.Data.Linq.Mapping
{
    using System;
    using System.Reflection;
    using System.Reflection.Emit;

    internal static class FieldAccessor
    {
        internal static MetaAccessor Create(Type objectType, FieldInfo fi)
        {
            if (!fi.ReflectedType.IsAssignableFrom(objectType))
            {
                throw Error.InvalidFieldInfo(objectType, fi.FieldType, fi);
            }
            Delegate delegate2 = null;
            Delegate delegate3 = null;
            if (!objectType.IsGenericType)
            {
                DynamicMethod method = new DynamicMethod("xget_" + fi.Name, fi.FieldType, new Type[] { objectType }, true);
                ILGenerator iLGenerator = method.GetILGenerator();
                iLGenerator.Emit(OpCodes.Ldarg_0);
                iLGenerator.Emit(OpCodes.Ldfld, fi);
                iLGenerator.Emit(OpCodes.Ret);
                delegate2 = method.CreateDelegate(typeof(DGet<,>).MakeGenericType(new Type[] { objectType, fi.FieldType }));
                DynamicMethod method2 = new DynamicMethod("xset_" + fi.Name, typeof(void), new Type[] { objectType.MakeByRefType(), fi.FieldType }, true);
                iLGenerator = method2.GetILGenerator();
                iLGenerator.Emit(OpCodes.Ldarg_0);
                if (!objectType.IsValueType)
                {
                    iLGenerator.Emit(OpCodes.Ldind_Ref);
                }
                iLGenerator.Emit(OpCodes.Ldarg_1);
                iLGenerator.Emit(OpCodes.Stfld, fi);
                iLGenerator.Emit(OpCodes.Ret);
                delegate3 = method2.CreateDelegate(typeof(DRSet<,>).MakeGenericType(new Type[] { objectType, fi.FieldType }));
            }
            return (MetaAccessor) Activator.CreateInstance(typeof(Accessor).MakeGenericType(new Type[] { objectType, fi.FieldType }), BindingFlags.NonPublic | BindingFlags.Instance, null, new object[] { fi, delegate2, delegate3 }, null);
        }

        private class Accessor<T, V> : MetaAccessor<T, V>
        {
            private DGet<T, V> dget;
            private DRSet<T, V> drset;
            private FieldInfo fi;

            internal Accessor(FieldInfo fi, DGet<T, V> dget, DRSet<T, V> drset)
            {
                this.fi = fi;
                this.dget = dget;
                this.drset = drset;
            }

            public override V GetValue(T instance)
            {
                if (this.dget != null)
                {
                    return this.dget(instance);
                }
                return (V) this.fi.GetValue(instance);
            }

            public override void SetValue(ref T instance, V value)
            {
                if (this.drset != null)
                {
                    this.drset(ref instance, value);
                }
                else
                {
                    this.fi.SetValue((T) instance, value);
                }
            }
        }
    }
}

