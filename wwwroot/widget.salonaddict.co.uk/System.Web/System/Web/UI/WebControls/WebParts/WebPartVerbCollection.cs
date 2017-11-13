namespace System.Web.UI.WebControls.WebParts
{
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Reflection;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class WebPartVerbCollection : ReadOnlyCollectionBase
    {
        private HybridDictionary _ids;
        public static readonly WebPartVerbCollection Empty = new WebPartVerbCollection();

        public WebPartVerbCollection()
        {
            this.Initialize(null, null);
        }

        public WebPartVerbCollection(ICollection verbs)
        {
            this.Initialize(null, verbs);
        }

        public WebPartVerbCollection(WebPartVerbCollection existingVerbs, ICollection verbs)
        {
            this.Initialize(existingVerbs, verbs);
        }

        internal int Add(WebPartVerb value) => 
            base.InnerList.Add(value);

        public bool Contains(WebPartVerb value) => 
            base.InnerList.Contains(value);

        public void CopyTo(WebPartVerb[] array, int index)
        {
            base.InnerList.CopyTo(array, index);
        }

        public int IndexOf(WebPartVerb value) => 
            base.InnerList.IndexOf(value);

        private void Initialize(WebPartVerbCollection existingVerbs, ICollection verbs)
        {
            int initialSize = ((existingVerbs != null) ? existingVerbs.Count : 0) + ((verbs != null) ? verbs.Count : 0);
            this._ids = new HybridDictionary(initialSize, true);
            if (existingVerbs != null)
            {
                foreach (WebPartVerb verb in existingVerbs)
                {
                    if (this._ids.Contains(verb.ID))
                    {
                        throw new ArgumentException(System.Web.SR.GetString("WebPart_Collection_DuplicateID", new object[] { "WebPartVerb", verb.ID }), "existingVerbs");
                    }
                    this._ids.Add(verb.ID, verb);
                    base.InnerList.Add(verb);
                }
            }
            if (verbs != null)
            {
                foreach (object obj2 in verbs)
                {
                    if (obj2 == null)
                    {
                        throw new ArgumentException(System.Web.SR.GetString("Collection_CantAddNull"), "verbs");
                    }
                    WebPartVerb verb2 = obj2 as WebPartVerb;
                    if (verb2 == null)
                    {
                        throw new ArgumentException(System.Web.SR.GetString("Collection_InvalidType", new object[] { "WebPartVerb" }), "verbs");
                    }
                    if (this._ids.Contains(verb2.ID))
                    {
                        throw new ArgumentException(System.Web.SR.GetString("WebPart_Collection_DuplicateID", new object[] { "WebPartVerb", verb2.ID }), "verbs");
                    }
                    this._ids.Add(verb2.ID, verb2);
                    base.InnerList.Add(verb2);
                }
            }
        }

        public WebPartVerb this[int index] =>
            ((WebPartVerb) base.InnerList[index]);

        internal WebPartVerb this[string id] =>
            ((WebPartVerb) this._ids[id]);
    }
}

