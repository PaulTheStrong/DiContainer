namespace DIContainer
{
    public interface IScope
    {
        object Get(string beanName, IBeanFactory beanFactory);
    }
}