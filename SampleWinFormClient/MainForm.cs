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
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using Bedrock.Util;
using Jabber;
using Jabber.Connection;
using Jabber.Protocol;
using Jabber.Protocol.Client;
using Jabber.Protocol.IQ;
using SampleWinFormClient.Controls;
using SampleWinFormClient.Forms;

namespace SampleWinFormClient
{
    /// <summary>
    /// Summary description for MainForm.
    /// </summary>
    public partial class MainForm : Form
    {
        private bool m_err = false;
        private bool m_connected = false;

        public MainForm()
        {
            InitializeComponent();

            Jabber.Connection.Ident ident2 = new Jabber.Connection.Ident();
            ident2.Category = "client";
            ident2.Lang = "en";
            ident2.Name = "Jabber-Net Test Client";
            ident2.Type = "pc";
            this.cm.Identities = new Jabber.Connection.Ident[] { ident2 };

            services.ImageList = roster.ImageList;
            cm.AddFeature(URI.TIME);
            cm.AddFeature(URI.VERSION);
            cm.AddFeature(URI.LAST);
            cm.AddFeature(URI.DISCO_INFO);

            tabControl1.TabPages.Remove(tpServices);
            tabControl1.TabPages.Remove(tpDebug);
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }


        void idler_OnUnIdle(object sender, TimeSpan span)
        {
            jc.Presence(PresenceType.available, "Available", null, 0);
            pnlPresence.Text = "Available";
        }

        private void idler_OnIdle(object sender, TimeSpan span)
        {
            jc.Presence(PresenceType.available, "Auto-away", "away", 0);
            pnlPresence.Text = "Away";
        }

        private void Connect()
        {
            Muzzle.Forms.ClientLogin.Login(jc, "login.xml");
        }

        private void jc_OnAuthenticate(object sender)
        {
            pnlPresence.Text = "Available";
            pnlCon.Text = "Connected";
            mnuAway.Enabled = mnuAvailable.Enabled = true;

            if (jc.SSLon)
            {

                pnlSSL.Text = "SSL";
                System.Security.Cryptography.X509Certificates.X509Certificate cert2 =
                    (System.Security.Cryptography.X509Certificates.X509Certificate)
                    jc[Options.REMOTE_CERTIFICATE];

                string cert_str = cert2.ToString(true);
                debug.Write("CERT:", cert_str);
                pnlSSL.ToolTipText = cert_str;
            }
            idler.Enabled = true;
        }

        private void jc_OnDisconnect(object sender)
        {
            m_connected = false;
            mnuAway.Enabled = mnuAvailable.Enabled = false;
            idler.Enabled = false;
            pnlPresence.Text = "Offline";
            pnlSSL.Text = "";
            pnlSSL.ToolTipText = "";
            connectToolStripMenuItem.Text = "&Connect";
            lvBookmarks.Items.Clear();

            if (!m_err)
                pnlCon.Text = "Disconnected";
        }

        private void jc_OnError(object sender, Exception ex)
        {
            m_connected = false;
            mnuAway.Enabled = mnuAvailable.Enabled = false;
            connectToolStripMenuItem.Text = "&Connect";
            idler.Enabled = false;
            lvBookmarks.Items.Clear();

            pnlCon.Text = "Error: " + ex.Message;
        }

