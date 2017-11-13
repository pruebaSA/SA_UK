namespace Microsoft.Transactions.Wsat.Protocol
{
    using System;

    internal class PerformanceCounterHolder
    {
        private PerformanceCounterWrapper averageParticipantCommitResponseTime = new PerformanceCounterWrapper("Average participant commit response time");
        private PerformanceCounterWrapper averageParticipantCommitResponseTimeBase = new PerformanceCounterWrapper("Average participant commit response time Base");
        private PerformanceCounterWrapper averageParticipantPrepareResponseTime = new PerformanceCounterWrapper("Average participant prepare response time");
        private PerformanceCounterWrapper averageParticipantPrepareResponseTimeBase = new PerformanceCounterWrapper("Average participant prepare response time Base");
        private PerformanceCounterWrapper commitRetryCountPerInterval = new PerformanceCounterWrapper("Commit retry count/sec");
        private PerformanceCounterWrapper faultsReceivedCountPerInterval = new PerformanceCounterWrapper("Faults received count/sec");
        private PerformanceCounterWrapper faultsSentCountPerInterval = new PerformanceCounterWrapper("Faults sent count/sec");
        private PerformanceCounterWrapper messageSendFailureCountPerInterval = new PerformanceCounterWrapper("Message send failures/sec");
        private PerformanceCounterWrapper preparedRetryCountPerInterval = new PerformanceCounterWrapper("Prepared retry count/sec");
        private PerformanceCounterWrapper prepareRetryCountPerInterval = new PerformanceCounterWrapper("Prepare retry count/sec");
        private PerformanceCounterWrapper replayRetryCountPerInterval = new PerformanceCounterWrapper("Replay retry count/sec");

        public PerformanceCounterWrapper AverageParticipantCommitResponseTime =>
            this.averageParticipantCommitResponseTime;

        public PerformanceCounterWrapper AverageParticipantCommitResponseTimeBase =>
            this.averageParticipantCommitResponseTimeBase;

        public PerformanceCounterWrapper AverageParticipantPrepareResponseTime =>
            this.averageParticipantPrepareResponseTime;

        public PerformanceCounterWrapper AverageParticipantPrepareResponseTimeBase =>
            this.averageParticipantPrepareResponseTimeBase;

        public PerformanceCounterWrapper CommitRetryCountPerInterval =>
            this.commitRetryCountPerInterval;

        public PerformanceCounterWrapper FaultsReceivedCountPerInterval =>
            this.faultsReceivedCountPerInterval;

        public PerformanceCounterWrapper FaultsSentCountPerInterval =>
            this.faultsSentCountPerInterval;

        public PerformanceCounterWrapper MessageSendFailureCountPerInterval =>
            this.messageSendFailureCountPerInterval;

        public PerformanceCounterWrapper PreparedRetryCountPerInterval =>
            this.preparedRetryCountPerInterval;

        public PerformanceCounterWrapper PrepareRetryCountPerInterval =>
            this.prepareRetryCountPerInterval;

        public PerformanceCounterWrapper ReplayRetryCountPerInterval =>
            this.replayRetryCountPerInterval;
    }
}

