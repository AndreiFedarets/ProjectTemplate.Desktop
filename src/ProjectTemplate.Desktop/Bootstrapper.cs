using Caliburn.Micro;
using ProjectTemplate.Desktop.ViewModels;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using Unity;

namespace ProjectTemplate.Desktop
{
    internal class Bootstrapper : BootstrapperBase
    {
        private readonly IUnityContainer _container;

        public Bootstrapper()
        {
            _container = new UnityContainer();
        }

        protected override void Configure()
        {
            base.Configure();
            Configure(_container);
        }

        protected virtual void Configure(IUnityContainer container)
        {
            container.RegisterType<IViewModelManager, ViewModelManager>(TypeLifetime.ContainerControlled);
            IWindowManager windowManager = _container.Resolve<WindowManager>();
            container.RegisterInstance<Caliburn.Micro.IWindowManager>(windowManager);
            container.RegisterInstance<IWindowManager>(windowManager);
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            base.OnStartup(sender, e);
            DisplayRootViewForAsync<MainViewModel>();
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.ResolveAll(service);
        }

        protected override object GetInstance(Type service, string key)
        {
            return _container.Resolve(service, key);
        }

        protected override void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.ToString(), "UNHANDLED EXCEPTION", MessageBoxButton.OK, MessageBoxImage.Error);
            base.OnUnhandledException(sender, e);
        }

        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            List<Assembly> assemblies = new List<Assembly>(base.SelectAssemblies())
            {
                Assembly.GetExecutingAssembly()
            };
            return assemblies;
        }
    }
}
