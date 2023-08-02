using System.Windows.Input;

namespace ProjectTemplate.Desktop.Views
{
    public partial class MainView
    {
        public MainView()
        {
            InitializeComponent();
        }

        private void OnTabItemPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle && e.ButtonState == MouseButtonState.Pressed)
            {
                System.Windows.Controls.TabItem tabItem = sender as System.Windows.Controls.TabItem;
                ViewModels.ViewModel viewModel = tabItem?.Content as ViewModels.ViewModel;
                viewModel?.Close();
                e.Handled = true;
            }
        }
    }
}
