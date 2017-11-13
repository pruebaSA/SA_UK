namespace MigraDoc.DocumentObjectModel
{
    using MigraDoc.DocumentObjectModel.Internals;
    using System;

    public class ListInfo : DocumentObject
    {
        [DV]
        internal NBool continuePreviousList;
        [DV(Type=typeof(MigraDoc.DocumentObjectModel.ListType))]
        internal NEnum listType;
        private static MigraDoc.DocumentObjectModel.Internals.Meta meta;
        [DV]
        internal Unit numberPosition;

        public ListInfo()
        {
            this.listType = NEnum.NullValue(typeof(MigraDoc.DocumentObjectModel.ListType));
            this.numberPosition = Unit.NullValue;
            this.continuePreviousList = NBool.NullValue;
        }

        internal ListInfo(DocumentObject parent) : base(parent)
        {
            this.listType = NEnum.NullValue(typeof(MigraDoc.DocumentObjectModel.ListType));
            this.numberPosition = Unit.NullValue;
            this.continuePreviousList = NBool.NullValue;
        }

        public ListInfo Clone() => 
            ((ListInfo) this.DeepCopy());

        internal override void Serialize(Serializer serializer)
        {
            if (!this.listType.IsNull)
            {
                serializer.WriteSimpleAttribute("ListInfo.ListType", this.ListType);
            }
            if (!this.numberPosition.IsNull)
            {
                serializer.WriteSimpleAttribute("ListInfo.NumberPosition", this.NumberPosition);
            }
            if (!this.continuePreviousList.IsNull)
            {
                serializer.WriteSimpleAttribute("ListInfo.ContinuePreviousList", this.ContinuePreviousList);
            }
        }

        public bool ContinuePreviousList
        {
            get => 
                this.continuePreviousList.Value;
            set
            {
                this.continuePreviousList.Value = value;
            }
        }

        public MigraDoc.DocumentObjectModel.ListType ListType
        {
            get => 
                ((MigraDoc.DocumentObjectModel.ListType) this.listType.Value);
            set
            {
                this.listType.Value = (int) value;
            }
        }

        internal override MigraDoc.DocumentObjectModel.Internals.Meta Meta
        {
            get
            {
                if (meta == null)
                {
                    meta = new MigraDoc.DocumentObjectModel.Internals.Meta(typeof(ListInfo));
                }
                return meta;
            }
        }

        public Unit NumberPosition
        {
            get => 
                this.numberPosition;
            set
            {
                this.numberPosition = value;
            }
        }
    }
}

