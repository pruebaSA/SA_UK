namespace System.DirectoryServices.ActiveDirectory
{
    using System;
    using System.Runtime.Serialization;
    using System.Security.Permissions;

    [Serializable]
    public class ActiveDirectoryObjectNotFoundException : Exception, ISerializable
    {
        private string name;
        private System.Type objectType;

        public ActiveDirectoryObjectNotFoundException()
        {
        }

        public ActiveDirectoryObjectNotFoundException(string message) : base(message)
        {
        }

        protected ActiveDirectoryObjectNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public ActiveDirectoryObjectNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }

        public ActiveDirectoryObjectNotFoundException(string message, System.Type type, string name) : base(message)
        {
            this.objectType = type;
            this.name = name;
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter=true)]
        public override void GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext)
        {
            base.GetObjectData(serializationInfo, streamingContext);
        }

        public string Name =>
            this.name;

        public System.Type Type =>
            this.objectType;
    }
}

