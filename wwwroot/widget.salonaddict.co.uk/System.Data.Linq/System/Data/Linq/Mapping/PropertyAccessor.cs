namespace System.Data.Linq.Mapping
{
    using System;
    using System.Reflection;
    using System.Reflection.Emit;

    internal static class PropertyAccessor
    {
        internal static MetaAccessor Create(Type objectType, PropertyInfo pi, MetaAccessor storageAccessor)
        {
            Delegate delegate2 = null;
            Delegate delegate3 = null;
            Type type = typeof(DGet<,>).MakeGenericType(new Type[] { objectType, pi.PropertyType });
            MethodInfo getMethod = pi.GetGetMethod(true);
            Delegate delegate4 = Delegate.CreateDelegate(type, getMethod, true);
            if (delegate4 == null)
            {
                throw Error.CouldNotCreateAccessorToProperty(objectType, pi.PropertyType, pi);
            }
            if (pi.CanWrite)
            {
                if (!objectType.IsValueType)
                {
                    delegate2 = Delegate.CreateDelegate(typeof(DSet<,>).MakeGenericType(new Type[] { objectType, pi.PropertyType }), pi.GetSetMethod(true), true);
                }
                else
                {
                    DynamicMethod method = new DynamicMethod("xset_" + pi.Name, typeof(void), new Type[] { objectType.MakeByRefType(), pi.PropertyType }, true);
                    ILGenerator iLGenerator = method.GetILGenerator();
                    iLGenerator.Emit(OpCodes.Ldarg_0);
                    if (!objectType.IsValueType)
                    {
                        iLGenerator.Emit(OpCodes.Ldind_Ref);
                    }
                    iLGenerator.Emit(OpCodes.Ldarg_1);
                    iLGenerator.Emit(OpCodes.Call, pi.GetSetMethod(true));
                    iLGenerator.Emit(OpCodes.Ret);
                    delegate3 = method.CreateDelegate(typeof(DRSet<,>).MakeGenericType(new Type[] { objectType, pi.PropertyType }));
                }
            }
            Type type2 = (storageAccessor != null) ? storageAccessor.Type : pi.PropertyType;
            return (MetaAccessor) Activator.CreateInstance(typeof(Accessor).MakeGenericType(new Type[] { objectType, pi.PropertyType, type2 }), BindingFlags.NonPublic | BindingFlags.Instance, null, new object[] { pi, delegate4, delegate2, delegate3, storageAccessor }, null);
        }

        private class Accessor<T, V, V2> : MetaAccessor<T, V> where V2: V
        {
            private DGet<T, V> dget;
            private DRSet<T, V> drset;
            private DSet<T, V> dset;
            private PropertyInfo pi;
            private MetaAccessor<T, V2> storage;

            internal Accessor(PropertyInfo pi, DGet<T, V> dget, DSet<T, V> dset, DRSet<T, V> drset, MetaAccessor<T, V2> storage)
            {
                this.pi = pi;
                this.dget = dget;
                this.dset = dset;
                this.drset = drset;
                this.storage = storage;
            }

            public override V GetValue(T instance) => 
                this.dget(instance);

            public override void SetValue(ref T instance, V value)
            {
                if (this.dset != null)
                {
                    this.dset(instance, value);
                }
                else if (this.drset != null)
                {
                    this.drset(ref instance, value);
                }
                else
                {
                    if (this.storage == null)
                    {
                        throw Error.UnableToAssignValueToReadonlyProperty(this.pi);
                    }
                    this.storage.SetValue(ref instance, (V2) value);
                }
            }
        }
    }
}

