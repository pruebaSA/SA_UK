﻿namespace System.Net.Mail
{
    using System;
    using System.Collections;
    using System.Net;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography.X509Certificates;

    internal class SmtpTransport
    {
        private ISmtpAuthenticationModule[] authenticationModules;
        private SmtpClient client;
        private X509CertificateCollection clientCertificates;
        private SmtpConnection connection;
        private ICredentialsByHost credentials;
        internal const int DefaultPort = 0x19;
        private bool enableSsl;
        private ArrayList failedRecipientExceptions;
        private bool m_IdentityRequired;
        private int timeout;

        internal SmtpTransport(SmtpClient client) : this(client, SmtpAuthenticationManager.GetModules())
        {
        }

        internal SmtpTransport(SmtpClient client, ISmtpAuthenticationModule[] authenticationModules)
        {
            this.timeout = 0x186a0;
            this.failedRecipientExceptions = new ArrayList();
            this.client = client;
            if (authenticationModules == null)
            {
                throw new ArgumentNullException("authenticationModules");
            }
            this.authenticationModules = authenticationModules;
        }

        internal void Abort()
        {
            if (this.connection != null)
            {
                this.connection.Abort();
            }
        }

        internal IAsyncResult BeginGetConnection(string host, int port, ContextAwareResult outerResult, AsyncCallback callback, object state)
        {
            if (host == null)
            {
                throw new ArgumentNullException("host");
            }
            if ((port < 0) || (port > 0xffff))
            {
                throw new ArgumentOutOfRangeException("port");
            }
            IAsyncResult result = null;
            try
            {
                this.connection = new SmtpConnection(this, this.client, this.credentials, this.authenticationModules);
                this.connection.Timeout = this.timeout;
                if (Logging.On)
                {
                    Logging.Associate(Logging.Web, this, this.connection);
                }
                if (this.EnableSsl)
                {
                    this.connection.EnableSsl = true;
                    this.connection.ClientCertificates = this.ClientCertificates;
                }
                result = this.connection.BeginGetConnection(host, port, outerResult, callback, state);
            }
            catch (Exception exception)
            {
                throw new SmtpException(SR.GetString("MailHostNotFound"), exception);
            }
            catch
            {
                throw new SmtpException(SR.GetString("MailHostNotFound"), new Exception(SR.GetString("net_nonClsCompliantException")));
            }
            return result;
        }

        internal IAsyncResult BeginSendMail(MailAddress sender, MailAddressCollection recipients, string deliveryNotify, AsyncCallback callback, object state)
        {
            if (sender == null)
            {
                throw new ArgumentNullException("sender");
            }
            if (recipients == null)
            {
                throw new ArgumentNullException("recipients");
            }
            SendMailAsyncResult result = new SendMailAsyncResult(this.connection, sender.SmtpAddress, recipients, this.connection.DSNEnabled ? deliveryNotify : null, callback, state);
            result.Send();
            return result;
        }

        internal void EndGetConnection(IAsyncResult result)
        {
            this.connection.EndGetConnection(result);
        }

        internal MailWriter EndSendMail(IAsyncResult result) => 
            SendMailAsyncResult.End(result);

        internal void GetConnection(string host, int port)
        {
            if (host == null)
            {
                throw new ArgumentNullException("host");
            }
            if ((port < 0) || (port > 0xffff))
            {
                throw new ArgumentOutOfRangeException("port");
            }
            this.connection = new SmtpConnection(this, this.client, this.credentials, this.authenticationModules);
            this.connection.Timeout = this.timeout;
            if (Logging.On)
            {
                Logging.Associate(Logging.Web, this, this.connection);
            }
            if (this.EnableSsl)
            {
                this.connection.EnableSsl = true;
                this.connection.ClientCertificates = this.ClientCertificates;
            }
            this.connection.GetConnection(host, port);
        }

        internal void ReleaseConnection()
        {
            if (this.connection != null)
            {
                this.connection.ReleaseConnection();
            }
        }

        internal MailWriter SendMail(MailAddress sender, MailAddressCollection recipients, string deliveryNotify, out SmtpFailedRecipientException exception)
        {
            if (sender == null)
            {
                throw new ArgumentNullException("sender");
            }
            if (recipients == null)
            {
                throw new ArgumentNullException("recipients");
            }
            MailCommand.Send(this.connection, SmtpCommands.Mail, sender.SmtpAddress);
            this.failedRecipientExceptions.Clear();
            exception = null;
            foreach (MailAddress address in recipients)
            {
                string str;
                if (!RecipientCommand.Send(this.connection, this.connection.DSNEnabled ? (address.SmtpAddress + deliveryNotify) : address.SmtpAddress, out str))
                {
                    this.failedRecipientExceptions.Add(new SmtpFailedRecipientException(this.connection.Reader.StatusCode, address.SmtpAddress, str));
                }
            }
            if (this.failedRecipientExceptions.Count > 0)
            {
                if (this.failedRecipientExceptions.Count == 1)
                {
                    exception = (SmtpFailedRecipientException) this.failedRecipientExceptions[0];
                }
                else
                {
                    exception = new SmtpFailedRecipientsException(this.failedRecipientExceptions, this.failedRecipientExceptions.Count == recipients.Count);
                }
                if (this.failedRecipientExceptions.Count == recipients.Count)
                {
                    exception.fatal = true;
                    throw exception;
                }
            }
            DataCommand.Send(this.connection);
            return new MailWriter(this.connection.GetClosableStream());
        }

        internal X509CertificateCollection ClientCertificates
        {
            get
            {
                if (this.clientCertificates == null)
                {
                    this.clientCertificates = new X509CertificateCollection();
                }
                return this.clientCertificates;
            }
        }

        internal ICredentialsByHost Credentials
        {
            get => 
                this.credentials;
            set
            {
                this.credentials = value;
            }
        }

        internal bool EnableSsl
        {
            get => 
                this.enableSsl;
            set
            {
                this.enableSsl = value;
            }
        }

        internal bool IdentityRequired
        {
            get => 
                this.m_IdentityRequired;
            set
            {
                this.m_IdentityRequired = value;
            }
        }

        internal bool IsConnected =>
            ((this.connection != null) && this.connection.IsConnected);

        internal int Timeout
        {
            get => 
                this.timeout;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                this.timeout = value;
            }
        }
    }
}

