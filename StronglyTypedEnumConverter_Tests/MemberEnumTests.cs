using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StronglyTypedEnumConverter
{
    [TestClass]
    public class MemberEnumTests : BasicEnumTests
    {

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            BasicEnumTests.ClassInit(options => options.AdditionPriority = AdditionPriority.Members);
        }

    }
}
