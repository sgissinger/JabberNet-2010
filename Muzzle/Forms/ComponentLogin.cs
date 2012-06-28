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
using Jabber.Server;

namespace Muzzle.Forms
{
    /// <summary>
    /// A login form for client connections.
    /// </summary>
    /// <example>
    /// ComponentLogin l = new ComponentLogin(jc);
    ///
    /// if (l.ShowDialog(this) == DialogResult.OK)
    /// {
    ///     jc.Connect();
    /// }
    /// </example>
    public partial class ComponentLogin : OptionForm
    {
        /// <summary>
        /// Create a Client Login dialog box that manages a component
        /// </summary>
        /// <param name="service">The component to manage</param>
        public ComponentLogin(Jabber.Server.JabberService service)
            : this()
        {
            this.Xmpp = service;
        }

        /// <summary>
        /// Create a Client Login dialog box
        /// </summary>
        public ComponentLogin()
        {
            InitializeComponent();

            for (ComponentType ct=ComponentType.Accept; ct <= ComponentType.Connect; ct++)
            {
                cmbType.Items.Add(ct);
            }
            cmbType.SelectedItem = ComponentType.Accept;

            txtUser.Tag = Options.TO;
            txtServer.Tag = Options.NETWORK_HOST;
            numPort.Tag = Options.PORT;
            txtPass.Tag = Options.PASSWORD;
            cmbType.Tag = Options.COMPONENT_DIRECTION;
        }

        /// <summary>
        /// Log in to the server
        /// </summary>
        /// <param name="service">The JabberClient instance to connect</param>
        /// <param name="propertyFile">The name of an XML file to store properties in.</param>
        /// <returns>True if the user clicked OK, false on cancel</returns>
        public static bool Login(Jabber.Server.JabberService service, string propertyFile)
        {
            return new ComponentLogin(service).Login(propertyFile);
        }

        private void Required_Validating(object sender, CancelEventArgs e)
        {
            this.Required(sender, e);
        }

        private void onValidated(object sender, EventArgs e)
        {
            this.ClearError(sender, e);
        }
    }
}
