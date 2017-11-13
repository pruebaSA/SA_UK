﻿namespace System.Runtime.Remoting.Channels
{
    using System;

    internal class CachedSocket
    {
        private CachedSocket _next;
        private SocketHandler _socket;
        private DateTime _socketLastUsed;

        internal CachedSocket(SocketHandler socket, CachedSocket next)
        {
            this._socket = socket;
            this._socketLastUsed = DateTime.UtcNow;
            this._next = next;
        }

        internal SocketHandler Handler =>
            this._socket;

        internal DateTime LastUsed =>
            this._socketLastUsed;

        internal CachedSocket Next
        {
            get => 
                this._next;
            set
            {
                this._next = value;
            }
        }
    }
}

