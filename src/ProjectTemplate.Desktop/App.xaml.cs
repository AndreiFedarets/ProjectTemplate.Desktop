using System.Windows;

namespace ProjectTemplate.Desktop
{
    public partial class App : Application
    {
        private readonly Bootstrapper _bootstrapper;

        public App()
        {
            _bootstrapper = new Bootstrapper();
            _bootstrapper.Initialize();
        }
    }
}
