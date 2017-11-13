namespace System.Web.Services.Protocols
{
    using System;
    using System.Security.Permissions;

    public abstract class MimeFormatter
    {
        protected MimeFormatter()
        {
        }

        [PermissionSet(SecurityAction.LinkDemand, Name="FullTrust")]
        public static MimeFormatter CreateInstance(Type type, object initializer)
        {
            MimeFormatter formatter = (MimeFormatter) Activator.CreateInstance(type);
            formatter.Initialize(initializer);
            return formatter;
        }

        public abstract object GetInitializer(LogicalMethodInfo methodInfo);
        [PermissionSet(SecurityAction.LinkDemand, Name="FullTrust")]
        public static object GetInitializer(Type type, LogicalMethodInfo methodInfo) => 
            ((MimeFormatter) Activator.CreateInstance(type)).GetInitializer(methodInfo);

        public virtual object[] GetInitializers(LogicalMethodInfo[] methodInfos)
        {
            object[] objArray = new object[methodInfos.Length];
            for (int i = 0; i < objArray.Length; i++)
            {
                objArray[i] = this.GetInitializer(methodInfos[i]);
            }
            return objArray;
        }

        [PermissionSet(SecurityAction.LinkDemand, Name="FullTrust")]
        public static object[] GetInitializers(Type type, LogicalMethodInfo[] methodInfos) => 
            ((MimeFormatter) Activator.CreateInstance(type)).GetInitializers(methodInfos);

        public abstract void Initialize(object initializer);
    }
}

