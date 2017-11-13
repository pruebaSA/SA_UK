﻿namespace System.Data
{
    using System;

    public sealed class StateChangeEventArgs : EventArgs
    {
        private ConnectionState currentState;
        private ConnectionState originalState;

        public StateChangeEventArgs(ConnectionState originalState, ConnectionState currentState)
        {
            this.originalState = originalState;
            this.currentState = currentState;
        }

        public ConnectionState CurrentState =>
            this.currentState;

        public ConnectionState OriginalState =>
            this.originalState;
    }
}

