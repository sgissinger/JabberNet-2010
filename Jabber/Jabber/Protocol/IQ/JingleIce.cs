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
using System.Text;
using System.Xml;

namespace Jabber.Protocol.IQ
{
    /// <summary>
    /// A jingle ice transport element which implements
    /// http://tools.ietf.org/html/rfc5245 hereafter referred to as ICE-CORE
    /// </summary>
    public class JingleIce : Element
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="doc"></param>
        public JingleIce(XmlDocument doc)
            : base("transport", URI.JINGLE_ICE, doc)
        { }

        /// <summary>
        ///
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="qname"></param>
        /// <param name="doc"></param>
        public JingleIce(string prefix, XmlQualifiedName qname, XmlDocument doc) :
            base(prefix, qname, doc)
        { }

        /// <summary>
        /// A Password as defined in ICE-CORE
        /// </summary>
        public String Pwd
        {
            get { return GetAttr("pwd"); }
            set { SetAttr("pwd", value); }
        }

        /// <summary>
        /// A User Fragment as defined in ICE-CORE
        /// </summary>
        public String Ufrag
        {
            get { return GetAttr("ufrag"); }
            set { SetAttr("ufrag", value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public JingleIceRemoteCandidate RemoteCandidate
        {
            get { return GetChildElement<JingleIceRemoteCandidate>(); }
            set { ReplaceChild<JingleIceRemoteCandidate>(value); }
        }

        /// <summary>
        /// Retrieve all of the candidates
        /// </summary>
        /// <returns></returns>
        public JingleIceCandidate[] GetCandidates()
        {
            return GetElements<JingleIceCandidate>().ToArray();
        }

        /// <summary>
        /// Add a new candidate to the list
        /// </summary>
        /// <returns></returns>
        public JingleIceCandidate AddCandidate()
        {
            //Debug.Assert(!String.IsNullOrEmpty(name));

            JingleIceCandidate cndt = CreateChildElement<JingleIceCandidate>();

            return cndt;
        }
    }

    /// <summary>
    /// A jingle ice candidate
    /// </summary>
    public class JingleIceCandidate : Element
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="doc"></param>
        public JingleIceCandidate(XmlDocument doc)
            : base("candidate", URI.JINGLE_ICE, doc)
        { }

        /// <summary>
        ///
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="qname"></param>
        /// <param name="doc"></param>
        public JingleIceCandidate(string prefix, XmlQualifiedName qname, XmlDocument doc) :
            base(prefix, qname, doc)
        { }

        /// <summary>
        /// A Component ID as defined in ICE-CORE
        /// </summary>
        public Byte Component
        {
            get { return GetByteAttr("component"); }
            set { SetByteAttr("component", value); }
        }

        /// <summary>
        /// A Foundation as defined in ICE-CORE
        /// </summary>
        public Byte Foundation
        {
            get { return GetByteAttr("foundation"); }
            set { SetByteAttr("foundation", value); }
        }

        /// <summary>
        /// An index, starting at 0, that enables the parties to keep
        /// track of updates to the candidate throughout the life of the session
        /// </summary>
        public Byte Generation
        {
            get { return GetByteAttr("generation"); }
            set { SetByteAttr("generation", value); }
        }

        /// <summary>
        /// A unique identifier for the candidate
        /// </summary>
        public String ID
        {
            get { return GetAttr("id"); }
            set { SetAttr("id", value); }
        }

        /// <summary>
        /// The Internet Protocol (IP) address for the candidate transport mechanism;
        /// this can be either an IPv4 address or an IPv6 address
        /// </summary>
        public String IP
        {
            get { return GetAttr("ip"); }
            set { SetAttr("ip", value); }
        }

        /// <summary>
        /// An index, starting at 0, referencing which network this candidate is on
        /// for a given peer
        /// (used for diagnostic purposes if the calling hardware has more than one Network Interface Card)
        /// </summary>
        public Byte Network
        {
            get { return GetByteAttr("network"); }
            set { SetByteAttr("network", value); }
        }

        /// <summary>
        /// The port at the candidate IP address
        /// </summary>
        public UInt16 Port
        {
            get { return GetUShortAttr("port"); }
            set { SetUShortAttr("port", value); }
        }

        /// <summary>
        /// A Priority as defined in ICE-CORE
        /// MUST be a positive integer
        /// </summary>
        public Int32 Priority
        {
            get { return GetIntAttr("priority"); }
            set { SetIntAttr("priority", value); }
        }

        /// <summary>
        /// The protocol to be used. The only value defined by XEP-0176 "udp"
        /// Future specifications might define other values such as "tcp"
        /// </summary>
        public IceProtocolType Protocol
        {
            get { return GetEnumAttr<IceProtocolType>("protocol"); }
            set { SetEnumAttr("protocol", value); }
        }

        /// <summary>
        /// A related address as defined in ICE-CORE
        /// </summary>
        public String RelatedAddress
        {
            get { return GetAttr("rel-addr"); }
            set { SetAttr("rel-addr", value); }
        }

        /// <summary>
        /// A related port as defined in ICE-CORE
        /// </summary>
        public UInt16 RelatedPort
        {
            get { return GetUShortAttr("rel-port"); }
            set { SetUShortAttr("rel-port", value); }
        }

        /// <summary>
        /// A Candidate Type as defined in ICE-CORE
        /// </summary>
        public IceCandidateType Type
        {
            get { return GetEnumAttr<IceCandidateType>("type"); }
            set { SetEnumAttr("type", value); }
        }
    }

    /// <summary>
    /// A Candidate Type as defined in ICE-CORE
    /// </summary>
    public enum IceCandidateType
    {
        /// <summary>
        /// None specified
        /// </summary>
        UNSPECIFIED = -1,
        /// <summary>
        /// Host candidates
        /// </summary>
        host,
        /// <summary>
        /// Peer reflexive 
        /// </summary>
        prflx,
        /// <summary>
        /// Relayed candidates
        /// </summary>
        relay,
        /// <summary>
        /// Server reflexive candidates
        /// </summary>
        srflx
    }

    /// <summary>
    /// The protocol to be used in ICE
    /// </summary>
    public enum IceProtocolType
    {
        /// <summary>
        /// None specified
        /// </summary>
        UNSPECIFIED = -1,
        /// <summary>
        /// udp protocol
        /// </summary>
        udp,
        /// <summary>
        /// tcp protocol. Not yet defined by XEP-0176
        /// I think that if UDP is defined, TCP will be defined one day
        /// </summary>
        tcp,
    }

    /// <summary>
    /// A jingle ice remote-candidate
    /// </summary>
    public class JingleIceRemoteCandidate : Element
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="doc"></param>
        public JingleIceRemoteCandidate(XmlDocument doc)
            : base("remote-candidate", URI.JINGLE_ICE, doc)
        { }

        /// <summary>
        ///
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="qname"></param>
        /// <param name="doc"></param>
        public JingleIceRemoteCandidate(string prefix, XmlQualifiedName qname, XmlDocument doc) :
            base(prefix, qname, doc)
        { }

        /// <summary>
        /// A Component ID as defined in ICE-CORE
        /// </summary>
        public Byte Component
        {
            get { return GetByteAttr("component"); }
            set { SetByteAttr("component", value); }
        }

        /// <summary>
        /// The Internet Protocol (IP) address for the remote-candidate transport mechanism;
        /// this can be either an IPv4 address or an IPv6 address
        /// </summary>
        public String IP
        {
            get { return GetAttr("ip"); }
            set { SetAttr("ip", value); }
        }

        /// <summary>
        /// The port at the remote-candidate IP address
        /// </summary>
        public UInt16 Port
        {
            get { return GetUShortAttr("port"); }
            set { SetUShortAttr("port", value); }
        }
    }
}