        private void jc_OnAuthError(object sender, XmlElement elem)
        {
            if (MessageBox.Show(this,
                "Create new account?",
                "Authentication error",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Warning) == DialogResult.OK)
            {
                if (!m_connected)
                {
                    MessageBox.Show("You have been disconnected by the server.  Registration is not enabled.", "Disconnected", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                jc.Register(new JID(jc.User, jc.Server, null));
            }
            else
            {
                jc.Close(false);
            }
        }

        private void jc_OnRegistered(object sender, IQ iq)
        {
            if (iq.Type == IQType.result)
                jc.Login();
            else
                pnlCon.Text = "Registration error";
        }

        private bool jc_OnRegisterInfo(object sender, Register r)
        {
            if (r.Form == null)
                return true;
            Muzzle.Forms.XDataForm f = new Muzzle.Forms.XDataForm(r.Form);
            if (f.ShowDialog() != DialogResult.OK)
                return false;
            f.FillInResponse(r.Form);
            return true;
        }

        private void jc_OnMessage(object sender, Jabber.Protocol.Client.Message msg)
        {
            Jabber.Protocol.X.Data x = msg["x", URI.XDATA] as Jabber.Protocol.X.Data;
            if (x != null)
            {
                Muzzle.Forms.XDataForm f = new Muzzle.Forms.XDataForm(msg);
                f.ShowDialog(this);
                jc.Write(f.GetResponse());
            }
            else
                MessageBox.Show(this, msg.Body, msg.From, MessageBoxButtons.OK);
        }

        private void jc_OnIQ(object sender, IQ iq)
        {
            if (iq.Type != IQType.get)
                return;

            XmlElement query = iq.Query;
            if (query == null)
                return;

            // <iq id="jcl_8" to="me" from="you" type="get"><query xmlns="jabber:iq:version"/></iq>
            if (query is Jabber.Protocol.IQ.Version)
            {
                iq = iq.GetResponse(jc.Document);
                Jabber.Protocol.IQ.Version ver = iq.Query as Jabber.Protocol.IQ.Version;
                if (ver != null)
                {
                    ver.OS = Environment.OSVersion.ToString();
                    ver.EntityName = Application.ProductName;
                    ver.Ver = Application.ProductVersion;
                }
                jc.Write(iq);
                return;
            }

            if (query is Time)
            {
                iq = iq.GetResponse(jc.Document);
                Time tim = iq.Query as Time;
                if (tim != null) tim.SetCurrentTime();
                jc.Write(iq);
                return;
            }

            if (query is Last)
            {
                iq = iq.GetResponse(jc.Document);
                Last last = iq.Query as Last;
                if (last != null) last.Seconds = (int)IdleTime.GetIdleTime();
                jc.Write(iq);
                return;
            }
        }

        private void roster_DoubleClick(object sender, EventArgs e)
        {
            Muzzle.Controls.RosterTree.ItemNode n = roster.SelectedNode as Muzzle.Controls.RosterTree.ItemNode;
            if (n == null)
                return;
            new SendMessage(jc, n.JID).Show();
        }

        private void sb_PanelClick(object sender, StatusBarPanelClickEventArgs e)
        {
            if (e.StatusBarPanel != pnlPresence)
                return;
            mnuPresence.Show(sb, new Point(e.X, e.Y));
        }

        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (jc.IsAuthenticated)
            {
                jc.Close(true);
                connectToolStripMenuItem.Text = "&Connect";
            }
            else
            {
                Connect();
                connectToolStripMenuItem.Text = "Dis&connect";
            }
        }

        private void mnuAvailable_Click(object sender, EventArgs e)
        {
            if (jc.IsAuthenticated)
            {
                jc.Presence(PresenceType.available, "Available", null, 0);
                pnlPresence.Text = "Available";
            }
            else
                Connect();
        }

        private void mnuAway_Click(object sender, EventArgs e)
        {
            if (jc.IsAuthenticated)
            {
                jc.Presence(PresenceType.available, "Away", "away", 0);
                pnlPresence.Text = "Away";
            }
            else
                Connect();
        }

        /*
        private void mnuOffline_Click(object sender, EventArgs e)
        {
            if (jc.IsAuthenticated)
                jc.Close();
        }
         */

        void jc_OnConnect(object sender, StanzaStream stream)
        {
            m_err = false;
            m_connected = true;
        }

        private void jc_OnStreamError(object sender, XmlElement rp)
        {
            m_err = true;
            pnlCon.Text = "Stream error: " + rp.InnerText;
        }

        /*
        private void txtDebugInput_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Enter) && e.Control)
            {
                try
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(txtDebugInput.Text);
                    XmlElement elem = doc.DocumentElement;
                    if (elem != null)
                        jc.Write(elem);
                    txtDebugInput.Clear();
                }
                catch (XmlException ex)
                {
                    MessageBox.Show("Invalid XML: " + ex.Message);
                }
            }

        }
*/
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.ExceptionObject.ToString(), "Unhandled exception: " + e.GetType());
        }

