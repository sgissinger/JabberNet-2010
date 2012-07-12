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
    public static class JingleUtilities
    {
        #region JINGLE CORE
        /// <summary>
        /// 
        /// </summary>
        public static String GenerateSid
        {
            get { return Guid.NewGuid().ToString("N"); }
        }
        #endregion

        #region JINGLE ICE
        /// <summary>
        /// 
        /// </summary>
        public static String GenerateIcePwd
        {
            get { return Guid.NewGuid().ToString("N").Substring(0, 22); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static String GenerateIceUfrag
        {
            get { return Guid.NewGuid().ToString("N").Substring(0, 4); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static String GenerateIceCandidateId
        {
            get { return Guid.NewGuid().ToString("N").Substring(0, 10); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static UInt32 GenerateIceCandidatePriority
        {
            get
            {
                Random random = new Random();
                String r = random.Next(1, 3).ToString();

                Int32 i;

                for (i = 1; i < 10; i++)
                    r += random.Next(0, 9).ToString();

                return UInt32.Parse(r);
            }
        }
        #endregion
    }

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