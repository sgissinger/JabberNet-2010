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
using System.Xml;

namespace Jabber.Protocol.IQ
{
    /// <summary>
    /// The party prefers to use an existing session with the peer rather
    /// than initiate a new session; the Jingle session ID of the alternative
    /// session SHOULD be provided as the XML character data of the <sid/> child
    /// </summary>
    public class AlternativeSession : AbstractJingleReason
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="doc"></param>
        public AlternativeSession(XmlDocument doc)
            : base("alternative-session", URI.JINGLE, doc)
        { }

        /// <summary>
        ///
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="qname"></param>
        /// <param name="doc"></param>
        public AlternativeSession(string prefix, XmlQualifiedName qname, XmlDocument doc)
            : base(prefix, qname, doc)
        { }

        /// <summary>
        /// What type of reason
        /// </summary>
        public override ReasonType ReasonType
        {
            get { return ReasonType.alternative_session; }
        }

        /// <summary>
        /// Pre-existent session Sid
        /// </summary>
        public String Sid
        {
            get { return GetElem("sid"); }
            set { SetElem("sid", value); }
        }
    }

    /// <summary>
    /// The party is busy and cannot accept a session
    /// </summary>
    public class Busy : AbstractJingleReason
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="doc"></param>
        public Busy(XmlDocument doc)
            : base("busy", URI.JINGLE, doc)
        { }

        /// <summary>
        ///
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="qname"></param>
        /// <param name="doc"></param>
        public Busy(string prefix, XmlQualifiedName qname, XmlDocument doc)
            : base(prefix, qname, doc)
        { }

        /// <summary>
        /// What type of reason
        /// </summary>
        public override ReasonType ReasonType
        {
            get { return ReasonType.busy; }
        }
    }

    /// <summary>
    /// The initiator wishes to formally cancel the session initiation request
    /// </summary>
    public class Cancel : AbstractJingleReason
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="doc"></param>
        public Cancel(XmlDocument doc)
            : base("cancel", URI.JINGLE, doc)
        { }

        /// <summary>
        ///
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="qname"></param>
        /// <param name="doc"></param>
        public Cancel(string prefix, XmlQualifiedName qname, XmlDocument doc)
            : base(prefix, qname, doc)
        { }

        /// <summary>
        /// What type of reason
        /// </summary>
        public override ReasonType ReasonType
        {
            get { return ReasonType.cancel; }
        }
    }

    /// <summary>
    /// The initiator wishes to formally cancel the session initiation request
    /// </summary>
    public class ConnectivityError : AbstractJingleReason
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="doc"></param>
        public ConnectivityError(XmlDocument doc)
            : base("connectivity-error", URI.JINGLE, doc)
        { }

        /// <summary>
        ///
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="qname"></param>
        /// <param name="doc"></param>
        public ConnectivityError(string prefix, XmlQualifiedName qname, XmlDocument doc)
            : base(prefix, qname, doc)
        { }

        /// <summary>
        /// What type of reason
        /// </summary>
        public override ReasonType ReasonType
        {
            get { return ReasonType.connectivity_error; }
        }
    }

    /// <summary>
    /// The party wishes to formally decline the session
    /// </summary>
    public class JingleDecline : AbstractJingleReason
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="doc"></param>
        public JingleDecline(XmlDocument doc)
            : base("decline", URI.JINGLE, doc)
        { }

        /// <summary>
        ///
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="qname"></param>
        /// <param name="doc"></param>
        public JingleDecline(string prefix, XmlQualifiedName qname, XmlDocument doc)
            : base(prefix, qname, doc)
        { }

        /// <summary>
        /// What type of reason
        /// </summary>
        public override ReasonType ReasonType
        {
            get { return ReasonType.decline; }
        }
    }

    /// <summary>
    /// The session length has exceeded a pre-defined time limit
    /// (e.g., a meeting hosted at a conference service)
    /// </summary>
    public class Expired : AbstractJingleReason
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="doc"></param>
        public Expired(XmlDocument doc)
            : base("expired", URI.JINGLE, doc)
        { }

        /// <summary>
        ///
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="qname"></param>
        /// <param name="doc"></param>
        public Expired(string prefix, XmlQualifiedName qname, XmlDocument doc)
            : base(prefix, qname, doc)
        { }

        /// <summary>
        /// What type of reason
        /// </summary>
        public override ReasonType ReasonType
        {
            get { return ReasonType.expired; }
        }
    }

    /// <summary>
    /// The party has been unable to initialize processing related to the application type
    /// </summary>
    public class FailedApplication : AbstractJingleReason
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="doc"></param>
        public FailedApplication(XmlDocument doc)
            : base("failed-application", URI.JINGLE, doc)
        { }

        /// <summary>
        ///
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="qname"></param>
        /// <param name="doc"></param>
        public FailedApplication(string prefix, XmlQualifiedName qname, XmlDocument doc)
            : base(prefix, qname, doc)
        { }

        /// <summary>
        /// What type of reason
        /// </summary>
        public override ReasonType ReasonType
        {
            get { return ReasonType.failed_application; }
        }
    }

    /// <summary>
    /// The party has been unable to establish connectivity for the transport method
    /// </summary>
    public class FailedTransport : AbstractJingleReason
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="doc"></param>
        public FailedTransport(XmlDocument doc)
            : base("failed-transport", URI.JINGLE, doc)
        { }

        /// <summary>
        ///
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="qname"></param>
        /// <param name="doc"></param>
        public FailedTransport(string prefix, XmlQualifiedName qname, XmlDocument doc)
            : base(prefix, qname, doc)
        { }

        /// <summary>
        /// What type of reason
        /// </summary>
        public override ReasonType ReasonType
        {
            get { return ReasonType.failed_transport; }
        }
    }

    /// <summary>
    /// The action is related to a non-specific application error
    /// </summary>
    public class GeneralError : AbstractJingleReason
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="doc"></param>
        public GeneralError(XmlDocument doc)
            : base("general-error", URI.JINGLE, doc)
        { }

        /// <summary>
        ///
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="qname"></param>
        /// <param name="doc"></param>
        public GeneralError(string prefix, XmlQualifiedName qname, XmlDocument doc)
            : base(prefix, qname, doc)
        { }

        /// <summary>
        /// What type of reason
        /// </summary>
        public override ReasonType ReasonType
        {
            get { return ReasonType.general_error; }
        }
    }

    /// <summary>
    /// The entity is going offline or is no longer available
    /// </summary>
    public class Gone : AbstractJingleReason
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="doc"></param>
        public Gone(XmlDocument doc)
            : base("gone", URI.JINGLE, doc)
        { }

        /// <summary>
        ///
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="qname"></param>
        /// <param name="doc"></param>
        public Gone(string prefix, XmlQualifiedName qname, XmlDocument doc)
            : base(prefix, qname, doc)
        { }

        /// <summary>
        /// What type of reason
        /// </summary>
        public override ReasonType ReasonType
        {
            get { return ReasonType.gone; }
        }
    }

    /// <summary>
    /// The party supports the offered application type but
    /// does not support the offered or negotiated parameters
    /// </summary>
    public class IncompatibleParameters : AbstractJingleReason
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="doc"></param>
        public IncompatibleParameters(XmlDocument doc)
            : base("incompatible-parameters", URI.JINGLE, doc)
        { }

        /// <summary>
        ///
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="qname"></param>
        /// <param name="doc"></param>
        public IncompatibleParameters(string prefix, XmlQualifiedName qname, XmlDocument doc)
            : base(prefix, qname, doc)
        { }

        /// <summary>
        /// What type of reason
        /// </summary>
        public override ReasonType ReasonType
        {
            get { return ReasonType.incompatible_parameters; }
        }
    }

    /// <summary>
    /// The action is related to media processing problems
    /// </summary>
    public class MediaError : AbstractJingleReason
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="doc"></param>
        public MediaError(XmlDocument doc)
            : base("media-error", URI.JINGLE, doc)
        { }

        /// <summary>
        ///
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="qname"></param>
        /// <param name="doc"></param>
        public MediaError(string prefix, XmlQualifiedName qname, XmlDocument doc)
            : base(prefix, qname, doc)
        { }

        /// <summary>
        /// What type of reason
        /// </summary>
        public override ReasonType ReasonType
        {
            get { return ReasonType.media_error; }
        }
    }

    /// <summary>
    /// The action is related to a violation of local security policies
    /// </summary>
    public class SecurityError : AbstractJingleReason
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="doc"></param>
        public SecurityError(XmlDocument doc)
            : base("security-error", URI.JINGLE, doc)
        { }

        /// <summary>
        ///
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="qname"></param>
        /// <param name="doc"></param>
        public SecurityError(string prefix, XmlQualifiedName qname, XmlDocument doc)
            : base(prefix, qname, doc)
        { }

        /// <summary>
        /// What type of reason
        /// </summary>
        public override ReasonType ReasonType
        {
            get { return ReasonType.security_error; }
        }
    }

    /// <summary>
    /// The action is generated during the normal course of state management
    /// and does not reflect any error
    /// </summary>
    public class Success : AbstractJingleReason
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="doc"></param>
        public Success(XmlDocument doc)
            : base("success", URI.JINGLE, doc)
        { }

        /// <summary>
        ///
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="qname"></param>
        /// <param name="doc"></param>
        public Success(string prefix, XmlQualifiedName qname, XmlDocument doc)
            : base(prefix, qname, doc)
        { }

        /// <summary>
        /// What type of reason
        /// </summary>
        public override ReasonType ReasonType
        {
            get { return ReasonType.success; }
        }
    }

    /// <summary>
    /// A request has not been answered so the sender is timing out the request.
    /// </summary>
    public class Timeout : AbstractJingleReason
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="doc"></param>
        public Timeout(XmlDocument doc)
            : base("timeout", URI.JINGLE, doc)
        { }

        /// <summary>
        ///
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="qname"></param>
        /// <param name="doc"></param>
        public Timeout(string prefix, XmlQualifiedName qname, XmlDocument doc)
            : base(prefix, qname, doc)
        { }

        /// <summary>
        /// What type of reason
        /// </summary>
        public override ReasonType ReasonType
        {
            get { return ReasonType.timeout; }
        }
    }

    /// <summary>
    /// The party supports none of the offered application types
    /// </summary>
    public class UnsupportedApplications : AbstractJingleReason
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="doc"></param>
        public UnsupportedApplications(XmlDocument doc)
            : base("unsupported-applications", URI.JINGLE, doc)
        { }

        /// <summary>
        ///
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="qname"></param>
        /// <param name="doc"></param>
        public UnsupportedApplications(string prefix, XmlQualifiedName qname, XmlDocument doc)
            : base(prefix, qname, doc)
        { }

        /// <summary>
        /// What type of reason
        /// </summary>
        public override ReasonType ReasonType
        {
            get { return ReasonType.unsupported_applications; }
        }
    }

    /// <summary>
    /// The party supports none of the offered transport methods
    /// </summary>
    public class UnsupportedTransports : AbstractJingleReason
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="doc"></param>
        public UnsupportedTransports(XmlDocument doc)
            : base("unsupported-transports", URI.JINGLE, doc)
        { }

        /// <summary>
        ///
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="qname"></param>
        /// <param name="doc"></param>
        public UnsupportedTransports(string prefix, XmlQualifiedName qname, XmlDocument doc)
            : base(prefix, qname, doc)
        { }

        /// <summary>
        /// What type of reason
        /// </summary>
        public override ReasonType ReasonType
        {
            get { return ReasonType.unsupported_transports; }
        }
    }
}
