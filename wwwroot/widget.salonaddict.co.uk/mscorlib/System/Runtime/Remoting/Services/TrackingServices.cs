namespace System.Runtime.Remoting.Services
{
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting;
    using System.Security.Permissions;
    using System.Threading;

    [ComVisible(true), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure)]
    public class TrackingServices
    {
        private static ITrackingHandler[] _Handlers = new ITrackingHandler[0];
        private static int _Size = 0;
        private static object s_TrackingServicesSyncObject = null;

        internal static void DisconnectedObject(object obj)
        {
            try
            {
                ITrackingHandler[] handlerArray = _Handlers;
                for (int i = 0; i < _Size; i++)
                {
                    handlerArray[i].DisconnectedObject(obj);
                }
            }
            catch
            {
            }
        }

        internal static void MarshaledObject(object obj, ObjRef or)
        {
            try
            {
                ITrackingHandler[] handlerArray = _Handlers;
                for (int i = 0; i < _Size; i++)
                {
                    handlerArray[i].MarshaledObject(obj, or);
                }
            }
            catch
            {
            }
        }

        private static int Match(ITrackingHandler handler)
        {
            for (int i = 0; i < _Size; i++)
            {
                if (_Handlers[i] == handler)
                {
                    return i;
                }
            }
            return -1;
        }

        public static void RegisterTrackingHandler(ITrackingHandler handler)
        {
            lock (TrackingServicesSyncObject)
            {
                if (handler == null)
                {
                    throw new ArgumentNullException("handler");
                }
                if (-1 != Match(handler))
                {
                    throw new RemotingException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Remoting_TrackingHandlerAlreadyRegistered"), new object[] { "handler" }));
                }
                if ((_Handlers == null) || (_Size == _Handlers.Length))
                {
                    ITrackingHandler[] destinationArray = new ITrackingHandler[(_Size * 2) + 4];
                    if (_Handlers != null)
                    {
                        Array.Copy(_Handlers, destinationArray, _Size);
                    }
                    _Handlers = destinationArray;
                }
                _Handlers[_Size++] = handler;
            }
        }

        internal static void UnmarshaledObject(object obj, ObjRef or)
        {
            try
            {
                ITrackingHandler[] handlerArray = _Handlers;
                for (int i = 0; i < _Size; i++)
                {
                    handlerArray[i].UnmarshaledObject(obj, or);
                }
            }
            catch
            {
            }
        }

        public static void UnregisterTrackingHandler(ITrackingHandler handler)
        {
            lock (TrackingServicesSyncObject)
            {
                if (handler == null)
                {
                    throw new ArgumentNullException("handler");
                }
                int destinationIndex = Match(handler);
                if (-1 == destinationIndex)
                {
                    throw new RemotingException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Remoting_HandlerNotRegistered"), new object[] { handler }));
                }
                Array.Copy(_Handlers, destinationIndex + 1, _Handlers, destinationIndex, (_Size - destinationIndex) - 1);
                _Size--;
            }
        }

        public static ITrackingHandler[] RegisteredHandlers
        {
            get
            {
                lock (TrackingServicesSyncObject)
                {
                    if (_Size == 0)
                    {
                        return new ITrackingHandler[0];
                    }
                    ITrackingHandler[] handlerArray = new ITrackingHandler[_Size];
                    for (int i = 0; i < _Size; i++)
                    {
                        handlerArray[i] = _Handlers[i];
                    }
                    return handlerArray;
                }
            }
        }

        private static object TrackingServicesSyncObject
        {
            get
            {
                if (s_TrackingServicesSyncObject == null)
                {
                    object obj2 = new object();
                    Interlocked.CompareExchange(ref s_TrackingServicesSyncObject, obj2, null);
                }
                return s_TrackingServicesSyncObject;
            }
        }
    }
}

