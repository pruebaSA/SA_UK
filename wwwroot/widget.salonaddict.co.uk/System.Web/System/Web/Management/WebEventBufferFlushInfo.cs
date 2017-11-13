namespace System.Web.Management
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class WebEventBufferFlushInfo
    {
        private WebBaseEventCollection _events;
        private int _eventsDiscardedSinceLastNotification;
        private int _eventsInBuffer;
        private DateTime _lastNotification;
        private int _notificationSequence;
        private EventNotificationType _notificationType;

        internal WebEventBufferFlushInfo(WebBaseEventCollection events, EventNotificationType notificationType, int notificationSequence, DateTime lastNotification, int eventsDiscardedSinceLastNotification, int eventsInBuffer)
        {
            this._events = events;
            this._notificationType = notificationType;
            this._notificationSequence = notificationSequence;
            this._lastNotification = lastNotification;
            this._eventsDiscardedSinceLastNotification = eventsDiscardedSinceLastNotification;
            this._eventsInBuffer = eventsInBuffer;
        }

        public WebBaseEventCollection Events =>
            this._events;

        public int EventsDiscardedSinceLastNotification =>
            this._eventsDiscardedSinceLastNotification;

        public int EventsInBuffer =>
            this._eventsInBuffer;

        public DateTime LastNotificationUtc =>
            this._lastNotification;

        public int NotificationSequence =>
            this._notificationSequence;

        public EventNotificationType NotificationType =>
            this._notificationType;
    }
}

