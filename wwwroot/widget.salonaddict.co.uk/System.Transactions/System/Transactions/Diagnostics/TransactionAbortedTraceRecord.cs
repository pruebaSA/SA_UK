﻿namespace System.Transactions.Diagnostics
{
    using System;
    using System.Diagnostics;
    using System.Transactions;
    using System.Xml;

    internal class TransactionAbortedTraceRecord : TraceRecord
    {
        private static TransactionAbortedTraceRecord record = new TransactionAbortedTraceRecord();
        private string traceSource;
        private TransactionTraceIdentifier txTraceId;

        internal static void Trace(string traceSource, TransactionTraceIdentifier txTraceId)
        {
            lock (record)
            {
                record.traceSource = traceSource;
                record.txTraceId = txTraceId;
                DiagnosticTrace.TraceEvent(TraceEventType.Warning, "http://msdn.microsoft.com/2004/06/System/Transactions/TransactionAborted", System.Transactions.SR.GetString("TraceTransactionAborted"), record);
            }
        }

        internal override void WriteTo(XmlWriter xml)
        {
            TraceHelper.WriteTraceSource(xml, this.traceSource);
            TraceHelper.WriteTxId(xml, this.txTraceId);
        }

        internal override string EventId =>
            "http://schemas.microsoft.com/2004/03/Transactions/TransactionAbortedTraceRecord";
    }
}

