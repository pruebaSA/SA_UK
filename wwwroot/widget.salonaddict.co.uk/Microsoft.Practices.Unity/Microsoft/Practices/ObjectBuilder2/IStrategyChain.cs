namespace Microsoft.Practices.ObjectBuilder2
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public interface IStrategyChain : IEnumerable<IBuilderStrategy>, IEnumerable
    {
        object ExecuteBuildUp(IBuilderContext context);
        void ExecuteTearDown(IBuilderContext context);
        IStrategyChain Reverse();
    }
}