        private void rm_OnRosterEnd(object sender)
        {
            roster.ExpandAll();
        }

        private void MainForm_Closing(object sender, CancelEventArgs e)
        {
            if (jc.IsAuthenticated)
                jc.Close();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            jc.Close();
            Close();
        }

        private void menuItem3_Click(object sender, EventArgs e)
        {
            AddContact ac = new AddContact();
            ac.AllGroups = roster.Groups;
            ac.DefaultDomain = jc.Server;
            if (ac.ShowDialog() != DialogResult.OK)
                return;

            jc.Subscribe(ac.JID, ac.Nickname, ac.SelectedGroups);
        }

        private void menuItem5_Click(object sender, EventArgs e)
        {
            Muzzle.Controls.RosterTree.ItemNode n = roster.SelectedNode as Muzzle.Controls.RosterTree.ItemNode;
            if (n == null)
                return;
            jc.RemoveRosterItem(n.JID);
        }


        // add group
        private void addGroupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddGroup ag = new AddGroup();
            if (ag.ShowDialog() == DialogResult.Cancel)
                return;

            if (ag.GroupName == "")
                return;

            roster.AddGroup(ag.GroupName).EnsureVisible();
        }

        private void servicesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tabControl1.TabPages.Contains(tpServices))
            {
                tabControl1.TabPages.Remove(tpServices);
                servicesToolStripMenuItem.Checked = false;
            }
            else
            {
                tabControl1.TabPages.Add(tpServices);
                tabControl1.SelectedTab = tpServices;
                servicesToolStripMenuItem.Checked = true;
            }
        }

        private void debugToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tabControl1.TabPages.Contains(tpDebug))
            {
                tabControl1.TabPages.Remove(tpDebug);
                debugToolStripMenuItem.Checked = false;
            }
            else
            {
                tabControl1.TabPages.Add(tpDebug);
                tabControl1.SelectedTab = tpDebug;
                debugToolStripMenuItem.Checked = true;
            }
        }

        private void subscribeToPubSubToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PubSubSubcribeForm ps = new PubSubSubcribeForm();
            // this is a small race.  to do it right, I should call dm.BeginFindServiceWithFeature,
            // and modify that to call back on all of the found services.  The idea is that
            // by the the time the user has a chance to click on the menu item, the DiscoManager
            // will be populated.
            ps.DiscoManager = dm;
            if (ps.ShowDialog() != DialogResult.OK)
                return;
            JID jid = ps.JID;
            string node = ps.Node;
            string text = string.Format("{0}/{1}", jid, node);

            TabPage tp = new TabPage(text);
            tp.Name = text;

            PubSubDisplay disp = new PubSubDisplay();
            disp.Node = psm.GetNode(jid, node, 10);
            tp.Controls.Add(disp);
            disp.Dock = DockStyle.Fill;

            tabControl1.TabPages.Add(tp);
            tabControl1.SelectedTab = tp;
        }

        private void rm_OnSubscription(Jabber.Client.RosterManager manager, Item ri, Presence pres)
        {
            DialogResult res = MessageBox.Show("Allow incoming presence subscription request from: " + pres.From,
                "Subscription Request",
                MessageBoxButtons.YesNoCancel);
            switch (res)
            {
                case DialogResult.Yes:
                    manager.ReplyAllow(pres);
                    break;
                case DialogResult.No:
                    manager.ReplyDeny(pres);
                    break;
                case DialogResult.Cancel:
                    // do nothing;
                    break;
            }
        }

        private void rm_OnUnsubscription(Jabber.Client.RosterManager manager, Presence pres, ref bool remove)
        {
            MessageBox.Show(pres.From + " has removed you from their roster.", "Unsubscription notification", MessageBoxButtons.OK);
        }

        private void closeTabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TabPage tp = tabControl1.SelectedTab;
            if (tp == tpRoster)
                return;
            else if (tp == tpDebug)
                debugToolStripMenuItem.Checked = false;
            else if (tp == tpServices)
                servicesToolStripMenuItem.Checked = false;
            tabControl1.TabPages.Remove(tp);
        }

        private void deletePubSubToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PubSubSubcribeForm fm = setupPubSubForm();
            if (fm.ShowDialog() != DialogResult.OK)
                return;

            JID jid = fm.JID;
            string node = fm.Node;

            psm.RemoveNode(jid, node,
                delegate
                {
                    MessageBox.Show("Remove Node unsuccessful.");
                });

            tabControl1.TabPages.RemoveByKey(string.Format("{0}/{1}", jid, node));
        }

        private PubSubSubcribeForm setupPubSubForm()
        {
            string JID = null;
            string node = null;
            if (tabControl1.SelectedTab.Name != null && tabControl1.SelectedTab.Name.Contains("/"))
            {
                string value = tabControl1.SelectedTab.Name;
                int index = value.IndexOf("/");

                JID = value.Substring(0, index);
                node = value.Substring(index + 1);
            }

            PubSubSubcribeForm fm = new PubSubSubcribeForm();
            fm.Text = "Delete PubSub";
            if (JID != null) fm.JID = JID;
            if (node != null) fm.Node = node;
            fm.DiscoManager = dm;

            return fm;
        }

        private void joinConferenceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConferenceForm cf = new ConferenceForm();
            cf.DiscoManager = dm;
            cf.Nick = muc.DefaultNick;
            if (cf.ShowDialog() != DialogResult.OK)
                return;

            muc.GetRoom(cf.RoomAndNick).Join();
        }

        private IQ muc_OnRoomConfig(Room room, IQ parent)
        {
            Muzzle.Forms.XDataForm form = new Muzzle.Forms.XDataForm(parent);
            if (form.ShowDialog() != DialogResult.OK)
                return null;

            return (IQ)form.GetResponse();
        }

        private void muc_OnPresenceError(Room room, Presence pres)
        {
            m_err = true;
            pnlCon.Text = "Groupchat error: " + pres.Error.OuterXml;
        }

        private void muc_OnInvite(object sender, Jabber.Protocol.Client.Message msg)
        {
            Room r = sender as Room;
            r.Join();
        }

        private void bmm_OnConferenceAdd(Jabber.Client.BookmarkManager manager, BookmarkConference conference)
        {
            string jid = conference.JID;
            string name = conference.ConferenceName;
            if (name == null)
                name = jid;
            if (lvBookmarks.Items.ContainsKey(jid))
                lvBookmarks.Items.RemoveByKey(jid);
            ListViewItem item = lvBookmarks.Items.Add(jid, name, -1);
            item.SubItems.Add(conference.Nick);
            item.SubItems.Add(conference.AutoJoin.ToString());
            item.Tag = conference.JID;
        }

        private void bmm_OnConferenceRemove(Jabber.Client.BookmarkManager manager, BookmarkConference conference)
        {
            string jid = conference.JID;
            if (lvBookmarks.Items.ContainsKey(jid))
                lvBookmarks.Items.RemoveByKey(jid);
        }

        private void lvBookmarks_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                removeToolStripMenuItem_Click(null, null);
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // pop up AddBookmark dialog
            ConferenceForm cf = new ConferenceForm();
            cf.DiscoManager = dm;
            cf.Nick = muc.DefaultNick;
            if (cf.ShowDialog() != DialogResult.OK)
                return;
            // TODO: add autojoin and name.
            bmm.AddConference(cf.RoomJID, null, false, cf.Nick);
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem lvi in lvBookmarks.SelectedItems)
            {
                bmm[(JID)lvi.Tag] = null;
            }
        }

        private void lvBookmarks_DoubleClick(object sender, EventArgs e)
        {
            if (lvBookmarks.SelectedItems.Count == 0)
                return;
            ListViewItem lvi = lvBookmarks.SelectedItems[0];

            JID jid = (JID)lvi.Tag;
            BookmarkConference conf = bmm[jid];
            Debug.Assert(conf != null);

            ConferenceForm cf = new ConferenceForm();
            cf.DiscoManager = dm;
            cf.RoomAndNick = new JID(jid.User, jid.Server, conf.Nick);

            if (cf.ShowDialog() != DialogResult.OK)
                return;
            bmm.AddConference(cf.RoomJID, null, false, cf.Nick);
        }
    }
}
