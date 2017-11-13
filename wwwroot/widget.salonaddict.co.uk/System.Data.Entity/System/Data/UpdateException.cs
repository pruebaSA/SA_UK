namespace System.Data
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Runtime.Serialization;

    [Serializable]
    public class UpdateException : DataException
    {
        [NonSerialized]
        private ReadOnlyCollection<ObjectStateEntry> _stateEntries;

        public UpdateException()
        {
        }

        public UpdateException(string message) : base(message)
        {
        }

        protected UpdateException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public UpdateException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public UpdateException(string message, Exception innerException, IEnumerable<ObjectStateEntry> stateEntries) : base(message, innerException)
        {
            this._stateEntries = new List<ObjectStateEntry>(stateEntries).AsReadOnly();
        }

        public ReadOnlyCollection<ObjectStateEntry> StateEntries =>
            this._stateEntries;
    }
}

