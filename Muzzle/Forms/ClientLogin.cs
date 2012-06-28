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
using System.ComponentModel;
using System.Windows.Forms;
using Jabber.Connection;
using Jabber.Connection.SASL;

namespace Muzzle.Forms
{
    /// <summary>
    /// A login form for client connections.
    /// </summary>
    /// <example>
    /// ClientLogin l = new ClientLogin(jc);
    ///
    /// if (l.ShowDialog(this) == DialogResult.OK)
    /// {
    ///     jc.Connect();
    /// }
    /// </example>
    public partial class ClientLogin : OptionForm
    {

        /// <summary>
        /// Create a Client Login dialog box than manages the connection properties of a particular client
        /// connection.
        /// </summary>
        /// <param name="cli">The client connection to modify</param>
        public ClientLogin(Jabber.Client.JabberClient cli)
            : this()
        {
            this.Xmpp = cli;
        }

        /// <summary>
        /// Create a Client Login dialog box
        /// </summary>
        public ClientLogin()
            : base()
        {
            InitializeComponent();

#if NO_SSL
            cbSSL.Visible = false;
#endif

            for (ProxyType pt = ProxyType.None; pt <= ProxyType.HTTP; pt++)
            {
                cmbProxy.Items.Add(pt);
            }
            cmbProxy.SelectedItem = ProxyType.None;

            for (ConnectionType ct = ConnectionType.Socket; ct <= ConnectionType.HTTP_Binding; ct++)
            {
                cmbConnectionType.Items.Add(ct);
            }
            cmbConnectionType.SelectedItem = ConnectionType.Socket;

            cbSSL.Tag = Options.SSL;
            txtPass.Tag = Options.PASSWORD;
            txtServer.Tag = Options.TO;
            txtUser.Tag = Options.USER;
            numPort.Tag = Options.PORT;
            txtNetworkHost.Tag = Options.NETWORK_HOST;
            cmbProxy.Tag = Options.PROXY_TYPE;
            numProxyPort.Tag = Options.PROXY_PORT;
            txtProxyUser.Tag = Options.PROXY_USER;
            txtProxyPassword.Tag = Options.PROXY_PW;
            txtProxyHost.Tag = Options.PROXY_HOST;
            cbPlaintext.Tag = Options.PLAINTEXT;
            txtURL.Tag = Options.POLL_URL;
            cmbConnectionType.Tag = Options.CONNECTION_TYPE;
            cbUseWinCreds.Tag = KerbProcessor.USE_WINDOWS_CREDS;
        }

        /// <summary>
        /// Log in to the server
        /// </summary>
        /// <param name="cli">The JabberClient instance to connect</param>
        /// <param name="propertyFile">The name of an XML file to store properties in.</param>
        /// <returns>True if the user clicked OK, false on cancel</returns>
        public static bool Login(Jabber.Client.JabberClient cli, string propertyFile)
        {
            return new ClientLogin(cli).Login(propertyFile);
        }

        private void cbSSL_CheckedChanged(object sender, System.EventArgs e)
        {
            if (cbSSL.Checked)
            {
                if (numPort.Value == 5222)
                    numPort.Value = 5223;
            }
            else
            {
                if (numPort.Value == 5223)
                    numPort.Value = 5222;
            }
        }

        private void cmbConnectionType_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool socket = (cmbConnectionType.SelectedIndex == 0);
            bool prox = (cmbProxy.SelectedIndex != 0);
            txtURL.Enabled = !socket;

            txtNetworkHost.Enabled = socket;
            cbSSL.Enabled = socket;
            numPort.Enabled = socket;

            txtProxyHost.Enabled = prox;
            numProxyPort.Enabled = prox;
            txtProxyUser.Enabled = prox;
            txtProxyPassword.Enabled = prox;
        }

        private void cmbProxy_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool prox = (cmbProxy.SelectedIndex != 0);
            txtProxyHost.Enabled = prox;
            numProxyPort.Enabled = prox;
            txtProxyUser.Enabled = prox;
            txtProxyPassword.Enabled = prox;
        }

        private void Required_Validating(object sender, CancelEventArgs e)
        {
            this.Required(sender, e);
        }

        private void onValidated(object sender, EventArgs e)
        {
            this.ClearError(sender, e);
        }

        private void cbUseWinCreds_CheckedChanged(object sender, EventArgs e)
        {
            if (cbUseWinCreds.Checked)
            {
                txtUser.Clear();
                txtPass.Clear();
                this.ClearError(txtUser, null);
                this.ClearError(txtPass, null);
            }
            txtUser.Enabled = txtPass.Enabled = !cbUseWinCreds.Checked;
        }
    }
}
