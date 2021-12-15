using System;
using System.Collections.Generic;
using System.Linq;
using DIContainer;
using DIContainer.Configuration;
using NUnit.Framework;

namespace DIContainerTest
{

    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestDependenciesAreSingletons()
        {
            DependencyConfiguration configuration = new DependencyConfiguration();
            IScope singleton = new Singleton();
            configuration.Register<A, A>(singleton);
            configuration.Register<B, B>(singleton);
            configuration.Register<C, C>(singleton);
            Context context = new Context(configuration);
            A a = context.Resolve<A>();
            B b = context.Resolve<B>();
            C c = context.Resolve<C>();
            Assert.AreEqual(a.C, c);
            Assert.AreEqual(b.C, c);
            Assert.AreEqual(a.B, b);
        }
        
        [Test]
        public void TestInterfaces()
        {
            DependencyConfiguration configuration = new DependencyConfiguration();
            IScope singleton = new Singleton();
            configuration.Register<IService, Service>(singleton);
            configuration.Register<IController, Controller>(singleton);
            Context context = new Context(configuration);
            IService service = context.Resolve<IService>();
            IController controller = context.Resolve<IController>();
            Assert.AreEqual(service.GetType(), typeof(Service));
            Assert.AreEqual(controller.GetType(), typeof(Controller));
        }
        
        [Test]
        public void TestPrototypesNotEqual()
        {
            DependencyConfiguration configuration = new DependencyConfiguration();
            IScope prototype = new Prototype();
            configuration.Register<IService, Service>(prototype);
            Context context = new Context(configuration);
            IService service1 = context.Resolve<IService>();
            IService service2 = context.Resolve<IService>();
            Assert.AreNotEqual(service1, service2);
        }

        [Test]
        public void TestAllImplementationsReturned()
        {
            DependencyConfiguration configuration = new DependencyConfiguration();
            IScope singleton = new Singleton();
            configuration.Register<IService, ServiceImpl1>(singleton);
            configuration.Register<IService, ServiceImpl2>(singleton);
            configuration.Register(typeof(IService), typeof(Service), singleton);
            Context context = new Context(configuration);
            IEnumerable<IService> services = context.Resolve<IEnumerable<IService>>();
            Assert.AreEqual(services.Count(), 3);
        }

        [Test]
        public void TestExceptionThrownWhenMultipleDependenciesInConfig()
        {
            DependencyConfiguration configuration = new DependencyConfiguration();
            IScope singleton = new Singleton();
            configuration.Register<IService, ServiceImpl1>(singleton);
            configuration.Register<IService, ServiceImpl2>(singleton);
            Context context = new Context(configuration);
            Assert.Throws<MultipleImplementationException>(() => context.Resolve<IService>());
        }

        [Test]
        public void TestGenericDependencyResolveWithOpenGeneric()
        {
            DependencyConfiguration configuration = new DependencyConfiguration();
            IScope singleton = new Singleton();
            configuration.Register<IGenericService<IRepository>, GenericService<IRepository>>(singleton);
            configuration.Register<IRepository, RepositoryImpl>(singleton);
            Context context = new Context(configuration);
            IRepository repository = context.Resolve<IRepository>();
            IGenericService<IRepository> genericService = context.Resolve<IGenericService<IRepository>>();
            Assert.NotNull(repository);
            Assert.AreEqual(repository, ((GenericService<IRepository>) genericService).Repository);
        }

        [Test]
        public void TestNamedDependencies()
        {
            DependencyConfiguration configuration = new DependencyConfiguration();
            IScope singleton = new Singleton();
            configuration.Register<IService, ServiceImpl1>(singleton, "Service 1");
            configuration.Register<IService, ServiceImpl2>(singleton, "Service 2");
            Context context = new Context(configuration);
            IService serivce1 = context.Resolve<IService>("Service 1");
            IService serivce2 = context.Resolve<IService>("Service 2");
            Assert.AreEqual(serivce1.GetType(), typeof(ServiceImpl1));
            Assert.AreEqual(serivce2.GetType(), typeof(ServiceImpl2));
        }

        [Test]
        public void TestSingletonScope()
        {
            IScope singleton = new Singleton();
            var mockFactory = new Moq.Mock<IBeanFactory>();
            string testBeanName = "bean";
            BeanDefinition beanDefinition = new BeanDefinition(null, singleton, testBeanName, typeof(Object));
            mockFactory.Setup(a => a.GetBeanDefinitionByBeanName(testBeanName)).Returns(beanDefinition);
            mockFactory.Setup(a => a.CreateBean(beanDefinition)).Returns(() => new Service());

            var o1 = singleton.Get("bean", mockFactory.Object);
            var o2 = singleton.Get("bean", mockFactory.Object);
            
            Assert.AreEqual(o1, o2);
        }
        
        [Test]
        public void TestPrototypeScope()
        {
            IScope prototype = new Prototype();
            var mockFactory = new Moq.Mock<IBeanFactory>();
            string testBeanName = "bean";
            BeanDefinition beanDefinition = new BeanDefinition(null, prototype, testBeanName, typeof(Object));
            mockFactory.Setup(a => a.GetBeanDefinitionByBeanName(testBeanName)).Returns(beanDefinition);
            mockFactory.Setup(a => a.CreateBean(beanDefinition)).Returns(() => new Service());

            var o1 = prototype.Get("bean", mockFactory.Object);
            var o2 = prototype.Get("bean", mockFactory.Object);
            
            Assert.AreNotEqual(o1, o2);
        }

        [Test]
        public void TestGenericDependencyResolveWithOpenGenerics()
        {
            DependencyConfiguration configuration = new DependencyConfiguration();
            IScope singleton = new Singleton();
            configuration.Register(typeof(IGenericService<IRepository>), typeof(GenericService<IRepository>), singleton);
            configuration.Register(typeof(IRepository), typeof(RepositoryImpl), singleton);
            Context context = new Context(configuration);
            IRepository repository = context.Resolve<IRepository>();
            IGenericService<IRepository> genericService = context.Resolve<IGenericService<IRepository>>();
            Assert.NotNull(repository);
            Assert.AreEqual(repository, ((GenericService<IRepository>) genericService).Repository);
        }
    }
}