namespace System.Net.Mail
{
    using System;
    using System.Collections.Specialized;
    using System.Net;
    using System.Net.Mime;
    using System.Text;

    internal class Message
    {
        private MailAddressCollection bcc;
        private MailAddressCollection cc;
        private MimeBasePart content;
        private HeaderCollection envelopeHeaders;
        private MailAddress from;
        private HeaderCollection headers;
        private MailPriority priority;
        private MailAddress replyTo;
        private MailAddress sender;
        private string subject;
        private Encoding subjectEncoding;
        private MailAddressCollection to;

        internal Message()
        {
            this.priority = ~MailPriority.Normal;
        }

        internal Message(MailAddress from, MailAddress to) : this()
        {
            this.from = from;
            this.To.Add(to);
        }

        internal Message(string from, string to) : this()
        {
            if (from == null)
            {
                throw new ArgumentNullException("from");
            }
            if (to == null)
            {
                throw new ArgumentNullException("to");
            }
            if (from == string.Empty)
            {
                throw new ArgumentException(SR.GetString("net_emptystringcall", new object[] { "from" }), "from");
            }
            if (to == string.Empty)
            {
                throw new ArgumentException(SR.GetString("net_emptystringcall", new object[] { "to" }), "to");
            }
            this.from = new MailAddress(from);
            MailAddressCollection addresss = new MailAddressCollection();
            addresss.Add(to);
            this.to = addresss;
        }

        internal virtual IAsyncResult BeginSend(BaseWriter writer, bool sendEnvelope, AsyncCallback callback, object state)
        {
            this.PrepareHeaders(sendEnvelope);
            writer.WriteHeaders(this.Headers);
            if (this.Content != null)
            {
                return this.Content.BeginSend(writer, callback, state);
            }
            LazyAsyncResult result = new LazyAsyncResult(this, state, callback);
            IAsyncResult result2 = writer.BeginGetContentStream(new AsyncCallback(this.EmptySendCallback), new EmptySendContext(writer, result));
            if (result2.CompletedSynchronously)
            {
                writer.EndGetContentStream(result2).Close();
            }
            return result;
        }

        internal void EmptySendCallback(IAsyncResult result)
        {
            Exception exception = null;
            if (!result.CompletedSynchronously)
            {
                EmptySendContext asyncState = (EmptySendContext) result.AsyncState;
                try
                {
                    asyncState.writer.EndGetContentStream(result).Close();
                }
                catch (Exception exception2)
                {
                    exception = exception2;
                }
                catch
                {
                    exception = new Exception(SR.GetString("net_nonClsCompliantException"));
                }
                asyncState.result.InvokeCallback(exception);
            }
        }

        internal virtual void EndSend(IAsyncResult asyncResult)
        {
            if (asyncResult == null)
            {
                throw new ArgumentNullException("asyncResult");
            }
            if (this.Content != null)
            {
                this.Content.EndSend(asyncResult);
            }
            else
            {
                LazyAsyncResult result = asyncResult as LazyAsyncResult;
                if ((result == null) || (result.AsyncObject != this))
                {
                    throw new ArgumentException(SR.GetString("net_io_invalidasyncresult"));
                }
                if (result.EndCalled)
                {
                    throw new InvalidOperationException(SR.GetString("net_io_invalidendcall", new object[] { "EndSend" }));
                }
                result.InternalWaitForCompletion();
                result.EndCalled = true;
                if (result.Result is Exception)
                {
                    throw ((Exception) result.Result);
                }
            }
        }

        internal void PrepareEnvelopeHeaders(bool sendEnvelope)
        {
            this.EnvelopeHeaders[MailHeaderInfo.GetString(MailHeaderID.XSender)] = this.From.ToEncodedString();
            this.EnvelopeHeaders.Remove(MailHeaderInfo.GetString(MailHeaderID.XReceiver));
            foreach (MailAddress address in this.To)
            {
                this.EnvelopeHeaders.Add(MailHeaderInfo.GetString(MailHeaderID.XReceiver), address.ToEncodedString());
            }
            foreach (MailAddress address2 in this.CC)
            {
                this.EnvelopeHeaders.Add(MailHeaderInfo.GetString(MailHeaderID.XReceiver), address2.ToEncodedString());
            }
            foreach (MailAddress address3 in this.Bcc)
            {
                this.EnvelopeHeaders.Add(MailHeaderInfo.GetString(MailHeaderID.XReceiver), address3.ToEncodedString());
            }
        }

