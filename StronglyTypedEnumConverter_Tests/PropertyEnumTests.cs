using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StronglyTypedEnumConverter
{

    [TestClass]
    public class PropertyEnumTests6 : BasicEnumTests
    {

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            var options = new GeneratorOptions
            {
                AdditionPriority = AdditionPriority.Properties,
                ImplementComparable = true,
            };
            BasicEnumTests.ClassInit(options);
        }

    }

}
