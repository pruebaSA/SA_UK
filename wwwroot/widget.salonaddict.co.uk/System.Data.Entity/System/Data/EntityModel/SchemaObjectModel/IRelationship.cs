namespace System.Data.EntityModel.SchemaObjectModel
{
    using System;
    using System.Collections.Generic;
    using System.Data.Objects.DataClasses;
    using System.Runtime.InteropServices;

    internal interface IRelationship
    {
        bool TryGetEnd(string roleName, out IRelationshipEnd end);

        IList<ReferentialConstraint> Constraints { get; }

        IList<IRelationshipEnd> Ends { get; }

        string FQName { get; }

        string Name { get; }

        System.Data.Objects.DataClasses.RelationshipKind RelationshipKind { get; }
    }
}

