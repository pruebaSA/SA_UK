namespace System.ServiceModel.PeerResolvers
{
    using System;
    using System.Runtime.Serialization;
    using System.ServiceModel;

    [MessageContract(IsWrapped=false)]
    public class RegisterResponseInfo
    {
        [MessageBodyMember(Name="Update", Namespace="http://schemas.microsoft.com/net/2006/05/peer")]
        private RegisterResponseInfoDC body;

        public RegisterResponseInfo()
        {
            this.body = new RegisterResponseInfoDC();
        }

        public RegisterResponseInfo(Guid registrationId, TimeSpan registrationLifetime)
        {
            this.body = new RegisterResponseInfoDC(registrationId, registrationLifetime);
        }

        public bool HasBody() => 
            (this.body != null);

        public Guid RegistrationId
        {
            get => 
                this.body.RegistrationId;
            set
            {
                this.body.RegistrationId = value;
            }
        }

        public TimeSpan RegistrationLifetime
        {
            get => 
                this.body.RegistrationLifetime;
            set
            {
                if (value < TimeSpan.Zero)
                {
                    throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value, System.ServiceModel.SR.GetString("SFxTimeoutOutOfRange0")));
                }
                if (TimeoutHelper.IsTooLarge(value))
                {
                    throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value, System.ServiceModel.SR.GetString("SFxTimeoutOutOfRangeTooBig")));
                }
                this.body.RegistrationLifetime = value;
            }
        }

        [DataContract(Name="RegisterResponse", Namespace="http://schemas.microsoft.com/net/2006/05/peer")]
        private class RegisterResponseInfoDC
        {
            [DataMember(Name="RegistrationId")]
            public Guid RegistrationId;
            [DataMember(Name="RegistrationLifetime")]
            public TimeSpan RegistrationLifetime;

            public RegisterResponseInfoDC()
            {
            }

            public RegisterResponseInfoDC(Guid registrationId, TimeSpan registrationLifetime)
            {
                this.RegistrationLifetime = registrationLifetime;
                this.RegistrationId = registrationId;
            }
        }
    }
}

