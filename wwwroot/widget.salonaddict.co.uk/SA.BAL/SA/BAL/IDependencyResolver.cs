namespace SA.BAL
{
    using System;
    using System.Collections.Generic;

    public interface IDependencyResolver
    {
        void Inject<T>(T existing);
        void Register<T>(T instance);
        T Resolve<T>();
        T Resolve<T>(string name);
        T Resolve<T>(Type type);
        T Resolve<T>(Type type, string name);
        IEnumerable<T> ResolveAll<T>();
    }
}

