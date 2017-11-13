namespace System.Data.EntityModel.SchemaObjectModel
{
    using System;
    using System.Collections.Generic;
    using System.Data.Metadata.Edm;

    internal interface IRelationshipEnd
    {
        RelationshipMultiplicity Multiplicity { get; }

        string Name { get; }

        ICollection<OnOperation> Operations { get; }

        SchemaEntityType Type { get; }
    }
}

