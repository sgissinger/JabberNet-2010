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
using System.Drawing;
using System.Globalization;
using Jabber;
using Jabber.Protocol.Client;

namespace Muzzle.Controls
{
    /// <summary>
    /// Keep track of the history of a conversation or room.
    /// </summary>
    public partial class ChatHistory : BottomScrollRichText
    {
        #region MEMBERS
        private Color m_sendColor = Color.Blue;
        private Color m_recvColor = Color.Red;
        private Color m_actionColor = Color.Purple;
        private Color m_presenceColor = Color.Green;
        private Int32 m_lastMessageDuration = 15;
        #endregion

        #region PROPERTIES
        /// <summary>
        /// What color to use for the sent messages header
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(typeof(Color), "Blue")]
        public Color SendColor
        {
            get { return m_sendColor; }
            set { m_sendColor = value; }
        }
        /// <summary>
        /// What color to use for the received messages header
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(typeof(Color), "Red")]
        public Color ReceiveColor
        {
            get { return m_recvColor; }
            set { m_recvColor = value; }
        }
        /// <summary>
        /// What color to use for the action messages
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(typeof(Color), "Purple")]
        public Color ActionColor
        {
            get { return m_actionColor; }
            set { m_actionColor = value; }
        }
        /// <summary>
        /// What color to use for the presence messages
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(typeof(Color), "Green")]
        public Color PresenceColor
        {
            get { return m_presenceColor; }
            set { m_presenceColor = value; }
        }
        /// <summary>
        /// Minimum of time in minutes between two peer header's message mandatory printing.
        /// </summary>
        [DefaultValue(15)]
        [Description("Gets or sets the minimum of time in minutes between two header's message mandatory printing.")]
        public Int32 LastMessageDuration
        {
            get { return m_lastMessageDuration; }
            set { m_lastMessageDuration = value; }
        }
        /// <summary>
        /// JID for the associated user. If null, the resource will be used (e.g. MUC)
        /// </summary>
        [Description("Gets or sets the JID who is associated to the peer in this History.")]
        public JID PeerJID { get; set; }
        /// <summary>
        /// Nickname for the associated user. If null, the resource will be used (e.g. MUC)
        /// </summary>
        [Description("Gets or sets the nickname who is associated to the peer in this History.")]
        public string PeerNickname { get; set; }
        /// <summary>
        /// Nickname for the self user. If null, 'Me' will be used
        /// </summary>
        [Description("Gets or sets the nickname who is associated to self in this History.")]
        public string SelfNickname { get; set; }
        /// <summary>
        /// TODO: Documentation Property
        /// </summary>
        private Boolean IsFirstLine { get; set; }
        /// <summary>
        /// TODO: Documentation Property
        /// </summary>
        private Boolean SelfIsLast { get; set; }
        /// <summary>
        /// TODO: Documentation Property
        /// </summary>
        private DateTime SelfLastMessageTimeout { get; set; }
        /// <summary>
        /// TODO: Documentation Property
        /// </summary>
        private DateTime PeerLastMessageTimeout { get; set; }
        #endregion

        #region CONSTRUCTORS & FINALIZERS
        /// <summary>
        /// Create. Make sure to set Client and From, at least.
        /// </summary>
        public ChatHistory()
        {
            InitializeComponent();

            this.IsFirstLine = true;
        }
        #endregion

        #region METHODS
        /// <summary>
        /// Insert the given message into the history.  The timestamp on the message will be used, if
        /// included, otherwise the current time will be used.
        /// Messages without bodies will be ignored.
        /// </summary>
        /// <param name="msg"></param>
        public void InsertMessage(Message msg)
        {
            string body = msg.Body;

            if (body == null)
                return;  // typing indicator, e.g.

            string nick = String.IsNullOrEmpty(this.PeerNickname) ? msg.From.Resource : this.PeerNickname;
            string tag = String.Empty;

            if (this.SelfIsLast || this.IsFirstLine || DateTime.Now > this.PeerLastMessageTimeout)
            {
                tag = String.Format(CultureInfo.CurrentCulture,
                                    @"[{0:t}] {1}\line",
                                    DateTime.Now,
                                    nick);
            }

            this.SelfIsLast = false;
            this.PeerLastMessageTimeout = DateTime.Now.AddMinutes(this.LastMessageDuration);

            this.IsFirstLine = false;
            this.AppendMaybeScroll(m_recvColor, tag, body);
        }

        /// <summary>
        /// We sent some text; insert it.
        /// </summary>
        /// <param name="text"></param>
        public void InsertSend(string text)
        {
            string nick = String.IsNullOrEmpty(this.SelfNickname) ? "Me" : this.SelfNickname;
            string tag = String.Empty;

            if (!this.SelfIsLast || this.IsFirstLine || DateTime.Now > this.SelfLastMessageTimeout)
            {
                tag = String.Format(CultureInfo.CurrentCulture,
                                    @"[{0:t}] {1}\line",
                                    DateTime.Now,
                                    nick);
            }

            this.SelfIsLast = true;
            this.SelfLastMessageTimeout = DateTime.Now.AddMinutes(this.LastMessageDuration);

            this.IsFirstLine = false;
            this.AppendMaybeScroll(m_sendColor, tag, text);
        }

        /// <summary>
        /// Add this method to the OnPresence event of a jabberClient
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="pres"></param>
        public void OnPresence(object sender, Presence pres)
        {
            if (!String.IsNullOrEmpty(pres.Status) &&
                pres.From.BareJID == this.PeerJID.BareJID)
            {
                string nick = String.IsNullOrEmpty(this.PeerNickname) ? pres.From.Resource : this.PeerNickname;

                this.AppendMaybeScroll(m_presenceColor, String.Format(CultureInfo.CurrentCulture,
                                                                      @"[{0:t}] {1} - {2}",
                                                                      DateTime.Now,
                                                                      nick,
                                                                      pres.Status), String.Empty);
                this.IsFirstLine = true;
            }
        }
        #endregion
    }
}