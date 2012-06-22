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

using Jabber.Protocol.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Jabber.Protocol.Client
{
    /// <summary>
    /// Summary description for PresenceTest.
    /// </summary>
    [TestClass]
    public class PresenceTest
    {
        XmlDocument doc = new XmlDocument();
        [TestMethod]
        public void Test_Create()
        {
            Presence p = new Presence(doc);
            p.Type = PresenceType.available;
            p.Status = "foo";
            Assert.AreEqual("<presence><status>foo</status></presence>", p.ToString());
        }

        [TestMethod]
        public void Test_Available()
        {
            Presence p = new Presence(doc);
            Assert.AreEqual(PresenceType.available, p.Type);
            Assert.AreEqual("", p.GetAttribute("type"));
            p.Type = PresenceType.unavailable;
            Assert.AreEqual(PresenceType.unavailable, p.Type);
            Assert.AreEqual("unavailable", p.GetAttribute("type"));
            p.Type = PresenceType.available;
            Assert.AreEqual(PresenceType.available, p.Type);
            Assert.AreEqual("", p.GetAttribute("type"));
        }

        [TestMethod]
        public void Test_Order()
        {
            Presence small = new Presence(doc);
            DateTime d = DateTime.Now;
            small.IntPriority = 0;
            small.Stamp = d;

            Presence big = new Presence(doc);
            big.IntPriority = 10;
            big.Stamp = d.AddSeconds(1);

            Assert.IsTrue(small < big);
            Assert.IsTrue(big > small);

            small.IntPriority = 10;
            small.Show = "dnd";
            Assert.IsTrue(small < big);

            big.Show = "chat";
            Assert.IsTrue(small < big);

            small.Show = "chat";
            Assert.IsTrue(small < big);
        }
    }
}
