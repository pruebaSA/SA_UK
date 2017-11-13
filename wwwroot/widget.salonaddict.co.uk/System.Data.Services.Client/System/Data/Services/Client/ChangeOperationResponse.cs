﻿namespace System.Data.Services.Client
{
    using System;
    using System.Collections.Generic;

    public sealed class ChangeOperationResponse : OperationResponse
    {
        private System.Data.Services.Client.Descriptor descriptor;

        internal ChangeOperationResponse(Dictionary<string, string> headers, System.Data.Services.Client.Descriptor descriptor) : base(headers)
        {
            this.descriptor = descriptor;
        }

        public System.Data.Services.Client.Descriptor Descriptor =>
            this.descriptor;
    }
}

