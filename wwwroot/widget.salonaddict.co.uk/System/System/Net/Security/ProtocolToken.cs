﻿namespace System.Net.Security
{
    using System;
    using System.ComponentModel;
    using System.Net;

    internal class ProtocolToken
    {
        internal byte[] Payload;
        internal int Size;
        internal SecurityStatus Status;

        internal ProtocolToken(byte[] data, SecurityStatus errorCode)
        {
            this.Status = errorCode;
            this.Payload = data;
            this.Size = (data != null) ? data.Length : 0;
        }

        internal Win32Exception GetException()
        {
            if (!this.Done)
            {
                return new Win32Exception((int) this.Status);
            }
            return null;
        }

        internal bool CloseConnection =>
            (this.Status == SecurityStatus.ContextExpired);

        internal bool Done =>
            (this.Status == SecurityStatus.OK);

        internal bool Failed =>
            ((this.Status != SecurityStatus.OK) && (this.Status != SecurityStatus.ContinueNeeded));

        internal bool Renegotiate =>
            (this.Status == SecurityStatus.Renegotiate);
    }
}

