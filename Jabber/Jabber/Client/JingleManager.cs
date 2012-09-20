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
using System.Globalization;
using System.Xml;
using Bedrock;
using Jabber.Connection;
using Jabber.Protocol;
using Jabber.Protocol.Client;
using Jabber.Protocol.IQ;
using System.IO;

namespace Jabber.Client
{
    /// <summary>
    /// Manages Jingle sessions with peers
    /// </summary>
    public partial class JingleManager : StreamComponent
    {
        #region MEMBERS
        /// <summary>
        /// TODO: Documentation Member
        /// </summary>
        private RosterManager m_roster = null;
        #endregion

        #region PROPERTIES
        /// <summary>
        /// Session informations about every opened sessions using their SID as key
        /// </summary>
        private Dictionary<String, JingleSession> Sessions { get; set; }
        /// <summary>
        /// TODO: Documentation Property
        /// </summary>
        private IQTracker Tracker { get; set; }
        /// <summary>
        /// TODO: Documentation Property
        /// </summary>
        [DefaultValue(false)]
        public Boolean AllowRosterSessionInitiateOnly { get; set; }
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
            }
        }
        #endregion

        #region EVENTS
        /// <summary>
        /// Informs the client that it received a session-initiate IQ packet
        /// This event is fired after XMPP acknowledge message has been sent
        /// 
        /// If the IQ has been handled by subscribed event handlers
        ///     the received message is added to sessions dictionary
        /// Else
        ///     a session-terminate message with decline reason is sent
        /// </summary>
        [Category("Protocol")]
        [Description("We received a session-initiate IQ packet.")]
        public event IQHandler OnReceivedSessionInitiate;
        /// <summary>
        /// Informs the client that it received a session-accept IQ packet
        /// This event is fired after XMPP acknowledge message has been sent
        /// 
        /// If the IQ has been handled by subscribed event handlers
        ///     the received message is added to sessions dictionary
        /// Else
        ///     a session-terminate message with cancel reason is sent
        /// </summary>
        [Category("Protocol")]
        [Description("We received a session-accept IQ packet.")]
        public event IQHandler OnReceivedSessionAccept;
        /// <summary>
        /// Informs the client that it received a session-terminate IQ packet
        /// This event is fired after XMPP acknowledge message has been sent
        /// 
        /// Whatever the IQ has been handled or not by subscribed event handlers
        /// the session is removed from sessions dictionary
        /// </summary>
        [Category("Protocol")]
        [Description("We received a session-terminate IQ packet.")]
        public event IQHandler OnReceivedSessionTerminate;
        /// <summary>
        /// Informs the client that it received a session-info IQ packet
        /// 
        /// If the IQ has not been handled by subscribed event handlers
        /// an XMPP acknowledge is sent back
        /// </summary>
        [Category("Protocol")]
        [Description("We received a session-info IQ packet.")]
        public event IQHandler OnReceivedSessionInfo;
        /// <summary>
        /// Informs the client that it received a transport-info IQ packet
        /// 
        /// If the IQ has not been handled by subscribed event handlers
        /// an XMPP acknowledge is sent back
        /// </summary>
        [Category("Protocol")]
        [Description("We received a transport-info IQ packet.")]
        public event IQHandler OnReceivedTransportInfo;
        #endregion

        #region CONSTRUCTORS & FINALIZERS
        /// <summary>
        /// Creates a new Jingle Manager inside a container
        /// </summary>
        /// <param name="container">Parent container</param>
        public JingleManager(IContainer container)
            : this()
        {
            container.Add(this);
        }

        /// <summary>
        /// Creates a new Jingle Manager
        /// </summary>
        public JingleManager()
        {
            InitializeComponent();

            this.Sessions = new Dictionary<String, JingleSession>(StringComparer.Create(CultureInfo.CurrentCulture, false));

            this.OnStreamChanged += new ObjectHandler(JingleManager_OnStreamChanged);
        }
        #endregion

        #region XMPP EVENTS
        /// <summary>
        /// Entry point for XMPP stream events
        /// </summary>
        /// <param name="sender"></param>
        private void JingleManager_OnStreamChanged(object sender)
        {
            //this.Tracker = new IQTracker(m_stream);

            JabberClient cli = m_stream as JabberClient;

            if (cli == null)
                return;

            cli.OnIQ += new IQHandler(this.GotIQ);
        }

        /// <summary>
        /// Analyses an IQ paquet and chooses to handle it or not
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="iq"></param>
        private void GotIQ(object sender, IQ iq)
        {
            Jingle jingle = iq.Query as Jingle;

            if (jingle != null)
            {
                if (jingle.Action == ActionType.session_terminate)
                {
                    this.Stream.Write(iq.GetAcknowledge(this.Stream.Document));

                    if (this.OnReceivedSessionTerminate != null)
                        this.OnReceivedSessionTerminate(this, iq);

                    if (this.Sessions.ContainsKey(jingle.Sid))
                        this.Sessions.Remove(jingle.Sid);

                    iq.Handled = true;
                }
                else
                {
                    if (!this.AllowSessionInitiateFrom(iq.From))
                    {
                        this.SessionTerminate(iq.From, jingle.Sid, ReasonType.security_error);
                        iq.Handled = true;
                    }
                    else
                    {
                        switch (jingle.Action)
                        {
                            case ActionType.session_initiate:
                                this.Stream.Write(iq.GetAcknowledge(this.Stream.Document));

                                if (!this.Sessions.ContainsKey(jingle.Sid))
                                {
                                    JingleSession jingleSession = new JingleSession();
                                    jingleSession.SID = jingle.Sid;
                                    jingleSession.Remote = jingle;

                                    this.Sessions.Add(jingle.Sid, jingleSession);
                                }

                                if (this.OnReceivedSessionInitiate != null)
                                    this.OnReceivedSessionInitiate(this, iq);

                                if (!iq.Handled)
                                {
                                    this.SessionTerminate(iq.From, jingle.Sid, ReasonType.decline);
                                    iq.Handled = true;
                                }

                                break;

                            case ActionType.session_accept:
                                this.Stream.Write(iq.GetAcknowledge(this.Stream.Document));

                                if (!this.Sessions.ContainsKey(jingle.Sid))
                                {
                                    JingleSession jingleSession = new JingleSession();
                                    jingleSession.SID = jingle.Sid;
                                    jingleSession.Remote = jingle;

                                    this.Sessions.Add(jingle.Sid, jingleSession);
                                }

                                if (this.OnReceivedSessionAccept != null)
                                    this.OnReceivedSessionAccept(this, iq);

                                if (!iq.Handled)
                                {
                                    this.SessionTerminate(iq.From, jingle.Sid, ReasonType.cancel);
                                    iq.Handled = true;
                                }

                                break;

                            case ActionType.session_info:
                                if (this.Sessions.ContainsKey(jingle.Sid))
                                    this.Stream.Write(iq.GetAcknowledge(this.Stream.Document));
                                else
                                    this.Stream.Write(new UnknownSession(this.Stream.Document));

                                if (this.OnReceivedSessionInfo != null)
                                    this.OnReceivedSessionInfo(this, iq);

                                break;

                            case ActionType.transport_info:
                                if (this.Sessions.ContainsKey(jingle.Sid))
                                    this.Stream.Write(iq.GetAcknowledge(this.Stream.Document));
                                else
                                    this.Stream.Write(new UnknownSession(this.Stream.Document));

                                if (this.OnReceivedTransportInfo != null)
                                    this.OnReceivedTransportInfo(this, iq);

                                break;
                        }
                    }
                }
            }
        }
        #endregion

        #region SESSIONS
        /// <summary>
        /// TODO: Documentation CanInitiateSession
        /// </summary>
        /// <param name="jid"></param>
        /// <returns></returns>
        public Boolean AllowSessionInitiateFrom(JID jid)
        {
            if (this.AllowRosterSessionInitiateOnly)
            {
                if (this.RosterManager != null)
                {
                    Item rosterItem = this.RosterManager[jid.BareJID];

                    return rosterItem != null;
                }
            }
            return true;
        }
        /// <summary>
        /// Returns the Jingle session which matches the given SID or null if no session is found in sessions dictionary
        /// </summary>
        /// <param name="sid">SID of the Jingle session to search for</param>
        /// <returns></returns>
        public JingleSession FindSession(String sid)
        {
            return this.Sessions.ContainsKey(sid) ? this.Sessions[sid] : null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        public Boolean SessionExists(String sid)
        {
            return this.Sessions.ContainsKey(sid);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sid"></param>
        public void RemoveSession(String sid)
        {
            if (this.Sessions.ContainsKey(sid))
                this.Sessions.Remove(sid);
        }
        #endregion

        #region JINGLE
        /// <summary>
        /// Constructs a Jingle IQ Paquet containing Jingle most common arguments
        /// </summary>
        /// <param name="to">Recipient of this Jingle IQ Paquet</param>
        /// <param name="action">Type of Action this request is about</param>
        /// <param name="sid">
        /// SID of the session which can be created using JingleUtilities.GenerateSid for a session-initiate 
        /// paquet construction or was given by a peer who initiated a session which had been accepted
        /// </param>
        /// <param name="contentName">The name of the content, only for informative purposes but mandatory</param>
        /// <param name="description">An XML element using root tag <description></description></param>
        /// <param name="transport">An XML element using root tag <transport></transport></param>
        /// <returns></returns>
        public JingleIQ SessionRequest(JID to, ActionType action, String sid, String contentName, Element description, Element transport)
        {
            JingleIQ jingleIq;

            if (description != null)
                jingleIq = new JingleIQ(description.OwnerDocument);
            else
                if (transport != null)
                    jingleIq = new JingleIQ(transport.OwnerDocument);
                else
                    jingleIq = new JingleIQ(new XmlDocument());

            jingleIq.From = this.Stream.JID;
            jingleIq.To = to;
            jingleIq.Type = IQType.set;
            jingleIq.Instruction.Action = action;
            jingleIq.Instruction.Sid = sid;

            if (action == ActionType.session_initiate)
                jingleIq.Instruction.Initiator = this.Stream.JID;
            else if (action == ActionType.session_accept)
                jingleIq.Instruction.Responder = this.Stream.JID;

            if (!String.IsNullOrEmpty(contentName))
            {
                JingleContent jcnt = jingleIq.Instruction.AddContent(contentName);

                if (description != null)
                    jcnt.AddChild(description);

                if (transport != null)
                    jcnt.AddChild(transport);
            }
            else
            {
                throw new InvalidOperationException("Content name cannot be null or an empty string");
            }

            return jingleIq;
        }

        /// <summary>
        /// TODO: Documentation SessionTerminate
        /// </summary>
        /// <param name="to"></param>
        /// <param name="sid"></param>
        /// <param name="reasonType"></param>
        /// <returns></returns>
        public void SessionTerminate(JID to, String sid, ReasonType? reasonType)
        {
            JingleIQ jingleIq = new JingleIQ(this.Stream.Document);
            jingleIq.From = this.Stream.JID;
            jingleIq.To = to;
            jingleIq.Type = IQType.set;
            jingleIq.Instruction.Action = ActionType.session_terminate;
            jingleIq.Instruction.Sid = sid;

            if (reasonType.HasValue)
            {
                JingleReason reason = new JingleReason(jingleIq.OwnerDocument);

                switch (reasonType)
                {
                    case ReasonType.alternative_session:
                        reason.Reason = new AlternativeSession(jingleIq.OwnerDocument);
                        break;

                    case ReasonType.busy:
                        reason.Reason = new Busy(jingleIq.OwnerDocument);
                        break;

                    case ReasonType.cancel:
                        reason.Reason = new Cancel(jingleIq.OwnerDocument);
                        break;

                    case ReasonType.connectivity_error:
                        reason.Reason = new ConnectivityError(jingleIq.OwnerDocument);
                        break;

                    case ReasonType.decline:
                        reason.Reason = new JingleDecline(jingleIq.OwnerDocument);
                        break;

                    case ReasonType.expired:
                        reason.Reason = new Expired(jingleIq.OwnerDocument);
                        break;

                    case ReasonType.failed_application:
                        reason.Reason = new FailedApplication(jingleIq.OwnerDocument);
                        break;

                    case ReasonType.failed_transport:
                        reason.Reason = new FailedTransport(jingleIq.OwnerDocument);
                        break;

                    case ReasonType.general_error:
                        reason.Reason = new GeneralError(jingleIq.OwnerDocument);
                        break;

                    case ReasonType.gone:
                        reason.Reason = new Gone(jingleIq.OwnerDocument);
                        break;

                    case ReasonType.incompatible_parameters:
                        reason.Reason = new IncompatibleParameters(jingleIq.OwnerDocument);
                        break;

                    case ReasonType.media_error:
                        reason.Reason = new MediaError(jingleIq.OwnerDocument);
                        break;

                    case ReasonType.security_error:
                        reason.Reason = new SecurityError(jingleIq.OwnerDocument);
                        break;

                    case ReasonType.success:
                        reason.Reason = new Success(jingleIq.OwnerDocument);
                        break;

                    case ReasonType.timeout:
                        reason.Reason = new Timeout(jingleIq.OwnerDocument);
                        break;

                    case ReasonType.unsupported_applications:
                        reason.Reason = new UnsupportedApplications(jingleIq.OwnerDocument);
                        break;

                    case ReasonType.unsupported_transports:
                        reason.Reason = new UnsupportedTransports(jingleIq.OwnerDocument);
                        break;
                }
                jingleIq.Instruction.Reason = reason;
            }

            this.Stream.Write(jingleIq);
        }

        /// <summary>
        /// TODO: Documentation SessionInfo
        /// </summary>
        /// <param name="to"></param>
        /// <param name="sid"></param>
        /// <returns></returns>
        public void SessionInfo(JID to, String sid)
        {
            this.SessionInfo(to, sid, null);
        }

        /// <summary>
        /// TODO: Documentation SessionInfo
        /// </summary>
        /// <param name="to"></param>
        /// <param name="sid"></param>
        /// <param name="payload"></param>
        /// <returns></returns>
        public void SessionInfo(JID to, String sid, Element payload)
        {
            JingleIQ jingleIq;

            if (payload != null)
                jingleIq = new JingleIQ(payload.OwnerDocument);
            else
                jingleIq = new JingleIQ(this.Stream.Document);

            jingleIq.From = this.Stream.JID;
            jingleIq.To = to;
            jingleIq.Type = IQType.set;
            jingleIq.Instruction.Action = ActionType.session_info;
            jingleIq.Instruction.Sid = sid;

            if (payload != null)
                jingleIq.Instruction.AddChild(payload);

            this.Stream.Write(jingleIq);
        }
        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public class JingleSession
    {
        public String SID { get; set; }
        public Jingle Remote { get; set; }
        //public Jingle Local { get; set; }
    }
}