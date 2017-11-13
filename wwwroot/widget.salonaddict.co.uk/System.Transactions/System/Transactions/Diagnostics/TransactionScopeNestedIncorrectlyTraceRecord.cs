﻿namespace System.Transactions.Diagnostics
{
    using System;
    using System.Diagnostics;
    using System.Transactions;
    using System.Xml;

    internal class TransactionScopeNestedIncorrectlyTraceRecord : TraceRecord
    {
        private static TransactionScopeNestedIncorrectlyTraceRecord record = new TransactionScopeNestedIncorrectlyTraceRecord();
        private string traceSource;
        private TransactionTraceIdentifier txTraceId;

        internal static void Trace(string traceSource, TransactionTraceIdentifier txTraceId)
        {
            lock (record)
            {
                record.traceSource = traceSource;
                record.txTraceId = txTraceId;
                DiagnosticTrace.TraceEvent(TraceEventType.Warning, "http://msdn.microsoft.com/2004/06/System/Transactions/TransactionScopeNestedIncorrectly", System.Transactions.SR.GetString("TraceTransactionScopeNestedIncorrectly"), record);
            }
        }

        internal override void WriteTo(XmlWriter xml)
        {
            TraceHelper.WriteTraceSource(xml, this.traceSource);
            TraceHelper.WriteTxId(xml, this.txTraceId);
        }

        internal override string EventId =>
            "http://schemas.microsoft.com/2004/03/Transactions/TransactionScopeNestedIncorrectlyTraceRecord";
    }
}

