using System;
using System.Linq;
using System.Reflection;

namespace ProjectTemplate.Desktop
{
    internal sealed class ResourceProvider : IResourceProvider
    {
        private static Lazy<IResourceProvider> _instance;
        private readonly Lazy<Type> _resourcesType;

        static ResourceProvider()
        {
            _instance = new Lazy<IResourceProvider>(() => new ResourceProvider());
        }

        private ResourceProvider()
        {
            _resourcesType = new Lazy<Type>(GetResourcesType);
        }

        public static IResourceProvider Instance
        {
            get { return _instance.Value; }
        }

        public string GetViewModelTitle(Type viewModelType)
        {
            if (_resourcesType.Value == null)
            {
                return string.Empty;
            }
            string propertyName = GetViewModelTitlePropertyName(viewModelType);
            PropertyInfo propertyInfo = _resourcesType.Value?.GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Static);
            if (propertyInfo == null)
            {
                return string.Empty;
            }
            return propertyInfo.GetValue(null) as string ?? string.Empty;
        }

        private string GetViewModelTitlePropertyName(Type viewModelType)
        {
            string viewModelTypeName = viewModelType?.FullName;
            if (string.IsNullOrEmpty(viewModelTypeName))
            {
                return string.Empty;
            }
            const string trimWord = ".ViewModels.";
            viewModelTypeName = viewModelTypeName.Remove(0, viewModelTypeName.IndexOf(trimWord) + trimWord.Length);
            viewModelTypeName = viewModelTypeName.Replace("ViewModel", string.Empty).Replace(".", "_");
            string propertyName = $"View_{viewModelTypeName}_DisplayName";
            return propertyName;
        }

        private Type GetResourcesType()
        {
            Assembly assembly = Assembly.GetEntryAssembly();
            if (assembly == null)
            {
                return null;
            }
            return assembly.GetTypes().FirstOrDefault(x => string.Equals(x.Name, "Resources", StringComparison.Ordinal));
        }
    }
}
