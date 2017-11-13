﻿namespace System.Runtime.Remoting.Metadata
{
    using System;
    using System.Reflection;
    using System.Runtime.Remoting;

    internal class RemotingTypeCachedData : RemotingCachedData
    {
        private string _assemblyName;
        private LastCalledMethodClass _lastMethodCalled;
        private string _qualifiedTypeName;
        private string _simpleAssemblyName;
        private System.Runtime.Remoting.TypeInfo _typeInfo;

        internal RemotingTypeCachedData(object ri) : base(ri)
        {
            this._lastMethodCalled = null;
        }

        internal MethodBase GetLastCalledMethod(string newMeth)
        {
            LastCalledMethodClass class2 = this._lastMethodCalled;
            if (class2 != null)
            {
                string methodName = class2.methodName;
                MethodBase mB = class2.MB;
                if ((mB == null) || (methodName == null))
                {
                    return null;
                }
                if (methodName.Equals(newMeth))
                {
                    return mB;
                }
            }
            return null;
        }

        internal void SetLastCalledMethod(string newMethName, MethodBase newMB)
        {
            LastCalledMethodClass class2 = new LastCalledMethodClass {
                methodName = newMethName,
                MB = newMB
            };
            this._lastMethodCalled = class2;
        }

        internal string AssemblyName
        {
            get
            {
                if (this._assemblyName == null)
                {
                    this._assemblyName = ((Type) base.RI).Module.Assembly.FullName;
                }
                return this._assemblyName;
            }
        }

        internal string QualifiedTypeName
        {
            get
            {
                if (this._qualifiedTypeName == null)
                {
                    this._qualifiedTypeName = RemotingServices.DetermineDefaultQualifiedTypeName((Type) base.RI);
                }
                return this._qualifiedTypeName;
            }
        }

        internal string SimpleAssemblyName
        {
            get
            {
                if (this._simpleAssemblyName == null)
                {
                    this._simpleAssemblyName = ((Type) base.RI).Module.Assembly.nGetSimpleName();
                }
                return this._simpleAssemblyName;
            }
        }

        internal System.Runtime.Remoting.TypeInfo TypeInfo
        {
            get
            {
                if (this._typeInfo == null)
                {
                    this._typeInfo = new System.Runtime.Remoting.TypeInfo((Type) base.RI);
                }
                return this._typeInfo;
            }
        }

        private class LastCalledMethodClass
        {
            public MethodBase MB;
            public string methodName;
        }
    }
}

