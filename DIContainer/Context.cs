using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using DIContainer.Configuration;

namespace DIContainer
{
    public class Context : IContext
    {
        private IBeanFactory _beanFactory;

        public Context(DependencyConfiguration dependencyConfiguration)
        {
            _beanFactory = new BeanFactory(dependencyConfiguration);
        }

        public T Resolve<T>()
        {
            var type = typeof(T);
            return (T) _beanFactory.GetBean(type);
        }

        public T Resolve<T>(string name)
        {
            var type = typeof(T);
            return (T) _beanFactory.GetBean(type, name);
        }
    }
}