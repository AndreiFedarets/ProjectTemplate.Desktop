using System;
using System.Windows;
using System.Windows.Controls;

namespace ProjectTemplate.Desktop
{
    internal sealed class WindowManager : Caliburn.Micro.WindowManager, IWindowManager
    {
        public void ShowError(string message, Exception exception = null)
        {
            string compositeMessage = message;
            if (exception != null)
            {
                string exceptionMessage = exception.GetDisplayMessage();
                if (!string.IsNullOrEmpty(message))
                {
                    compositeMessage += $": {Environment.NewLine}";
                }
                compositeMessage += exceptionMessage;
            }
            ShowMessageBox(compositeMessage, MessageBoxImage.Error, MessageBoxButton.OK);
        }


        public void ShowInformation(string message)
        {
            ShowMessageBox(message, MessageBoxImage.Information, MessageBoxButton.OK);
        }

        public DialogResult<MessageBoxResult> ShowQuestion(string message)
        {
            MessageBoxResult result = ShowMessageBox(message, MessageBoxImage.Question, MessageBoxButton.YesNo);
            return new DialogResult<MessageBoxResult>(true, result);
        }

        protected override Window EnsureWindow(object model, object viewObject, bool isDialog)
        {
            Window window = base.EnsureWindow(model, viewObject, isDialog);
            window.SizeToContent = SizeToContent.Manual;
            CloneSize(window, viewObject);
            return window;
        }

        private MessageBoxResult ShowMessageBox(string message, MessageBoxImage image, MessageBoxButton button)
        {
            MessageBoxResult result = MessageBox.Show(message, string.Empty, button, image);
            return result;
        }

        private void CloneSize(Window window, object viewObject)
        {
            Controls.View view = viewObject as Controls.View;
            if (view == null)
            {
                return;
            }
            if (view.WindowStyle == WindowStyle.ToolWindow)
            {
                window.SourceInitialized += OnToolWindowSourceInitialized;
            }
            window.WindowState = view.WindowState;
            window.SizeToContent = view.SizeToContent;
            if (window.WindowState != WindowState.Maximized)
            {
                Thickness padding = (Thickness)view.GetValue(Control.PaddingProperty);
                Thickness margin = view.Margin;
                double extraHeight = margin.Top + margin.Bottom + padding.Top + padding.Bottom + SystemParameters.WindowCaptionHeight + SystemParameters.ResizeFrameHorizontalBorderHeight * 2;
                double extraWidth = margin.Left + margin.Right + padding.Left + padding.Right + SystemParameters.ResizeFrameVerticalBorderWidth * 2;
                if (!double.IsNaN(view.MinHeight) && !double.IsInfinity(view.MinHeight))
                {
                    window.MinHeight = view.MinHeight + extraHeight;
                    window.Height = window.MinHeight;
                }
                if (!double.IsNaN(view.MinWidth) && !double.IsInfinity(view.MinWidth))
                {
                    window.MinWidth = view.MinWidth + extraWidth;
                    window.Width = window.MinWidth;
                }
                if (!double.IsNaN(view.MaxHeight) && !double.IsInfinity(view.MaxHeight))
                {
                    window.MaxHeight = view.MaxHeight = extraHeight;
                }
                if (!double.IsNaN(view.MaxWidth) && !double.IsInfinity(view.MaxWidth))
                {
                    window.MaxWidth = view.MaxWidth + extraWidth;
                }
                if (!double.IsNaN(view.Height) && !double.IsInfinity(view.Height))
                {
                    window.Height = view.Height + extraHeight;
                    view.Height = double.NaN;
                }
                if (!double.IsNaN(view.Width) && !double.IsInfinity(view.Width))
                {
                    window.Width = view.Width + extraWidth;
                    view.Width = double.NaN;
                }
            }
        }

        private void OnToolWindowSourceInitialized(object sender, EventArgs e)
        {
            if (!(sender is Window))
            {
                return;
            }
            Window window = (Window)sender;
            window.SourceInitialized -= OnToolWindowSourceInitialized;
            Win32.DisableMinimizeMaximizeButtons(window);
        }
    }
}
