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

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using Jabber.Client;
using Jabber.Connection;

namespace SampleWinFormClient.Controls
{
    public partial class ServiceDisplay : UserControl
    {
        private DiscoManager m_disco = null;
        private JabberClient m_stream = null;

        public ServiceDisplay()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The JabberClient or JabberService to hook up to.
        /// </summary>
        [Description("The JabberClient to hook up to.")]
        [Category("Jabber")]
        public virtual JabberClient Stream
        {
            get
            {
                // If we are running in the designer, let's try to get an XmppStream control
                // from the environment.
                if ((this.m_stream == null) && DesignMode)
                {
                    IDesignerHost host = (IDesignerHost)base.GetService(typeof(IDesignerHost));
                    this.Stream = (JabberClient)Jabber.Connection.StreamComponent.GetComponentFromHost(host, typeof(JabberClient));
                }
                return m_stream;
            }
            set
            {
                if ((object)m_stream != (object)value)
                {
                    m_stream = value;
                    m_stream.OnAuthenticate += new Bedrock.ObjectHandler(m_stream_OnAuthenticate);
                    m_stream.OnDisconnect += new Bedrock.ObjectHandler(m_stream_OnDisconnect);
                }
            }
        }

        [Category("Jabber")]
        public DiscoManager DiscoManager
        {
            get
            {
                // If we are running in the designer, let's try to get a DiscoManager control
                // from the environment.
                if ((this.m_disco == null) && DesignMode)
                {
                    IDesignerHost host = (IDesignerHost)base.GetService(typeof(IDesignerHost));
                    this.m_disco = (DiscoManager)StreamComponent.GetComponentFromHost(host, typeof(DiscoManager));
                }
                return m_disco;
            }
            set
            {
                if ((object)m_disco != (object)value)
                    m_disco = value;
            }
        }

        [Category("Appearance")]
        public ImageList ImageList
        {
            get { return tvServices.ImageList; }
            set { tvServices.ImageList = value; }
        }

        private void m_stream_OnAuthenticate(object sender)
        {
            // TODO: some of this will break in 2003.
            Jabber.Connection.DiscoNode dn = m_disco.GetNode(m_stream.Server, null);
            TreeNode tn = tvServices.Nodes.Add(dn.Key, dn.Name);
            tn.ToolTipText = dn.Key.Replace('\u0000', '\n');
            tn.Tag = dn;
            tn.ImageIndex = 8;
            tn.SelectedImageIndex = 8;
            m_disco.BeginGetFeatures(dn, new Jabber.Connection.DiscoNodeHandler(GotInitialFeatures), null);
        }

        private void m_stream_OnDisconnect(object sender)
        {
            pgServices.SelectedObject = null;
            tvServices.Nodes.Clear();
        }


        private void tvServices_AfterExpand(object sender, TreeViewEventArgs e)
        {
            e.Node.ImageIndex = 6;
            e.Node.SelectedImageIndex = 6;
        }

        private void tvServices_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            e.Node.ImageIndex = 7;
            e.Node.SelectedImageIndex = 7;
        }

        private void tvServices_NodeMouseDoubleClick(object sender,
                                             TreeNodeMouseClickEventArgs e)
        {
            Jabber.Connection.DiscoNode dn = (Jabber.Connection.DiscoNode)e.Node.Tag;
            if (dn.Children == null)
                m_disco.BeginGetItems(dn.JID, dn.Node, new Jabber.Connection.DiscoNodeHandler(GotItems), null);
        }

        private void tvServices_AfterSelect(object sender, TreeViewEventArgs e)
        {
            Jabber.Connection.DiscoNode dn = (Jabber.Connection.DiscoNode)e.Node.Tag;
            m_disco.BeginGetFeatures(dn, new Jabber.Connection.DiscoNodeHandler(GotInfo), null);
        }

        private void GotInitialFeatures(DiscoManager sender, Jabber.Connection.DiscoNode node, object state)
        {
            m_disco.BeginGetItems(node, new Jabber.Connection.DiscoNodeHandler(GotItems), state);
        }

        private void GotItems(DiscoManager sender, Jabber.Connection.DiscoNode node, object state)
        {
            // TODO: some of this will break in 2003.
            TreeNode[] nodes = tvServices.Nodes.Find(node.Key, true);
            foreach (TreeNode n in nodes)
            {
                n.ImageIndex = 7;
                n.SelectedImageIndex = 7;
                foreach (Jabber.Connection.DiscoNode dn in node.Children)
                {
                    TreeNode tn = n.Nodes.Add(dn.Key, dn.Name);
                    tn.ToolTipText = dn.Key.Replace('\u0000', '\n');
                    tn.Tag = dn;
                    tn.ImageIndex = 8;
                    tn.SelectedImageIndex = 8;
                }
            }
            pgServices.Refresh();
        }

        private void GotInfo(DiscoManager sender, Jabber.Connection.DiscoNode node, object state)
        {
            pgServices.SelectedObject = node;
        }
    }
}
