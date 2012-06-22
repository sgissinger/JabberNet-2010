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

using System.Xml;

using Jabber.Protocol;
using Jabber.Protocol.X;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Jabber.Protocol.X
{
    /// <summary>
    /// Summary description for AuthTest.
    /// </summary>
    [TestClass]
    public class EventTest
    {
        XmlDocument doc = new XmlDocument();
        [TestInitialize]
        public void SetUp()
        {
            Element.ResetID();
        }
        [TestMethod]
        public void Test_Create()
        {
            Event e = new Event(doc);
            Assert.AreEqual("<x xmlns=\"jabber:x:event\" />", e.ToString());
            e.ID = "foo";
            Assert.AreEqual("<x xmlns=\"jabber:x:event\"><id>foo</id></x>", e.ToString());
            Assert.AreEqual("foo", e.ID);
            Assert.AreEqual(EventType.NONE, e.Type);
            e.Type = EventType.composing;
            Assert.AreEqual(EventType.composing, e.Type);
            e.Type = EventType.delivered;
            Assert.AreEqual(EventType.delivered, e.Type);
            Assert.AreEqual("<x xmlns=\"jabber:x:event\"><id>foo</id><delivered /></x>", e.ToString());
            Assert.AreEqual(true, e.IsDelivered);
            Assert.AreEqual(false, e.IsComposing);
            e.IsComposing = true;
            Assert.AreEqual("<x xmlns=\"jabber:x:event\"><id>foo</id><delivered /><composing /></x>", e.ToString());
            Assert.AreEqual(EventType.composing | EventType.delivered, e.Type);
        }
    }
}
