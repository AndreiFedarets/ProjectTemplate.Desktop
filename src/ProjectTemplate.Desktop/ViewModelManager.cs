using ProjectTemplate.Desktop.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Unity;

namespace ProjectTemplate.Desktop
{
    internal sealed class ViewModelManager : IViewModelManager
    {
        private readonly IWindowManager _windowManager;
        private readonly IUnityContainer _container;

        public ViewModelManager(IUnityContainer container, IWindowManager windowManager)
        {
            _container = container;
            _windowManager = windowManager;
            Documents = new DocumentCollectionViewModel();
        }

        public DocumentCollectionViewModel Documents { get; private set; }

        public TViewModel CreateViewModel<TViewModel>()
        {
            IUnityContainer container = _container.CreateChildContainer();
            return container.Resolve<TViewModel>();
        }

        public TViewModel CreateViewModel<TViewModel, TArg>(TArg arg)
        {
            IUnityContainer container = _container.CreateChildContainer();
            container.RegisterInstance<TArg>(arg);
            return container.Resolve<TViewModel>();
        }

        public TViewModel CreateViewModel<TViewModel, TArg1, TArg2>(TArg1 arg1, TArg2 arg2)
        {
            IUnityContainer container = _container.CreateChildContainer();
            container.RegisterInstance<TArg1>(arg1);
            container.RegisterInstance<TArg2>(arg2);
            return container.Resolve<TViewModel>();
        }

        private async Task ActiveViewModelAsync(ViewModel viewModel)
        {
            bool addBackground = Keyboard.GetKeyStates(Key.LeftShift) == KeyStates.Down || Keyboard.GetKeyStates(Key.RightShift) == KeyStates.Down;
            ViewModel activeViewModel = Documents.ActiveItem;
            await Documents.ActivateItemAsync(viewModel);
            if (addBackground && activeViewModel != null)
            {
                await Documents.ActivateItemAsync(activeViewModel);
            }
        }

        private T FindViewModel<T>(Func<T, bool> filter = null) where T : ViewModel
        {
            filter = filter == null ? (item) => true : filter;
            return Documents.Items.FirstOrDefault(x => x is T && filter((T)x)) as T;
        }

        private T FindViewModel<T>(CollectionViewModel<T> collection, Func<T, bool> filter = null) where T : ViewModel
        {
            filter = filter == null ? (item) => true : filter;
            return collection.Items.FirstOrDefault(x => x is T && filter((T)x)) as T;
        }

        public void ShowError(string message, Exception exception = null)
        {
            _windowManager.ShowError(message, exception);
        }

        public bool? ShowQuestion(string message)
        {
            var result = _windowManager.ShowQuestion(message);
            if (!result.Success)
            {
                return null;
            }
            return result.Value == System.Windows.MessageBoxResult.Yes;
        }
    }
}
