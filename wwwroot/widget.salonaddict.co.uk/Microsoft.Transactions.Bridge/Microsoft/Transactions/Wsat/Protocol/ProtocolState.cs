namespace Microsoft.Transactions.Wsat.Protocol
{
    using Microsoft.Transactions.Bridge;
    using Microsoft.Transactions.Wsat.InputOutput;
    using Microsoft.Transactions.Wsat.Messaging;
    using Microsoft.Transactions.Wsat.Recovery;
    using Microsoft.Transactions.Wsat.StateMachines;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.ServiceModel;

    internal class ProtocolState
    {
        private Microsoft.Transactions.Wsat.InputOutput.ActivationCoordinator activationCoordinator;
        private ICoordinationListener activationCoordinatorListener;
        private StateContainer allStates;
        private static Stopwatch clock = Stopwatch.StartNew();
        private Microsoft.Transactions.Wsat.InputOutput.CompletionCoordinator completionCoordinator;
        private ICoordinationListener completionCoordinatorListener;
        private Configuration config;
        private CoordinationService coordination;
        private DebugTracingEventSink debugTrace = new DebugTracingEventSink();
        private Microsoft.Transactions.Wsat.Messaging.Faults faults;
        private Microsoft.Transactions.Wsat.InputOutput.FaultSender faultsender;
        private LookupTables lookupTables;
        private static PerformanceCounterHolder perfCounters = new PerformanceCounterHolder();
        private Guid processId = Guid.NewGuid();
        private Microsoft.Transactions.Wsat.Protocol.ProtocolVersion protocolVersion;
        private bool recovering;
        private object recoveryLock = new object();
        private Queue<SynchronizationEvent> recoveryQueue = new Queue<SynchronizationEvent>();
        private Microsoft.Transactions.Wsat.InputOutput.RegistrationCoordinator registrationCoordinator;
        private ICoordinationListener registrationCoordinatorListener;
        private Microsoft.Transactions.Wsat.InputOutput.RegistrationParticipant registrationParticipant;
        private Microsoft.Transactions.Wsat.Recovery.LogEntrySerialization serializer;
        private Microsoft.Transactions.Wsat.Protocol.TimerManager timerManager;
        private Microsoft.Transactions.Bridge.TransactionManager tm;
        private Microsoft.Transactions.Wsat.InputOutput.TransactionManagerReceive tmReceive;
        private Microsoft.Transactions.Wsat.InputOutput.TransactionManagerSend tmSend;
        private Microsoft.Transactions.Wsat.InputOutput.TwoPhaseCommitCoordinator twoPhaseCommitCoordinator;
        private ICoordinationListener twoPhaseCommitCoordinatorListener;
        private Microsoft.Transactions.Wsat.InputOutput.TwoPhaseCommitParticipant twoPhaseCommitParticipant;
        private ICoordinationListener twoPhaseCommitParticipantListener;

        public ProtocolState(Microsoft.Transactions.Bridge.TransactionManager transactionManager, Microsoft.Transactions.Wsat.Protocol.ProtocolVersion protocolVersion)
        {
            this.protocolVersion = protocolVersion;
            this.tm = transactionManager;
            try
            {
                this.config = new Configuration(this);
            }
            catch (ConfigurationProviderException exception)
            {
                throw Microsoft.Transactions.Bridge.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new PluggableProtocolException(exception.Message, exception));
            }
            this.lookupTables = new LookupTables(this);
            this.tmReceive = new Microsoft.Transactions.Wsat.InputOutput.TransactionManagerReceive(this);
            this.tmSend = new Microsoft.Transactions.Wsat.InputOutput.TransactionManagerSend(this);
            this.timerManager = new Microsoft.Transactions.Wsat.Protocol.TimerManager(this);
            this.recovering = true;
            this.serializer = new Microsoft.Transactions.Wsat.Recovery.LogEntrySerialization(this);
            this.allStates = new StateContainer(this);
            this.faults = Microsoft.Transactions.Wsat.Messaging.Faults.Version(protocolVersion);
            this.faultsender = new Microsoft.Transactions.Wsat.InputOutput.FaultSender(this);
            this.activationCoordinator = new Microsoft.Transactions.Wsat.InputOutput.ActivationCoordinator(this);
            this.registrationCoordinator = new Microsoft.Transactions.Wsat.InputOutput.RegistrationCoordinator(this);
            this.registrationParticipant = new Microsoft.Transactions.Wsat.InputOutput.RegistrationParticipant(this);
            this.completionCoordinator = new Microsoft.Transactions.Wsat.InputOutput.CompletionCoordinator(this);
            this.twoPhaseCommitCoordinator = new Microsoft.Transactions.Wsat.InputOutput.TwoPhaseCommitCoordinator(this);
            this.twoPhaseCommitParticipant = new Microsoft.Transactions.Wsat.InputOutput.TwoPhaseCommitParticipant(this);
        }

        private void CleanupOnFailure()
        {
            this.StopListeners();
        }

        public void EnqueueRecoveryReplay(TmReplayEvent e)
        {
            if (!this.recovering)
            {
                Microsoft.Transactions.Bridge.DiagnosticUtility.FailFast("Cannot enqueue recovery event outside of recovery");
            }
            if (DebugTrace.Info)
            {
                CoordinatorEnlistment coordinator = e.Coordinator;
                DebugTrace.TxTrace(TraceLevel.Info, coordinator.EnlistmentId, "Enqueuing recovery replay for coordinator at {0}", Ports.TryGetAddress(coordinator.CoordinatorProxy));
            }
            this.recoveryQueue.Enqueue(e);
        }

        private void ProcessPendingRecoveryEvents()
        {
            DebugTrace.TraceEnter(this, "ProcessPendingRecoveryEvents");
            if (this.recovering)
            {
                Microsoft.Transactions.Bridge.DiagnosticUtility.FailFast("Cannot process recovery events while recovering");
            }
            if (this.recoveryQueue.Count == 0)
            {
                DebugTrace.Trace(TraceLevel.Verbose, "No events were queued during recovery");
            }
            else
            {
                DebugTrace.Trace(TraceLevel.Verbose, "Processing events queued during recovery");
                while (this.recoveryQueue.Count > 0)
                {
                    SynchronizationEvent e = this.recoveryQueue.Dequeue();
                    e.Enlistment.StateMachine.Enqueue(e);
                }
            }
            DebugTrace.TraceLeave(this, "ProcessPendingRecoveryEvents");
        }

        public void RecoveryBeginning()
        {
            DebugTrace.TraceEnter(this, "RecoveryBeginning");
            if (this.config.NetworkEndpointsEnabled)
            {
                try
                {
                    this.coordination = new CoordinationService(this.config.PortConfiguration, this.protocolVersion);
                    this.activationCoordinatorListener = this.coordination.Add(this.activationCoordinator);
                    this.registrationCoordinatorListener = this.coordination.Add(this.registrationCoordinator);
                    this.completionCoordinatorListener = this.coordination.Add(this.completionCoordinator);
                    this.twoPhaseCommitCoordinatorListener = this.coordination.Add(this.twoPhaseCommitCoordinator);
                    this.twoPhaseCommitParticipantListener = this.coordination.Add(this.twoPhaseCommitParticipant);
                }
                catch (MessagingInitializationException exception)
                {
                    if (DebugTrace.Error)
                    {
                        DebugTrace.Trace(TraceLevel.Error, "Error initializing CoordinationService: {0}", exception);
                    }
                    this.CleanupOnFailure();
                    throw Microsoft.Transactions.Bridge.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new PluggableProtocolException(exception.Message, exception));
                }
                catch (Exception exception2)
                {
                    if (DebugTrace.Error)
                    {
                        DebugTrace.Trace(TraceLevel.Error, "Unknown exception initializing CoordinationService: {0}", exception2);
                    }
                    this.CleanupOnFailure();
                    throw;
                }
            }
            DebugTrace.TraceLeave(this, "RecoveryBeginning");
        }

        public void RecoveryComplete()
        {
            DebugTrace.TraceEnter(this, "RecoveryComplete");
            if (this.config.NetworkEndpointsEnabled)
            {
                this.StartListeners();
            }
            lock (this.recoveryLock)
            {
                this.recovering = false;
                this.ProcessPendingRecoveryEvents();
                if (this.recoveryQueue.Count != 0)
                {
                    Microsoft.Transactions.Bridge.DiagnosticUtility.FailFast("Recovery queue should be empty");
                }
                this.recoveryQueue = null;
            }
            DebugTrace.TraceLeave(this, "RecoveryComplete");
        }

        public void Start()
        {
            DebugTrace.TraceEnter(this, "Start");
            DebugTrace.TraceLeave(this, "Start");
        }

        private void StartListeners()
        {
            DebugTrace.TraceEnter(this, "StartListeners");
            try
            {
                this.twoPhaseCommitCoordinatorListener.Start();
                this.twoPhaseCommitParticipantListener.Start();
                this.completionCoordinatorListener.Start();
                this.registrationCoordinatorListener.Start();
                this.activationCoordinatorListener.Start();
            }
            catch (MessagingInitializationException exception)
            {
                if (DebugTrace.Error)
                {
                    DebugTrace.Trace(TraceLevel.Error, "Error starting a listener: {0}", exception);
                }
                this.CleanupOnFailure();
                throw Microsoft.Transactions.Bridge.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new PluggableProtocolException(exception.Message, exception));
            }
            catch (Exception exception2)
            {
                Microsoft.Transactions.Bridge.DiagnosticUtility.ExceptionUtility.TraceHandledException(exception2, TraceEventType.Error);
                if (DebugTrace.Error)
                {
                    DebugTrace.Trace(TraceLevel.Error, "Unknown exception starting a listener: {0}", exception2);
                }
                this.CleanupOnFailure();
                throw;
            }
            DebugTrace.TraceLeave(this, "StartListeners");
        }

        public void Stop()
        {
            DebugTrace.TraceEnter(this, "Stop");
            this.StopListeners();
            DebugTrace.TraceLeave(this, "Stop");
        }

        private void StopListeners()
        {
            DebugTrace.TraceEnter(this, "StopListeners");
            if (this.twoPhaseCommitCoordinatorListener != null)
            {
                this.twoPhaseCommitCoordinatorListener.Stop();
                this.twoPhaseCommitCoordinatorListener = null;
            }
            if (this.twoPhaseCommitParticipantListener != null)
            {
                this.twoPhaseCommitParticipantListener.Stop();
                this.twoPhaseCommitParticipantListener = null;
            }
            if (this.completionCoordinatorListener != null)
            {
                this.completionCoordinatorListener.Stop();
                this.completionCoordinatorListener = null;
            }
            if (this.registrationCoordinatorListener != null)
            {
                this.registrationCoordinatorListener.Stop();
                this.registrationCoordinatorListener = null;
            }
            if (this.activationCoordinatorListener != null)
            {
                this.activationCoordinatorListener.Stop();
                this.activationCoordinatorListener = null;
            }
            if (this.coordination != null)
            {
                this.coordination.Cleanup();
                this.coordination = null;
            }
            DebugTrace.TraceLeave(this, "StopListeners");
        }

        public CompletionParticipantProxy TryCreateCompletionParticipantProxy(EndpointAddress to)
        {
            try
            {
                return this.coordination.CreateCompletionParticipantProxy(to);
            }
            catch (CreateChannelFailureException exception)
            {
                Microsoft.Transactions.Bridge.DiagnosticUtility.ExceptionUtility.TraceHandledException(exception, TraceEventType.Warning);
                if (DebugTrace.Warning)
                {
                    DebugTrace.Trace(TraceLevel.Warning, "Could not create proxy to completion participant at {0}: {1}", to.Uri, exception.Message);
                }
            }
            return null;
        }

        public RegistrationProxy TryCreateRegistrationProxy(EndpointAddress to)
        {
            try
            {
                return this.coordination.CreateRegistrationProxy(to);
            }
            catch (CreateChannelFailureException exception)
            {
                Microsoft.Transactions.Bridge.DiagnosticUtility.ExceptionUtility.TraceHandledException(exception, TraceEventType.Warning);
                if (DebugTrace.Warning)
                {
                    DebugTrace.Trace(TraceLevel.Warning, "Could not create proxy to Registration coordinator at {0}: {1}", to.Uri, exception.Message);
                }
            }
            return null;
        }

        public TwoPhaseCommitCoordinatorProxy TryCreateTwoPhaseCommitCoordinatorProxy(EndpointAddress to)
        {
            if (this.config.NetworkEndpointsEnabled)
            {
                try
                {
                    return this.coordination.CreateTwoPhaseCommitCoordinatorProxy(to, null);
                }
                catch (CreateChannelFailureException exception)
                {
                    Microsoft.Transactions.Bridge.DiagnosticUtility.ExceptionUtility.TraceHandledException(exception, TraceEventType.Warning);
                    if (DebugTrace.Warning)
                    {
                        DebugTrace.Trace(TraceLevel.Warning, "Could not create proxy to 2PC coordinator at {0}: {1}", to.Uri, exception.Message);
                    }
                }
            }
            return null;
        }

        public TwoPhaseCommitParticipantProxy TryCreateTwoPhaseCommitParticipantProxy(EndpointAddress to)
        {
            if (this.config.NetworkEndpointsEnabled)
            {
                try
                {
                    return this.coordination.CreateTwoPhaseCommitParticipantProxy(to, null);
                }
                catch (CreateChannelFailureException exception)
                {
                    Microsoft.Transactions.Bridge.DiagnosticUtility.ExceptionUtility.TraceHandledException(exception, TraceEventType.Warning);
                    if (DebugTrace.Warning)
                    {
                        DebugTrace.Trace(TraceLevel.Warning, "Could not create proxy to 2PC participant at {0}: {1}", to.Uri, exception.Message);
                    }
                }
            }
            return null;
        }

        public bool TryEnqueueRecoveryOutcome(ParticipantCallbackEvent e)
        {
            lock (this.recoveryLock)
            {
                if (!this.recovering)
                {
                    return false;
                }
                if (DebugTrace.Info)
                {
                    ParticipantEnlistment participant = e.Participant;
                    DebugTrace.TxTrace(TraceLevel.Info, participant.EnlistmentId, "Queuing recovery outcome {0} for participant at {1}", e, Ports.TryGetAddress(participant.ParticipantProxy));
                }
                this.recoveryQueue.Enqueue(e);
            }
            return true;
        }

        public Microsoft.Transactions.Wsat.InputOutput.ActivationCoordinator ActivationCoordinator =>
            this.activationCoordinator;

        public Microsoft.Transactions.Wsat.InputOutput.CompletionCoordinator CompletionCoordinator =>
            this.completionCoordinator;

        public ICoordinationListener CompletionCoordinatorListener =>
            this.completionCoordinatorListener;

        public Configuration Config =>
            this.config;

        public DebugTracingEventSink DebugTraceSink =>
            this.debugTrace;

        public TimeSpan ElapsedTime =>
            clock.Elapsed;

        public Microsoft.Transactions.Wsat.Messaging.Faults Faults =>
            this.faults;

        public Microsoft.Transactions.Wsat.InputOutput.FaultSender FaultSender =>
            this.faultsender;

        public Microsoft.Transactions.Wsat.Recovery.LogEntrySerialization LogEntrySerialization =>
            this.serializer;

        public LookupTables Lookup =>
            this.lookupTables;

        public PerformanceCounterHolder Perf =>
            perfCounters;

        public Guid ProcessId =>
            this.processId;

        public Microsoft.Transactions.Wsat.Protocol.ProtocolVersion ProtocolVersion =>
            this.protocolVersion;

        public bool Recovering =>
            this.recovering;

        public Microsoft.Transactions.Wsat.InputOutput.RegistrationCoordinator RegistrationCoordinator =>
            this.registrationCoordinator;

        public ICoordinationListener RegistrationCoordinatorListener =>
            this.registrationCoordinatorListener;

        public Microsoft.Transactions.Wsat.InputOutput.RegistrationParticipant RegistrationParticipant =>
            this.registrationParticipant;

        public CoordinationService Service =>
            this.coordination;

        public StateContainer States =>
            this.allStates;

        public Microsoft.Transactions.Wsat.Protocol.TimerManager TimerManager =>
            this.timerManager;

        public Microsoft.Transactions.Bridge.TransactionManager TransactionManager =>
            this.tm;

        public Microsoft.Transactions.Wsat.InputOutput.TransactionManagerReceive TransactionManagerReceive =>
            this.tmReceive;

        public Microsoft.Transactions.Wsat.InputOutput.TransactionManagerSend TransactionManagerSend =>
            this.tmSend;

        public Microsoft.Transactions.Wsat.InputOutput.TwoPhaseCommitCoordinator TwoPhaseCommitCoordinator =>
            this.twoPhaseCommitCoordinator;

        public ICoordinationListener TwoPhaseCommitCoordinatorListener =>
            this.twoPhaseCommitCoordinatorListener;

        public Microsoft.Transactions.Wsat.InputOutput.TwoPhaseCommitParticipant TwoPhaseCommitParticipant =>
            this.twoPhaseCommitParticipant;

        public ICoordinationListener TwoPhaseCommitParticipantListener =>
            this.twoPhaseCommitParticipantListener;
    }
}

