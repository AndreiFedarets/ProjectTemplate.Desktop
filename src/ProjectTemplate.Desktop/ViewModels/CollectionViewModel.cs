using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ProjectTemplate.Desktop.ViewModels
{
    public class CollectionViewModel<T> : ViewModel 
        where T : ViewModel
    {
        protected readonly IViewModelManager ViewModelManager;
        protected readonly CollectionView CollectionView;
        private readonly ObservableCollection<T> _items;
        private IBatchOperation _currentBatchOperation;
        private IEnumerable _selection;

        public CollectionViewModel(IViewModelManager viewModelManager)
        {
            ViewModelManager = viewModelManager;
            _items = new ObservableCollection<T>();
            CollectionView = (CollectionView)CollectionViewSource.GetDefaultView(_items);
        }

        public int Count
        {
            get { return _items.Count; }
        }

        public IEnumerable Selection
        {
            get { return _selection; }
            set
            {
                _selection = value;
                NotifyOfPropertyChange();
            }
        }

        public IEnumerable<T> SelectedItems
        {
            get { return Selection == null ? Enumerable.Empty<T>() : Selection.OfType<T>(); }
        }

        public IEnumerable<T> Items
        {
            get { return _items; }
        }

        public virtual Task LoadAsync()
        {
            return Task.CompletedTask;
        }

        public void RefreshView()
        {
            CollectionView.Refresh();
        }

        public override void Close()
        {
            try
            {
                _currentBatchOperation?.Cancel();
            }
            catch
            {
            }
            base.Close();
        }

        protected void SetItems(IEnumerable<T> items)
        {
            OnUIThread(() =>
            {
                _items.Clear();
                foreach (T item in items)
                {
                    _items.Add(item);
                }
                CollectionView.Refresh();
            });
            NotifyOfPropertyChange(() => DisplayName);
        }

        protected void AddItem(T item)
        {
            OnUIThread(() =>
            {
                _items.Add(item);
                CollectionView.Refresh();
            });
            NotifyOfPropertyChange(() => DisplayName);
        }

        protected void AddItems(IEnumerable<T> items)
        {
            OnUIThread(() =>
            {
                foreach (T item in items)
                {
                    _items.Add(item);
                }
                CollectionView.Refresh();
            });
            NotifyOfPropertyChange(() => DisplayName);
        }

        protected void RemoveItem(T item)
        {
            OnUIThread(() =>
            {
                _items.Remove(item);
            });
            NotifyOfPropertyChange(() => DisplayName);
        }

        protected async Task RunBatchOperation(Func<Task<IBatchOperation>> runBatchOperation)
        {
            if (_currentBatchOperation != null)
            {
                return;
            }
            IBatchOperation batchOperation = await runBatchOperation();
            _currentBatchOperation = batchOperation;
            using (BeginLoading(batchOperation.TotalCount, batchOperation))
            {
                while (!batchOperation.IsCompleted)
                {
                    await Task.Delay(500);
                    string message = $"{Properties.Resources.Loading_Message} {batchOperation.CompletedCount}/{batchOperation.TotalCount}";
                    BeginOnUIThread(() =>
                    {
                        UpdateLoadingMessage(message);
                        ReportSteps(batchOperation.CompletedCount);
                    });
                }
            }
            _currentBatchOperation = null;
        }

        protected async Task<IEnumerable<D>> RunBatchOperation<D>(Func<Task<IBatchOperation<D>>> runBatchOperation)
        {
            if (_currentBatchOperation != null)
            {
                return Enumerable.Empty<D>();
            }
            IBatchOperation<D> batchOperation = await runBatchOperation();
            _currentBatchOperation = batchOperation;
            using (BeginLoading(batchOperation.TotalCount, batchOperation))
            {
                while (!batchOperation.IsCompleted)
                {
                    await Task.Delay(500);
                    string message = $"{Properties.Resources.Loading_Message} {batchOperation.CompletedCount}/{batchOperation.TotalCount}";
                    BeginOnUIThread(() =>
                    {
                        UpdateLoadingMessage(message);
                        ReportSteps(batchOperation.CompletedCount);
                    });
                }
                _currentBatchOperation = null;
                return batchOperation.Result;
            }
        }
    }
}
