namespace System.Runtime.Remoting
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting.Contexts;
    using System.Threading;

    [ComVisible(true)]
    public class ActivatedServiceTypeEntry : TypeEntry
    {
        private IContextAttribute[] _contextAttributes;

        public ActivatedServiceTypeEntry(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            base.TypeName = type.FullName;
            base.AssemblyName = type.Module.Assembly.nGetSimpleName();
        }

        public ActivatedServiceTypeEntry(string typeName, string assemblyName)
        {
            if (typeName == null)
            {
                throw new ArgumentNullException("typeName");
            }
            if (assemblyName == null)
            {
                throw new ArgumentNullException("assemblyName");
            }
            base.TypeName = typeName;
            base.AssemblyName = assemblyName;
        }

        public override string ToString() => 
            ("type='" + base.TypeName + ", " + base.AssemblyName + "'");

        public IContextAttribute[] ContextAttributes
        {
            get => 
                this._contextAttributes;
            set
            {
                this._contextAttributes = value;
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
    }
}

