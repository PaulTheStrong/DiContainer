using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using DIContainer.Configuration;

namespace DIContainer
{
    public class BeanFactory : IBeanFactory
    { 
        private DependencyConfiguration Configuration;
        private Dictionary<string, BeanDefinition> BeanNamesToDefinitions = new Dictionary<string, BeanDefinition>();

        public BeanFactory(DependencyConfiguration configuration)
        {
            Configuration = configuration;
            var beanDefinitions = configuration.BeanDefintions;
            foreach (var (type, beanDefinitionList) in beanDefinitions)
            {
                foreach (BeanDefinition beanDefinition in beanDefinitionList)
                {
                    var beanType = beanDefinition.Type;
                    if (!CheckCompatibility(type, beanType))
                    {
                        throw new BeanDefinitionTypeMismatchException();
                    }
                    if (!BeanNamesToDefinitions.ContainsKey(beanDefinition.Name))
                    {
                        BeanNamesToDefinitions.Add(beanDefinition.Name, beanDefinition);
                    }
                    
                }
            }

            foreach (var (type, beanDefinitionList) in beanDefinitions)
            {
                foreach (BeanDefinition beanDefinition in beanDefinitionList)
                {
                    IScope scope = beanDefinition.Scope;
                    if (scope.GetType() == typeof(Singleton))
                    {
                        scope.Get(beanDefinition.Name, this);
                    }
                }
            }
        }

        private bool CheckCompatibility(Type type, Type beanDefinitionType)
        {
            Type[] genericArguments = type.GetGenericArguments();
            if (genericArguments.Length != 0)
            {
                var interfaceTypes = beanDefinitionType.GetInterfaces();

                foreach (var it in interfaceTypes)
                {
                    if (it.IsGenericType && it.GetGenericTypeDefinition() == type)
                        return true;
                }

                if (beanDefinitionType.IsGenericType && beanDefinitionType.GetGenericTypeDefinition() == type)
                    return true;

                Type baseType = beanDefinitionType.BaseType;
                if (baseType == null) return false;

                return CheckCompatibility(baseType, type);
            }
            return beanDefinitionType.IsSubclassOf(type) 
                   || type == beanDefinitionType 
                   || type.IsAssignableFrom(beanDefinitionType);
        }

        public object CreateBean(BeanDefinition beanDefinition)
        {
            IEnumerable<Type> beanDefinitionDependencies = beanDefinition.Dependencies;
            List<object?> dependencies = new List<object?>();
            Dictionary<Type,List<BeanDefinition>> configurationBeanDefintions = Configuration.BeanDefintions;
            foreach (var beanDefinitionDependency in beanDefinitionDependencies)
            {
                object? dependency;
                if (!configurationBeanDefintions.ContainsKey(beanDefinitionDependency))
                {
                    dependency = null;
                }
                else
                {
                    List<BeanDefinition> possibleDependencyBeanDefinitions = 
                        configurationBeanDefintions[beanDefinitionDependency];
                    if (possibleDependencyBeanDefinitions.Count > 1)
                    {
                        throw new MultipleImplementationException();
                    }

                    BeanDefinition definition = possibleDependencyBeanDefinitions[0];
                    dependency = definition.Scope.Get(definition.Name, this);
                }

                dependencies.Add(dependency);
            }

            var instance = Activator.CreateInstance(beanDefinition.Type, dependencies.ToArray());
            return instance;
        }

        public object GetBean(Type t)
        {
            if (t.Name == typeof(IEnumerable<>).Name)
            {
                Type generic = t.GenericTypeArguments[0];
                MethodInfo genericMethod = this.GetType().GetMethod("GetAllBeansOfType", BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(generic);
                return genericMethod.Invoke(this, null);
            }
            List<BeanDefinition> bds = Configuration.BeanDefintions[t];
            if (bds.Count > 1)
            {
                throw new MultipleImplementationException();
            }
            var beanDefinition = bds[0];
            return beanDefinition.Scope.Get(beanDefinition.Name, this);
        }

        public object GetBean(Type t, string name)
        {
            BeanDefinition beanDefinition = Configuration.BeanDefintions[t].Find(bd => bd.Alias.Equals(name));
            return beanDefinition.Scope.Get(beanDefinition.Name, this);
        }

        private IEnumerable<T> GetAllBeansOfType<T>()
        {
            var t = typeof(T);
            var beanDefinitions = Configuration.BeanDefintions[t];
            List<T> beans = new List<T>();
            foreach (BeanDefinition beanDefinition in beanDefinitions)
            {
                beans.Add((T)beanDefinition.Scope.Get(beanDefinition.Name, this));
            }
            return beans;
        }
        
        public BeanDefinition GetBeanDefinitionByBeanName(string beanName)
        {
            return BeanNamesToDefinitions[beanName];
        }
    }

    public class MultipleImplementationException : Exception
    {
    }

    public class BeanDefinitionTypeMismatchException : Exception
    {
    }
}