﻿namespace System.Runtime.Remoting
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;

    [ComVisible(true)]
    public class WellKnownClientTypeEntry : TypeEntry
    {
        private string _appUrl;
        private string _objectUrl;

        public WellKnownClientTypeEntry(Type type, string objectUrl)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (objectUrl == null)
            {
                throw new ArgumentNullException("objectUrl");
            }
            base.TypeName = type.FullName;
            base.AssemblyName = type.Module.Assembly.nGetSimpleName();
            this._objectUrl = objectUrl;
        }

        public WellKnownClientTypeEntry(string typeName, string assemblyName, string objectUrl)
        {
            if (typeName == null)
            {
                throw new ArgumentNullException("typeName");
            }
            if (assemblyName == null)
            {
                throw new ArgumentNullException("assemblyName");
            }
            if (objectUrl == null)
            {
                throw new ArgumentNullException("objectUrl");
            }
            base.TypeName = typeName;
            base.AssemblyName = assemblyName;
            this._objectUrl = objectUrl;
        }

        public override string ToString()
        {
            string str = "type='" + base.TypeName + ", " + base.AssemblyName + "'; url=" + this._objectUrl;
            if (this._appUrl != null)
            {
                str = str + "; appUrl=" + this._appUrl;
            }
            return str;
        }

        public string ApplicationUrl
        {
            get => 
                this._appUrl;
            set
            {
                this._appUrl = value;
            }
        }

        public Type ObjectType
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            get
            {
                StackCrawlMark lookForMyCaller = StackCrawlMark.LookForMyCaller;
                return RuntimeType.PrivateGetType(base.TypeName + ", " + base.AssemblyName, false, false, ref lookForMyCaller);
            }
        }

        public string ObjectUrl =>
            this._objectUrl;
    }
}

