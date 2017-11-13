namespace System.Web.Management
{
    using System;
    using System.Net.Mail;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class MailEventNotificationInfo
    {
        private int _discardedSinceLastNotification;
        private WebBaseEventCollection _events;
        private int _eventsInBuffer;
        private int _eventsInNotification;
        private int _eventsLostDueToMessageLimit;
        private int _eventsRemaining;
        private DateTime _lastNotificationUtc;
        private int _messageSequence;
        private int _messagesInNotification;
        private MailMessage _msg;
        private int _notificationSequence;
        private EventNotificationType _notificationType;

        internal MailEventNotificationInfo(MailMessage msg, WebBaseEventCollection events, DateTime lastNotificationUtc, int discardedSinceLastNotification, int eventsInBuffer, int notificationSequence, EventNotificationType notificationType, int eventsInNotification, int eventsRemaining, int messagesInNotification, int eventsLostDueToMessageLimit, int messageSequence)
        {
            this._events = events;
            this._lastNotificationUtc = lastNotificationUtc;
            this._discardedSinceLastNotification = discardedSinceLastNotification;
            this._eventsInBuffer = eventsInBuffer;
            this._notificationSequence = notificationSequence;
            this._notificationType = notificationType;
            this._eventsInNotification = eventsInNotification;
            this._eventsRemaining = eventsRemaining;
            this._messagesInNotification = messagesInNotification;
            this._eventsLostDueToMessageLimit = eventsLostDueToMessageLimit;
            this._messageSequence = messageSequence;
            this._msg = msg;
        }

        public WebBaseEventCollection Events =>
            this._events;

        public int EventsDiscardedByBuffer =>
            this._discardedSinceLastNotification;

        public int EventsDiscardedDueToMessageLimit =>
            this._eventsLostDueToMessageLimit;

        public int EventsInBuffer =>
            this._eventsInBuffer;

        public int EventsInNotification =>
            this._eventsInNotification;

        public int EventsRemaining =>
            this._eventsRemaining;

        public DateTime LastNotificationUtc =>
            this._lastNotificationUtc;

        public MailMessage Message =>
            this._msg;

        public int MessageSequence =>
            this._messageSequence;

        public int MessagesInNotification =>
            this._messagesInNotification;

        public int NotificationSequence =>
            this._notificationSequence;

        public EventNotificationType NotificationType =>
            this._notificationType;
    }
}

