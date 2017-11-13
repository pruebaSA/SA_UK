namespace System.Data.Objects.DataClasses
{
    using System;

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class EdmRelationshipNavigationPropertyAttribute : EdmPropertyAttribute
    {
        private string _relationshipName;
        private string _relationshipNamespaceName;
        private string _targetRoleName;

        public EdmRelationshipNavigationPropertyAttribute(string relationshipNamespaceName, string relationshipName, string targetRoleName)
        {
            this._relationshipNamespaceName = relationshipNamespaceName;
            this._relationshipName = relationshipName;
            this._targetRoleName = targetRoleName;
        }

        public string RelationshipName =>
            this._relationshipName;

        public string RelationshipNamespaceName =>
            this._relationshipNamespaceName;

        public string TargetRoleName =>
            this._targetRoleName;
    }
}

