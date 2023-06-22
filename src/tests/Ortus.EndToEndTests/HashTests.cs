using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ortus.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ortus.EndToEndTests
{
    [TestClass]
    public class HashTests
    {
        [TestMethod]
        public void computed_hash_matches_known_hash()
        {
            Assert.AreEqual( Hasher.ComputeNameHash( "d_01_switch" ), 0x2f756947u );
        }

        [TestMethod]
        public void computed_hash_matches_known_hash2()
        {
            Assert.AreEqual( Hasher.ComputeNameHash( "pl0010_a103.mot" ), 0x1D04B5CB );
        }
    }
}
