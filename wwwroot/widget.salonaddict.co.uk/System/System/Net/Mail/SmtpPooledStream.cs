namespace System.Net.Mail
{
    using System;
    using System.Net;

    internal class SmtpPooledStream : PooledStream
    {
        internal ICredentialsByHost creds;
        internal bool dsnEnabled;
        internal bool previouslyUsed;

        internal SmtpPooledStream(ConnectionPool connectionPool, TimeSpan lifetime, bool checkLifetime) : base(connectionPool, lifetime, checkLifetime)
        {
        }
    }
}

