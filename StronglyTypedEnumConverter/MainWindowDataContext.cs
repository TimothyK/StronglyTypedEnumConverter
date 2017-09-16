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
            DbValue = true;
            ImplementComparable = false;
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

        private bool _implementComparable;

        public bool ImplementComparable
        {
            get => _implementComparable;
            set => SetProperty(ref _implementComparable, value);
        }

        private bool _dbValue;

        public bool DbValue
        {
            get => _dbValue;
            set => SetProperty(ref _dbValue, value);
        }

        public GeneratorOptions GeneratorOptions => new GeneratorOptions
        {
            AdditionPriority = AdditionPriority,
            LanguageVersion = CurrentLanguageVersion,
            DbValue = DbValue,
            ImplementComparable = ImplementComparable,
        };
    }
}
