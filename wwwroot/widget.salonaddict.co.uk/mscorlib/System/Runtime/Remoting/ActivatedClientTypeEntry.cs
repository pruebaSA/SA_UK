namespace System.Runtime.Remoting
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting.Contexts;
    using System.Threading;

    [ComVisible(true)]
    public class ActivatedClientTypeEntry : TypeEntry
    {
        private string _appUrl;
        private IContextAttribute[] _contextAttributes;

        public ActivatedClientTypeEntry(Type type, string appUrl)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (appUrl == null)
            {
                throw new ArgumentNullException("appUrl");
            }
            base.TypeName = type.FullName;
            base.AssemblyName = type.Module.Assembly.nGetSimpleName();
            this._appUrl = appUrl;
        }

        public ActivatedClientTypeEntry(string typeName, string assemblyName, string appUrl)
        {
            if (typeName == null)
            {
                throw new ArgumentNullException("typeName");
            }
            if (assemblyName == null)
            {
                throw new ArgumentNullException("assemblyName");
            }
            if (appUrl == null)
            {
                throw new ArgumentNullException("appUrl");
            }
            base.TypeName = typeName;
            base.AssemblyName = assemblyName;
            this._appUrl = appUrl;
        }

        public override string ToString() => 
            ("type='" + base.TypeName + ", " + base.AssemblyName + "'; appUrl=" + this._appUrl);

        public string ApplicationUrl =>
            this._appUrl;

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

