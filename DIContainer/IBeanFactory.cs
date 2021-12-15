using System;
using DIContainer.Configuration;

namespace DIContainer
{
    public interface IBeanFactory
    {
        object CreateBean(BeanDefinition beanDefinition);

        object GetBean(Type t);

        object GetBean(Type t, String name);
        BeanDefinition GetBeanDefinitionByBeanName(string beanName);
    }
}