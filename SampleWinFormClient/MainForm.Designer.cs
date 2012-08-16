
namespace SampleWinFormClient
{
    public partial class MainForm
    {

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.sb = new System.Windows.Forms.StatusBar();
            this.pnlCon = new System.Windows.Forms.StatusBarPanel();
            this.pnlSSL = new System.Windows.Forms.StatusBarPanel();
            this.pnlPresence = new System.Windows.Forms.StatusBarPanel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tpRoster = new System.Windows.Forms.TabPage();
            this.roster = new Muzzle.Controls.RosterTree();
            this.jc = new Jabber.Client.JabberClient(this.components);
            this.pm = new Jabber.Client.PresenceManager(this.components);
            this.cm = new Jabber.Connection.CapsManager(this.components);
            this.dm = new Jabber.Connection.DiscoManager(this.components);
            this.rm = new Jabber.Client.RosterManager(this.components);
            this.tpServices = new System.Windows.Forms.TabPage();
            this.services = new SampleWinFormClient.Controls.ServiceDisplay();
            this.tpBookmarks = new System.Windows.Forms.TabPage();
            this.lvBookmarks = new System.Windows.Forms.ListView();
            this.chName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chNick = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chAutoJoin = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tpDebug = new System.Windows.Forms.TabPage();
            this.debug = new Muzzle.Controls.XmppDebugger();
            this.idler = new Bedrock.Util.IdleTime();
            this.psm = new Jabber.Connection.PubSubManager(this.components);
            this.muc = new Jabber.Connection.ConferenceManager(this.components);
            this.bmm = new Jabber.Client.BookmarkManager(this.components);
            this.mnuPresence = new System.Windows.Forms.ContextMenu();
            this.mnuAvailable = new System.Windows.Forms.MenuItem();
            this.mnuAway = new System.Windows.Forms.MenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.joinConferenceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.servicesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.debugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rosterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addContactToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeContactToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addGroupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bookmarkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.windowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeTabToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pubSubToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.subscribePubSubToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deletePubSubToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.pnlCon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlSSL)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlPresence)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tpRoster.SuspendLayout();
            this.tpServices.SuspendLayout();
            this.tpBookmarks.SuspendLayout();
            this.tpDebug.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // sb
            // 
            this.sb.Location = new System.Drawing.Point(0, 416);
            this.sb.Name = "sb";
            this.sb.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
            this.pnlCon,
            this.pnlSSL,
            this.pnlPresence});
            this.sb.ShowPanels = true;
            this.sb.Size = new System.Drawing.Size(632, 22);
            this.sb.TabIndex = 0;
            this.sb.PanelClick += new System.Windows.Forms.StatusBarPanelClickEventHandler(this.sb_PanelClick);
            // 
            // pnlCon
            // 
            this.pnlCon.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring;
            this.pnlCon.Name = "pnlCon";
            this.pnlCon.Text = "Click on \"Offline\", and select a presence to log in.";
            this.pnlCon.Width = 539;
            // 
            // pnlSSL
            // 
            this.pnlSSL.Alignment = System.Windows.Forms.HorizontalAlignment.Center;
            this.pnlSSL.Name = "pnlSSL";
            this.pnlSSL.Width = 30;
            // 
            // pnlPresence
            // 
            this.pnlPresence.Alignment = System.Windows.Forms.HorizontalAlignment.Right;
            this.pnlPresence.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
            this.pnlPresence.Name = "pnlPresence";
            this.pnlPresence.Text = "Offline";
            this.pnlPresence.Width = 47;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tpRoster);
            this.tabControl1.Controls.Add(this.tpServices);
            this.tabControl1.Controls.Add(this.tpBookmarks);
            this.tabControl1.Controls.Add(this.tpDebug);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 24);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(632, 392);
            this.tabControl1.TabIndex = 2;
            // 
            // tpRoster
            // 
            this.tpRoster.Controls.Add(this.roster);
            this.tpRoster.Location = new System.Drawing.Point(4, 22);
            this.tpRoster.Name = "tpRoster";
            this.tpRoster.Size = new System.Drawing.Size(624, 366);
            this.tpRoster.TabIndex = 1;
            this.tpRoster.Text = "Roster";
            this.tpRoster.UseVisualStyleBackColor = true;
            // 
            // roster
            // 
            this.roster.AllowDrop = true;
            this.roster.Client = this.jc;
            this.roster.Dock = System.Windows.Forms.DockStyle.Fill;
            this.roster.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawText;
            this.roster.ImageIndex = 1;
            this.roster.Location = new System.Drawing.Point(0, 0);
            this.roster.Name = "roster";
            this.roster.PresenceManager = this.pm;
            this.roster.RosterManager = this.rm;
            this.roster.SelectedImageIndex = 0;
            this.roster.ShowLines = false;
            this.roster.ShowRootLines = false;
            this.roster.Size = new System.Drawing.Size(624, 366);
            this.roster.Sorted = true;
            this.roster.StatusColor = System.Drawing.Color.Teal;
            this.roster.TabIndex = 0;
            this.roster.Unfiled = null;
            this.roster.DoubleClick += new System.EventHandler(this.roster_DoubleClick);
            // 
            // jc
            // 
            this.jc.AutoReconnect = 3F;
            this.jc.AutoStartCompression = true;
            this.jc.AutoStartTLS = true;
            this.jc.InvokeControl = this;
            this.jc.KeepAlive = 30F;
            this.jc.LocalCertificate = null;
            this.jc.Password = null;
            this.jc.User = null;
            this.jc.OnMessage += new Jabber.Client.MessageHandler(this.jc_OnMessage);
            this.jc.OnIQ += new Jabber.Client.IQHandler(this.jc_OnIQ);
            this.jc.OnAuthError += new Jabber.Protocol.ProtocolHandler(this.jc_OnAuthError);
            this.jc.OnRegistered += new Jabber.Client.IQHandler(this.jc_OnRegistered);
            this.jc.OnRegisterInfo += new Jabber.Client.RegisterInfoHandler(this.jc_OnRegisterInfo);
            this.jc.OnError += new Bedrock.ExceptionHandler(this.jc_OnError);
            this.jc.OnStreamError += new Jabber.Protocol.ProtocolHandler(this.jc_OnStreamError);
            this.jc.OnAuthenticate += new Bedrock.ObjectHandler(this.jc_OnAuthenticate);
            this.jc.OnConnect += new Jabber.Connection.StanzaStreamHandler(this.jc_OnConnect);
            this.jc.OnDisconnect += new Bedrock.ObjectHandler(this.jc_OnDisconnect);
            // 
            // pm
            // 
            this.pm.CapsManager = this.cm;
            this.pm.OverrideFrom = null;
            this.pm.Stream = this.jc;
            // 
            // cm
            // 
            this.cm.DiscoManager = this.dm;
            this.cm.Features = new string[0];
            this.cm.Identities = new Jabber.Connection.Ident[0];
            this.cm.Node = "http://cursive.net/clients/csharp-example";
            this.cm.OverrideFrom = null;
            this.cm.Stream = this.jc;
            // 
            // dm
            // 
            this.dm.OverrideFrom = null;
            this.dm.Stream = this.jc;
            // 
            // rm
            // 
            this.rm.AutoAllow = Jabber.Client.AutoSubscriptionHanding.AllowIfSubscribed;
            this.rm.AutoSubscribe = true;
            this.rm.OverrideFrom = null;
            this.rm.Stream = this.jc;
            this.rm.OnRosterEnd += new Bedrock.ObjectHandler(this.rm_OnRosterEnd);
            this.rm.OnSubscription += new Jabber.Client.SubscriptionHandler(this.rm_OnSubscription);
            this.rm.OnUnsubscription += new Jabber.Client.UnsubscriptionHandler(this.rm_OnUnsubscription);
            // 
            // tpServices
            // 
            this.tpServices.Controls.Add(this.services);
            this.tpServices.Location = new System.Drawing.Point(4, 22);
            this.tpServices.Name = "tpServices";
            this.tpServices.Size = new System.Drawing.Size(624, 366);
            this.tpServices.TabIndex = 2;
            this.tpServices.Text = "Services";
            this.tpServices.UseVisualStyleBackColor = true;
            // 
            // services
            // 
            this.services.DiscoManager = this.dm;
            this.services.Dock = System.Windows.Forms.DockStyle.Fill;
            this.services.ImageList = null;
            this.services.Location = new System.Drawing.Point(0, 0);
            this.services.Name = "services";
            this.services.Size = new System.Drawing.Size(624, 366);
            this.services.Stream = this.jc;
            this.services.TabIndex = 0;
            // 
            // tpBookmarks
            // 
            this.tpBookmarks.Controls.Add(this.lvBookmarks);
            this.tpBookmarks.Location = new System.Drawing.Point(4, 22);
            this.tpBookmarks.Name = "tpBookmarks";
            this.tpBookmarks.Size = new System.Drawing.Size(624, 366);
            this.tpBookmarks.TabIndex = 3;
            this.tpBookmarks.Text = "Bookmarks";
            this.tpBookmarks.UseVisualStyleBackColor = true;
            // 
            // lvBookmarks
            // 
            this.lvBookmarks.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chName,
            this.chNick,
            this.chAutoJoin});
            this.lvBookmarks.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvBookmarks.Location = new System.Drawing.Point(0, 0);
            this.lvBookmarks.Name = "lvBookmarks";
            this.lvBookmarks.Size = new System.Drawing.Size(624, 366);
            this.lvBookmarks.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lvBookmarks.TabIndex = 0;
            this.lvBookmarks.UseCompatibleStateImageBehavior = false;
            this.lvBookmarks.View = System.Windows.Forms.View.Details;
            this.lvBookmarks.DoubleClick += new System.EventHandler(this.lvBookmarks_DoubleClick);
            this.lvBookmarks.KeyUp += new System.Windows.Forms.KeyEventHandler(this.lvBookmarks_KeyUp);
            // 
            // chName
            // 
            this.chName.Text = "Room";
            this.chName.Width = 198;
            // 
            // chNick
            // 
            this.chNick.Text = "Nick";
            this.chNick.Width = 88;
            // 
            // chAutoJoin
            // 
            this.chAutoJoin.Text = "AutoJoin";
            // 
            // tpDebug
            // 
            this.tpDebug.Controls.Add(this.debug);
            this.tpDebug.Location = new System.Drawing.Point(4, 22);
            this.tpDebug.Name = "tpDebug";
            this.tpDebug.Size = new System.Drawing.Size(624, 366);
            this.tpDebug.TabIndex = 0;
            this.tpDebug.Text = "Debug";
            this.tpDebug.UseVisualStyleBackColor = true;
            // 
            // debug
            // 
            this.debug.Dock = System.Windows.Forms.DockStyle.Fill;
            this.debug.ErrorColor = System.Drawing.Color.Red;
            this.debug.Location = new System.Drawing.Point(0, 0);
            this.debug.Name = "debug";
            this.debug.OtherColor = System.Drawing.Color.Green;
            this.debug.OverrideFrom = null;
            this.debug.ReceiveColor = System.Drawing.Color.Orange;
            this.debug.SendColor = System.Drawing.Color.Blue;
            this.debug.Size = new System.Drawing.Size(624, 366);
            this.debug.Stream = this.jc;
            this.debug.TabIndex = 0;
            this.debug.TextColor = System.Drawing.Color.Black;
            // 
            // idler
            // 
            this.idler.InvokeControl = this;
            this.idler.OnIdle += new Bedrock.Util.SpanEventHandler(this.idler_OnIdle);
            this.idler.OnUnIdle += new Bedrock.Util.SpanEventHandler(this.idler_OnUnIdle);
            // 
            // psm
            // 
            this.psm.OverrideFrom = null;
            this.psm.Stream = this.jc;
            // 
            // muc
            // 
            this.muc.OverrideFrom = null;
            this.muc.Stream = this.jc;
            // 
            // bmm
            // 
            this.bmm.ConferenceManager = this.muc;
            this.bmm.OverrideFrom = null;
            this.bmm.Stream = this.jc;
            this.bmm.OnConferenceAdd += new Jabber.Client.BookmarkConferenceDelegate(this.bmm_OnConferenceAdd);
            this.bmm.OnConferenceRemove += new Jabber.Client.BookmarkConferenceDelegate(this.bmm_OnConferenceRemove);
            // 
            // mnuPresence
            // 
            this.mnuPresence.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuAvailable,
            this.mnuAway});
            // 
            // mnuAvailable
            // 
            this.mnuAvailable.Enabled = false;
            this.mnuAvailable.Index = 0;
            this.mnuAvailable.Shortcut = System.Windows.Forms.Shortcut.CtrlO;
            this.mnuAvailable.Text = "&Available";
            this.mnuAvailable.Click += new System.EventHandler(this.mnuAvailable_Click);
            // 
            // mnuAway
            // 
            this.mnuAway.Enabled = false;
            this.mnuAway.Index = 1;
            this.mnuAway.Shortcut = System.Windows.Forms.Shortcut.CtrlA;
            this.mnuAway.Text = "A&way";
            this.mnuAway.Click += new System.EventHandler(this.mnuAway_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.rosterToolStripMenuItem,
            this.bookmarkToolStripMenuItem,
            this.windowToolStripMenuItem,
            this.pubSubToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(632, 24);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.connectToolStripMenuItem,
            this.joinConferenceToolStripMenuItem,
            this.toolStripMenuItem1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // connectToolStripMenuItem
            // 
            this.connectToolStripMenuItem.Name = "connectToolStripMenuItem";
            this.connectToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F9;
            this.connectToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.connectToolStripMenuItem.Text = "&Connect";
            this.connectToolStripMenuItem.Click += new System.EventHandler(this.connectToolStripMenuItem_Click);
            // 
            // joinConferenceToolStripMenuItem
            // 
            this.joinConferenceToolStripMenuItem.Name = "joinConferenceToolStripMenuItem";
            this.joinConferenceToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.J)));
            this.joinConferenceToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.joinConferenceToolStripMenuItem.Text = "&Join Conference";
            this.joinConferenceToolStripMenuItem.Click += new System.EventHandler(this.joinConferenceToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(186, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Q)));
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.servicesToolStripMenuItem,
            this.debugToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(41, 20);
            this.viewToolStripMenuItem.Text = "&View";
            // 
            // servicesToolStripMenuItem
            // 
            this.servicesToolStripMenuItem.Name = "servicesToolStripMenuItem";
            this.servicesToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F8;
            this.servicesToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.servicesToolStripMenuItem.Text = "&Services";
            this.servicesToolStripMenuItem.Click += new System.EventHandler(this.servicesToolStripMenuItem_Click);
            // 
            // debugToolStripMenuItem
            // 
            this.debugToolStripMenuItem.Name = "debugToolStripMenuItem";
            this.debugToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F12;
            this.debugToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.debugToolStripMenuItem.Text = "&Debug";
            this.debugToolStripMenuItem.Click += new System.EventHandler(this.debugToolStripMenuItem_Click);
            // 
            // rosterToolStripMenuItem
            // 
            this.rosterToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addContactToolStripMenuItem,
            this.removeContactToolStripMenuItem,
            this.addGroupToolStripMenuItem});
            this.rosterToolStripMenuItem.Name = "rosterToolStripMenuItem";
            this.rosterToolStripMenuItem.Size = new System.Drawing.Size(51, 20);
            this.rosterToolStripMenuItem.Text = "&Roster";
            // 
            // addContactToolStripMenuItem
            // 
            this.addContactToolStripMenuItem.Name = "addContactToolStripMenuItem";
            this.addContactToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Insert;
            this.addContactToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.addContactToolStripMenuItem.Text = "&Add Contact";
            this.addContactToolStripMenuItem.Click += new System.EventHandler(this.menuItem3_Click);
            // 
            // removeContactToolStripMenuItem
            // 
            this.removeContactToolStripMenuItem.Name = "removeContactToolStripMenuItem";
            this.removeContactToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.removeContactToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.removeContactToolStripMenuItem.Text = "&Remove Contact";
            this.removeContactToolStripMenuItem.Click += new System.EventHandler(this.menuItem5_Click);
            // 
            // addGroupToolStripMenuItem
            // 
            this.addGroupToolStripMenuItem.Name = "addGroupToolStripMenuItem";
            this.addGroupToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.G)));
            this.addGroupToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.addGroupToolStripMenuItem.Text = "&Add Group";
            this.addGroupToolStripMenuItem.Click += new System.EventHandler(this.addGroupToolStripMenuItem_Click);
            // 
            // bookmarkToolStripMenuItem
            // 
            this.bookmarkToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToolStripMenuItem,
            this.removeToolStripMenuItem});
            this.bookmarkToolStripMenuItem.Name = "bookmarkToolStripMenuItem";
            this.bookmarkToolStripMenuItem.Size = new System.Drawing.Size(65, 20);
            this.bookmarkToolStripMenuItem.Text = "Bookmark";
            // 
            // addToolStripMenuItem
            // 
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            this.addToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.B)));
            this.addToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.addToolStripMenuItem.Text = "Add";
            this.addToolStripMenuItem.Click += new System.EventHandler(this.addToolStripMenuItem_Click);
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.B)));
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.removeToolStripMenuItem.Text = "Remove";
            this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
            // 
            // windowToolStripMenuItem
            // 
            this.windowToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.closeTabToolStripMenuItem});
            this.windowToolStripMenuItem.Name = "windowToolStripMenuItem";
            this.windowToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
            this.windowToolStripMenuItem.Text = "&Window";
            // 
            // closeTabToolStripMenuItem
            // 
            this.closeTabToolStripMenuItem.Name = "closeTabToolStripMenuItem";
            this.closeTabToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.W)));
            this.closeTabToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.closeTabToolStripMenuItem.Text = "&Close Tab";
            this.closeTabToolStripMenuItem.Click += new System.EventHandler(this.closeTabToolStripMenuItem_Click);
            // 
            // pubSubToolStripMenuItem
            // 
            this.pubSubToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.subscribePubSubToolStripMenuItem,
            this.deletePubSubToolStripMenuItem});
            this.pubSubToolStripMenuItem.Name = "pubSubToolStripMenuItem";
            this.pubSubToolStripMenuItem.Size = new System.Drawing.Size(55, 20);
            this.pubSubToolStripMenuItem.Text = "PubSub";
            // 
            // subscribePubSubToolStripMenuItem
            // 
            this.subscribePubSubToolStripMenuItem.Name = "subscribePubSubToolStripMenuItem";
            this.subscribePubSubToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F10;
            this.subscribePubSubToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.subscribePubSubToolStripMenuItem.Text = "&Subscribe";
            this.subscribePubSubToolStripMenuItem.Click += new System.EventHandler(this.subscribeToPubSubToolStripMenuItem_Click);
            // 
            // deletePubSubToolStripMenuItem
            // 
            this.deletePubSubToolStripMenuItem.Name = "deletePubSubToolStripMenuItem";
            this.deletePubSubToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F11;
            this.deletePubSubToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.deletePubSubToolStripMenuItem.Text = "&Delete";
            this.deletePubSubToolStripMenuItem.Click += new System.EventHandler(this.deletePubSubToolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(632, 438);
            this.ContextMenu = this.mnuPresence;
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.sb);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.MainForm_Closing);
            ((System.ComponentModel.ISupportInitialize)(this.pnlCon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlSSL)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlPresence)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tpRoster.ResumeLayout(false);
            this.tpServices.ResumeLayout(false);
            this.tpBookmarks.ResumeLayout(false);
            this.tpDebug.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private Controls.ServiceDisplay services;
        private Bedrock.Util.IdleTime idler;
        private Muzzle.Controls.RosterTree roster;
        private Muzzle.Controls.XmppDebugger debug;
        private Jabber.Connection.DiscoManager dm;
        private Jabber.Connection.CapsManager cm;
        private Jabber.Connection.PubSubManager psm;
        private Jabber.Connection.ConferenceManager muc;
        private Jabber.Client.JabberClient jc;
        private Jabber.Client.RosterManager rm;
        private Jabber.Client.PresenceManager pm;
        private Jabber.Client.BookmarkManager bmm;

        private System.Windows.Forms.StatusBar sb;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tpDebug;
        private System.Windows.Forms.TabPage tpRoster;
        private System.Windows.Forms.StatusBarPanel pnlCon;
        private System.Windows.Forms.StatusBarPanel pnlPresence;
        private System.Windows.Forms.ContextMenu mnuPresence;
        private System.Windows.Forms.MenuItem mnuAvailable;
        private System.Windows.Forms.MenuItem mnuAway;
        private System.Windows.Forms.StatusBarPanel pnlSSL;

        private System.Windows.Forms.TabPage tpServices;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem connectToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem servicesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem debugToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rosterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addContactToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeContactToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addGroupToolStripMenuItem;


        private System.Windows.Forms.ToolStripMenuItem windowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeTabToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deletePubSubToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem subscribePubSubToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pubSubToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem joinConferenceToolStripMenuItem;

        private System.Windows.Forms.TabPage tpBookmarks;
        private System.Windows.Forms.ListView lvBookmarks;
        private System.Windows.Forms.ColumnHeader chName;
        private System.Windows.Forms.ColumnHeader chNick;
        private System.Windows.Forms.ColumnHeader chAutoJoin;
        private System.Windows.Forms.ToolStripMenuItem bookmarkToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
        private System.ComponentModel.IContainer components;
    }
}