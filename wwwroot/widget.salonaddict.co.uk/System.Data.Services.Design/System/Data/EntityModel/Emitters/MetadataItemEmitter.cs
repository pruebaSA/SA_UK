namespace System.Data.EntityModel.Emitters
{
    using System;
    using System.CodeDom;
    using System.Data.EntityModel;
    using System.Data.Metadata.Edm;
    using System.Reflection;

    internal abstract class MetadataItemEmitter : Emitter
    {
        private MetadataItem _item;
        private const string CodeGenerationValueAccessibilityInternal = "Internal";
        private const string CodeGenerationValueAccessibilityPrivate = "Private";
        private const string CodeGenerationValueAccessibilityProtected = "Protected";
        private const string CodeGenerationValueAccessibilityPublic = "Public";

        protected MetadataItemEmitter(ClientApiGenerator generator, MetadataItem item) : base(generator)
        {
            this._item = item;
        }

        protected static MemberAttributes AccessibilityFromGettersAndSetters(EdmMember property)
        {
            MemberAttributes @private = MemberAttributes.Private;
            MemberAttributes getterAccessibility = GetGetterAccessibility(property);
            if (IsLeftMoreAccessableThanRight(getterAccessibility, @private))
            {
                @private = getterAccessibility;
            }
            MemberAttributes setterAccessibility = GetSetterAccessibility(property);
            if (IsLeftMoreAccessableThanRight(setterAccessibility, @private))
            {
                @private = setterAccessibility;
            }
            return @private;
        }

        protected static string GetAccessibilityCsdlStringFromMemberAttribute(MemberAttributes attribute)
        {
            MemberAttributes attributes = attribute;
            if (attributes != MemberAttributes.Assembly)
            {
                if (attributes == MemberAttributes.Family)
                {
                    return "Protected";
                }
                if (attributes == MemberAttributes.Private)
                {
                    return "Private";
                }
                return "Public";
            }
            return "Internal";
        }

        protected static int GetAccessibilityRank(MemberAttributes accessibility)
        {
            MemberAttributes attributes = accessibility;
            if (attributes != MemberAttributes.Assembly)
            {
                if (attributes != MemberAttributes.Family)
                {
                    if (attributes == MemberAttributes.Public)
                    {
                        return 0;
                    }
                    return 3;
                }
                return 2;
            }
            return 1;
        }

        private static MemberAttributes GetAccessibilityValue(MetadataItem item, string attribute)
        {
            MetadataProperty property;
            MemberAttributes @public = MemberAttributes.Public;
            if (item.MetadataProperties.TryGetValue(Utils.GetFullyQualifiedCodeGenerationAttributeName(attribute), false, out property))
            {
                @public = GetCodeAccessibilityMemberAttribute(property.Value.ToString());
            }
            return @public;
        }

        private static MemberAttributes GetCodeAccessibilityMemberAttribute(string accessibility)
        {
            switch (accessibility)
            {
                case "Internal":
                    return MemberAttributes.Assembly;

                case "Private":
                    return MemberAttributes.Private;

                case "Protected":
                    return MemberAttributes.Family;
            }
            return MemberAttributes.Public;
        }

        private static TypeAttributes GetCodeAccessibilityTypeAttribute(string accessibility)
        {
            if ((accessibility != "Internal") && (accessibility != "Protected"))
            {
                return TypeAttributes.Public;
            }
            return TypeAttributes.AnsiClass;
        }

        protected static MemberAttributes GetEntitySetPropertyAccessibility(EntitySet item) => 
            GetAccessibilityValue(item, "GetterAccess");

        protected static MemberAttributes GetEntityTypeAccessibility(EntityType item) => 
            GetAccessibilityValue(item, "TypeAccess");

        protected static MemberAttributes GetGetterAccessibility(EdmMember item) => 
            GetAccessibilityValue(item, "GetterAccess");

        protected static MemberAttributes GetSetterAccessibility(EdmMember item) => 
            GetAccessibilityValue(item, "SetterAccess");

        protected static TypeAttributes GetTypeAccessibilityValue(MetadataItem item)
        {
            MetadataProperty property;
            TypeAttributes @public = TypeAttributes.Public;
            if (item.MetadataProperties.TryGetValue(Utils.GetFullyQualifiedCodeGenerationAttributeName("TypeAccess"), false, out property))
            {
                @public = GetCodeAccessibilityTypeAttribute(property.Value.ToString());
            }
            return @public;
        }

        private static bool IsLeftMoreAccessableThanRight(MemberAttributes left, MemberAttributes right) => 
            (GetAccessibilityRank(left) < GetAccessibilityRank(right));

        protected abstract void Validate();

        protected MetadataItem Item =>
            this._item;
    }
}

