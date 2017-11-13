namespace System.Data.Mapping.Update.Internal
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Data.Metadata.Edm;
    using System.Data.Objects;

    internal abstract class PropagatorResult
    {
        internal const long NullIdentifier = -1L;
        internal const int NullOrdinal = -1;

        private PropagatorResult()
        {
        }

        internal static PropagatorResult CreateKeyValue(System.Data.Mapping.Update.Internal.PropagatorFlags flags, object value, IEntityStateEntry stateEntry, long identifier) => 
            new KeyValue(flags, value, stateEntry, identifier);

        internal static PropagatorResult CreateServerGenKeyValue(System.Data.Mapping.Update.Internal.PropagatorFlags flags, object value, IEntityStateEntry stateEntry, long identifier, int recordOrdinal) => 
            new ServerGenKeyValue(flags, value, stateEntry, identifier, recordOrdinal);

        internal static PropagatorResult CreateServerGenSimpleValue(System.Data.Mapping.Update.Internal.PropagatorFlags flags, object value, CurrentValueRecord record, int recordOrdinal) => 
            new ServerGenSimpleValue(flags, value, record, recordOrdinal);

        internal static PropagatorResult CreateSimpleValue(System.Data.Mapping.Update.Internal.PropagatorFlags flags, object value) => 
            new SimpleValue(flags, value);

        internal static PropagatorResult CreateStructuralValue(PropagatorResult[] values, System.Data.Metadata.Edm.StructuralType structuralType, bool isModified)
        {
            if (isModified)
            {
                return new StructuralValue(values, structuralType);
            }
            return new UnmodifiedStructuralValue(values, structuralType);
        }

        internal PropagatorResult GetMemberValue(EdmMember member)
        {
            int index = TypeHelpers.GetAllStructuralMembers(this.StructuralType).IndexOf(member);
            return this.GetMemberValue(index);
        }

        internal virtual PropagatorResult GetMemberValue(int ordinal)
        {
            throw EntityUtil.InternalError(EntityUtil.InternalErrorCode.UpdatePipelineResultRequestInvalid, 0, "PropagatorResult.GetMemberValue");
        }

        internal virtual PropagatorResult[] GetMemberValues()
        {
            throw EntityUtil.InternalError(EntityUtil.InternalErrorCode.UpdatePipelineResultRequestInvalid, 0, "PropagatorResult.GetMembersValues");
        }

        internal virtual object GetSimpleValue()
        {
            throw EntityUtil.InternalError(EntityUtil.InternalErrorCode.UpdatePipelineResultRequestInvalid, 0, "PropagatorResult.GetSimpleValue");
        }

        internal abstract PropagatorResult ReplicateResultWithNewFlags(System.Data.Mapping.Update.Internal.PropagatorFlags flags);
        internal virtual PropagatorResult ReplicateResultWithNewValue(object value)
        {
            throw EntityUtil.InternalError(EntityUtil.InternalErrorCode.UpdatePipelineResultRequestInvalid, 0, "PropagatorResult.ReplicateResultWithNewValue");
        }

        internal virtual long Identifier =>
            -1L;

        internal abstract bool IsNull { get; }

        internal abstract bool IsSimple { get; }

        internal virtual System.Data.Mapping.Update.Internal.PropagatorFlags PropagatorFlags =>
            System.Data.Mapping.Update.Internal.PropagatorFlags.NoFlags;

        internal virtual CurrentValueRecord Record =>
            null;

        internal virtual int RecordOrdinal =>
            -1;

        internal virtual IEntityStateEntry StateEntry =>
            null;

        internal virtual System.Data.Metadata.Edm.StructuralType StructuralType =>
            null;

        private class KeyValue : PropagatorResult.SimpleValue
        {
            private readonly long m_identifier;
            private readonly IEntityStateEntry m_stateEntry;

            internal KeyValue(PropagatorFlags flags, object value, IEntityStateEntry stateEntry, long identifier) : base(flags, value)
            {
                this.m_stateEntry = stateEntry;
                this.m_identifier = identifier;
            }

            internal override PropagatorResult ReplicateResultWithNewFlags(PropagatorFlags flags) => 
                new PropagatorResult.KeyValue(flags, base.m_value, this.StateEntry, this.Identifier);

            internal override PropagatorResult ReplicateResultWithNewValue(object value) => 
                new PropagatorResult.KeyValue(this.PropagatorFlags, value, this.StateEntry, this.Identifier);

            internal override long Identifier =>
                this.m_identifier;

            internal override CurrentValueRecord Record =>
                this.m_stateEntry.CurrentValues;

            internal override IEntityStateEntry StateEntry =>
                this.m_stateEntry;
        }

        private class ServerGenKeyValue : PropagatorResult.KeyValue
        {
            private readonly int m_recordOrdinal;

            internal ServerGenKeyValue(PropagatorFlags flags, object value, IEntityStateEntry stateEntry, long identifier, int recordOrdinal) : base(flags, value, stateEntry, identifier)
            {
                this.m_recordOrdinal = recordOrdinal;
            }

            internal override PropagatorResult ReplicateResultWithNewFlags(PropagatorFlags flags) => 
                new PropagatorResult.ServerGenKeyValue(flags, base.m_value, this.StateEntry, this.Identifier, this.RecordOrdinal);

            internal override PropagatorResult ReplicateResultWithNewValue(object value) => 
                new PropagatorResult.ServerGenKeyValue(this.PropagatorFlags, value, this.StateEntry, this.Identifier, this.RecordOrdinal);

            internal override int RecordOrdinal =>
                this.m_recordOrdinal;
        }

        private class ServerGenSimpleValue : PropagatorResult.SimpleValue
        {
            private readonly CurrentValueRecord m_record;
            private readonly int m_recordOrdinal;

            internal ServerGenSimpleValue(PropagatorFlags flags, object value, CurrentValueRecord record, int recordOrdinal) : base(flags, value)
            {
                this.m_record = record;
                this.m_recordOrdinal = recordOrdinal;
            }

            internal override PropagatorResult ReplicateResultWithNewFlags(PropagatorFlags flags) => 
                new PropagatorResult.ServerGenSimpleValue(flags, base.m_value, this.Record, this.RecordOrdinal);

            internal override PropagatorResult ReplicateResultWithNewValue(object value) => 
                new PropagatorResult.ServerGenSimpleValue(this.PropagatorFlags, value, this.Record, this.RecordOrdinal);

            internal override CurrentValueRecord Record =>
                this.m_record;

            internal override int RecordOrdinal =>
                this.m_recordOrdinal;
        }

        private class SimpleValue : PropagatorResult
        {
            private readonly System.Data.Mapping.Update.Internal.PropagatorFlags m_flags;
            protected readonly object m_value;

            internal SimpleValue(System.Data.Mapping.Update.Internal.PropagatorFlags flags, object value)
            {
                this.m_flags = flags;
                this.m_value = value ?? DBNull.Value;
            }

            internal override object GetSimpleValue() => 
                this.m_value;

            internal override PropagatorResult ReplicateResultWithNewFlags(System.Data.Mapping.Update.Internal.PropagatorFlags flags) => 
                new PropagatorResult.SimpleValue(flags, this.m_value);

            internal override PropagatorResult ReplicateResultWithNewValue(object value) => 
                new PropagatorResult.SimpleValue(this.PropagatorFlags, value);

            internal override bool IsNull =>
                (DBNull.Value == this.m_value);

            internal override bool IsSimple =>
                true;

            internal override System.Data.Mapping.Update.Internal.PropagatorFlags PropagatorFlags =>
                this.m_flags;
        }

        private class StructuralValue : PropagatorResult
        {
            private readonly System.Data.Metadata.Edm.StructuralType m_structuralType;
            private readonly PropagatorResult[] m_values;

            internal StructuralValue(PropagatorResult[] values, System.Data.Metadata.Edm.StructuralType structuralType)
            {
                this.m_values = values;
                this.m_structuralType = structuralType;
            }

            internal override PropagatorResult GetMemberValue(int ordinal) => 
                this.m_values[ordinal];

            internal override PropagatorResult[] GetMemberValues() => 
                this.m_values;

            internal override PropagatorResult ReplicateResultWithNewFlags(PropagatorFlags flags)
            {
                throw EntityUtil.InternalError(EntityUtil.InternalErrorCode.UpdatePipelineResultRequestInvalid, 0, "StructuralValue.ReplicateResultWithNewFlags");
            }

            internal override bool IsNull =>
                false;

            internal override bool IsSimple =>
                false;

            internal override System.Data.Metadata.Edm.StructuralType StructuralType =>
                this.m_structuralType;
        }

        private class UnmodifiedStructuralValue : PropagatorResult.StructuralValue
        {
            internal UnmodifiedStructuralValue(PropagatorResult[] values, StructuralType structuralType) : base(values, structuralType)
            {
            }

            internal override System.Data.Mapping.Update.Internal.PropagatorFlags PropagatorFlags =>
                (System.Data.Mapping.Update.Internal.PropagatorFlags.NoFlags | System.Data.Mapping.Update.Internal.PropagatorFlags.Preserve);
        }
    }
}

