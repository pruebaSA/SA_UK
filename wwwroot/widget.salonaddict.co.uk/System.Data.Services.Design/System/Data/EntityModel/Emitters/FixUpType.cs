namespace System.Data.EntityModel.Emitters
{
    using System;

    internal enum FixUpType
    {
        MarkAbstractMethodAsPartial = 9,
        MarkClassAsStatic = 3,
        MarkOverrideMethodAsSealed = 1,
        MarkPropertyGetAsInternal = 5,
        MarkPropertyGetAsPrivate = 4,
        MarkPropertyGetAsProtected = 10,
        MarkPropertyGetAsPublic = 6,
        MarkPropertySetAsInternal = 2,
        MarkPropertySetAsPrivate = 7,
        MarkPropertySetAsProtected = 11,
        MarkPropertySetAsPublic = 8
    }
}

