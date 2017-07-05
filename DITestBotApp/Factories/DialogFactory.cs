using Autofac;

namespace DITestBotApp.Factories
{
    public interface IDialogFactory
    {
        T Create<T>();
    }

    public class DialogFactory : IDialogFactory
    {
        private IComponentContext Scope { get; }

        public DialogFactory(IComponentContext scope)
        {
            this.Scope = scope;
        }

        public T Create<T>()
        {
            return this.Scope.Resolve<T>();
        }
    }
}