using System.IO;
using CodeTitans.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeTitans.UnitTests.Core
{
    [TestClass]
    public class BinaryTests
    {
        [TestMethod]
        public void ReadByByteFromNonEmptyArray()
        {
            var buffer = new byte[] { 0, 1, 2, 3, 4, 5 };
            IBinaryReader reader = BinaryHelper.CreateReader(buffer);

            Assert.IsNotNull(reader);

            for (int i = 0; i < 3; i++)
            {
                Assert.AreEqual((byte)i, reader.ReadByte());
            }
        }

        [TestMethod]
        [ExpectedException(typeof(EndOfStreamException))]
        public void ReadByByteTooMuchDataFromShortArray()
        {
            var buffer = new byte[] { 0, 1, 2 };
            IBinaryReader reader = BinaryHelper.CreateReader(buffer);

            Assert.IsNotNull(reader);

            for (int i = 0; i < 3; i++)
            {
                Assert.AreEqual((byte)i, reader.ReadByte());
            }

            // should trigger exception here
            reader.ReadByte();
            Assert.Fail("Expected exception before");
        }

        [TestMethod]
        [ExpectedException(typeof(EndOfStreamException))]
        public void ReadBytesDataFromShortArray()
        {
            var buffer = new byte[] { 0, 1, 2 };
            IBinaryReader reader = BinaryHelper.CreateReader(buffer);

            Assert.IsNotNull(reader);
            CollectionAssert.AreEqual(new byte[] { 0, 1 }, reader.ReadBytes(2));

            // should trigger exception here
            reader.ReadBytes(2);
            Assert.Fail("Expected exception before");
        }
    }
}
