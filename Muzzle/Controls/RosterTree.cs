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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Bedrock;
using Bedrock.Collections;
using Jabber;
using Jabber.Client;
using Jabber.Connection;
using Jabber.Protocol.Client;
using Jabber.Protocol.IQ;

namespace Muzzle.Controls
{
    /// <summary>
    /// A TreeView optimized for showing Jabber roster items.  Make sure that the
    /// form you drop this on has a JabberClient, a PresenceManager, and a RosterManager
    /// on the form first, and this widget will automatically connect to them.
    /// </summary>
    public partial class RosterTree : TreeView
    {
        #region P/INVOKE
        private const int TVM_SETEXTENDEDSTYLE = 0x1100 + 44;
        private const int TVM_GETEXTENDEDSTYLE = 0x1100 + 45;
        private const int TVS_EX_DOUBLEBUFFER = 0x0004;
        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);
        #endregion

        #region CONSTANTS
        // image list offsets
        private const int OFFLINE = 0;
        private const int AVAILABLE = 1;
        private const int AWAY = 2;
        private const int XA = 3;
        private const int DND = 4;
        private const int CHATTY = 5;
        private const int EXPANDED = 6;
        private const int COLLAPSED = 7;
        #endregion

        #region MEMBERS
        private RosterManager m_roster = null;
        private PresenceManager m_pres = null;
        private JabberClient m_client = null;

        private IDictionary m_groups = new SkipList();
        private IDictionary m_items = new SkipList();

        private Color m_statusColor = Color.Teal;

        private Dictionary<String, Boolean> m_expandedGroups = new Dictionary<String, Boolean>();
        private Dictionary<String, Boolean> m_excludedGroups = new Dictionary<String, Boolean>();
        #endregion

        #region PROPERTIES
        /// <summary>
        /// The text filter applied to roster items
        /// </summary>
        [Category("Managers")]
        [DefaultValue("")]
        [Description("When set to a value, the roster items displayed in the tree are filtered on their username, nickname and resource")]
        public String Filter { get; set; }

        /// <summary>
        /// The availability filter applied to roster items
        /// </summary>
        [Category("Managers")]
        [DefaultValue(false)]
        [Description("When set to true, only available roster items are displayed in the tree")]
        public Boolean ShowOnlyOnline { get; set; }

        /// <summary>
        /// Overrides default property to make it useable with the tooltip object
        /// and avoiding exceptions when base property was setted to true
        /// </summary>
        [DefaultValue(false)]
        public new Boolean ShowNodeToolTips { get; set; }

        /// <summary>
        /// The name of the default group
        /// </summary>
        [Category("Managers")]
        [DefaultValue("Unfiled")]
        [Description("The name of the default group when roster items are not in any group")]
        public String Unfiled { get; set; }

        /// <summary>
        /// The RosterManager for this view
        /// </summary>
        [Category("Managers")]
        public RosterManager RosterManager
        {
            get
            {
                // If we are running in the designer, let's try to auto-hook a RosterManager
                if (m_roster == null && this.DesignMode)
                {
                    IDesignerHost host = (IDesignerHost)base.GetService(typeof(IDesignerHost));
                    this.RosterManager = (RosterManager)StreamComponent.GetComponentFromHost(host, typeof(RosterManager));
                }
                return m_roster;
            }
            set
            {
                if ((object)m_roster == (object)value)
                    return;

                m_roster = value;

                if (m_roster != null)
                {
                    m_roster.OnRosterBegin += new ObjectHandler(m_roster_OnRosterBegin);
                    m_roster.OnRosterEnd += new ObjectHandler(m_roster_OnRosterEnd);
                    m_roster.OnRosterItem += new RosterItemHandler(m_roster_OnRosterItem);
                }
            }
        }

