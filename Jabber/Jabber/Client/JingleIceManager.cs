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
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using Bedrock.Net;
using Jabber.Connection;
using Jabber.Protocol;
using Jabber.Protocol.Client;
using Jabber.Protocol.IQ;
using Jabber.Stun;
using Jabber.Stun.Attributes;
using NetLib.DNS;
using NetLib.DNS.Records;

namespace Jabber.Client
{
    /// <summary>
    /// TODO: Documentation Class
    /// </summary>
    public partial class JingleIceManager : StreamComponent
    {
        #region MEMBERS
        private JingleManager jingleManager = null;
        private HolePuncher holePuncher = null;

        private Dictionary<String, TurnSession> turnSessions = new Dictionary<String, TurnSession>();
        private Dictionary<String, JingleIceCandidate[]> localCandidates = new Dictionary<String, JingleIceCandidate[]>();
        #endregion

        #region EVENTS
        public event EventHandler OnBeforeInitiatorAllocate;
        public event JingleSessionIdHandler OnBeforeResponderAllocate;
        public event JingleDescriptionGatheringHandler OnDescriptionGathering;
        public event JingleIceCandidatesGatheringHandler OnIceCandidatesGathering;
        public event JingleHolePunchSucceedHandler OnHolePunchSucceed;
        public event JingleTurnStartHandler OnTurnStart;
        public event JingleTurnConnectionBindSucceedHandler OnTurnConnectionBindSucceed;
        public event JingleSessionIdHandler OnConnectionTryEnded;
        #endregion

        #region PROPERTIES
        private ActionType StartingSessionAction { get; set; }
        public String StartingSessionSid { get; private set; }
        public JID StartingSessionRecipient { get; private set; }

        [DefaultValue("")]
        public String StunServerHostDomain { get; set; }
        public String StunServerSrvDomain { get; set; }
        public IPAddress StunServerIP { get; set; }
        public Int32 StunServerPort { get; set; }

        /// <summary>
        /// TODO: Documentation Property
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IPEndPoint StunServerEP
        {
            get
            {
                if (!String.IsNullOrEmpty(this.StunServerHostDomain) &&
                    this.StunServerIP == null)
                {
                    DnsRequest request = new DnsRequest(this.StunServerHostDomain);
                    DnsResponse response = request.GetResponse(DnsRecordType.A);

                    ARecord stunARecord = (ARecord)response.GetRecords(DnsRecordType.A)[0];

                    this.StunServerIP = stunARecord.IPAddress;
                }
                else if (!String.IsNullOrEmpty(this.StunServerSrvDomain) &&
                         this.StunServerIP == null)
                {
                    String stunDomain = null;
                    Int32 stunPort = 0;

                    Address.LookupSRV("_stun._tcp.", this.StunServerSrvDomain, ref stunDomain, ref stunPort);

                    DnsRequest request = new DnsRequest(stunDomain);
                    DnsResponse response = request.GetResponse(DnsRecordType.A);

                    ARecord stunARecord = (ARecord)response.GetRecords(DnsRecordType.A)[0];

                    this.StunServerIP = stunARecord.IPAddress;
                    this.StunServerPort = stunPort;
                }

                return new IPEndPoint(this.StunServerIP, this.StunServerPort);
            }
        }

        public Boolean TurnSupported { get; set; }
        public String TurnUsername { get; set; }
        public String TurnPassword { get; set; }

        /// <summary>
        /// TODO: Documentation Property
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public X509Certificate2 TurnClientCertificate { get; set; }

        /// <summary>
        /// TODO: Documentation Property
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RemoteCertificateValidationCallback TurnRemoteCertificateValidation { get; set; }

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
                    if (this.jingleManager != null)
                    {
                        this.jingleManager.OnReceivedSessionInitiate -= this.jingleManager_OnReceivedSessionInitiate;
                        this.jingleManager.OnReceivedSessionAccept -= this.jingleManager_OnReceivedSessionAccept;
                        this.jingleManager.OnReceivedTransportInfo -= this.jingleManager_OnReceivedTransportInfo;
                        this.jingleManager.OnReceivedSessionTerminate -= this.jingleManager_OnReceivedSessionTerminate;
                    }

