using System;
using System.Collections.Generic;

namespace DIContainer.Configuration
{
    public class BeanDefinition
    {
        public IEnumerable<Type> Dependencies { get; private set; }
        public IScope Scope { get; private set; }
        public String Name { get; private set; }
        public Type Type { get; private set; }
        
        public String Alias { get; private set; }

        public BeanDefinition(IEnumerable<Type> dependencies, IScope scope, string name, Type type, String alias)
        {
            Dependencies = dependencies;
            Scope = scope;
            Name = name;
            Alias = alias;
            Type = type;
        }
        public BeanDefinition(IEnumerable<Type> dependencies, IScope scope, string name, Type type)
        {
            Dependencies = dependencies;
            Scope = scope;
            Name = name;
            Alias = name;
            Type = type;
        }
    }
}