using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StronglyTypedEnumConverter
{
    [TestClass]
    public class MemberEnumTests6 : BasicEnumTests
    {

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            var options = new GeneratorOptions
            {
                AdditionPriority = AdditionPriority.Members,
                ImplementComparable = true,
            };
            BasicEnumTests.ClassInit(options);
        }

    }

}
