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
using System.Windows.Forms;
using Jabber;
using Jabber.Connection;

namespace SampleWinFormClient.Forms
{
    public partial class ConferenceForm : Form
    {
        private DiscoManager m_disco = null;

        public ConferenceForm()
        {
            InitializeComponent();
        }

        public DiscoManager DiscoManager
        {
            get { return m_disco; }
            set { m_disco = value; }
        }

        public JID RoomJID
        {
            get
            {
                return new JID(txtRoom.Text, cmbJID.Text, null);
            }
        }

        public JID RoomAndNick
        {
            get
            {
                return new JID(txtRoom.Text, cmbJID.Text, txtNick.Text);
            }
            set
            {
                if (value == null)
                {
                    cmbJID.Text = txtRoom.Text = txtNick.Text = "";
                }
                else
                {
                    cmbJID.Text = value.Server;
                    txtRoom.Text = value.User;
                    txtNick.Text = value.Resource;
                }
            }
        }

        public string Nick
        {
            get { return txtNick.Text; }
            set { txtNick.Text = value; }
        }

        private void ConferenceForm_Shown(object sender, EventArgs e)
        {
            cmbJID.BeginUpdate();
            cmbJID.Items.Clear();
            if (m_disco != null)
                m_disco.BeginGetItems(null, GotRoot, null);
            else
                cmbJID.EndUpdate();
        }

        private void GotRoot(DiscoManager sender, DiscoNode node, object state)
        {
            if (node.Children != null)
            {
                foreach (DiscoNode component in node.Children)
                {
                    if (component.HasFeature(Jabber.Protocol.URI.MUC))
                        cmbJID.Items.Add(component.JID);
                }
                if (cmbJID.Items.Count > 0)
                    cmbJID.SelectedIndex = 0;
            }
            cmbJID.EndUpdate();
        }
    }
}
