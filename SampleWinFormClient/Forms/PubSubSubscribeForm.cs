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
using Jabber;
using Jabber.Connection;

namespace SampleWinFormClient.Forms
{
    public partial class PubSubSubcribeForm : Form
    {
        private DiscoManager m_disco = null;

        public PubSubSubcribeForm()
        {
            InitializeComponent();
        }

        public DiscoManager DiscoManager
        {
            get { return m_disco; }
            set { m_disco = value; }
        }

        public JID JID
        {
            get { return cmbJID.Text; }
            set { cmbJID.Text = value.ToString(); }
        }

        public string Node
        {
            get { return txtNode.Text; }
            set { txtNode.Text = value; }
        }

        private void PubSub_Shown(object sender, EventArgs e)
        {
            cmbJID.BeginUpdate();
            cmbJID.Items.Clear();
            foreach (DiscoNode component in m_disco.Root.Children)
            {
                if (component.HasFeature(Jabber.Protocol.URI.PUBSUB))
                    cmbJID.Items.Add(component.JID);
            }
            if (cmbJID.Items.Count > 0)
                cmbJID.SelectedIndex = 0;
            cmbJID.EndUpdate();
        }

    }
}
