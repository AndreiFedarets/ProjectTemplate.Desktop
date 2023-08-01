using System;

namespace ProjectTemplate.Desktop
{
    public interface IResourceProvider
    {
        string GetViewModelTitle(Type viewModelType);
    }
}
