namespace System.ServiceModel
{
    using System;
    using System.ServiceModel.Channels;

    public class ReliableSession
    {
        private ReliableSessionBindingElement element;

        public ReliableSession(ReliableSessionBindingElement reliableSessionBindingElement)
        {
            if (reliableSessionBindingElement == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("reliableSessionBindingElement");
            }
            this.element = reliableSessionBindingElement;
        }

        public TimeSpan InactivityTimeout
        {
            get => 
                this.element.InactivityTimeout;
            set
            {
                if (value <= TimeSpan.Zero)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value, System.ServiceModel.SR.GetString("ValueMustBePositive")));
                }
                this.element.InactivityTimeout = value;
            }
        }

        public bool Ordered
        {
            get => 
                this.element.Ordered;
            set
            {
                this.element.Ordered = value;
            }
        }
    }
}

