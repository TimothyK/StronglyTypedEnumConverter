using System.Collections.Generic;
using System.Linq;

namespace StronglyTypedEnumConverter
{
    class MainWindowDataContext : ObservableObject
    {
        public MainWindowDataContext()
        {
            AdditionPriority = AdditionPriority.Members;
            CurrentLanguageVersion = LanguageVersion.Max;
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

        public AdditionPriority AdditionPriority { get; set; }

        public GeneratorOptions GeneratorOptions => new GeneratorOptions
        {
            AdditionPriority = AdditionPriority,
            LanguageVersion = CurrentLanguageVersion
        };
    }
}
