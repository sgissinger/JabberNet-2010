using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Jabber.Protocol.Client;

namespace Jabber.Protocol.IQ
{
    /// <summary>
    /// The request cannot occur at this point in the state machine
    /// (e.g., session-initiate after session-accept).
    /// </summary>
    public class OutOfOrder : Element
    {
        /// <summary>
        /// Create for outbound.
        /// </summary>
        /// <param name="doc"></param>
        public OutOfOrder(XmlDocument doc)
            : base("out-of-order", URI.JINGLE_ERRORS, doc)
        { }

        /// <summary>
        /// Create for inbound
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="qname"></param>
        /// <param name="doc"></param>
        public OutOfOrder(string prefix, XmlQualifiedName qname, XmlDocument doc)
            : base(prefix, qname, doc)
        { }
    }

    /// <summary>
    /// The request is rejected because it was sent while
    /// the initiator was awaiting a reply on a similar request.
    /// </summary>
    public class TieBreak : Element
    {
        /// <summary>
        /// Create for outbound.
        /// </summary>
        /// <param name="doc"></param>
        public TieBreak(XmlDocument doc)
            : base("tie-break", URI.JINGLE_ERRORS, doc)
        { }

        /// <summary>
        /// Create for inbound
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="qname"></param>
        /// <param name="doc"></param>
        public TieBreak(string prefix, XmlQualifiedName qname, XmlDocument doc)
            : base(prefix, qname, doc)
        { }
    }

    /// <summary>
    /// The 'sid' attribute specifies a session that is unknown to the recipient
    /// (e.g., no longer live according to the recipient's state machine because
    /// the recipient previously terminated the session).
    /// </summary>
    public class UnknownSession : Element
    {
        /// <summary>
        /// Create for outbound.
        /// </summary>
        /// <param name="doc"></param>
        public UnknownSession(XmlDocument doc)
            : base("unknown-session", URI.JINGLE_ERRORS, doc)
        { }

        /// <summary>
        /// Create for inbound
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="qname"></param>
        /// <param name="doc"></param>
        public UnknownSession(string prefix, XmlQualifiedName qname, XmlDocument doc)
            : base(prefix, qname, doc)
        { }
    }

    /// <summary>
    /// The recipient does not support the informational payload of a session-info action.
    /// </summary>
    public class UnsupportedInfo : Element
    {
        /// <summary>
        /// Create for outbound.
        /// </summary>
        /// <param name="doc"></param>
        public UnsupportedInfo(XmlDocument doc)
            : base("unsupported-info", URI.JINGLE_ERRORS, doc)
        { }

        /// <summary>
        /// Create for inbound
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="qname"></param>
        /// <param name="doc"></param>
        public UnsupportedInfo(string prefix, XmlQualifiedName qname, XmlDocument doc)
            : base(prefix, qname, doc)
        { }
    }

    /// <summary>
    /// If one of the parties attempts to send information over the unsecured XMPP signalling
    /// channel that the other party expects to receive over the encrypted data channel
    /// the receiving party SHOULD return a <not-acceptable/> error to the sender,
    /// including a <security-required/> element
    /// </summary>
    public class SecurityRequired : Element
    {
        /// <summary>
        /// Create for outbound.
        /// </summary>
        /// <param name="doc"></param>
        public SecurityRequired(XmlDocument doc)
            : base("security-required", URI.JINGLE_ERRORS, doc)
        { }

        /// <summary>
        /// Create for inbound
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="qname"></param>
        /// <param name="doc"></param>
        public SecurityRequired(string prefix, XmlQualifiedName qname, XmlDocument doc)
            : base(prefix, qname, doc)
        { }
    }
}