namespace System.Net.Configuration
{
    using System.Configuration;

    public sealed class MailSettingsSectionGroup : ConfigurationSectionGroup
    {
        public SmtpSection Smtp =>
            ((SmtpSection) base.Sections["smtp"]);
    }
}

