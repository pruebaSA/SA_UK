namespace Microsoft.Practices.Unity.Utility
{
    using System;

    public static class Pair
    {
        public static Pair<TFirstParameter, TSecondParameter> Make<TFirstParameter, TSecondParameter>(TFirstParameter first, TSecondParameter second) => 
            new Pair<TFirstParameter, TSecondParameter>(first, second);
    }
}

