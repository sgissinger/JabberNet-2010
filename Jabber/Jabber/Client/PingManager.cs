/* --------------------------------------------------------------------------
 * Copyrights
 *
 * Portions created by or assigned to Sébastien Gissinger
 *
 * License
 *
 * Jabber-Net is licensed under the LGPL.
 * See LICENSE.txt for details.
 * --------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Xml;
using Jabber.Connection;
using Jabber.Protocol;
using Jabber.Protocol.Client;
using Jabber.Protocol.IQ;

namespace Jabber.Client
{
    /// <summary>
    /// Manages Jingle sessions with peers
    /// </summary>
    public partial class PingManager : StreamComponent
    {
        #region PROPERTIES
        /// <summary>
        /// Session informations about every opened sessions using their SID as key
        /// </summary>
        private Dictionary<String, JingleSession> Sessions { get; set; }
        #endregion

        #region CONSTRUCTORS & FINALIZERS
        /// <summary>
        /// Creates a new Jingle Manager inside a container
        /// </summary>
        /// <param name="container">Parent container</param>
        public PingManager(IContainer container)
            : this()
        {
            container.Add(this);
        }

        /// <summary>
        /// Creates a new Jingle Manager
        /// </summary>
        public PingManager()
        {
            InitializeComponent();

            this.OnStreamChanged += new Bedrock.ObjectHandler(JingleManager_OnStreamChanged);
        }
        #endregion

        #region XMPP EVENTS
        /// <summary>
        /// Entry point for XMPP stream events
        /// </summary>
        /// <param name="sender"></param>
        private void JingleManager_OnStreamChanged(object sender)
        {
            JabberClient cli = m_stream as JabberClient;

            if (cli == null)
                return;

            cli.OnIQ += new IQHandler(this.GotIQ);
        }

        /// <summary>
        /// Analyses an IQ paquet and chooses to handle it or not
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="iq"></param>
        private void GotIQ(object sender, IQ iq)
        {
            if (!iq.Handled &&
                iq.Query != null && iq.Type == IQType.get &&
                iq.Query.NamespaceURI == Jabber.Protocol.URI.PING)
            {
                Write(iq.GetAcknowledge(new XmlDocument()));
            }
        }
        #endregion
    }
}