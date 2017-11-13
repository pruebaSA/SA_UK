namespace Microsoft.Transactions.Wsat.StateMachines
{
    using System;

    internal class TimerInstance
    {
        public static readonly TimerInstance Committing = new TimerInstance(TimerProfile.Committing);
        public static readonly TimerInstance Prepared = new TimerInstance(TimerProfile.Prepared);
        public static readonly TimerInstance Preparing = new TimerInstance(TimerProfile.Preparing);
        private TimerProfile profile;
        public static readonly TimerInstance Replaying = new TimerInstance(TimerProfile.Replaying);
        public static readonly TimerInstance VolatileOutcomeAssurance = new TimerInstance(TimerProfile.VolatileOutcomeAssurance);

        public TimerInstance(TimerProfile profile)
        {
            this.profile = profile;
        }

        public TimerProfile Profile =>
            this.profile;
    }
}