        /// <summary>
        /// The PresenceManager for this view
        /// </summary>
        [Category("Managers")]
        public PresenceManager PresenceManager
        {
            get
            {
                // If we are running in the designer, let's try to auto-hook a PresenceManager
                if (m_pres == null&& this.DesignMode)
                {
                    IDesignerHost host = (IDesignerHost)base.GetService(typeof(IDesignerHost));
                    this.PresenceManager = (PresenceManager)StreamComponent.GetComponentFromHost(host, typeof(PresenceManager));
                }
                return m_pres;
            }
            set
            {
                if ((object)m_pres == (object)value)
                    return;

                m_pres = value;

                if (m_pres != null)
                    m_pres.OnPrimarySessionChange += new PrimarySessionHandler(m_pres_OnPrimarySessionChange);
            }
        }

        /// <summary>
        /// The PresenceManager for this view
        /// </summary>
        [Category("Managers")]
        public JabberClient Client
        {
            get
            {
                // If we are running in the designer, let's try to auto-hook a JabberClient
                if (m_client == null && this.DesignMode)
                {
                    IDesignerHost host = (IDesignerHost)base.GetService(typeof(IDesignerHost));
                    this.Client = (JabberClient)StreamComponent.GetComponentFromHost(host, typeof(JabberClient));
                }
                return m_client;
            }
            set
            {
                if ((object)m_client == (object)value)
                    return;

                m_client = value;

                if (m_client != null)
                    m_client.OnDisconnect += new ObjectHandler(m_client_OnDisconnect);
            }
        }

        /// <summary>
        /// Color to draw status text with. Not applicable until .Net 2.0.
        /// </summary>
        [Category("Appearance")]
        public Color StatusColor
        {
            get { return m_statusColor; }
            set { m_statusColor = value; }
        }

        /// <summary>
        /// Should we draw status text next to each roster item? Not applicable until .Net 2.0.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(true)]
        public bool DrawStatus
        {
            get
            {
                return this.DrawMode == TreeViewDrawMode.OwnerDrawText;
            }
            set
            {
                if (value)
                    this.DrawMode = TreeViewDrawMode.OwnerDrawText;
                else
                    this.DrawMode = TreeViewDrawMode.Normal;
            }
        }

        /// <summary>
        /// The group names for the roster
        /// </summary>
        public string[] Groups
        {
            get
            {
                String[] g = new String[m_groups.Count];
                m_groups.Keys.CopyTo(g, 0);
                return g;
            }
        }

        /// <summary>
        /// The excluded group names for the roster
        /// </summary>
        public string[] ExcludedGroups
        {
            get
            {
                String[] g = new String[m_excludedGroups.Count];
                m_excludedGroups.Keys.CopyTo(g, 0);

                return g;
            }
        }
        #endregion

        #region CONSTRUCTORS & FINALIZERS
        /// <summary>
        /// Create a new RosterTree
        /// </summary>
        public RosterTree()
        {
            InitializeComponent();
        }
   
        /// <summary>
        /// Turn the erase background message into a null message
        /// </summary>
        /// <param name="message"></param>
        protected override void WndProc(ref System.Windows.Forms.Message message)
        {
            if ((int)0x0014 == message.Msg) //if message is is erase background
            {
                message.Msg = (int)0x0000; //reset message to null
            }

            base.WndProc(ref message);
        }

        /// <summary>
        /// Enables double-buffering which exists for Tree but is hidden
        /// </summary>
        /// <param name="levent"></param>
        protected override void OnLayout(LayoutEventArgs levent)
        {
            base.OnLayout(levent);

            int style = (int)RosterTree.SendMessage(this.Handle, TVM_GETEXTENDEDSTYLE, IntPtr.Zero, IntPtr.Zero);
            style |= TVS_EX_DOUBLEBUFFER;

            RosterTree.SendMessage(this.Handle, TVM_SETEXTENDEDSTYLE, this.Handle, (IntPtr)style);
        }
        #endregion

        #region TREENODE DRAWING
        protected override void OnDrawNode(DrawTreeNodeEventArgs e)
        {
            base.OnDrawNode(e);

            if (e.Node.GetType() == typeof(GroupNode))
                this.DrawGroup(e);
            else if (e.Node.GetType() == typeof(ItemNode))
                this.DrawItem(e);
            else
                e.DrawDefault = true; // or assert(false)
        }

