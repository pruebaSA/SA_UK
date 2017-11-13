namespace System.Data.SqlClient
{
    using Microsoft.SqlServer.Server;
    using System;

    internal class TdsParameterSetter : SmiTypedGetterSetter
    {
        private TdsRecordBufferSetter _target;

        internal TdsParameterSetter(TdsParserStateObject stateObj, SmiMetaData md)
        {
            this._target = new TdsRecordBufferSetter(stateObj, md);
        }

        internal override SmiTypedGetterSetter GetTypedGetterSetter(SmiEventSink sink, int ordinal) => 
            this._target;

        public override void SetDBNull(SmiEventSink sink, int ordinal)
        {
            this._target.EndElements(sink);
        }

        internal override bool CanGet =>
            false;

        internal override bool CanSet =>
            true;
    }
}

