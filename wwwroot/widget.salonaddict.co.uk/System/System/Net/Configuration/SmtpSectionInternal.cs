namespace System.Net.Configuration
{
    using System;
    using System.Configuration;
    using System.Net.Mail;
    using System.Threading;

    internal sealed class SmtpSectionInternal
    {
        private static object classSyncObject;
        private SmtpDeliveryMethod deliveryMethod;
        private string from;
        private SmtpNetworkElementInternal network;
        private SmtpSpecifiedPickupDirectoryElementInternal specifiedPickupDirectory;

        internal SmtpSectionInternal(SmtpSection section)
        {
            this.deliveryMethod = section.DeliveryMethod;
            this.from = section.From;
            this.network = new SmtpNetworkElementInternal(section.Network);
            this.specifiedPickupDirectory = new SmtpSpecifiedPickupDirectoryElementInternal(section.SpecifiedPickupDirectory);
        }

        internal static SmtpSectionInternal GetSection()
        {
            lock (ClassSyncObject)
            {
                SmtpSection section = System.Configuration.PrivilegedConfigurationManager.GetSection(ConfigurationStrings.SmtpSectionPath) as SmtpSection;
                if (section == null)
                {
                    return null;
                }
                return new SmtpSectionInternal(section);
            }
        }

        internal static object ClassSyncObject
        {
            get
            {
                if (classSyncObject == null)
                {
                    Interlocked.CompareExchange(ref classSyncObject, new object(), null);
                }
                return classSyncObject;
            }
        }

        internal SmtpDeliveryMethod DeliveryMethod =>
            this.deliveryMethod;

        internal string From =>
            this.from;

        internal SmtpNetworkElementInternal Network =>
            this.network;

        internal SmtpSpecifiedPickupDirectoryElementInternal SpecifiedPickupDirectory =>
            this.specifiedPickupDirectory;
    }
}

