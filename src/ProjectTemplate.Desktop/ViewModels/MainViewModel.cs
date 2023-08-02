using System.Threading.Tasks;

namespace ProjectTemplate.Desktop.ViewModels
{
    public class MainViewModel : ViewModel
    {
        private readonly IViewModelManager _viewModelManager;

        public MainViewModel(IViewModelManager viewModelManager)
        {
            _viewModelManager = viewModelManager;
        }

        public DocumentCollectionViewModel Documents
        {
            get { return _viewModelManager.Documents; }
        }

        public async Task OpenTestAsync()
        {
            await _viewModelManager.OpenTestAsync();
        }
    }
}
