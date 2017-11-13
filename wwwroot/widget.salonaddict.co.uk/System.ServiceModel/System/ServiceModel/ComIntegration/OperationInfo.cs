namespace System.ServiceModel.ComIntegration
{
    using System;

    internal class OperationInfo
    {
        private string[] methodRoleMembers;
        private string name;

        public OperationInfo(ComCatalogObject methodObject, ComCatalogObject application)
        {
            this.name = (string) methodObject.GetValue("Name");
            ComCatalogCollection collection = methodObject.GetCollection("RolesForMethod");
            this.methodRoleMembers = CatalogUtil.GetRoleMembers(application, collection);
        }

        public string[] MethodRoleMembers =>
            this.methodRoleMembers;

        public string Name =>
            this.name;
    }
}

