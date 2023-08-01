using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ProjectTemplate.Desktop.Controls
{
    public abstract class View : UserControl
    {
        public static readonly DependencyProperty LoadingProperty;
        public static readonly DependencyProperty WindowStyleProperty;
        public static readonly DependencyProperty WindowStateProperty;
        public static readonly DependencyProperty SizeToContentProperty;

        static View()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(View), new FrameworkPropertyMetadata(typeof(View)));
            LoadingProperty = DependencyProperty.Register("Loading", typeof(bool), typeof(View), new PropertyMetadata(false));
            WindowStyleProperty = DependencyProperty.Register("WindowStyle", typeof(WindowStyle), typeof(View), new PropertyMetadata(WindowStyle.SingleBorderWindow));
            WindowStateProperty = DependencyProperty.Register("WindowState", typeof(WindowState), typeof(View), new PropertyMetadata(WindowState.Normal));
            SizeToContentProperty = DependencyProperty.Register("SizeToContent", typeof(SizeToContent), typeof(View), new PropertyMetadata(SizeToContent.Manual));
        }

        public View()
        {
            SetResourceReference(StyleProperty, "ViewStyle");
            IsTabStop = false;
            DataContextChanged += OnDataContextChanged;
            SnapsToDevicePixels = true;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Binding binding = new Binding(nameof(Loading)) { Source = DataContext };
            SetBinding(LoadingProperty, binding);
        }

        public bool Loading
        {
            get { return (bool)GetValue(LoadingProperty); }
            set { SetValue(LoadingProperty, value); }
        }

        public WindowStyle WindowStyle
        {
            get { return (WindowStyle)GetValue(WindowStyleProperty); }
            set { SetValue(WindowStyleProperty, value); }
        }

        public WindowState WindowState
        {
            get { return (WindowState)GetValue(WindowStateProperty); }
            set { SetValue(WindowStateProperty, value); }
        }

        public SizeToContent SizeToContent
        {
            get { return (SizeToContent)GetValue(SizeToContentProperty); }
            set { SetValue(SizeToContentProperty, value); }
        }
    }
}
