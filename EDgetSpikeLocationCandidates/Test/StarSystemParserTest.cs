using System.Linq;
using EDgetSpikeLocationCandidates.Parsers;

namespace EDgetSpikeLocationCandidates.Test
{
    public class StarSystemParserTest
    {
        /*
        [TestCase("Plimbeau ZE-R e4-2732 1 c", "Plimbeau ZE-R e4-2732", "1 c")]
        [TestCase("Byua Aeb AA-A h0 A", "Byua Aeb AA-A h0", "A")]
        [TestCase("Iowholz AA-A h0 A", "Iowholz AA-A h0", "A")]
        [TestCase("Pleia Fleau AA-A h0 6", "Pleia Fleau AA-A h0", "6")]
        public void AssertThatOwnIdentifiersAreCalculatedCorrectly(string fullName, string systemName, string expected)
        {
            Assert.AreEqual(expected, StarSystemParser.GetOwnIdentier(systemName, fullName));
        }

        [TestCase("1 c", "1")]
        [TestCase("ABC 1 a", "ABC 1")]
        [TestCase("1", "")]
        public void AssertThatSingleParentIdentifiersAreCalculatedCorrectly(string input, string expected)
        {
            Assert.AreEqual(expected, StarSystemParser.GetParentIdentifiers(input).First());
        }

        [TestCase("ABC 1", new string[] {"A","B","C"})]
        public void AssertThatMultipleParentIdentifiersAreCalculatedCorrectly(string input, string[] expected)
        {
            var result = StarSystemParser.GetParentIdentifiers(input).ToArray();
            Assert.AreEqual(expected.Length, result.Length);
            for (int i = 0; i < result.Length; i++)
            {
                Assert.AreEqual(expected[i], result[i]);
            }
        }
        */
    }
}