        private void DrawGroup(DrawTreeNodeEventArgs e)
        {
            GroupNode node = e.Node as GroupNode;

            if (node.Total > 0)
            {
                Graphics graphics = e.Graphics;
                Brush statusBrush = new SolidBrush(this.StatusColor);

                SizeF groupNameSize = graphics.MeasureString(node.GroupName, this.Font);

                String counts = String.Format(CultureInfo.CurrentCulture,
                                              " ({0}/{1})",
                                              node.Current,
                                              node.Total);

                graphics.DrawString(counts, this.Font, statusBrush,
                                    new PointF(e.Bounds.Left + groupNameSize.Width, e.Bounds.Top),
                                    StringFormat.GenericTypographic);
            }

            e.DrawDefault = true;
        }

        private void DrawItem(DrawTreeNodeEventArgs e)
        {
            ItemNode node = e.Node as ItemNode;

            if (node.Status != null)
            {
                Graphics graphics = e.Graphics;
                Brush statusBrush = new SolidBrush(this.StatusColor);
                Font statusFont = new Font(this.Font, FontStyle.Italic);

                SizeF nickNameSize = graphics.MeasureString(node.Nickname, this.Font);

                String status = String.Format(CultureInfo.CurrentCulture,
                                              "({0})",
                                              node.Status);

                graphics.DrawString(status, statusFont, statusBrush,
                                    new PointF(e.Bounds.Left + nickNameSize.Width, e.Bounds.Top),
                                    StringFormat.GenericTypographic);
            }

            e.DrawDefault = true;
        }
        #endregion

