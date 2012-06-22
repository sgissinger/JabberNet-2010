/* --------------------------------------------------------------------------
 * Copyrights
 *
 * Portions created by or assigned to Cursive Systems, Inc. are
 * Copyright (c) 2002-2008 Cursive Systems, Inc.  All Rights Reserved.  Contact
 * information for Cursive Systems, Inc. is available at
 * http://www.cursive.net/.
 *
 * License
 *
 * Jabber-Net is licensed under the LGPL.
 * See LICENSE.txt for details.
 * --------------------------------------------------------------------------*/

using Bedrock.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Bedrock.Collections
{
    /// <summary>
    /// Summary description for SetTest.
    /// </summary>
    [TestClass]
    public class SetTest
    {
        //[ExpectedException(typeof(ArgumentException))]
        [TestMethod]
        public void Test_Hashtable_Double_Add()
        {
            Set s = new Set(SetImplementation.Hashtable);
            Assert.AreEqual(0, s.Count);
            s.Add("one");
            Assert.AreEqual(1, s.Count);
            s.Add("one");
        }
        //[ExpectedException(typeof(ArgumentException))]
        [TestMethod]
        public void Test_SkipList_Double_Add()
        {
            Set s = new Set(SetImplementation.SkipList);
            Assert.AreEqual(0, s.Count);
            s.Add("one");
            Assert.AreEqual(1, s.Count);
            s.Add("one");
        }
        //[ExpectedException(typeof(ArgumentException))]
        [TestMethod]
        public void Test_Tree_Double_Add()
        {
            Set s = new Set(SetImplementation.Tree);
            Assert.AreEqual(0, s.Count);
            s.Add("one");
            Assert.AreEqual(1, s.Count);
            s.Add("one");
        }

        private void all(SetImplementation i)
        {
            Set s = new Set(i);
            Assert.AreEqual(0, s.Count);

            s.Add("one");
            Assert.AreEqual(1, s.Count);
            Assert.IsTrue(s.Contains("one"));
            Assert.IsTrue(!s.Contains("two"));
            Assert.IsTrue(!s.Contains("three"));

            s.Add("two");
            Assert.AreEqual(2, s.Count);
            Assert.IsTrue(s.Contains("one"));
            Assert.IsTrue(s.Contains("two"));
            Assert.IsTrue(!s.Contains("three"));

            s.Remove("one");
            Assert.AreEqual(1, s.Count);
            Assert.IsTrue(!s.Contains("one"));
            Assert.IsTrue(s.Contains("two"));
            Assert.IsTrue(!s.Contains("three"));

            s.Add("one");
            Assert.AreEqual(2, s.Count);
            Assert.IsTrue(s.Contains("one"));
            Assert.IsTrue(s.Contains("two"));
            Assert.IsTrue(!s.Contains("three"));

            s.Add("one");
            Assert.AreEqual(2, s.Count);
            Assert.IsTrue(s.Contains("one"));
            Assert.IsTrue(s.Contains("two"));
            Assert.IsTrue(!s.Contains("three"));

            int count = 0;
            foreach (string str in s)
            {
                count++;
                Assert.AreEqual(3, str.Length);
            }
            Assert.AreEqual(2, count);
        }

        [TestMethod]
        public void Test_Hashtable()
        {
            all(SetImplementation.Hashtable);
        }
        [TestMethod]
        public void Test_Skiplist()
        {
            all(SetImplementation.SkipList);
        }
        [TestMethod]
        public void Test_Tree()
        {
            all(SetImplementation.Tree);
        }
    }
}
