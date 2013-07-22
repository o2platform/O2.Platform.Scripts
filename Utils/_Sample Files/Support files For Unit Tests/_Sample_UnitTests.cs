// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)

//O2Ref:nunit.framework.dll
using NUnit.Framework;

namespace O2.Core.XRules._UnitTests.CSharp_Tests
{
    [TestFixture]
    public class _Sample_UnitTests
    {
        [Test]
        public void test1()
        {
            Assert.That(true);
        }

        [Test]
        public void test2_willSucessed()
        {
            Assert.That(1==1);
        }

        [Test]
        public void test1_willFail()
        {
            Assert.Fail("This is a message");
        }
    }
}
