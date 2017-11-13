namespace System.Runtime.Remoting.Messaging
{
    using System;
    using System.Collections;
    using System.Globalization;
    using System.Runtime.Remoting;
    using System.Runtime.Remoting.Activation;

    [Serializable]
    internal class InternalSink
    {
        internal static IMessage DisallowAsyncActivation(IMessage reqMsg)
        {
            if (reqMsg is IConstructionCallMessage)
            {
                return new ReturnMessage(new RemotingException(Environment.GetResourceString("Remoting_Activation_AsyncUnsupported")), null);
            }
            return null;
        }

        internal static Identity GetIdentity(IMessage reqMsg)
        {
            Identity identityObject = null;
            if (reqMsg is IInternalMessage)
            {
                identityObject = ((IInternalMessage) reqMsg).IdentityObject;
            }
            else if (reqMsg is InternalMessageWrapper)
            {
                identityObject = (Identity) ((InternalMessageWrapper) reqMsg).GetIdentityObject();
            }
            if (identityObject == null)
            {
                string uRI = GetURI(reqMsg);
                identityObject = IdentityHolder.ResolveIdentity(uRI);
                if (identityObject == null)
                {
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Remoting_ServerObjectNotFound"), new object[] { uRI }));
                }
            }
            return identityObject;
        }

        internal static ServerIdentity GetServerIdentity(IMessage reqMsg)
        {
            ServerIdentity serverIdentityObject = null;
            bool flag = false;
            IInternalMessage message = reqMsg as IInternalMessage;
            if (message != null)
            {
                serverIdentityObject = ((IInternalMessage) reqMsg).ServerIdentityObject;
                flag = true;
            }
            else if (reqMsg is InternalMessageWrapper)
            {
                serverIdentityObject = (ServerIdentity) ((InternalMessageWrapper) reqMsg).GetServerIdentityObject();
            }
            if (serverIdentityObject == null)
            {
                Identity identity2 = IdentityHolder.ResolveIdentity(GetURI(reqMsg));
                if (identity2 is ServerIdentity)
                {
                    serverIdentityObject = (ServerIdentity) identity2;
                    if (flag)
                    {
                        message.ServerIdentityObject = serverIdentityObject;
                    }
                }
            }
            return serverIdentityObject;
        }

        internal static string GetURI(IMessage msg)
        {
            string str = null;
            IMethodMessage message = msg as IMethodMessage;
            if (message != null)
            {
                return message.Uri;
            }
            IDictionary properties = msg.Properties;
            if (properties != null)
            {
                str = (string) properties["__Uri"];
            }
            return str;
        }

        internal static IMessage ValidateMessage(IMessage reqMsg)
        {
            IMessage message = null;
            if (reqMsg == null)
            {
                message = new ReturnMessage(new ArgumentNullException("reqMsg"), null);
            }
            return message;
        }
    }
}