        internal void PrepareHeaders(bool sendEnvelope)
        {
            this.Headers[MailHeaderInfo.GetString(MailHeaderID.MimeVersion)] = "1.0";
            this.Headers[MailHeaderInfo.GetString(MailHeaderID.From)] = this.From.ToEncodedString();
            if (this.Sender != null)
            {
                this.Headers[MailHeaderInfo.GetString(MailHeaderID.Sender)] = this.Sender.ToEncodedString();
            }
            else
            {
                this.Headers.Remove(MailHeaderInfo.GetString(MailHeaderID.Sender));
            }
            if (this.To.Count > 0)
            {
                this.Headers[MailHeaderInfo.GetString(MailHeaderID.To)] = this.To.ToEncodedString();
            }
            else
            {
                this.Headers.Remove(MailHeaderInfo.GetString(MailHeaderID.To));
            }
            if (this.CC.Count > 0)
            {
                this.Headers[MailHeaderInfo.GetString(MailHeaderID.Cc)] = this.CC.ToEncodedString();
            }
            else
            {
                this.Headers.Remove(MailHeaderInfo.GetString(MailHeaderID.Cc));
            }
            if (this.replyTo != null)
            {
                this.Headers[MailHeaderInfo.GetString(MailHeaderID.ReplyTo)] = this.ReplyTo.ToEncodedString();
            }
            if (this.priority == MailPriority.High)
            {
                this.Headers[MailHeaderInfo.GetString(MailHeaderID.XPriority)] = "1";
                this.Headers[MailHeaderInfo.GetString(MailHeaderID.Priority)] = "urgent";
                this.Headers[MailHeaderInfo.GetString(MailHeaderID.Importance)] = "high";
            }
            else if (this.priority == MailPriority.Low)
            {
                this.Headers[MailHeaderInfo.GetString(MailHeaderID.XPriority)] = "5";
                this.Headers[MailHeaderInfo.GetString(MailHeaderID.Priority)] = "non-urgent";
                this.Headers[MailHeaderInfo.GetString(MailHeaderID.Importance)] = "low";
            }
            else if (this.priority != ~MailPriority.Normal)
            {
                this.Headers.Remove(MailHeaderInfo.GetString(MailHeaderID.XPriority));
                this.Headers.Remove(MailHeaderInfo.GetString(MailHeaderID.Priority));
                this.Headers.Remove(MailHeaderInfo.GetString(MailHeaderID.Importance));
            }
            this.Headers[MailHeaderInfo.GetString(MailHeaderID.Date)] = MailBnfHelper.GetDateTimeString(DateTime.Now, null);
            if ((this.subject != null) && (this.subject != string.Empty))
            {
                this.Headers[MailHeaderInfo.GetString(MailHeaderID.Subject)] = MimeBasePart.EncodeHeaderValue(this.subject, this.subjectEncoding, MimeBasePart.ShouldUseBase64Encoding(this.subjectEncoding));
            }
            else
            {
                this.Headers.Remove(MailHeaderInfo.GetString(MailHeaderID.Subject));
            }
        }

        internal virtual void Send(BaseWriter writer, bool sendEnvelope)
        {
            if (sendEnvelope)
            {
                this.PrepareEnvelopeHeaders(sendEnvelope);
                writer.WriteHeaders(this.EnvelopeHeaders);
            }
            this.PrepareHeaders(sendEnvelope);
            writer.WriteHeaders(this.Headers);
            if (this.Content != null)
            {
                this.Content.Send(writer);
            }
            else
            {
                writer.GetContentStream().Close();
            }
        }

        internal MailAddressCollection Bcc
        {
            get
            {
                if (this.bcc == null)
                {
                    this.bcc = new MailAddressCollection();
                }
                return this.bcc;
            }
        }

        internal MailAddressCollection CC
        {
            get
            {
                if (this.cc == null)
                {
                    this.cc = new MailAddressCollection();
                }
                return this.cc;
            }
        }

        internal virtual MimeBasePart Content
        {
            get => 
                this.content;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                this.content = value;
            }
        }

        internal NameValueCollection EnvelopeHeaders
        {
            get
            {
                if (this.envelopeHeaders == null)
                {
                    this.envelopeHeaders = new HeaderCollection();
                    if (Logging.On)
                    {
                        Logging.Associate(Logging.Web, this, this.envelopeHeaders);
                    }
                }
                return this.envelopeHeaders;
            }
        }

        internal MailAddress From
        {
            get => 
                this.from;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                this.from = value;
            }
        }

        internal NameValueCollection Headers
        {
            get
            {
                if (this.headers == null)
                {
                    this.headers = new HeaderCollection();
                    if (Logging.On)
                    {
                        Logging.Associate(Logging.Web, this, this.headers);
                    }
                }
                return this.headers;
            }
        }

        public MailPriority Priority
        {
            get
            {
                if (this.priority != ~MailPriority.Normal)
                {
                    return this.priority;
                }
                return MailPriority.Normal;
            }
            set
            {
                this.priority = value;
            }
        }

        internal MailAddress ReplyTo
        {
            get => 
                this.replyTo;
            set
            {
                this.replyTo = value;
            }
        }

        internal MailAddress Sender
        {
            get => 
                this.sender;
            set
            {
                this.sender = value;
            }
        }

        internal string Subject
        {
            get => 
                this.subject;
            set
            {
                if ((value != null) && MailBnfHelper.HasCROrLF(value))
                {
                    throw new ArgumentException(SR.GetString("MailSubjectInvalidFormat"));
                }
                this.subject = value;
                if (((this.subject != null) && (this.subjectEncoding == null)) && !MimeBasePart.IsAscii(this.subject, false))
                {
                    this.subjectEncoding = Encoding.GetEncoding("utf-8");
                }
            }
        }

        internal Encoding SubjectEncoding
        {
            get => 
                this.subjectEncoding;
            set
            {
                this.subjectEncoding = value;
            }
        }

        internal MailAddressCollection To
        {
            get
            {
                if (this.to == null)
                {
                    this.to = new MailAddressCollection();
                }
                return this.to;
            }
        }

        internal class EmptySendContext
        {
            internal LazyAsyncResult result;
            internal BaseWriter writer;

            internal EmptySendContext(BaseWriter writer, LazyAsyncResult result)
            {
                this.writer = writer;
                this.result = result;
            }
        }
    }
}

