using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StronglyTypedEnumConverter
{
    [TestClass]
    public class MemberEnumTests5 : BasicEnumTests
    {

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            var options = new GeneratorOptions
            {
                AdditionPriority = AdditionPriority.Members,
                LanguageVersion = LanguageVersion.CSharp5,
                ImplementComparable = true,
            };
            BasicEnumTests.ClassInit(options);
        }

    }

    [TestClass]
    public class MemberEnumTests6 : BasicEnumTests
    {

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            var options = new GeneratorOptions
            {
                AdditionPriority = AdditionPriority.Members,
                LanguageVersion = LanguageVersion.CSharp6,
                ImplementComparable = true,
            };
            BasicEnumTests.ClassInit(options);
        }

    }

}
