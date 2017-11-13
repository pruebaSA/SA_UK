namespace System.Web.UI.WebControls.WebParts
{
    using System;
    using System.Reflection;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class ConsumerConnectionPoint : ConnectionPoint
    {
        internal static readonly Type[] ConstructorTypes;

        static ConsumerConnectionPoint()
        {
            ConstructorInfo constructor = typeof(ConsumerConnectionPoint).GetConstructors()[0];
            ConstructorTypes = WebPartUtil.GetTypesForConstructor(constructor);
        }

        public ConsumerConnectionPoint(MethodInfo callbackMethod, Type interfaceType, Type controlType, string displayName, string id, bool allowsMultipleConnections) : base(callbackMethod, interfaceType, controlType, displayName, id, allowsMultipleConnections)
        {
        }

        public virtual void SetObject(Control control, object data)
        {
            if (control == null)
            {
                throw new ArgumentNullException("control");
            }
            base.CallbackMethod.Invoke(control, new object[] { data });
        }

        public virtual bool SupportsConnection(Control control, ConnectionInterfaceCollection secondaryInterfaces) => 
            true;
    }
}

