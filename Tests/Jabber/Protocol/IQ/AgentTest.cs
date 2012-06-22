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

using Jabber;
using Jabber.Protocol;
using Jabber.Protocol.Client;
using Jabber.Protocol.IQ;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Jabber.Protocol.IQ
{
    /// <summary>
    /// Test Agents
    /// </summary>
    [TestClass]
    public class AgentTest
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
            AgentsQuery r = new AgentsQuery(doc);
            Assert.AreEqual("<query xmlns=\"jabber:iq:agents\" />", r.ToString());
        }

        [TestMethod]
        public void Test_Item()
        {
            AgentsIQ aiq = new AgentsIQ(doc);
            AgentsQuery q = (AgentsQuery)aiq.Query;
            Agent a = q.AddAgent();
            a.JID = new JID("hildjj@jabber.com");
            Assert.AreEqual("<iq id=\"" + aiq.ID + "\" type=\"get\"><query xmlns=\"jabber:iq:agents\">" +
                "<agent jid=\"hildjj@jabber.com\" /></query></iq>",
                aiq.ToString());
        }
        [TestMethod]
        public void Test_GetItems()
        {
            AgentsIQ aiq = new AgentsIQ(doc);
            AgentsQuery r = (AgentsQuery)aiq.Query;
            Agent a = r.AddAgent();
            a.JID = new JID("hildjj@jabber.com");
            a = r.AddAgent();
            a.JID = new JID("hildjj@jabber.org");
            Agent[] agents = r.GetAgents();
            Assert.AreEqual(agents.Length, 2);
            Assert.AreEqual(agents[0].JID, "hildjj@jabber.com");
            Assert.AreEqual(agents[1].JID, "hildjj@jabber.org");
        }
        [TestMethod]
        public void Test_Transport()
        {
            AgentsIQ aiq = new AgentsIQ(doc);
            aiq.Type = IQType.result;
            AgentsQuery r = (AgentsQuery)aiq.Query;
            Agent a = r.AddAgent();
            a.JID = new JID("hildjj@jabber.com");
            a.Transport = true;
            Assert.AreEqual(a.Transport, true);
            Assert.AreEqual("<iq id=\"" + aiq.ID + "\" type=\"result\"><query xmlns=\"jabber:iq:agents\">" +
                "<agent jid=\"hildjj@jabber.com\"><transport /></agent></query></iq>",
                aiq.ToString());
            a.Transport = false;
            Assert.AreEqual(a.Transport, false);
            a.Groupchat = true;
            Assert.AreEqual(a.Groupchat, true);
            Assert.AreEqual("<iq id=\"" + aiq.ID + "\" type=\"result\"><query xmlns=\"jabber:iq:agents\">" +
                "<agent jid=\"hildjj@jabber.com\"><groupchat /></agent></query></iq>",
                aiq.ToString());
        }
    }
}
