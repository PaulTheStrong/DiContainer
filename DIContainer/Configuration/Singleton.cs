using System;
using System.Collections.Generic;
using DIContainer.Configuration;

namespace DIContainer
{
    public class Singleton : IScope
    {
        private static Dictionary<String, Object> _objects = new Dictionary<string, object>();

        public object Get(string beanName, IBeanFactory beanFactory)
        {
            BeanDefinition beanDefinition = beanFactory.GetBeanDefinitionByBeanName(beanName);
            if (_objects.ContainsKey(beanName))
            {
                return _objects[beanName];
            }
            object bean = beanFactory.CreateBean(beanDefinition);
            _objects.Add(beanName, bean);
            return bean;
        }
    }
}