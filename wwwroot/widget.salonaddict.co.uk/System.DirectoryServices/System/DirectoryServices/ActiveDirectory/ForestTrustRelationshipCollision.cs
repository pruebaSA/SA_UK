namespace System.DirectoryServices.ActiveDirectory
{
    using System;

    public class ForestTrustRelationshipCollision
    {
        private DomainCollisionOptions domainFlag;
        private string record;
        private TopLevelNameCollisionOptions tlnFlag;
        private ForestTrustCollisionType type;

        internal ForestTrustRelationshipCollision(ForestTrustCollisionType collisionType, TopLevelNameCollisionOptions TLNFlag, DomainCollisionOptions domainFlag, string record)
        {
            this.type = collisionType;
            this.tlnFlag = TLNFlag;
            this.domainFlag = domainFlag;
            this.record = record;
        }

        public string CollisionRecord =>
            this.record;

        public ForestTrustCollisionType CollisionType =>
            this.type;

        public DomainCollisionOptions DomainCollisionOption =>
            this.domainFlag;

        public TopLevelNameCollisionOptions TopLevelNameCollisionOption =>
            this.tlnFlag;
    }
}

