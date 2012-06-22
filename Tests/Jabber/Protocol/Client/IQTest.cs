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

namespace Tests.Jabber.Protocol.Client
{
    /// <summary>
    /// Summary description for IQTest.
    /// </summary>
    [TestClass]
    public class IQTest
    {
        XmlDocument doc = new XmlDocument();

        [TestMethod]
        public void Create()
        {
            Element.ResetID();

            AuthIQ iq = new AuthIQ(doc);
            Assert.AreEqual("<iq id=\"JN_1\" type=\"get\" />", iq.ToString());
            iq = new AuthIQ(doc);
            Assert.AreEqual("<iq id=\"JN_2\" type=\"get\" />", iq.ToString());
            iq.Query = new Auth(doc);
            Assert.AreEqual("<iq id=\"JN_2\" type=\"get\"><query xmlns=\"jabber:iq:auth\" /></iq>", iq.ToString());
        }
    }
}
