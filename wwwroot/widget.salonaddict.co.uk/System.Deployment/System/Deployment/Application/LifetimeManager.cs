namespace System.Deployment.Application
{
    using System;
    using System.Threading;

    internal static class LifetimeManager
    {
        private static bool _immediate;
        private static bool _lifetimeEnded;
        private static ManualResetEvent _lifetimeEndedEvent = new ManualResetEvent(false);
        private static bool _lifetimeExtended;
        private static int _operationsInProgress;
        private static Timer _periodicTimer;

        static LifetimeManager()
        {
            TimeSpan dueTime = new TimeSpan(0, 0, 10, 0);
            _periodicTimer = new Timer(new TimerCallback(LifetimeManager.PeriodicTimerCallback), null, dueTime, dueTime);
        }

        private static void CheckAlive()
        {
            if (_lifetimeEnded)
            {
                throw new InvalidOperationException(Resources.GetString("Ex_LifetimeEnded"));
            }
        }

        public static void EndImmediately()
        {
            lock (_periodicTimer)
            {
                if (_operationsInProgress != 0)
                {
                    Logger.StartCurrentThreadLogging();
                    Logger.AddPhaseInformation(Resources.GetString("Life_OperationsInProgress"), new object[] { _operationsInProgress });
                    Logger.EndCurrentThreadLogging();
                }
                _lifetimeEndedEvent.Set();
                _lifetimeEnded = true;
                _immediate = true;
            }
        }

        public static void EndOperation()
        {
            lock (_periodicTimer)
            {
                CheckAlive();
                _operationsInProgress--;
                _lifetimeExtended = true;
            }
        }

        public static void ExtendLifetime()
        {
            lock (_periodicTimer)
            {
                CheckAlive();
                _lifetimeExtended = true;
            }
        }

        private static void PeriodicTimerCallback(object state)
        {
            lock (_periodicTimer)
            {
                if ((_operationsInProgress == 0) && !_lifetimeExtended)
                {
                    _lifetimeEndedEvent.Set();
                    _lifetimeEnded = true;
                }
                else
                {
                    _lifetimeExtended = false;
                }
            }
        }

        public static void StartOperation()
        {
            lock (_periodicTimer)
            {
                CheckAlive();
                _operationsInProgress++;
            }
        }

        public static bool WaitForEnd()
        {
            _lifetimeEndedEvent.WaitOne();
            return _immediate;
        }
    }
}

