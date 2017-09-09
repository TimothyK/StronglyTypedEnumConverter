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
                LanguageVersion = LanguageVersion.CSharp50
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
                LanguageVersion = LanguageVersion.CSharp60
            };
            BasicEnumTests.ClassInit(options);
        }

    }

    [TestClass]
    public class MemberEnumTests7 : BasicEnumTests
    {

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            var options = new GeneratorOptions
            {
                AdditionPriority = AdditionPriority.Members,
                LanguageVersion = LanguageVersion.CSharp70
            };
            BasicEnumTests.ClassInit(options);
        }

    }
}