                    this.jingleManager = value;

                    this.jingleManager.OnReceivedSessionInitiate += new IQHandler(jingleManager_OnReceivedSessionInitiate);
                    this.jingleManager.OnReceivedSessionAccept += new IQHandler(jingleManager_OnReceivedSessionAccept);
                    this.jingleManager.OnReceivedTransportInfo += new IQHandler(jingleManager_OnReceivedTransportInfo);
                    this.jingleManager.OnReceivedSessionTerminate += new IQHandler(jingleManager_OnReceivedSessionTerminate);
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

            this.Disposed += new EventHandler(this.JingleIceManager_Disposed);
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

        /// <summary>
        /// TODO: Documentation JingleIceManager_Disposed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void JingleIceManager_Disposed(object sender, EventArgs e)
        {
            lock (this)
            {
                foreach (var item in this.turnSessions)
                {
                    if (item.Value.TurnAllocation != null)
                        item.Value.TurnAllocation.StopAutoRefresh();

                    item.Value.TurnManager.OnAllocateSucceed -= this.turnManager_OnAllocateSucceed;
                    item.Value.TurnManager.OnAllocateFailed -= this.turnManager_OnAllocateFailed;
                    item.Value.TurnManager.OnConnectionAttemptReceived -= this.turnManager_OnConnectionAttemptReceived;
                    item.Value.TurnManager.OnConnectionBindSucceed -= this.turnManager_OnConnectionBindSucceed;
                    item.Value.TurnManager.Disconnect();

                    item.Value.TurnManager = null;
                    item.Value.TurnAllocation = null;
                }
            }
        }
        #endregion

