﻿namespace System.Runtime.Remoting
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting.Contexts;
    using System.Threading;

    [ComVisible(true)]
    public class WellKnownServiceTypeEntry : TypeEntry
    {
        private IContextAttribute[] _contextAttributes;
        private WellKnownObjectMode _mode;
        private string _objectUri;

        public WellKnownServiceTypeEntry(Type type, string objectUri, WellKnownObjectMode mode)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (objectUri == null)
            {
                throw new ArgumentNullException("objectUri");
            }
            base.TypeName = type.FullName;
            base.AssemblyName = type.Module.Assembly.FullName;
            this._objectUri = objectUri;
            this._mode = mode;
        }

        public WellKnownServiceTypeEntry(string typeName, string assemblyName, string objectUri, WellKnownObjectMode mode)
        {
            if (typeName == null)
            {
                throw new ArgumentNullException("typeName");
            }
            if (assemblyName == null)
            {
                throw new ArgumentNullException("assemblyName");
            }
            if (objectUri == null)
            {
                throw new ArgumentNullException("objectUri");
            }
            base.TypeName = typeName;
            base.AssemblyName = assemblyName;
            this._objectUri = objectUri;
            this._mode = mode;
        }

        public override string ToString() => 
            ("type='" + base.TypeName + ", " + base.AssemblyName + "'; objectUri=" + this._objectUri + "; mode=" + this._mode.ToString());

        public IContextAttribute[] ContextAttributes
        {
            get => 
                this._contextAttributes;
            set
            {
                this._contextAttributes = value;
            }
        }

        public WellKnownObjectMode Mode =>
            this._mode;

        public Type ObjectType
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            get
            {
                StackCrawlMark lookForMyCaller = StackCrawlMark.LookForMyCaller;
                return RuntimeType.PrivateGetType(base.TypeName + ", " + base.AssemblyName, false, false, ref lookForMyCaller);
            }
        }

        public string ObjectUri =>
            this._objectUri;
    }
}

