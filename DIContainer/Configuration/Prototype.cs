using System;

namespace DIContainer.Configuration
{
    public class Prototype : IScope
    {
        public object Get(string beanName, IBeanFactory beanFactory)
        {
            var beanDefinition = beanFactory.GetBeanDefinitionByBeanName(beanName);
            return beanFactory.CreateBean(beanDefinition);
        }
    }
}