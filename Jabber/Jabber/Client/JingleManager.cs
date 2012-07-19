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
using System.Xml;
using Jabber.Connection;
using Jabber.Protocol;
using Jabber.Protocol.Client;
using Jabber.Protocol.IQ;

namespace Jabber.Client
{
    /// <summary>
    /// Manages Jingle sessions with peers
    /// </summary>
    public partial class JingleManager : StreamComponent
    {
        #region PROPERTIES
        /// <summary>
        /// Gets or sets the Jabber client associated with the Jingle Manager
        /// </summary>
        [Description("The JabberClient to hook up to.")]
        [Category("Jabber")]
        [Browsable(false)]
        [Obsolete("Use the Stream property instead")]
        [ReadOnly(true)]
        public JabberClient Client
        {
            get { return (JabberClient)this.Stream; }
            set { this.Stream = value; }
        }
        /// <summary>
        /// Session informations about every opened sessions using their SID as key
        /// </summary>
        private Dictionary<String, Jingle> Sessions { get; set; }
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

            this.Sessions = new Dictionary<String, Jingle>();

            this.OnStreamChanged += new Bedrock.ObjectHandler(JingleManager_OnStreamChanged);
        }
        #endregion

        #region XMPP EVENTS
        /// <summary>
        /// Entry point for XMPP stream events
        /// </summary>
        /// <param name="sender"></param>
        private void JingleManager_OnStreamChanged(object sender)
        {
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
                switch (jingle.Action)
                {
                    case ActionType.session_initiate:
                        this.Stream.Write(iq.GetAcknowledge(this.Stream.Document));

                        if (this.OnReceivedSessionInitiate != null)
                            this.OnReceivedSessionInitiate(this, iq);

                        if (iq.Handled)
                        {
                            if (!this.Sessions.ContainsKey(jingle.Sid))
                                this.Sessions.Add(jingle.Sid, jingle);
                        }
                        else
                            this.Stream.Write(this.SessionTerminate(iq.From, jingle.Sid, ReasonType.decline));
                        break;

                    case ActionType.session_accept:
                        this.Stream.Write(iq.GetAcknowledge(this.Stream.Document));

                        if (this.OnReceivedSessionAccept != null)
                            this.OnReceivedSessionAccept(this, iq);

                        if (iq.Handled)
                        {
                            if (!this.Sessions.ContainsKey(jingle.Sid))
                                this.Sessions.Add(jingle.Sid, jingle);
                        }
                        else
                            this.Stream.Write(this.SessionTerminate(iq.From, jingle.Sid, ReasonType.cancel));
                        break;

                    case ActionType.session_terminate:
                        this.Stream.Write(iq.GetAcknowledge(this.Stream.Document));

                        if (this.OnReceivedSessionTerminate != null)
                            this.OnReceivedSessionTerminate(this, iq);

                        if (this.Sessions.ContainsKey(jingle.Sid))
                            this.Sessions.Remove(jingle.Sid);

                        iq.Handled = true;
                        break;

                    case ActionType.session_info:
                        if (this.OnReceivedSessionInfo != null)
                            this.OnReceivedSessionInfo(this, iq);

                        if (!iq.Handled)
                        {
                            if (this.Sessions.ContainsKey(jingle.Sid))
                                this.Stream.Write(iq.GetAcknowledge(this.Stream.Document));
                            else
                                this.Stream.Write(new UnknownSession(this.Stream.Document));

                            iq.Handled = true;
                        }
                        break;
                }
            }
        }
        #endregion

        #region JINGLE
        /// <summary>
        /// Returns the Jingle session which matches the given SID or null if no session is found in sessions dictionary
        /// </summary>
        /// <param name="sid">SID of the Jingle session to search for</param>
        /// <returns></returns>
        public Jingle FindSession(String sid)
        {
            return this.Sessions.ContainsKey(sid) ? this.Sessions[sid] : null;
        }

        /// <summary>
        /// Returns the first Jingle session opened with the given peer's JID or null if no session is found in sessions dictionary
        /// </summary>
        /// <param name="jid"></param>
        /// <returns></returns>
        public Jingle FindSession(JID jid)
        {
            foreach (Jingle session in this.Sessions.Values)
            {
                if (session.Initiator != null && session.Initiator == jid)
                    return session;
                else if (session.Responder != null && session.Responder == jid)
                    return session;
            }
            return null;
        }

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
        /// <returns></returns>
        public JingleIQ SessionTerminate(JID to, String sid)
        {
            return this.SessionTerminate(to, sid, null);
        }

        /// <summary>
        /// TODO: Documentation SessionTerminate
        /// </summary>
        /// <param name="to"></param>
        /// <param name="sid"></param>
        /// <param name="reasonType"></param>
        /// <returns></returns>
        public JingleIQ SessionTerminate(JID to, String sid, ReasonType? reasonType)
        {
            JingleIQ jingleIq = new JingleIQ(new XmlDocument());
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

            return jingleIq;
        }

        /// <summary>
        /// TODO: Documentation SessionInfo
        /// </summary>
        /// <param name="to"></param>
        /// <param name="sid"></param>
        /// <returns></returns>
        public JingleIQ SessionInfo(JID to, String sid)
        {
            return this.SessionInfo(to, sid, null);
        }

        /// <summary>
        /// TODO: Documentation SessionInfo
        /// </summary>
        /// <param name="to"></param>
        /// <param name="sid"></param>
        /// <param name="payload"></param>
        /// <returns></returns>
        public JingleIQ SessionInfo(JID to, String sid, Element payload)
        {
            JingleIQ jingleIq;

            if (payload != null)
                jingleIq = new JingleIQ(payload.OwnerDocument);
            else
                jingleIq = new JingleIQ(new XmlDocument());

            jingleIq.From = this.Stream.JID;
            jingleIq.To = to;
            jingleIq.Type = IQType.set;
            jingleIq.Instruction.Action = ActionType.session_info;
            jingleIq.Instruction.Sid = sid;

            if (payload != null)
                jingleIq.Instruction.AddChild(payload);

            return jingleIq;
        }
        #endregion
    }
}