        #region METHODS
        /// <summary>
        /// TODO: Documentation CheckConnectivity
        /// </summary>
        /// <param name="sid"></param>
        private void CheckConnectivity(String sid)
        {
            if (this.turnSessions.ContainsKey(sid))
            {
                this.holePuncher = new HolePuncher(this.turnSessions[sid].TurnManager.HostEP, sid);

                JingleSession jingleSession = this.JingleManager.FindSession(sid);

                JingleContent jingleContent = jingleSession.Remote.GetContent(0);

                JingleIce jingleIce = jingleContent.GetElement<JingleIce>(0);

                foreach (JingleIceCandidate remoteCandidate in jingleIce.GetCandidates())
                {
                    switch (remoteCandidate.Type)
                    {
                        case IceCandidateType.host:
                        case IceCandidateType.prflx:
                        case IceCandidateType.srflx:
                            foreach (JingleIceCandidate localCandidate in this.localCandidates[sid])
                            {
                                if (localCandidate.Type == remoteCandidate.Type)
                                {
                                    this.holePuncher.AddEP(remoteCandidate.Priority, remoteCandidate.EndPoint);
                                    break;
                                }
                            }
                            break;

                        case IceCandidateType.relay:
                            if (this.TurnSupported &&
                                jingleSession.Remote.Action == ActionType.session_accept)
                            {
                                this.turnSessions[sid].TurnManager.CreatePermission(new XorMappedAddress(remoteCandidate.RelatedEndPoint),
                                                                                    this.turnSessions[sid].TurnAllocation);
                            }
                            break;
                    }
                }

                if (!this.holePuncher.CanStart && this.TurnSupported)
                {
                    this.StartTurnPeer(sid);
                }
                else
                {
                    this.holePuncher.StartTcpPunch(this.HolePunchSuccess, this.HolePunchFailure);
                }
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
            JingleSession jingleSession = this.JingleManager.FindSession(punchData as String);

            if (this.OnConnectionTryEnded != null)
                this.OnConnectionTryEnded(this, punchData as String);

            if (this.OnHolePunchSucceed != null)
                this.OnHolePunchSucceed(this, connectedSocket, jingleSession.Remote);


            this.DestroyTurnSession(punchData as String);
            this.CancelStartingSession();
        }

        /// <summary>
        /// TODO: Documentation HolePunchFailure
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="punchData"></param>
        private void HolePunchFailure(object sender, Object punchData)
        {
            if (this.TurnSupported)
                this.StartTurnPeer(punchData as String);
        }

        /// <summary>
        /// TODO: Documentation CancelStartingSession
        /// </summary>
        /// <param name="sid"></param>
        private void CancelStartingSession(String sid)
        {
            if (this.StartingSessionRecipient != null && 
                this.StartingSessionSid == sid)
            {
                this.CancelStartingSession();
            }
        }

        /// <summary>
        /// TODO: Documentation CancelStartingSession
        /// </summary>
        private void CancelStartingSession()
        {
            this.StartingSessionRecipient = null;
            this.StartingSessionSid = null;
            this.StartingSessionAction = ActionType.UNSPECIFIED;
        }
        #endregion

        #region JINGLE
        /// <summary>
        /// TODO: Documentation InitiateSession
        /// </summary>
        /// <param name="to"></param>
        /// <param name="useTurnOnly"></param>
        /// <returns></returns>
        public String InitiateSession(JID to, Boolean useTurnOnly)
        {
            if (this.StartingSessionRecipient == null && to != this.Stream.JID)
            {
                this.StartingSessionRecipient = to;
                this.StartingSessionSid = JingleUtilities.GenerateSid;
                this.StartingSessionAction = ActionType.session_initiate;

                if (this.OnBeforeInitiatorAllocate != null)
                    this.OnBeforeInitiatorAllocate(this, new EventArgs());

                if (this.TurnSupported)
                {
                    this.CreateTurnSession(this.StartingSessionSid, useTurnOnly);
                }
                else
                {
                    throw new NotSupportedException();
                }

                return this.StartingSessionSid;
            }
            return null;
        }

        /// <summary>
        /// TODO: Documentation TerminateSession
        /// </summary>
        /// <param name="to"></param>
        /// <param name="sid"></param>
        public void TerminateSession(JID to, String sid)
        {
            this.TerminateSession(to, sid, ReasonType.success);
        }

        /// <summary>
        /// TODO: Documentation TerminateSession
        /// </summary>
        /// <param name="to"></param>
        /// <param name="sid"></param>
        /// <param name="reasonType"></param>
        private void TerminateSession(JID to, String sid, ReasonType? reasonType)
        {
            this.StartingSessionRecipient = null;
            this.StartingSessionSid = null;
            this.StartingSessionAction = ActionType.UNSPECIFIED;

            if (to != null && !String.IsNullOrEmpty(sid))
                this.jingleManager.SessionTerminate(to, sid, reasonType);
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
                Jingle jingle = iq.Query as Jingle;

                this.StartingSessionRecipient = iq.From;
                this.StartingSessionSid = jingle.Sid;
                this.StartingSessionAction = ActionType.session_accept;

                if (this.OnBeforeResponderAllocate != null)
                    this.OnBeforeResponderAllocate(this, jingle.Sid);

                if (this.TurnSupported)
                {
                    this.CreateTurnSession(this.StartingSessionSid, false);
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

            JingleIQ jingleIq = new JingleIQ(this.Stream.Document);

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

        /// <summary>
        /// TODO: Documentation jingleManager_OnReceivedSessionTerminate
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="iq"></param>
        private void jingleManager_OnReceivedSessionTerminate(object sender, IQ iq)
        {
            Jingle jingle = iq.Query as Jingle;

            if (jingle != null)
            {
                if (this.TurnSupported)
                    this.DestroyTurnSession(jingle.Sid);
                else
                    throw new NotSupportedException();

                this.CancelStartingSession(jingle.Sid);
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

            if (jingleSession != null &&
                jingleSession.Remote.Action == ActionType.session_initiate)
            {
                JingleContent jingleContent = jingleSession.Remote.GetContent(0);
                JingleIce jingleIce = jingleContent.GetElement<JingleIce>(0);

                foreach (JingleIceCandidate candidate in jingleIce.GetCandidates())
                {
                    if (candidate.Type == IceCandidateType.relay)
                    {
                        if (this.OnTurnStart != null)
                            this.OnTurnStart(this, candidate.EndPoint, sid);

                        if (this.OnConnectionTryEnded != null)
                            this.OnConnectionTryEnded(this, sid);

                        this.CancelStartingSession();
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// TODO: Documentation CreateTurnSession
        /// </summary>
        /// <param name="sid"></param>
        /// <param name="useTurnOnly"></param>
        private void CreateTurnSession(String sid, Boolean useTurnOnly)
        {
            TurnManager turnManager = new TurnManager(this.StunServerEP, ProtocolType.Tcp, this.TurnClientCertificate, this.TurnRemoteCertificateValidation);

            turnManager.OnAllocateSucceed += new TurnAllocateSuccessHandler(this.turnManager_OnAllocateSucceed);
            turnManager.OnAllocateFailed += new StunMessageReceptionHandler(this.turnManager_OnAllocateFailed);
            turnManager.OnConnectionAttemptReceived += new StunIndicationReceptionHandler(this.turnManager_OnConnectionAttemptReceived);
            turnManager.OnConnectionBindSucceed += new TurnConnectionBindSuccessHandler(this.turnManager_OnConnectionBindSucceed);

            turnManager.Connect();
            turnManager.Allocate(this.TurnUsername, this.TurnPassword);

            this.turnSessions.Add(sid, new TurnSession() { TurnManager = turnManager, UseTurnOnly = useTurnOnly });
        }

        /// <summary>
        /// TODO: Documentation DestroyTurnSession
        /// </summary>
        /// <param name="sid"></param>
        public void DestroyTurnSession(String sid)
        {
            lock (this)
            {
                if (this.turnSessions.ContainsKey(sid))
                {
                    if (this.turnSessions[sid].TurnAllocation != null)
                        this.turnSessions[sid].TurnAllocation.StopAutoRefresh();

                    this.turnSessions[sid].TurnManager.OnAllocateSucceed -= this.turnManager_OnAllocateSucceed;
                    this.turnSessions[sid].TurnManager.OnAllocateFailed -= this.turnManager_OnAllocateFailed;
                    this.turnSessions[sid].TurnManager.OnConnectionAttemptReceived -= this.turnManager_OnConnectionAttemptReceived;
                    this.turnSessions[sid].TurnManager.OnConnectionBindSucceed -= this.turnManager_OnConnectionBindSucceed;
                    this.turnSessions[sid].TurnManager.Disconnect();

                    this.turnSessions[sid].TurnManager = null;
                    this.turnSessions[sid].TurnAllocation = null;

                    this.localCandidates.Remove(sid);
                    this.turnSessions.Remove(sid);
                }
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

                XmlDocument doc = new XmlDocument();

                // Jingle Transport
                JingleIce jingleIce = new JingleIce(doc)
                {
                    Pwd = JingleUtilities.GenerateIcePwd,
                    Ufrag = JingleUtilities.GenerateIceUfrag
                };

                if (this.OnIceCandidatesGathering != null)
                    this.OnIceCandidatesGathering(this, jingleIce, (sender as TurnManager).HostEP, this.turnSessions[this.StartingSessionSid].UseTurnOnly, allocation);

                this.localCandidates.Add(this.StartingSessionSid, jingleIce.GetCandidates());

                JingleIQ jingleIq = null;

                // Jingle Description
                Element jingleDescription = null;
                String contentName = null;

                if (this.OnDescriptionGathering != null)
                    this.OnDescriptionGathering(this, doc, this.StartingSessionSid, ref jingleDescription, ref contentName);

                jingleIq = this.JingleManager.SessionRequest(this.StartingSessionRecipient,
                                                             this.StartingSessionAction,
                                                             this.StartingSessionSid, contentName,
                                                             jingleDescription, jingleIce);

                //this.jingleManager.FindSession(this.StartingSessionSid).Local = jingleIq.Instruction;

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
                if (this.OnConnectionTryEnded != null)
                    this.OnConnectionTryEnded(this, this.StartingSessionSid);

                this.DestroyTurnSession(this.StartingSessionSid);

                this.CancelStartingSession();
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
            if (this.OnTurnConnectionBindSucceed != null)
                this.OnTurnConnectionBindSucceed(this, connectedSocket, this.StartingSessionSid, this.StartingSessionRecipient);

            if (this.OnConnectionTryEnded != null)
                this.OnConnectionTryEnded(this, this.StartingSessionSid);

            this.CancelStartingSession();
        }
        #endregion
    }
}