using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StronglyTypedEnumConverter
{
    [TestClass]
    public class PropertyEnumTests5 : BasicEnumTests
    {

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            var options = new GeneratorOptions
            {
                AdditionPriority = AdditionPriority.Properties,
                LanguageVersion = LanguageVersion.CSharp50
            };
            BasicEnumTests.ClassInit(options);
        }

    }
    [TestClass]
    public class PropertyEnumTests6 : BasicEnumTests
    {

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            var options = new GeneratorOptions
            {
                AdditionPriority = AdditionPriority.Properties,
                LanguageVersion = LanguageVersion.CSharp60
            };
            BasicEnumTests.ClassInit(options);
        }

    }

    [TestClass]
    public class PropertyEnumTests7 : BasicEnumTests
    {

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            var options = new GeneratorOptions
            {
                AdditionPriority = AdditionPriority.Properties,
                LanguageVersion = LanguageVersion.CSharp70
            };
            BasicEnumTests.ClassInit(options);
        }

    }
}
