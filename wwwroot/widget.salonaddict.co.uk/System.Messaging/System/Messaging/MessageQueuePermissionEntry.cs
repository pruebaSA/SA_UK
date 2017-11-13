namespace System.Messaging
{
    using System;
    using System.ComponentModel;

    [Serializable]
    public class MessageQueuePermissionEntry
    {
        private string category;
        private string label;
        private string machineName;
        private string path;
        private MessageQueuePermissionAccess permissionAccess;

        public MessageQueuePermissionEntry(MessageQueuePermissionAccess permissionAccess, string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }
            if ((path != "*") && !MessageQueue.ValidatePath(path, false))
            {
                throw new ArgumentException(Res.GetString("PathSyntax"));
            }
            this.path = path;
            this.permissionAccess = permissionAccess;
        }

        public MessageQueuePermissionEntry(MessageQueuePermissionAccess permissionAccess, string machineName, string label, string category)
        {
            if (((machineName == null) && (label == null)) && (category == null))
            {
                throw new ArgumentNullException("machineName");
            }
            if ((machineName != null) && !SyntaxCheck.CheckMachineName(machineName))
            {
                throw new ArgumentException(Res.GetString("InvalidParameter", new object[] { "MachineName", machineName }));
            }
            this.permissionAccess = permissionAccess;
            this.machineName = machineName;
            this.label = label;
            this.category = category;
        }

        public string Category =>
            this.category;

        public string Label =>
            this.label;

        public string MachineName =>
            this.machineName;

        public string Path =>
            this.path;

        public MessageQueuePermissionAccess PermissionAccess =>
            this.permissionAccess;
    }
}

