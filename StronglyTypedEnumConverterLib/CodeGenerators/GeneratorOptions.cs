namespace StronglyTypedEnumConverter
{
    public class GeneratorOptions
    {
        public AdditionPriority AdditionPriority = AdditionPriority.Members;
    }

    public enum AdditionPriority
    {
        Members,
        Properties
    }
}
