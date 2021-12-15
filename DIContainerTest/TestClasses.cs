namespace DIContainerTest
{
    public class A
    {
        public B B { get; private set; }
        public C C { get; private set; }

        public A(B b, C c)
        {
            B = b;
            C = c;
        }
    }

    public class B
    {
        public C C { get; private set; }

        public B(C c)
        {
            C = c;
        }
    }

    public class C
    {
        public int i { get; private set; }
    }

    public interface IService
    {
        
    }

    public class Service : IService
    {
        
    }

    public class ServiceImpl1 : IService {}
    public class ServiceImpl2 : IService {}
    
    public interface IController {}

    public class Controller : IController
    {
        public IService Service;
        public Controller(IService service) {}
    }

    public interface IGenericService<TRepository> where TRepository : IRepository {}
    public interface IRepository {}
    
    public class RepositoryImpl : IRepository {}

    public class GenericService<TRepository> : IGenericService<TRepository> where TRepository : IRepository
    {
        public IRepository Repository { get; private set; }

        public GenericService(TRepository repository)
        {
            Repository = repository;
        }
    }
}