using System;
using System.Windows;

namespace ProjectTemplate.Desktop
{
    public interface IWindowManager : Caliburn.Micro.IWindowManager
    {
        void ShowError(string message, Exception exception = null);

        void ShowInformation(string message);

        DialogResult<MessageBoxResult> ShowQuestion(string message);
    }
}
