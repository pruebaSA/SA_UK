namespace System.ServiceModel.PeerResolvers
{
    using System;
    using System.Runtime.Serialization;
    using System.ServiceModel;

    [MessageContract(IsWrapped=false)]
    public class UpdateInfo
    {
        [MessageBodyMember(Name="Update", Namespace="http://schemas.microsoft.com/net/2006/05/peer")]
        private UpdateInfoDC body;

        public UpdateInfo()
        {
            this.body = new UpdateInfoDC();
        }

        public UpdateInfo(Guid registrationId, Guid client, string meshId, PeerNodeAddress address)
        {
            this.body = new UpdateInfoDC(registrationId, client, meshId, address);
        }

        public bool HasBody() => 
            (this.body != null);

        public Guid ClientId =>
            this.body.ClientId;

        public string MeshId =>
            this.body.MeshId;

        public PeerNodeAddress NodeAddress =>
            this.body.NodeAddress;

        public Guid RegistrationId =>
            this.body.RegistrationId;

        [DataContract(Name="Update", Namespace="http://schemas.microsoft.com/net/2006/05/peer")]
        private class UpdateInfoDC
        {
            [DataMember(Name="ClientId")]
            public Guid ClientId;
            [DataMember(Name="MeshId")]
            public string MeshId;
            [DataMember(Name="NodeAddress")]
            public PeerNodeAddress NodeAddress;
            [DataMember(Name="RegistrationId")]
            public Guid RegistrationId;

            public UpdateInfoDC()
            {
            }

            public UpdateInfoDC(Guid registrationId, Guid client, string meshId, PeerNodeAddress address)
            {
                this.ClientId = client;
                this.MeshId = meshId;
                this.NodeAddress = address;
                this.RegistrationId = registrationId;
            }
        }
    }
}

