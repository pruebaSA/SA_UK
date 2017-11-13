namespace System.Web.UI.Design
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel.Design;
    using System.Runtime.Serialization;

    [Serializable]
    public class ExtenderControlToolboxItem : WebControlToolboxItem
    {
        public ExtenderControlToolboxItem()
        {
        }

        public ExtenderControlToolboxItem(Type type) : base(type)
        {
        }

        protected ExtenderControlToolboxItem(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public ReadOnlyCollection<Type> GetTargetControlTypes(IDesignerHost host)
        {
            ReadOnlyCollection<Type> onlys = (ReadOnlyCollection<Type>) base.Properties["TargetControlTypes"];
            if (onlys == null)
            {
                onlys = ExtenderControlDesigner.ExtractTargetControlTypes(base.GetToolType(host));
                base.Properties["TargetControlTypes"] = onlys;
            }
            return onlys;
        }

        public override void Initialize(Type type)
        {
            base.Initialize(type);
            base.Properties["TargetControlTypes"] = ExtenderControlDesigner.ExtractTargetControlTypes(type);
        }
    }
}

