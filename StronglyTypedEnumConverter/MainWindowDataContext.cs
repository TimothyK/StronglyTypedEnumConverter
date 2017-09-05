using System.Collections.Generic;
using System.Linq;

namespace StronglyTypedEnumConverter
{
    class MainWindowDataContext : ObservableObject
    {
        public MainWindowDataContext()
        {
            _currentLanguageVersion = LanguageVersion.All().OrderBy(x => (int) x).Last();
        }

        public List<LanguageVersion> LanguageVersions => 
            LanguageVersion.All()
            .OrderBy(ver => ver.ToString())
            .ToList();

        private LanguageVersion _currentLanguageVersion;

        public LanguageVersion CurrentLanguageVersion
        {
            get => _currentLanguageVersion; 
            set => SetProperty(ref _currentLanguageVersion, value);
        }
    }
}
