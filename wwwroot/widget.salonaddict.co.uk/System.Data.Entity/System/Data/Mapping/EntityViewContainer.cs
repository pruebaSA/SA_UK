namespace System.Data.Mapping
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;

    public abstract class EntityViewContainer
    {
        private int _viewCount;
        private string m_storededmEntityContainerName;
        private string m_storedhashOverAllExtentViews;
        private string m_storedHashOverMappingClosure;
        private string m_storedStoreEntityContainerName;

        protected EntityViewContainer()
        {
        }

        protected abstract KeyValuePair<string, string> GetViewAt(int index);

        public string EdmEntityContainerName
        {
            get => 
                this.m_storededmEntityContainerName;
            set
            {
                this.m_storededmEntityContainerName = value;
            }
        }

        internal IEnumerable<KeyValuePair<string, string>> ExtentViews
        {
            get
            {
                int index = 0;
                while (true)
                {
                    if (index >= this.ViewCount)
                    {
                        yield break;
                    }
                    yield return this.GetViewAt(index);
                    index++;
                }
            }
        }

        public string HashOverAllExtentViews
        {
            get => 
                this.m_storedhashOverAllExtentViews;
            set
            {
                this.m_storedhashOverAllExtentViews = value;
            }
        }

        public string HashOverMappingClosure
        {
            get => 
                this.m_storedHashOverMappingClosure;
            set
            {
                this.m_storedHashOverMappingClosure = value;
            }
        }

        public string StoreEntityContainerName
        {
            get => 
                this.m_storedStoreEntityContainerName;
            set
            {
                this.m_storedStoreEntityContainerName = value;
            }
        }

        public int ViewCount
        {
            get => 
                this._viewCount;
            protected set
            {
                this._viewCount = value;
            }
        }

    }
}

