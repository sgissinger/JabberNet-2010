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
using System.Text.RegularExpressions;
using System.Xml;
using Jabber.Protocol.Client;

namespace Jabber.Protocol.IQ
{
    /// <summary>
    /// IQ packet with an adhoc command query element inside.
    /// </summary>
    public class RpcIQ : TypedIQ<Rpc>
    {
        /// <summary>
        /// Create an adhoc command IQ
        /// </summary>
        /// <param name="doc"></param>
        public RpcIQ(XmlDocument doc)
            : base(doc)
        { }
    }

    /// <summary>
    /// An adhoc command query element.
    /// </summary>
    public class Rpc : Element
    {
        /// <summary>
        /// Create for outbound
        /// </summary>
        /// <param name="doc"></param>
        public Rpc(XmlDocument doc)
            : base("query", URI.RPC, doc)
        { }

        /// <summary>
        /// Create for inbound
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="qname"></param>
        /// <param name="doc"></param>
        public Rpc(string prefix, XmlQualifiedName qname, XmlDocument doc)
            : base(prefix, qname, doc)
        { }

        /// <summary>
        /// TODO: Documentation Property
        /// </summary>
        public void SetXmlRpcPayload(String value)
        {
            this.InnerXml = Regex.Replace(value, @"<\?xml(.+?)\?>", String.Empty);
        }
    }
}