        #region TREENODE DRAG & DROP
        private GroupNode GetDropGroup(DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(ItemNode)))
                return null;

            Point pt = this.PointToClient(new Point(e.X, e.Y));
            TreeNode node = this.GetNodeAt(pt);

            while (!(node is GroupNode) && node != null)
            {
                node = node.Parent;
            }
            if (node == null)
                return null;

            ItemNode item = e.Data.GetData(typeof(ItemNode)) as ItemNode;

            if (item.Parent == node)
                return null;

            return (GroupNode)node;
        }

        protected override void OnDragDrop(DragEventArgs drgevent)
        {
            base.OnDragDrop(drgevent);

            GroupNode group = this.GetDropGroup(drgevent);

            if (group == null)
                return;

            ItemNode item = drgevent.Data.GetData(typeof(ItemNode)) as ItemNode;
            GroupNode parent = (GroupNode)item.Parent;
            Item i = (Item)item.Item.CloneNode(true, m_client.Document);

            String parentGroupName = this.Client.SupportNestedGroups ? parent.FullPath : parent.GroupName;
            i.RemoveGroup(parentGroupName);

            String groupName = this.Client.SupportNestedGroups ? group.FullPath : group.GroupName;

            if (groupName != this.Unfiled)
                i.AddGroup(groupName);

            m_roster.Modify(i);
        }

        protected override void OnDragOver(DragEventArgs drgevent)
        {
            base.OnDragOver(drgevent);

            if (this.GetDropGroup(drgevent) == null)
                drgevent.Effect = DragDropEffects.None;
            else
                drgevent.Effect = DragDropEffects.Move;
        }

        protected override void OnDragEnter(DragEventArgs drgevent)
        {
            base.OnDragEnter(drgevent);

            if (drgevent.Data.GetDataPresent(typeof(ItemNode)))
                drgevent.Effect = DragDropEffects.Move;
            else
                drgevent.Effect = DragDropEffects.None;
        }

        protected override void OnItemDrag(ItemDragEventArgs e)
        {
            base.OnItemDrag(e);

            if (e.Item is ItemNode)
                this.DoDragDrop(e.Item, DragDropEffects.Move);
        }
        #endregion

        #region TREENODE RENAMING
        protected override void OnBeforeLabelEdit(NodeLabelEditEventArgs e)
        {
            base.OnBeforeLabelEdit(e);

            e.CancelEdit = e.Node.GetType() != typeof(ItemNode);
        }

        protected override void OnAfterLabelEdit(NodeLabelEditEventArgs e)
        {
            base.OnAfterLabelEdit(e);

            if (e.Node.GetType() == typeof(ItemNode) &&
                e.Label != null && e.Label != e.Node.Text)
            {
                ItemNode itemNode = e.Node as ItemNode;

                if (e.Label == itemNode.Item.JID.User)
                    itemNode.Item.Nickname = null;
                else
                    itemNode.Item.Nickname = e.Label;

                RosterIQ riq = new RosterIQ(this.m_client.Document);
                riq.Type = IQType.set;
                riq.Instruction.AddChild(itemNode.Item);

                this.m_client.Write(riq);
            }
        }
        #endregion

        #region TREE COLLAPSING
        /// <summary>
        /// TODO: Documentation RefreshCollapsing
        /// </summary>
        private void RefreshCollapsing()
        {
            foreach (String expandedGroup in this.m_expandedGroups.Keys)
            {
                TreeNode tn = this.FindTreeNode(this.Nodes, expandedGroup);

                if (tn != null)
                    tn.Expand();
            }
        }

        /// <summary>
        /// After a group node is expanded, change to the down-triangle image.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnAfterExpand(TreeViewEventArgs e)
        {
            base.OnAfterExpand(e);

            e.Node.ImageIndex = EXPANDED;
            e.Node.SelectedImageIndex = EXPANDED;

            if (!this.m_expandedGroups.ContainsKey(e.Node.FullPath))
                this.m_expandedGroups.Add(e.Node.FullPath, true);
        }

        /// <summary>
        /// After a group node is collapsed, change to the right-triangle image.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnAfterCollapse(TreeViewEventArgs e)
        {
            base.OnAfterCollapse(e);

            e.Node.ImageIndex = COLLAPSED;
            e.Node.SelectedImageIndex = COLLAPSED;

            if (this.m_expandedGroups.ContainsKey(e.Node.FullPath))
                this.m_expandedGroups.Remove(e.Node.FullPath);
        }
        #endregion

        #region TREE CONSTRUCTION
        /// <summary>
        /// TODO: Documentation ProcessRosterItem
        /// </summary>
        /// <param name="ri"></param>
        private void ProcessRosterItem(Item ri)
        {
            bool remove = ri.Subscription == Subscription.remove;

            LinkedList nodelist = m_items[ri.JID.ToString()] as LinkedList;

            if (nodelist == null)
            {
                // First time through.
                if (!remove)
                {
                    nodelist = new LinkedList();
                    m_items.Add(ri.JID.ToString(), nodelist);
                }
            }
            else
            {
                // update to an existing item. remove all of them, and start over.
                foreach (ItemNode i in nodelist)
                    this.RemoveItemNode(i);

                nodelist.Clear();

                if (remove)
                    m_items.Remove(ri.JID.ToString());
            }

            if (remove)
                return;

            if (this.ExcludeRosterItem(ri))
                return;

            // add the new ones back
            Hashtable ghash = new Hashtable();
            Group[] groups = ri.GetGroups();

            foreach (Group g in groups)
            {
                if (String.IsNullOrEmpty(g.GroupName))
                    g.GroupName = this.Unfiled;
            }

            if (groups.Length == 0)
            {
                groups = new Group[] { new Group(ri.OwnerDocument) };
                groups[0].GroupName = this.Unfiled;
            }

            foreach (Group g in groups)
            {
                // might have the same group twice.
                if (ghash.Contains(g.GroupName))
                    continue;

                ghash.Add(g.GroupName, g);

                if (this.Client.SupportNestedGroups)
                {
                    IEnumerable<String> groupies = this.ConstructNestedGroupList(g.GroupName);
                    Boolean useContinue = false;

                    foreach (String groupie in groupies)
                    {
                        if (this.m_excludedGroups.ContainsKey(groupie))
                        {
                            useContinue = true;
                            break;
                        }
                    }

                    if (useContinue)
                        continue;
                }
                else
                {
                    if (this.m_excludedGroups.ContainsKey(g.GroupName))
                        continue;
                }

                ItemNode i = new ItemNode(ri);
                i.ChangePresence(m_pres[ri.JID]);
                nodelist.Add(i);

                GroupNode gn = this.AddGroupNode(g);
                gn.Nodes.Add(i);
            }
        }

        /// <summary>
        /// TODO: Documentation RemoveItemNode
        /// </summary>
        /// <param name="itemNode"></param>
        private void RemoveItemNode(ItemNode itemNode)
        {
            GroupNode groupNode = itemNode.Parent as GroupNode;
            itemNode.Remove();

            this.RemoveGroupNode(groupNode);
        }

        /// <summary>
        /// TODO: Documentation RemoveGroupNode
        /// </summary>
        /// <param name="groupNode"></param>
        private void RemoveGroupNode(GroupNode groupNode)
        {
            if (groupNode != null && groupNode.Nodes.Count == 0)
            {
                m_groups.Remove(groupNode.FullPath);

                GroupNode groupNodeParent = groupNode.Parent as GroupNode;
                groupNode.Remove();

                this.RemoveGroupNode(groupNodeParent);
            }
        }

        /// <summary>
        /// Exclude an item from the roster based on the filter
        /// </summary>
        /// <param name="ri"></param>
        /// <returns></returns>
        private Boolean ExcludeRosterItem(Item ri)
        {
            Boolean isAvailable = this.PresenceManager.IsAvailable(ri.JID);

            if (String.IsNullOrEmpty(this.Filter))
                return this.ShowOnlyOnline && !isAvailable;

            Boolean excludeItem = true;
            String filter = this.Filter.ToLower(CultureInfo.CurrentCulture);

            if (ri.JID.User.ToLower(CultureInfo.CurrentCulture).Contains(filter))
                excludeItem = false;

            if (excludeItem)
            {
                if (!String.IsNullOrEmpty(ri.Nickname) &&
                    ri.Nickname.ToLower(CultureInfo.CurrentCulture).Contains(filter))
                    excludeItem = false;

                if (excludeItem)
                {
                    foreach (Presence item in this.PresenceManager.GetAll(ri.JID))
                    {
                        if (!String.IsNullOrEmpty(item.From.Resource) &&
                            item.From.Resource.ToLower(CultureInfo.CurrentCulture).Contains(filter))
                            excludeItem = false;
                    }
                }
            }

            if (!excludeItem)
                return this.ShowOnlyOnline && !isAvailable;

            return excludeItem;
        }

        private GroupNode AddGroupNode(Group g)
        {
            GroupNode gn = null;

            if (this.Client.SupportNestedGroups)
            {
                String[] groups = g.GroupName.Split(new String[] { this.Client.NestedGroupDelimiter },
                                                    StringSplitOptions.RemoveEmptyEntries);

                IEnumerable<String> nestedGroupList = this.ConstructNestedGroupList(groups);
                String parent = null;
                Int32 baseNameCounter = 0;

                foreach (String nestedGroup in nestedGroupList)
                {
                    gn = m_groups[nestedGroup] as GroupNode;

                    if (gn == null)
                    {
                        gn = new GroupNode(groups[baseNameCounter]);
                        m_groups.Add(nestedGroup, gn);

                        if (!String.IsNullOrEmpty(parent))
                        {
                            TreeNode tn = this.FindTreeNode(this.Nodes, parent);
                            tn.Nodes.Add(gn);
                        }
                        else
                            this.Nodes.Add(gn);
                    }

                    parent = nestedGroup;
                    baseNameCounter++;
                }
            }
            else
            {
                gn = m_groups[g.GroupName] as GroupNode;

                if (gn == null)
                {
                    gn = new GroupNode(g.GroupName);
                    m_groups.Add(g.GroupName, gn);
                    this.Nodes.Add(gn);
                }
            }
            return gn;
        }
        #endregion

        /// <summary>
        /// Add a new, empty group, if this group doesn't exist, otherwise a no-op.
        /// </summary>
        /// <param name="groupName"></param>
        public TreeNode AddGroup(string groupName)
        {
            Group g = new Group(m_client.Document);
            g.GroupName = groupName;

            return this.AddGroupNode(g);
        }

        /// <summary>
        /// Apply text filter on the roster items usernames, nicknames and resources
        /// </summary>
        public void ApplyFilter()
        {
            this.BeginUpdate();

            foreach (JID item in this.RosterManager)
            {
                this.ProcessRosterItem(this.RosterManager[item]);
            }

            this.RefreshCollapsing();

            this.EndUpdate();
        }

        public void ExcludeGroup(String groupName)
        {
            if (!this.m_excludedGroups.ContainsKey(groupName))
                this.m_excludedGroups.Add(groupName, true);
        }

        public void IncludeGroup(String groupName)
        {
            if (this.m_excludedGroups.ContainsKey(groupName))
                this.m_excludedGroups.Remove(groupName);
        }

        /// <summary>
        /// TODO: Documentation FindTreeNode
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        private TreeNode FindTreeNode(TreeNodeCollection nodes, String fullPath)
        {
            foreach (TreeNode node in nodes)
            {
                if (fullPath.Equals(node.FullPath))
                    return node;

                TreeNode candidate = this.FindTreeNode(node.Nodes, fullPath);

                if (candidate != null)
                    return candidate;
            }

            return null;
        }

        /// <summary>
        /// TODO; Documentation ConstructNestedGroupList
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        private IEnumerable<String> ConstructNestedGroupList(String groupName)
        {
            String[] groups = groupName.Split(new String[] { this.Client.NestedGroupDelimiter },
                                              StringSplitOptions.RemoveEmptyEntries);

            return this.ConstructNestedGroupList(groups);
        }

        /// <summary>
        /// TODO: Documentation ConstructNestedGroupList
        /// </summary>
        /// <param name="groups"></param>
        /// <returns></returns>
        private IEnumerable<String> ConstructNestedGroupList(String[] groups)
        {
            List<String> result = new List<String>();
            String tmp = String.Empty;

            foreach (String group in groups)
            {
                StringBuilder sb = new StringBuilder();

                sb.Append(tmp);
                sb.Append(group);

                result.Add(sb.ToString());

                sb.Append(this.Client.NestedGroupDelimiter);

                tmp = sb.ToString();
            }

            return result;
        }

        #region MISC EVENTS
        /// <summary>
        /// When mousing over a node, show a tooltip with the full JID.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (this.ShowNodeToolTips)
            {
                ItemNode node = this.GetNodeAt(e.X, e.Y) as ItemNode;

                if (node == null)
                { // none selected, or a group
                    tt.SetToolTip(this, String.Empty);
                    return;
                }

                if (node.JID.ToString() != tt.GetToolTip(this))
                {
                    tt.SetToolTip(this, node.JID.ToString());
                }
            }
        }
         
        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            if (e.Button == MouseButtons.Right)
            {
                TreeNode node = this.GetNodeAt(e.Location);

                this.SelectedNode = node;
            }
        }

        private void m_roster_OnRosterBegin(object sender)
        {
            this.BeginUpdate();

            if (this.Client.SupportNestedGroups)
                this.PathSeparator = this.Client.NestedGroupDelimiter;
        }

        private void m_roster_OnRosterItem(object sender, Jabber.Protocol.IQ.Item ri)
        {
            this.ProcessRosterItem(ri);
        }

        private void m_roster_OnRosterEnd(object sender)
        {
            this.RefreshCollapsing();
            this.EndUpdate();
        }

        private void m_client_OnDisconnect(object sender)
        {
            this.Nodes.Clear();
            m_groups.Clear();
            m_items.Clear();
            m_excludedGroups.Clear();

            this.Filter = String.Empty;
        }

        private void m_pres_OnPrimarySessionChange(object sender, JID bare)
        {
            Presence pres = m_pres[bare];
            LinkedList nodelist = m_items[bare.ToString()] as LinkedList;

            if (nodelist == null)
                return;

            foreach (ItemNode n in nodelist)
                n.ChangePresence(pres);

            if (this.ShowOnlyOnline)
                this.ApplyFilter();
        }
        #endregion

        /// <summary>
        /// A TreeNode to hold a Roster Group
        /// </summary>
        public class GroupNode : TreeNode
        {
            /// <summary>
            /// Create a GroupNode
            /// </summary>
            /// <param name="groupName"></param>
            public GroupNode(String groupName)
                : base(groupName, COLLAPSED, COLLAPSED)
            {
                this.GroupName = groupName;
            }

            /// <summary>
            /// The name of the group
            /// </summary>
            public String GroupName { get; private set; }

            /// <summary>
            /// Total number of members of the group
            /// </summary>
            public Int32 Total
            {
                // TODO: what if we're not showing offline?
                get { return this.GetTotalNodes(this.Nodes); }
            }

            /// <summary>
            /// Current number of online members of the group
            /// </summary>
            public Int32 Current
            {
                get
                {
                    Int32 count = 0;

                    foreach (TreeNode i in this.Nodes)
                    {
                        if (i.GetType() == typeof(ItemNode))
                        {
                            if (i.ImageIndex != OFFLINE)
                                count++;
                        }
                        else
                        {
                            count += (i as GroupNode).Current;
                        }
                    }
                    return count;
                }
            }

            /// <summary>
            /// TODO: Documentation GetTotalNodes
            /// </summary>
            /// <param name="nodes"></param>
            /// <returns></returns>
            private Int32 GetTotalNodes(TreeNodeCollection nodes)
            {
                Int32 allNodes = nodes.Count;

                foreach (TreeNode node in nodes)
                {
                    allNodes += this.GetTotalNodes(node.Nodes);

                    if (node as GroupNode != null)
                        allNodes--;
                }

                return allNodes;
            }
        }

        /// <summary>
        /// A TreeNode to hold a RosterItem
        /// </summary>
        public class ItemNode : TreeNode
        {
            /// <summary>
            /// Create an ItemNode
            /// </summary>
            /// <param name="ri">The roster item to create from</param>
            public ItemNode(Item ri)
            {
                this.Item = ri;
                this.Nickname = ri.Nickname;

                if (String.IsNullOrEmpty(this.Nickname))
                {
                    this.Nickname = ri.JID.User;

                    if (String.IsNullOrEmpty(this.Nickname))
                        this.Nickname = ri.JID.ToString(); // punt.
                }
                this.Text = this.Nickname;
            }

            /// <summary>
            /// The JID of this Roster Item
            /// </summary>
            public JID JID { get { return this.Item.JID; } }

            /// <summary>
            /// Roster nickname for this user.
            /// </summary>
            public String Nickname { get; private set; }

            /// <summary>
            /// Last presence status for this item
            /// </summary>
            public String Status { get; private set; }

            /// <summary>
            /// The roster item.  Please make a clone before using it.
            /// </summary>
            public Item Item { get; private set; }

            /// <summary>
            /// Update this roster item with new presence information
            /// </summary>
            /// <param name="p"></param>
            public void ChangePresence(Presence p)
            {
                this.SelectedImageIndex = this.ImageIndex = ItemNode.GetPresenceImage(p);

                if (p == null || String.IsNullOrEmpty(p.Status))
                    this.Status = null;
                else
                    this.Status = p.Status;
            }

            private static Int32 GetPresenceImage(Presence p)
            {
                if (p == null || p.Type == PresenceType.unavailable)
                    return OFFLINE;

                switch (p.Show)
                {
                    case null:
                    case "":
                        return AVAILABLE;
                    case "away":
                        return AWAY;
                    case "xa":
                        return XA;
                    case "dnd":
                        return DND;
                    case "chat":
                        return CHATTY;
                    default:
                        return OFFLINE;
                }
            }
        }
    }
}