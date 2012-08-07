/* --------------------------------------------------------------------------
 * Copyrights
 *
 * Portions created by or assigned to Sébastien Gissinger
 *
 * License
 *
 * Jabber-Net is licensed under the LGPL.
 * See LICENSE.txt for details.
 * --------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Net;
using System.Net.Sockets;
using System.Xml;
using Jabber.Connection;
using Jabber.Protocol;
using Jabber.Protocol.Client;
using Jabber.Protocol.IQ;
using Jabber.Stun;
using Jabber.Stun.Attributes;

namespace Jabber.Client
{
    public delegate void JingleDescriptionHandler(object sender, XmlDocument ownerDoc, out Element jingleDescription, out String contentName);

    public delegate void JingleIceCandidatesHandler(object sender, JingleIce jingleIce, IPEndPoint hostEP, TurnAllocation allocation);

    public delegate void TurnStartHandler(object sender, IPEndPoint peerEP, String sid);

    public delegate void HolePunchSucceedHandler(object sender, Socket connectedSocket, Jingle jingle);

    public delegate void TurnConnectionBindSucceedHandler(object sender, Socket connectedSocket, String sid, JID recipient);

    /// <summary>
    /// TODO: Documentation Class
    /// </summary>
    public partial class JingleIceManager : StreamComponent
    {
        #region MEMBERS
        private static Object processLocker = new Object();

        private JingleManager jingleManager = null;
        private HolePuncher holePuncher = null;
        private Boolean p2pConnectionSucceed = false;
        private Dictionary<String, TurnSession> turnSessions = new Dictionary<String, TurnSession>();
        #endregion

        #region EVENTS
        public event EventHandler OnBeforeInitiateSessionAllocate;
        public event EventHandler OnBeforeAcceptSessionAllocate;
        public event HolePunchSucceedHandler OnHolePunchSucceed;
        public event TurnStartHandler OnTurnStart;
        public event TurnConnectionBindSucceedHandler OnTurnConnectionBindSucceed;
        public event JingleDescriptionHandler OnJingleDescription;
        public event JingleIceCandidatesHandler OnJingleIceCandidates;
        #endregion

        #region PROPERTIES
        private ActionType StartingSessionAction { get; set; }
        private JID StartingSessionRecipient { get; set; }
        private String StartingSessionSid { get; set; }

        public String StunServerIP { get; set; }
        public Int32 StunServerPort { get; set; }

        /// <summary>
        /// TODO: Documentation Property
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IPEndPoint StunServerEP
        {
            get { return new IPEndPoint(IPAddress.Parse(this.StunServerIP), this.StunServerPort); }
        }

        public Boolean TurnSupported { get; set; }
        public String TurnUsername { get; set; }
        public String TurnPassword { get; set; }

        /// <summary>
        /// The JingleManager for this view
        /// </summary>
        [Category("Jabber")]
        public JingleManager JingleManager
        {
            get
            {
                // If we are running in the designer, let's try to auto-hook a JingleManager
                if ((this.jingleManager == null) && this.DesignMode)
                {
                    IDesignerHost host = (IDesignerHost)base.GetService(typeof(IDesignerHost));
                    this.JingleManager = (JingleManager)StreamComponent.GetComponentFromHost(host, typeof(JingleManager));
                }

                return this.jingleManager;
            }
            set
            {
                if ((object)this.jingleManager != (object)value)
                {
                    this.jingleManager = value;

                    this.jingleManager.OnReceivedSessionInitiate += new IQHandler(jingleManager_OnReceivedSessionInitiate);
                    this.jingleManager.OnReceivedSessionAccept += new IQHandler(jingleManager_OnReceivedSessionAccept);
                    this.jingleManager.OnReceivedTransportInfo += new IQHandler(jingleManager_OnReceivedTransportInfo);
                }
            }
        }
        #endregion

        #region CONSTRUCTORS & FINALIZERS
        /// <summary>
        /// TODO: Documentation Constructor
        /// </summary>
        public JingleIceManager()
        {
            InitializeComponent();
        }

        /// <summary>
        /// TODO: Documentation Constructor
        /// </summary>
        /// <param name="container"></param>
        public JingleIceManager(IContainer container)
            : this()
        {
            container.Add(this);
        }
        #endregion

        #region METHODS
        /// <summary>
        /// TODO: Documentation CheckConnectivity
        /// </summary>
        /// <param name="sid"></param>
        private void CheckConnectivity(String sid)
        {
            this.holePuncher = new HolePuncher(this.turnSessions[sid].TurnManager.HostEP, sid);

            JingleSession jingleSession = this.JingleManager.FindSession(sid);

            JingleContent jingleContent = jingleSession.Remote.GetContent(0);

            JingleIce jingleIce = jingleContent.GetElement<JingleIce>(0);

            foreach (JingleIceCandidate candidate in jingleIce.GetCandidates())
            {
                switch (candidate.Type)
                {
                    case IceCandidateType.host:
                    case IceCandidateType.prflx:
                    case IceCandidateType.srflx:
                        this.holePuncher.AddEP(candidate.Priority, candidate.EndPoint);
                        break;

                    case IceCandidateType.relay:
                        if (this.TurnSupported &&
                            jingleSession.Remote.Action == ActionType.session_accept)
                        {
                                this.turnSessions[sid].TurnManager.CreatePermission(new XorMappedAddress(candidate.RelatedEndPoint),
                                                                                    this.turnSessions[sid].TurnAllocation);
                        }
                        break;
                }
            }

            if (!this.holePuncher.CanStart && this.TurnSupported)
            {
                this.p2pConnectionSucceed = true;

                this.StartTurnPeer(sid);
            }
            else
            {
                this.p2pConnectionSucceed = false;

                this.holePuncher.StartTcpPunch(this.HolePunchSuccess, this.HolePunchFailure);
            }
        }

        /// <summary>
        /// TODO: Documentation HolePunchSuccess
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="connectedSocket"></param>
        /// <param name="punchData"></param>
        private void HolePunchSuccess(object sender, Socket connectedSocket, Object punchData)
        {
            lock (JingleIceManager.processLocker)
            {
                this.p2pConnectionSucceed = true;

                JingleSession jingleSession = this.JingleManager.FindSession(punchData as String);

                this.OnHolePunchSucceed(this, connectedSocket, jingleSession.Remote);

                this.DestroyTurnSession(punchData as String);
                this.StartingSessionSid = null;
                this.StartingSessionRecipient = null;
            }
        }

        /// <summary>
        /// TODO: Documentation HolePunchFailure
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="punchData"></param>
        private void HolePunchFailure(object sender, Object punchData)
        {
            lock (JingleIceManager.processLocker)
            {
                if (!this.p2pConnectionSucceed && this.TurnSupported)
                    this.StartTurnPeer(punchData as String);
            }
        }
        #endregion

        #region JINGLE
        /// <summary>
        /// TODO: Documentation InitiateSession
        /// </summary>
        /// <param name="to"></param>
        public void InitiateSession(JID to)
        {
            if (this.StartingSessionRecipient == null)
            {
                this.StartingSessionRecipient = to;
                this.StartingSessionSid = JingleUtilities.GenerateSid;
                this.StartingSessionAction = ActionType.session_initiate;

                if (this.OnBeforeInitiateSessionAllocate != null)
                    this.OnBeforeInitiateSessionAllocate(this, new EventArgs());

                if (this.TurnSupported)
                {
                    var t = this.CreateTurnSession(this.StartingSessionSid);

                    t.Allocate(this.TurnUsername, this.TurnPassword);
                }
                else
                {
                    throw new NotSupportedException();
                }
            }
        }

        /// <summary>
        /// TODO: Documentation jingleManager_OnReceivedSessionInitiate
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="iq"></param>
        private void jingleManager_OnReceivedSessionInitiate(object sender, IQ iq)
        {
            if (this.StartingSessionRecipient == null)
            {
                this.StartingSessionRecipient = iq.From;

                Jingle jingle = iq.Query as Jingle;

                this.StartingSessionSid = jingle.Sid;
                this.StartingSessionAction = ActionType.session_accept;

                if (this.OnBeforeAcceptSessionAllocate != null)
                    this.OnBeforeAcceptSessionAllocate(this, new EventArgs());

                if (this.TurnSupported)
                {
                    var t = this.CreateTurnSession(this.StartingSessionSid);

                    t.Allocate(this.TurnUsername, this.TurnPassword);
                }
                else
                {
                    throw new NotSupportedException();
                }

                iq.Handled = true;
            }
        }

        /// <summary>
        /// TODO: Documentation jingleManager_OnReceivedSessionAccept
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="iq"></param>
        private void jingleManager_OnReceivedSessionAccept(object sender, IQ iq)
        {
            Jingle jingle = iq.Query as Jingle;

            JingleIQ jingleIq = new JingleIQ(new XmlDocument());

            jingleIq.From = this.Stream.JID;
            jingleIq.To = iq.From;
            jingleIq.Type = IQType.set;
            jingleIq.Instruction.Action = ActionType.transport_info;
            jingleIq.Instruction.Sid = jingle.Sid;

            JingleContent jcnt = jingleIq.Instruction.AddContent("checkConnectivity");

            this.Stream.Write(jingleIq);

            this.CheckConnectivity(jingle.Sid);

            iq.Handled = true;
        }

        /// <summary>
        /// TODO: Documentation jingleManager_OnReceivedTransportInfo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="iq"></param>
        private void jingleManager_OnReceivedTransportInfo(object sender, IQ iq)
        {
            Jingle jingle = iq.Query as Jingle;

            JingleContent jingleContent = jingle.GetContent(0);

            if (jingleContent.ContentName == "checkConnectivity")
            {
                this.CheckConnectivity(jingle.Sid);

                iq.Handled = true;
            }
        }
        #endregion

        #region TURN
        /// <summary>
        /// TODO: Documentation StartTurnPeer
        /// </summary>
        /// <param name="sid"></param>
        private void StartTurnPeer(String sid)
        {
            JingleSession jingleSession = this.JingleManager.FindSession(sid);

            if (jingleSession.Remote.Action == ActionType.session_initiate)
            {
                JingleContent jingleContent = jingleSession.Remote.GetContent(0);
                JingleIce jingleIce = jingleContent.GetElement<JingleIce>(0);

                foreach (JingleIceCandidate candidate in jingleIce.GetCandidates())
                {
                    if (candidate.Type == IceCandidateType.relay)
                    {
                        if (this.OnTurnStart != null)
                            this.OnTurnStart(this, candidate.EndPoint, sid);

                        this.StartingSessionSid = null;
                        this.StartingSessionRecipient = null;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// TODO: Documentation CreateTurnSession
        /// </summary>
        /// <param name="sid"></param>
        private TurnManager CreateTurnSession(String sid)
        {
            TurnManager turnManager = new TurnManager(this.StunServerEP, ProtocolType.Tcp, null, null);

            turnManager.OnAllocateSucceed += new AllocateSuccessHandler(this.turnManager_OnAllocateSucceed);
            turnManager.OnAllocateFailed += new MessageReceptionHandler(this.turnManager_OnAllocateFailed);
            turnManager.OnConnectionAttemptReceived += new IndicationReceptionHandler(this.turnManager_OnConnectionAttemptReceived);
            turnManager.OnConnectionBindSucceed += new ConnectionBindSuccessHandler(this.turnManager_OnConnectionBindSucceed);

            turnManager.Connect();

            this.turnSessions.Add(sid, new TurnSession() { TurnManager = turnManager });

            return turnManager;
        }

        /// <summary>
        /// TODO: Documentation DestroyTurnSession
        /// </summary>
        /// <param name="sid"></param>
        public void DestroyTurnSession(String sid)
        {
            if (this.turnSessions.ContainsKey(sid))
            {
                if (this.turnSessions[sid].TurnAllocation != null)
                    this.turnSessions[sid].TurnAllocation.StopAutoRefresh();

                this.turnSessions[sid].TurnManager.Disconnect();

                this.turnSessions[sid].TurnManager = null;
                this.turnSessions[sid].TurnAllocation = null;

                this.turnSessions.Remove(sid);
            }
        }

        /// <summary>
        /// TODO: Documentation turnManager_OnAllocateSucceed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="allocation"></param>
        /// <param name="sentMsg"></param>
        /// <param name="receivedMsg"></param>
        private void turnManager_OnAllocateSucceed(object sender, TurnAllocation allocation, StunMessage sentMsg, StunMessage receivedMsg)
        {
            if (this.StartingSessionRecipient != null)
            {
                this.turnSessions[this.StartingSessionSid].TurnAllocation = allocation;
                this.turnSessions[this.StartingSessionSid].TurnAllocation.StartAutoRefresh((sender as TurnManager));

                XmlDocument doc = new XmlDocument();

                // Jingle Transport
                JingleIce jingleIce = new JingleIce(doc)
                {
                    Pwd = JingleUtilities.GenerateIcePwd,
                    Ufrag = JingleUtilities.GenerateIceUfrag
                };

                if (this.OnJingleIceCandidates != null)
                    this.OnJingleIceCandidates(this, jingleIce, (sender as TurnManager).HostEP, allocation);

                JingleIQ jingleIq = null;

                // Jingle Description
                Element jingleDescription = null;
                String contentName = null;

                if (this.OnJingleDescription != null)
                    this.OnJingleDescription(this, doc, out jingleDescription, out contentName);

                jingleIq = this.JingleManager.SessionRequest(this.StartingSessionRecipient,
                                                             this.StartingSessionAction,
                                                             this.StartingSessionSid, contentName,
                                                             jingleDescription, jingleIce);

                this.Stream.Write(jingleIq);
            }
        }

        /// <summary>
        /// TODO: Documentation turnManager_OnAllocateFailed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="receivedMsg"></param>
        /// <param name="sentMsg"></param>
        /// <param name="transactionObject"></param>
        private void turnManager_OnAllocateFailed(object sender, StunMessage receivedMsg, StunMessage sentMsg, object transactionObject)
        {
            if (this.StartingSessionRecipient != null)
            {
                this.DestroyTurnSession(this.StartingSessionSid);

                this.StartingSessionSid = null;
                this.StartingSessionRecipient = null;
            }
        }

        /// <summary>
        /// TODO: Documentation turnManager_OnConnectionAttemptReceived
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="receivedMsg"></param>
        private void turnManager_OnConnectionAttemptReceived(object sender, StunMessage receivedMsg)
        {
            (sender as TurnManager).ConnectionBind(receivedMsg.Turn.ConnectionId, this.TurnUsername, this.TurnPassword);
        }

        /// <summary>
        /// TODO: Documentation turnManager_OnConnectionBindSucceed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="connectedSocket"></param>
        /// <param name="receivedMsg"></param>
        private void turnManager_OnConnectionBindSucceed(object sender, Socket connectedSocket, StunMessage receivedMsg)
        {
            this.p2pConnectionSucceed = true;

            if (this.OnTurnConnectionBindSucceed != null)
                this.OnTurnConnectionBindSucceed(this, connectedSocket, this.StartingSessionSid, this.StartingSessionRecipient);

            this.StartingSessionSid = null;
            this.StartingSessionRecipient = null;
        }
        #endregion
    }
}