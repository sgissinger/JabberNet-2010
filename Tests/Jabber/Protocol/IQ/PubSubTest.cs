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
using Jabber.Protocol.IQ;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Jabber.Protocol.IQ
{
    [TestClass]
    public class PubSubTest
    {
        private const string NODE = "TestNode";

        private XmlDocument doc;

        [TestInitialize]
        public void Setup()
        {
            doc = new XmlDocument();
        }

        [TestMethod]
        public void AffiliationsTest()
        {
            PubSubIQ iq = new PubSubIQ(doc, PubSubCommandType.affiliations, NODE);
            Affiliations test = iq.Command as Affiliations;
            Assert.IsNotNull(test);
        }

        [TestMethod]
        public void PubSubCreateTest()
        {
            PubSubIQ iq = new PubSubIQ(doc, PubSubCommandType.create, NODE);
            Assert.IsFalse(((Create)iq.Command).HasConfigure);

            Create create = (Create)iq.Command;

            create.HasConfigure = true;
            Assert.IsTrue(create.HasConfigure);
            Assert.IsNotNull(create.GetConfiguration());
        }

    }
}
