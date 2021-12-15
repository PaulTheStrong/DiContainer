using System.Collections.Generic;

namespace DIContainer
{
    public interface IContext
    {
        T Resolve<T>();
    }
}