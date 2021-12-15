using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DIContainer.Configuration
{
    public class DependencyConfiguration
    {
        public Dictionary<Type, List<BeanDefinition>> BeanDefintions { get; private set; }

        public DependencyConfiguration()
        {
            this.BeanDefintions = new Dictionary<Type, List<BeanDefinition>>();
        }
        
        public void Register<I, T>(IScope scope)
        {
            Type type = typeof(I);
            Type implType = typeof(T);
            Register(type, implType, scope, implType.Name);
        }

        public void Register<I, T>(IScope scope, string name)
        {
            Register(typeof(I), typeof(T), scope, name);
        }

        public void Register(Type basicType, Type implType, IScope scope)
        {
            Register(basicType, implType, scope, implType.Name);
        }

        public void Register(Type basicType, Type implType, IScope scope, string name)
        {
            ConstructorInfo[] constructors = implType.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            var maxParamConstructorInfo = constructors.MaxBy(info => info.GetParameters().Length);
            IEnumerable<Type> referenceDependencyTypes = maxParamConstructorInfo
                .GetParameters()
                .Where(param => !param.GetType().IsPrimitive)
                .Select(param => param.ParameterType);
            var beanDefinition = new BeanDefinition(referenceDependencyTypes, scope, name, implType);
            if (!BeanDefintions.ContainsKey(basicType))
            {
                BeanDefintions.Add(basicType, new List<BeanDefinition>());
            }
            BeanDefintions[basicType].Add(beanDefinition);
        }
    }
}