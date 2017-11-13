namespace Microsoft.Transactions.Wsat.Protocol
{
    using Microsoft.Transactions.Wsat.StateMachines;
    using System;

    internal class StateContainer
    {
        private State completionAborted;
        private State completionAborting;
        private State completionActive;
        private State completionCommitted;
        private State completionCommitting;
        private State completionCreated;
        private State completionCreating;
        private State completionInitializationFailed;
        private State completionInitializing;
        private State coordinatorAborted;
        private State coordinatorActive;
        private State coordinatorAwaitingEndOfRecovery;
        private State coordinatorCommitted;
        private State coordinatorCommitting;
        private State coordinatorEnlisted;
        private State coordinatorEnlisting;
        private State coordinatorFailedRecovery;
        private State coordinatorForgotten;
        private State coordinatorInitializationFailed;
        private State coordinatorInitializing;
        private State coordinatorPrepared;
        private State coordinatorPreparing;
        private State coordinatorReadOnlyInDoubt;
        private State coordinatorRecovered;
        private State coordinatorRecovering;
        private State coordinatorRegisteringBoth;
        private State coordinatorRegisteringDurable;
        private State coordinatorRegisteringVolatile;
        private State coordinatorVolatileActive;
        private State coordinatorVolatilePreparing;
        private State coordinatorVolatilePreparingRegistered;
        private State coordinatorVolatilePreparingRegistering;
        private State durableAborted;
        private State durableActive;
        private State durableCommitted;
        private State durableCommitting;
        private State durableFailedRecovery;
        private State durableInDoubt;
        private State durableInitializationFailed;
        private State durablePrepared;
        private State durablePreparing;
        private State durableRecovering;
        private State durableRecoveryAwaitingCommit;
        private State durableRecoveryAwaitingRollback;
        private State durableRecoveryReceivedCommit;
        private State durableRecoveryReceivedRollback;
        private State durableRegistering;
        private State durableRejoined;
        private State durableUnregistered;
        private State subordinateActive;
        private State subordinateFinished;
        private State subordinateInitializing;
        private State subordinateRegistering;
        private State transactionContextActive;
        private State transactionContextFinished;
        private State transactionContextInitializing;
        private State transactionContextInitializingCoordinator;
        private State volatileAborted;
        private State volatileAborting;
        private State volatileCommitted;
        private State volatileCommitting;
        private State volatileInDoubt;
        private State volatileInitializationFailed;
        private State volatilePhaseOneUnregistered;
        private State volatilePhaseZeroActive;
        private State volatilePhaseZeroUnregistered;
        private State volatilePrepared;
        private State volatilePrePrepared;
        private State volatilePrePreparing;
        private State volatileRegistering;

        public StateContainer(ProtocolState state)
        {
            this.coordinatorInitializing = new Microsoft.Transactions.Wsat.StateMachines.CoordinatorInitializing(state);
            this.coordinatorEnlisting = new Microsoft.Transactions.Wsat.StateMachines.CoordinatorEnlisting(state);
            this.coordinatorEnlisted = new Microsoft.Transactions.Wsat.StateMachines.CoordinatorEnlisted(state);
            this.coordinatorRegisteringBoth = new Microsoft.Transactions.Wsat.StateMachines.CoordinatorRegisteringBoth(state);
            this.coordinatorRegisteringDurable = new Microsoft.Transactions.Wsat.StateMachines.CoordinatorRegisteringDurable(state);
            this.coordinatorRegisteringVolatile = new Microsoft.Transactions.Wsat.StateMachines.CoordinatorRegisteringVolatile(state);
            this.coordinatorVolatileActive = new Microsoft.Transactions.Wsat.StateMachines.CoordinatorVolatileActive(state);
            this.coordinatorVolatilePreparing = new Microsoft.Transactions.Wsat.StateMachines.CoordinatorVolatilePreparing(state);
            this.coordinatorVolatilePreparingRegistering = new Microsoft.Transactions.Wsat.StateMachines.CoordinatorVolatilePreparingRegistering(state);
            this.coordinatorVolatilePreparingRegistered = new Microsoft.Transactions.Wsat.StateMachines.CoordinatorVolatilePreparingRegistered(state);
            this.coordinatorActive = new Microsoft.Transactions.Wsat.StateMachines.CoordinatorActive(state);
            this.coordinatorPreparing = new Microsoft.Transactions.Wsat.StateMachines.CoordinatorPreparing(state);
            this.coordinatorPrepared = new Microsoft.Transactions.Wsat.StateMachines.CoordinatorPrepared(state);
            this.coordinatorCommitting = new Microsoft.Transactions.Wsat.StateMachines.CoordinatorCommitting(state);
            this.coordinatorRecovering = new Microsoft.Transactions.Wsat.StateMachines.CoordinatorRecovering(state);
            this.coordinatorRecovered = new Microsoft.Transactions.Wsat.StateMachines.CoordinatorRecovered(state);
            this.coordinatorAwaitingEndOfRecovery = new Microsoft.Transactions.Wsat.StateMachines.CoordinatorAwaitingEndOfRecovery(state);
            this.coordinatorFailedRecovery = new Microsoft.Transactions.Wsat.StateMachines.CoordinatorFailedRecovery(state);
            this.coordinatorCommitted = new Microsoft.Transactions.Wsat.StateMachines.CoordinatorCommitted(state);
            this.coordinatorAborted = new Microsoft.Transactions.Wsat.StateMachines.CoordinatorAborted(state);
            this.coordinatorForgotten = new Microsoft.Transactions.Wsat.StateMachines.CoordinatorForgotten(state);
            this.coordinatorReadOnlyInDoubt = new Microsoft.Transactions.Wsat.StateMachines.CoordinatorReadOnlyInDoubt(state);
            this.coordinatorInitializationFailed = new Microsoft.Transactions.Wsat.StateMachines.CoordinatorInitializationFailed(state);
            this.completionInitializing = new Microsoft.Transactions.Wsat.StateMachines.CompletionInitializing(state);
            this.completionCreating = new Microsoft.Transactions.Wsat.StateMachines.CompletionCreating(state);
            this.completionCreated = new Microsoft.Transactions.Wsat.StateMachines.CompletionCreated(state);
            this.completionActive = new Microsoft.Transactions.Wsat.StateMachines.CompletionActive(state);
            this.completionCommitting = new Microsoft.Transactions.Wsat.StateMachines.CompletionCommitting(state);
            this.completionAborting = new Microsoft.Transactions.Wsat.StateMachines.CompletionAborting(state);
            this.completionCommitted = new Microsoft.Transactions.Wsat.StateMachines.CompletionCommitted(state);
            this.completionAborted = new Microsoft.Transactions.Wsat.StateMachines.CompletionAborted(state);
            this.completionInitializationFailed = new Microsoft.Transactions.Wsat.StateMachines.CompletionInitializationFailed(state);
            this.subordinateInitializing = new Microsoft.Transactions.Wsat.StateMachines.SubordinateInitializing(state);
            this.subordinateRegistering = new Microsoft.Transactions.Wsat.StateMachines.SubordinateRegistering(state);
            this.subordinateActive = new Microsoft.Transactions.Wsat.StateMachines.SubordinateActive(state);
            this.subordinateFinished = new Microsoft.Transactions.Wsat.StateMachines.SubordinateFinished(state);
            this.durableRegistering = new Microsoft.Transactions.Wsat.StateMachines.DurableRegistering(state);
            this.durableActive = new Microsoft.Transactions.Wsat.StateMachines.DurableActive(state);
            this.durableUnregistered = new Microsoft.Transactions.Wsat.StateMachines.DurableUnregistered(state);
            this.durablePreparing = new Microsoft.Transactions.Wsat.StateMachines.DurablePreparing(state);
            this.durablePrepared = new Microsoft.Transactions.Wsat.StateMachines.DurablePrepared(state);
            this.durableCommitting = new Microsoft.Transactions.Wsat.StateMachines.DurableCommitting(state);
            this.durableRecovering = new Microsoft.Transactions.Wsat.StateMachines.DurableRecovering(state);
            this.durableRejoined = new Microsoft.Transactions.Wsat.StateMachines.DurableRejoined(state);
            this.durableRecoveryAwaitingCommit = new Microsoft.Transactions.Wsat.StateMachines.DurableRecoveryAwaitingCommit(state);
            this.durableRecoveryReceivedCommit = new Microsoft.Transactions.Wsat.StateMachines.DurableRecoveryReceivedCommit(state);
            this.durableRecoveryAwaitingRollback = new Microsoft.Transactions.Wsat.StateMachines.DurableRecoveryAwaitingRollback(state);
            this.durableRecoveryReceivedRollback = new Microsoft.Transactions.Wsat.StateMachines.DurableRecoveryReceivedRollback(state);
            this.durableFailedRecovery = new Microsoft.Transactions.Wsat.StateMachines.DurableFailedRecovery(state);
            this.durableCommitted = new Microsoft.Transactions.Wsat.StateMachines.DurableCommitted(state);
            this.durableAborted = new Microsoft.Transactions.Wsat.StateMachines.DurableAborted(state);
            this.durableInDoubt = new Microsoft.Transactions.Wsat.StateMachines.DurableInDoubt(state);
            this.durableInitializationFailed = new Microsoft.Transactions.Wsat.StateMachines.DurableInitializationFailed(state);
            this.volatileRegistering = new Microsoft.Transactions.Wsat.StateMachines.VolatileRegistering(state);
            this.volatilePhaseZeroActive = new Microsoft.Transactions.Wsat.StateMachines.VolatilePhaseZeroActive(state);
            this.volatilePhaseZeroUnregistered = new Microsoft.Transactions.Wsat.StateMachines.VolatilePhaseZeroUnregistered(state);
            this.volatilePhaseOneUnregistered = new Microsoft.Transactions.Wsat.StateMachines.VolatilePhaseOneUnregistered(state);
            this.volatilePrePreparing = new Microsoft.Transactions.Wsat.StateMachines.VolatilePrePreparing(state);
            this.volatilePrePrepared = new Microsoft.Transactions.Wsat.StateMachines.VolatilePrePrepared(state);
            this.volatilePrepared = new Microsoft.Transactions.Wsat.StateMachines.VolatilePrepared(state);
            this.volatileCommitting = new Microsoft.Transactions.Wsat.StateMachines.VolatileCommitting(state);
            this.volatileAborting = new Microsoft.Transactions.Wsat.StateMachines.VolatileAborting(state);
            this.volatileCommitted = new Microsoft.Transactions.Wsat.StateMachines.VolatileCommitted(state);
            this.volatileAborted = new Microsoft.Transactions.Wsat.StateMachines.VolatileAborted(state);
            this.volatileInDoubt = new Microsoft.Transactions.Wsat.StateMachines.VolatileInDoubt(state);
            this.volatileInitializationFailed = new Microsoft.Transactions.Wsat.StateMachines.VolatileInitializationFailed(state);
            this.transactionContextInitializing = new Microsoft.Transactions.Wsat.StateMachines.TransactionContextInitializing(state);
            this.transactionContextInitializingCoordinator = new Microsoft.Transactions.Wsat.StateMachines.TransactionContextInitializingCoordinator(state);
            this.transactionContextActive = new Microsoft.Transactions.Wsat.StateMachines.TransactionContextActive(state);
            this.transactionContextFinished = new Microsoft.Transactions.Wsat.StateMachines.TransactionContextFinished(state);
        }

        public State CompletionAborted =>
            this.completionAborted;

        public State CompletionAborting =>
            this.completionAborting;

        public State CompletionActive =>
            this.completionActive;

        public State CompletionCommitted =>
            this.completionCommitted;

        public State CompletionCommitting =>
            this.completionCommitting;

        public State CompletionCreated =>
            this.completionCreated;

        public State CompletionCreating =>
            this.completionCreating;

        public State CompletionInitializationFailed =>
            this.completionInitializationFailed;

        public State CompletionInitializing =>
            this.completionInitializing;

        public State CoordinatorAborted =>
            this.coordinatorAborted;

        public State CoordinatorActive =>
            this.coordinatorActive;

        public State CoordinatorAwaitingEndOfRecovery =>
            this.coordinatorAwaitingEndOfRecovery;

        public State CoordinatorCommitted =>
            this.coordinatorCommitted;

        public State CoordinatorCommitting =>
            this.coordinatorCommitting;

        public State CoordinatorEnlisted =>
            this.coordinatorEnlisted;

        public State CoordinatorEnlisting =>
            this.coordinatorEnlisting;

        public State CoordinatorFailedRecovery =>
            this.coordinatorFailedRecovery;

        public State CoordinatorForgotten =>
            this.coordinatorForgotten;

        public State CoordinatorInitializationFailed =>
            this.coordinatorInitializationFailed;

        public State CoordinatorInitializing =>
            this.coordinatorInitializing;

        public State CoordinatorPrepared =>
            this.coordinatorPrepared;

        public State CoordinatorPreparing =>
            this.coordinatorPreparing;

        public State CoordinatorReadOnlyInDoubt =>
            this.coordinatorReadOnlyInDoubt;

        public State CoordinatorRecovered =>
            this.coordinatorRecovered;

        public State CoordinatorRecovering =>
            this.coordinatorRecovering;

        public State CoordinatorRegisteringBoth =>
            this.coordinatorRegisteringBoth;

        public State CoordinatorRegisteringDurable =>
            this.coordinatorRegisteringDurable;

        public State CoordinatorRegisteringVolatile =>
            this.coordinatorRegisteringVolatile;

        public State CoordinatorVolatileActive =>
            this.coordinatorVolatileActive;

        public State CoordinatorVolatilePreparing =>
            this.coordinatorVolatilePreparing;

        public State CoordinatorVolatilePreparingRegistered =>
            this.coordinatorVolatilePreparingRegistered;

        public State CoordinatorVolatilePreparingRegistering =>
            this.coordinatorVolatilePreparingRegistering;

        public State DurableAborted =>
            this.durableAborted;

        public State DurableActive =>
            this.durableActive;

        public State DurableCommitted =>
            this.durableCommitted;

        public State DurableCommitting =>
            this.durableCommitting;

        public State DurableFailedRecovery =>
            this.durableFailedRecovery;

        public State DurableInDoubt =>
            this.durableInDoubt;

        public State DurableInitializationFailed =>
            this.durableInitializationFailed;

        public State DurablePrepared =>
            this.durablePrepared;

        public State DurablePreparing =>
            this.durablePreparing;

        public State DurableRecovering =>
            this.durableRecovering;

        public State DurableRecoveryAwaitingCommit =>
            this.durableRecoveryAwaitingCommit;

        public State DurableRecoveryAwaitingRollback =>
            this.durableRecoveryAwaitingRollback;

        public State DurableRecoveryReceivedCommit =>
            this.durableRecoveryReceivedCommit;

        public State DurableRecoveryReceivedRollback =>
            this.durableRecoveryReceivedRollback;

        public State DurableRegistering =>
            this.durableRegistering;

        public State DurableRejoined =>
            this.durableRejoined;

        public State DurableUnregistered =>
            this.durableUnregistered;

        public State SubordinateActive =>
            this.subordinateActive;

        public State SubordinateFinished =>
            this.subordinateFinished;

        public State SubordinateInitializing =>
            this.subordinateInitializing;

        public State SubordinateRegistering =>
            this.subordinateRegistering;

        public State TransactionContextActive =>
            this.transactionContextActive;

        public State TransactionContextFinished =>
            this.transactionContextFinished;

        public State TransactionContextInitializing =>
            this.transactionContextInitializing;

        public State TransactionContextInitializingCoordinator =>
            this.transactionContextInitializingCoordinator;

        public State VolatileAborted =>
            this.volatileAborted;

        public State VolatileAborting =>
            this.volatileAborting;

        public State VolatileCommitted =>
            this.volatileCommitted;

        public State VolatileCommitting =>
            this.volatileCommitting;

        public State VolatileInDoubt =>
            this.volatileInDoubt;

        public State VolatileInitializationFailed =>
            this.volatileInitializationFailed;

        public State VolatilePhaseOneUnregistered =>
            this.volatilePhaseOneUnregistered;

        public State VolatilePhaseZeroActive =>
            this.volatilePhaseZeroActive;

        public State VolatilePhaseZeroUnregistered =>
            this.volatilePhaseZeroUnregistered;

        public State VolatilePrepared =>
            this.volatilePrepared;

        public State VolatilePrePrepared =>
            this.volatilePrePrepared;

        public State VolatilePrePreparing =>
            this.volatilePrePreparing;

        public State VolatileRegistering =>
            this.volatileRegistering;
    }
}

