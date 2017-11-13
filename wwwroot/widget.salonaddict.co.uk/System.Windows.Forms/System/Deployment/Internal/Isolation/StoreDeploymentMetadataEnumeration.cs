﻿namespace System.Deployment.Internal.Isolation
{
    using System;
    using System.Collections;

    internal class StoreDeploymentMetadataEnumeration : IEnumerator
    {
        private System.Deployment.Internal.Isolation.IDefinitionAppId _current;
        private System.Deployment.Internal.Isolation.IEnumSTORE_DEPLOYMENT_METADATA _enum;
        private bool _fValid;

        public StoreDeploymentMetadataEnumeration(System.Deployment.Internal.Isolation.IEnumSTORE_DEPLOYMENT_METADATA pI)
        {
            this._enum = pI;
        }

        private System.Deployment.Internal.Isolation.IDefinitionAppId GetCurrent()
        {
            if (!this._fValid)
            {
                throw new InvalidOperationException();
            }
            return this._current;
        }

        public IEnumerator GetEnumerator() => 
            this;

        public bool MoveNext()
        {
            System.Deployment.Internal.Isolation.IDefinitionAppId[] appIds = new System.Deployment.Internal.Isolation.IDefinitionAppId[1];
            uint num = this._enum.Next(1, appIds);
            if (num == 1)
            {
                this._current = appIds[0];
            }
            return (this._fValid = num == 1);
        }

        public void Reset()
        {
            this._fValid = false;
            this._enum.Reset();
        }

        public System.Deployment.Internal.Isolation.IDefinitionAppId Current =>
            this.GetCurrent();

        object IEnumerator.Current =>
            this.GetCurrent();
    }
}

