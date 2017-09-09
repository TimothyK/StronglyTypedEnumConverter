using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace StronglyTypedEnumConverter
{
    [TestClass]
    public class VersionTests
    {
        [TestMethod]
        public void Version_5LessThan6_True()
        {
            var lhs = LanguageVersion.CSharp50;
            var rhs = LanguageVersion.CSharp60;
            (lhs < rhs).ShouldBeTrue();
        }

        [TestMethod]
        public void Version_5LessThanOrEqual6_True()
        {
            var lhs = LanguageVersion.CSharp50;
            var rhs = LanguageVersion.CSharp60;
            (lhs <= rhs).ShouldBeTrue();
        }

        [TestMethod]
        public void Version_5GreaterThan6_False()
        {
            var lhs = LanguageVersion.CSharp50;
            var rhs = LanguageVersion.CSharp60;
            (lhs > rhs).ShouldBeFalse();
        }

        [TestMethod]
        public void Version_5GreaterThanOrEqual6_False()
        {
            var lhs = LanguageVersion.CSharp50;
            var rhs = LanguageVersion.CSharp60;
            (lhs >= rhs).ShouldBeFalse();
        }


        [TestMethod]
        public void Version_6LessThan6_False()
        {
            var lhs = LanguageVersion.CSharp60;
            var rhs = LanguageVersion.CSharp60;
            (lhs < rhs).ShouldBeFalse();
        }

        [TestMethod]
        public void Version_6LessThanOrEqual6_True()
        {
            var lhs = LanguageVersion.CSharp60;
            var rhs = LanguageVersion.CSharp60;
            (lhs <= rhs).ShouldBeTrue();
        }

        [TestMethod]
        public void Version_6GreaterThan6_False()
        {
            var lhs = LanguageVersion.CSharp60;
            var rhs = LanguageVersion.CSharp60;
            (lhs > rhs).ShouldBeFalse();
        }

        [TestMethod]
        public void Version_6GreaterThanOrEqual6_True()
        {
            var lhs = LanguageVersion.CSharp60;
            var rhs = LanguageVersion.CSharp60;
            (lhs >= rhs).ShouldBeTrue();
        }

    }
}
