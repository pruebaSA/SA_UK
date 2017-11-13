namespace System.EnterpriseServices
{
    using System;
    using System.Collections;
    using System.EnterpriseServices.Admin;
    using System.Reflection;
    using System.Runtime.InteropServices;

    internal class MethodConfigCallback : IConfigCallback
    {
        private Hashtable _cache;
        private ICatalogCollection _coll;
        private RegistrationDriver _driver;
        private Type _impl;
        private InterfaceMapping _map;
        private Type _type;

        public MethodConfigCallback(ICatalogCollection coll, Type t, Type impl, Hashtable cache, RegistrationDriver driver)
        {
            this._type = t;
            this._impl = impl;
            this._coll = coll;
            this._cache = cache;
            this._driver = driver;
            try
            {
                this._map = this._impl.GetInterfaceMap(this._type);
            }
            catch (ArgumentException)
            {
                this._map.InterfaceMethods = null;
                this._map.InterfaceType = null;
                this._map.TargetMethods = null;
                this._map.TargetType = null;
            }
            RegistrationDriver.Populate(coll);
        }

        public bool AfterSaveChanges(object a, object key)
        {
            if (a == null)
            {
                return false;
            }
            return this._driver.AfterSaveChanges((MethodInfo) a, (ICatalogObject) key, this._coll, "Method", this._cache);
        }

        public bool Configure(object a, object key)
        {
            if (a == null)
            {
                return false;
            }
            return this._driver.ConfigureObject((MethodInfo) a, (ICatalogObject) key, this._coll, "Method", this._cache);
        }

        public void ConfigureDefaults(object a, object key)
        {
            if (!Platform.IsLessThan(Platform.W2K))
            {
                ((ICatalogObject) key).SetValue("AutoComplete", false);
            }
        }

        public void ConfigureSubCollections(ICatalogCollection coll)
        {
        }

        public object FindObject(ICatalogCollection coll, object key)
        {
            ICatalogObject obj2 = (ICatalogObject) key;
            int slot = (int) obj2.GetValue("Index");
            ComMemberType method = ComMemberType.Method;
            MemberInfo setMethod = Marshal.GetMethodInfoForComSlot(this._type, slot, ref method);
            if (setMethod is PropertyInfo)
            {
                switch (method)
                {
                    case ComMemberType.PropSet:
                        setMethod = ((PropertyInfo) setMethod).GetSetMethod();
                        break;

                    case ComMemberType.PropGet:
                        setMethod = ((PropertyInfo) setMethod).GetGetMethod();
                        break;
                }
            }
            if (this._map.InterfaceMethods != null)
            {
                for (int i = 0; i < this._map.InterfaceMethods.Length; i++)
                {
                    if (this._map.InterfaceMethods[i] == setMethod)
                    {
                        return this._map.TargetMethods[i];
                    }
                }
            }
            return setMethod;
        }

        public IEnumerator GetEnumerator()
        {
            IEnumerator pEnum = null;
            this._coll.GetEnumerator(out pEnum);
            return pEnum;
        }
    }
}

