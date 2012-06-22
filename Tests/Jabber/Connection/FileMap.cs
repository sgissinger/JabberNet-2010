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
using System;

using System.Xml;

using Jabber.Connection;
using Jabber.Protocol;
using Jabber.Protocol.IQ;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Jabber.Connection
{
    [TestClass]
    public class FileMapTest
    {
        XmlDocument doc = new XmlDocument();

        DiscoInfo Element
        {
            get
            {
                XmlDocument doc = new XmlDocument();
                global::Jabber.Protocol.IQ.DiscoInfo di = new global::Jabber.Protocol.IQ.DiscoInfo(doc);
                di.AddFeature(global::Jabber.Protocol.URI.DISCO_INFO);
                di.AddFeature(global::Jabber.Protocol.URI.DISCO_ITEMS);
                return di;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestNull()
        {
            FileMap<Element> fm = new FileMap<Element>("test.xml", null);
            Assert.IsNotNull(fm);
            FileMap<DiscoInfo> fm2 = new FileMap<DiscoInfo>("test.xml", null);
        }

        [TestMethod]
        public void TestCreate()
        {
            ElementFactory ef = new ElementFactory();
            ef.AddType(new global::Jabber.Protocol.IQ.Factory());

            string g = new Guid().ToString();
            FileMap<DiscoInfo> fm = new FileMap<DiscoInfo>("test.xml", ef);
            fm.Clear();
            Assert.AreEqual(0, fm.Count);

            fm[g] = Element;
            Assert.IsTrue(fm.Contains(g));
            Assert.IsFalse(fm.Contains("foo"));
            Assert.IsInstanceOfType(fm[g], typeof(DiscoInfo));
            Assert.AreEqual(1, fm.Count);

            // re-read, to reparse
            fm = new FileMap<DiscoInfo>("test.xml", ef);
            Assert.IsTrue(fm.Contains(g));
            Assert.IsInstanceOfType(fm[g], typeof(DiscoInfo));

            fm[g] = null;
            Assert.AreEqual(1, fm.Count);

            fm.Remove(g);
            Assert.AreEqual(0, fm.Count);
        }
    }
}
