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
using System.Diagnostics;
using System.Net;
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
        /// Create for outbound
        /// </summary>
        /// <param name="doc"></param>
        public JingleIce(XmlDocument doc)
            : base("transport", URI.JINGLE_ICE, doc)
        { }

        /// <summary>
        /// Create for inbound
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
        /// Retrieve a single candidate
        /// </summary>
        /// <returns></returns>
        public JingleIceCandidate GetCandidate(Int32 index)
        {
            return GetCandidates()[index];
        }

        /// <summary>
        /// Add a new candidate to the list
        /// </summary>
        /// <param name="component"></param>
        /// <param name="foundation"></param>
        /// <param name="generation"></param>
        /// <param name="id"></param>
        /// <param name="ip"></param>
        /// <param name="network"></param>
        /// <param name="port"></param>
        /// <param name="priority"></param>
        /// <param name="protocol"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public JingleIceCandidate AddCandidate(Byte component, Byte foundation, Byte generation, String id, Byte network, IPAddress ip, UInt16 port, UInt32 priority, IceProtocolType protocol, IceCandidateType type)
        {
            return AddCandidate(component, foundation, generation, id, network, ip, port, priority, protocol, type, null, 0);
        }

        /// <summary>
        /// Add a new candidate to the list
        /// </summary>
        /// <param name="component"></param>
        /// <param name="foundation"></param>
        /// <param name="generation"></param>
        /// <param name="id"></param>
        /// <param name="ip"></param>
        /// <param name="network"></param>
        /// <param name="port"></param>
        /// <param name="priority"></param>
        /// <param name="protocol"></param>
        /// <param name="type"></param>
        /// <param name="relatedAddress"></param>
        /// <param name="relatedPort"></param>
        /// <returns></returns>
        public JingleIceCandidate AddCandidate(Byte component, Byte foundation, Byte generation, String id, Byte network, IPAddress ip, UInt16 port, UInt32 priority, IceProtocolType protocol, IceCandidateType type, IPAddress relatedAddress, UInt16 relatedPort)
        {
            Debug.Assert(component > 0);
            Debug.Assert(foundation > 0);
            Debug.Assert(!String.IsNullOrEmpty(id));
            Debug.Assert(!String.IsNullOrEmpty(ip.ToString()));
            Debug.Assert(network > 0);
            Debug.Assert(port > 1024);
            Debug.Assert(priority.ToString().Length == 10);
            Debug.Assert(protocol != IceProtocolType.UNSPECIFIED);
            Debug.Assert(type != IceCandidateType.UNSPECIFIED);

            JingleIceCandidate cndt = CreateChildElement<JingleIceCandidate>();
            cndt.Component = component;
            cndt.Foundation = foundation;
            cndt.Generation = generation;
            cndt.ID = id;
            cndt.IP = ip;
            cndt.Network = network;
            cndt.Port = port;
            cndt.Priority = priority;
            cndt.Protocol = protocol;

            if (relatedAddress != null)
                cndt.RelatedAddress = relatedAddress;

            if (relatedPort != 0)
                cndt.RelatedPort = relatedPort;

            cndt.Type = type;

            return cndt;
        }

        /// <summary>
        /// TODO: Documentation CandidatesEP
        /// </summary>
        /// <param name="jingle"></param>
        /// <returns></returns>
        public static IEnumerable<IPEndPoint> CandidatesEP(Jingle jingle)
        {
            List<IPEndPoint> result = new List<IPEndPoint>();

            JingleContent jingleContent = jingle.GetContent(0);
            JingleIce ice = jingleContent.GetElement<JingleIce>(0);

            if (ice != null)
            {
                foreach (JingleIceCandidate iceCandidate in ice.GetCandidates())
                {
                    result.Add(new IPEndPoint(iceCandidate.IP, (Int32)iceCandidate.Port));
                }
            }
            return result;
        }
    }

    /// <summary>
    /// A jingle ice candidate
    /// </summary>
    public class JingleIceCandidate : Element
    {
        /// <summary>
        /// Create for inbound
        /// </summary>
        /// <param name="doc"></param>
        public JingleIceCandidate(XmlDocument doc)
            : base("candidate", URI.JINGLE_ICE, doc)
        { }

        /// <summary>
        /// Create for outbound
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
        /// The Internet Protocol (IP) address for the candidate transport mechanism;
        /// this can be either an IPv4 address or an IPv6 address
        /// </summary>
        public IPAddress IP
        {
            get { return IPAddress.Parse(GetAttr("ip")); }
            set { SetAttr("ip", value.ToString()); }
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
        public UInt32 Priority
        {
            get { return GetUIntAttr("priority"); }
            set { SetUIntAttr("priority", value); }
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
        public IPAddress RelatedAddress
        {
            get { return IPAddress.Parse(GetAttr("rel-addr")); }
            set { SetAttr("rel-addr", value.ToString()); }
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
        /// Create for outbound
        /// </summary>
        /// <param name="doc"></param>
        public JingleIceRemoteCandidate(XmlDocument doc)
            : base("remote-candidate", URI.JINGLE_ICE, doc)
        { }

        /// <summary>
        /// Create for inbound
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
