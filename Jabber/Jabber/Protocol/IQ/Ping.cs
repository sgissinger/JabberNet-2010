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
using System.Xml;
using Jabber.Protocol.Client;

namespace Jabber.Protocol.IQ
{
    /// <summary>
    /// IQ packet with a ping query element inside.
    /// </summary>
    public class PingIQ : TypedIQ<Ping>
    {
        /// <summary>
        /// Create a ping IQ
        /// </summary>
        /// <param name="doc"></param>
        public PingIQ(XmlDocument doc)
            : base(doc)
        { }
    }

    /// <summary>
    /// A ping query element.
    /// </summary>
    public class Ping : Element
    {
        /// <summary>
        /// Create for outbound
        /// </summary>
        /// <param name="doc"></param>
        public Ping(XmlDocument doc)
            : base("ping", URI.PING, doc)
        { }

        /// <summary>
        /// Create for inbound
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="qname"></param>
        /// <param name="doc"></param>
        public Ping(string prefix, XmlQualifiedName qname, XmlDocument doc)
            : base(prefix, qname, doc)
        { }
    }
}