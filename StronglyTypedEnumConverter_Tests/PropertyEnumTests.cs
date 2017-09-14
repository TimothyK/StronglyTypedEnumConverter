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
                LanguageVersion = LanguageVersion.CSharp5,
                ImplementComparable = true,
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
                LanguageVersion = LanguageVersion.CSharp6,
                ImplementComparable = true,
            };
            BasicEnumTests.ClassInit(options);
        }

    }

}
