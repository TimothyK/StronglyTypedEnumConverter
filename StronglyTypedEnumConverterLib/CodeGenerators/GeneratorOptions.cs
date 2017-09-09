
namespace StronglyTypedEnumConverter
{
    public class GeneratorOptions
    {
        public AdditionPriority AdditionPriority { get; set; } = AdditionPriority.Members;

        public LanguageVersion LanguageVersion { get; set; } = LanguageVersion.Max;
    }

    public enum AdditionPriority
    {
        Members,
        Properties
    }

    
}
