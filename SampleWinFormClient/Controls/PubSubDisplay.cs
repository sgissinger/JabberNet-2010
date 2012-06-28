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
using System.Diagnostics;
using System.Windows.Forms;
using Jabber.Connection;

namespace SampleWinFormClient.Controls
{
    public partial class PubSubDisplay : UserControl
    {
        private PubSubNode m_node = null;

        public PubSubDisplay()
        {
            InitializeComponent();
        }

        public PubSubNode Node
        {
            get { return m_node; }
            set
            {
                if (m_node == value)
                    return;
                if (m_node != null)
                {
                    m_node.OnItemAdd -= m_node_OnItemAdd;
                    m_node.OnItemRemove -= m_node_OnItemRemove;
                }
                m_node = value;
                m_node.OnItemAdd += m_node_OnItemAdd;
                m_node.OnItemRemove += m_node_OnItemRemove;
                m_node.AutomatedSubscribe();
            }
        }

        private void m_node_OnItemAdd(PubSubNode node, Jabber.Protocol.IQ.PubSubItem item)
        {
            // OnItemRemove should have fired first, so no reason to remove it here.
            // Hopefully.
            Debug.Assert(lbID.Items.IndexOf(item.ID) == -1);
            lbID.Items.Add(item.ID);
        }

        private void m_node_OnItemRemove(PubSubNode node, Jabber.Protocol.IQ.PubSubItem item)
        {
            int index = lbID.Items.IndexOf(item.ID);
            if (lbID.SelectedIndex == index)
                rtItem.Clear();
            if (index >= 0)
                lbID.Items.RemoveAt(index);
        }

        private void lbID_SelectedIndexChanged(object sender, EventArgs e)
        {
            rtItem.Clear();
            if (lbID.SelectedIndex == -1)
                return;
            // TODO: XML2RTF
            rtItem.Text = m_node[(string)lbID.SelectedItem].OuterXml;
        }
    }
}
