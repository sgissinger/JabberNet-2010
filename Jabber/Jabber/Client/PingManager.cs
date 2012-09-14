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
using Bedrock;
using Jabber.Connection;
using Jabber.Protocol;
using Jabber.Protocol.Client;

namespace Jabber.Client
{
    /// <summary>
    /// Manages Jingle sessions with peers
    /// </summary>
    public partial class PingManager : StreamComponent
    {
        #region CONSTRUCTORS & FINALIZERS
        /// <summary>
        /// Creates a new Ping Manager inside a container
        /// </summary>
        /// <param name="container">Parent container</param>
        public PingManager(IContainer container)
            : this()
        {
            container.Add(this);
        }

        /// <summary>
        /// Creates a new Ping Manager
        /// </summary>
        public PingManager()
        {
            InitializeComponent();

            this.OnStreamChanged += new ObjectHandler(self_OnStreamChanged);
        }
        #endregion

        #region XMPP EVENTS
        /// <summary>
        /// Entry point for XMPP stream events
        /// </summary>
        /// <param name="sender"></param>
        private void self_OnStreamChanged(object sender)
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
                iq.Query.NamespaceURI == URI.PING)
            {
                iq.Handled = true;

                this.Write(iq.GetAcknowledge(m_stream.Document));
            }
        }
        #endregion
    }
}