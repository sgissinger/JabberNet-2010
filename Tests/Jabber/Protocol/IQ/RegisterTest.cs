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
using Jabber.Protocol.IQ;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Jabber.Protocol.IQ
{
    /// <summary>
    /// Summary description for RosterTest.
    /// </summary>
    [TestClass]
    public class RegisterTest
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
            Register r = new Register(doc);
            Assert.AreEqual("<query xmlns=\"jabber:iq:register\" />", r.ToString());
        }
        [TestMethod]
        public void Test_Registered()
        {
            Register r = new Register(doc);
            r.Registered = true;
            Assert.AreEqual("<query xmlns=\"jabber:iq:register\"><registered /></query>", r.ToString());
            Assert.IsTrue(r.Registered);
        }
    }
}
