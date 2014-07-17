using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StronglyTypedEnumConverter
{
    [TestClass]
    public class CowboyTypeTests
    {
        [TestMethod]
        public void CowboyType_FromInvalidString_ThrowArgRange()
        {
            try
            {
                CowboyType.FromString("Jolly");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.WriteLine(ex.ToString());
                return;
            }

            Assert.Fail("Expected exception did not occur");
            
        }
        
        [TestMethod]
        public void CowboyType_FromNullString_ThrowArgNull()
        {
            try
            {
                CowboyType.FromString(null);
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine(ex.ToString());
                return;
            }

            Assert.Fail("Expected exception did not occur");
            
        }

        [TestMethod]
        public void CowboyType_All_ReturnsAllThreeValues()
        {
            var types = CowboyType.All().ToArray();

            Assert.IsTrue(types.Contains(CowboyType.Good));
            Assert.IsTrue(types.Contains(CowboyType.Bad));
            Assert.IsTrue(types.Contains(CowboyType.Ugly));
        }

        [TestMethod]
        public void CowboyType_StringValuesRoundTrip_ReturnsOriginalValue()
        {
            foreach (var type in CowboyType.All())
                Assert.AreSame(type, CowboyType.FromString(type.ToString()), type + " did not round trip successfully");
        }

        [TestMethod]
        public void CowboyType_CastToInt_ReturnsExpected()
        {
            Assert.AreEqual(0, (int) CowboyType.Good);
            Assert.AreEqual(1, (int) CowboyType.Bad);
            Assert.AreEqual(2, (int) CowboyType.Ugly);
        }
        
        [TestMethod]
        public void CowboyType_CastFromInt_ReturnsExpected()
        {
            Assert.AreEqual(CowboyType.Good, (CowboyType) 0);
            Assert.AreEqual(CowboyType.Bad, (CowboyType) 1);
            Assert.AreEqual(CowboyType.Ugly, (CowboyType) 2);
        }

        [TestMethod]
        public void CowboyType_FromInvalidInt_ThrowInvalidCast()
        {
            try
            {
                var dummy = (CowboyType) 4;
            }
            catch (InvalidCastException ex)
            {
                Console.WriteLine(ex.ToString());
                return;
            }

            Assert.Fail("Expected exception did not occur");

        }

    }
}
