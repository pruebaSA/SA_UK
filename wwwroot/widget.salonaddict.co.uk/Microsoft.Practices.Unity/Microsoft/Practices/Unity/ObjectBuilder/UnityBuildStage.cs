namespace Microsoft.Practices.Unity.ObjectBuilder
{
    using System;

    public enum UnityBuildStage
    {
        Setup,
        TypeMapping,
        Lifetime,
        PreCreation,
        Creation,
        Initialization,
        PostInitialization
    }
}

