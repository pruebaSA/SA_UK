namespace System.Data.Services.Common
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Xml;

    internal abstract class EpmContentSerializerBase
    {
        protected EpmContentSerializerBase(EpmTargetTree tree, bool isSyndication, object element, XmlWriter target)
        {
            this.Root = isSyndication ? tree.SyndicationRoot : tree.NonSyndicationRoot;
            this.Element = element;
            this.Target = target;
            this.Success = false;
        }

        internal void Serialize()
        {
            foreach (EpmTargetPathSegment segment in this.Root.SubSegments)
            {
                this.Serialize(segment, EpmSerializationKind.All);
            }
            this.Success = true;
        }

        protected virtual void Serialize(EpmTargetPathSegment targetSegment, EpmSerializationKind kind)
        {
            IEnumerable<EpmTargetPathSegment> subSegments;
            switch (kind)
            {
                case EpmSerializationKind.Attributes:
                    subSegments = from s in targetSegment.SubSegments
                        where s.IsAttribute
                        select s;
                    break;

                case EpmSerializationKind.Elements:
                    subSegments = from s in targetSegment.SubSegments
                        where !s.IsAttribute
                        select s;
                    break;

                default:
                    subSegments = targetSegment.SubSegments;
                    break;
            }
            foreach (EpmTargetPathSegment segment in subSegments)
            {
                this.Serialize(segment, kind);
            }
        }

        protected object Element { get; private set; }

        protected EpmTargetPathSegment Root { get; private set; }

        protected bool Success { get; private set; }

        protected XmlWriter Target { get; private set; }
    }
}

