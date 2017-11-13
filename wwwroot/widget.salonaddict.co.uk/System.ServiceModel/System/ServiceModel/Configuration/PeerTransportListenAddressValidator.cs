﻿namespace System.ServiceModel.Configuration
{
    using System;
    using System.Configuration;
    using System.Net;
    using System.ServiceModel.Channels;

    internal class PeerTransportListenAddressValidator : ConfigurationValidatorBase
    {
        public override bool CanValidate(Type type) => 
            (type == typeof(IPAddress));

        public override void Validate(object value)
        {
            PeerValidateHelper.ValidateListenIPAddress(value as IPAddress);
        }
    }
}

