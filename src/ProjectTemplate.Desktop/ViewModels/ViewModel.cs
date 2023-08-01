using Caliburn.Micro;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace ProjectTemplate.Desktop.ViewModels
{
    public abstract class ViewModel : Screen
    {
        private bool _loading;
        private string _loadingMessage;
        private int _progressStep;
        private int _progressAmount;
        private IBatchOperation _batchOperation;

        protected override void OnViewAttached(object view, object context)
        {
            base.OnViewAttached(view, context);
        }

        public override string DisplayName
        {
            get { return GetDisplayName(); }
            set { }
        }

        public bool Loading
        {
            get { return _loading; }
            private set
            {
                _loading = value;
                NotifyOfPropertyChange();
            }
        }

        public string LoadingMessage
        {
            get { return _loadingMessage; }
            private set
            {
                _loadingMessage = value;
                NotifyOfPropertyChange();
            }
        }

        public int ProgressStep
        {
            get { return _progressStep; }
            private set
            {
                _progressStep = value;
                NotifyOfPropertyChange();
            }
        }

        public int ProgressAmount
        {
            get { return _progressAmount; }
            private set
            {
                _progressAmount = value;
                NotifyOfPropertyChange();
                NotifyOfPropertyChange(nameof(DisplayProgress));
            }
        }

        public bool DisplayProgress
        {
            get { return _progressAmount > 0; }
        }

        public bool DisplayProgressCancel
        {
            get {  return Loading && _batchOperation != null; }
        }

        public IBatchOperation BatchOperation
        {
            get { return _batchOperation; }
            set 
            {
                _batchOperation = value;
                NotifyOfPropertyChange(() => DisplayProgressCancel);
            }
        }

        public void CancelProgress()
        {
            _batchOperation?.Cancel();
        }

        protected IDisposable BeginLoading(IBatchOperation batchOperation = null)
        {
            return new LoadingContext(this, Properties.Resources.Loading_Message, 0, batchOperation);
            //return BeginLoading(Properties.Resources.Loading_Message);
        }

        protected IDisposable BeginLoading(int progressAmount, IBatchOperation batchOperation = null)
        {
            return new LoadingContext(this, Properties.Resources.Loading_Message, progressAmount, batchOperation);
        }

        protected Task OnUIThreadAsync(Func<Task> action)
        {
            return Caliburn.Micro.Execute.OnUIThreadAsync(action);
        }

        protected void BeginOnUIThread(System.Action action)
        {
            Caliburn.Micro.Execute.BeginOnUIThread(action);
        }

        protected void UpdateLoadingMessage(string messageFormat, params string[] args)
        {
            if (Loading)
            {
                LoadingMessage = string.Format(messageFormat, args);
            }
        }

        protected void ReportStep()
        {
            if (Loading)
            {
                ProgressStep++;
            }
        }

        protected void ReportSteps(int progress)
        {
            if (Loading)
            {
                ProgressStep = progress;
            }
        }

        public virtual void Submit()
        {
            OnUIThread(async () => await TryCloseAsync(true));
        }

        public virtual void Close()
        {
            OnUIThread(async () => await TryCloseAsync(false));
        }

        public virtual Task SubmitAsync()
        {
            return TryCloseAsync(true);
        }

        public virtual Task CloseAsync()
        {
            return TryCloseAsync(false);
        }

        public override async Task TryCloseAsync(bool? dialogResult = null)
        {
            await base.TryCloseAsync(dialogResult);
            OnClosing();
        }

        protected virtual void OnClosing()
        {
            PropertyInfo[] properties = GetType().GetProperties();
            foreach (PropertyInfo property in properties)
            {
                if (typeof(ViewModel).IsAssignableFrom(property.PropertyType) ||
                    typeof(CollectionViewModel<>).IsAssignableFrom(property.PropertyType))
                {
                    dynamic viewModel = property.GetValue(this);
                    if (viewModel != null)
                    {
                        viewModel.Close();
                    }
                }
                //TODO: extend with collections
            }
        }

        protected virtual string GetDisplayName()
        {
            return ResourceProvider.Instance.GetViewModelTitle(GetType());
        }

        private class LoadingContext : IDisposable
        {
            private readonly ViewModel _viewModel;

            public LoadingContext(ViewModel viewModel, string message, int progressAmount, IBatchOperation batchOperation)
            {
                _viewModel = viewModel;
                _viewModel.Loading = true;
                _viewModel.LoadingMessage = message;
                _viewModel.ProgressAmount = progressAmount;
                _viewModel.ProgressStep = 0;
                _viewModel.BatchOperation = batchOperation;
            }

            public void Dispose()
            {
                _viewModel.Loading = false;
                _viewModel.LoadingMessage = string.Empty;
                _viewModel.ProgressAmount = 0;
                _viewModel.BatchOperation?.Cancel();
                _viewModel.BatchOperation = null;
            }
        }
    }
}
