namespace System.Runtime.Remoting.Channels.Http
{
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Runtime.Remoting.Messaging;

    internal class ErrorMessage : IMethodCallMessage, IMethodMessage, IMessage
    {
        private int m_ArgCount;
        private string m_ArgName = "Unknown";
        private string m_MethodName = "Unknown";
        private object m_MethodSignature;
        private string m_TypeName = "Unknown";
        private string m_URI = "Exception";

        public object GetArg(int argNum) => 
            null;

        public string GetArgName(int index) => 
            this.m_ArgName;

        public object GetInArg(int argNum) => 
            null;

        public string GetInArgName(int index) => 
            null;

        public int ArgCount =>
            this.m_ArgCount;

        public object[] Args =>
            null;

        public bool HasVarArgs =>
            false;

        public int InArgCount =>
            this.m_ArgCount;

        public object[] InArgs =>
            null;

        public System.Runtime.Remoting.Messaging.LogicalCallContext LogicalCallContext =>
            null;

        public System.Reflection.MethodBase MethodBase =>
            null;

        public string MethodName =>
            this.m_MethodName;

        public object MethodSignature =>
            this.m_MethodSignature;

        public IDictionary Properties =>
            null;

        public string TypeName =>
            this.m_TypeName;

        public string Uri =>
            this.m_URI;
    }
}

