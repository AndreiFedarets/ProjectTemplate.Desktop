using System;
using ProjectTemplate.Desktop.ViewModels;

namespace ProjectTemplate.Desktop
{
    public interface IViewModelManager
    {
        DocumentCollectionViewModel Documents { get; }

        TViewModel CreateViewModel<TViewModel>();

        TViewModel CreateViewModel<TViewModel, TArg>(TArg arg);

        TViewModel CreateViewModel<TViewModel, TArg1, TArg2>(TArg1 arg1, TArg2 arg2);

        void ShowError(string message, Exception exception = null);

        bool? ShowQuestion(string message);
    }
}
