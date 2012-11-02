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
using System;

namespace Jabber.Protocol.IQ
{
    /// <summary>
    /// IQ packet with an adhoc command query element inside.
    /// </summary>
    public class CommandIQ : TypedIQ<Command>
    {
        /// <summary>
        /// Create an adhoc command IQ
        /// </summary>
        /// <param name="doc"></param>
        public CommandIQ(XmlDocument doc)
            : base(doc)
        { }
    }

    /// <summary>
    /// An adhoc command query element.
    /// </summary>
    public class Command : Element
    {
        /// <summary>
        /// Create for outbound
        /// </summary>
        /// <param name="doc"></param>
        public Command(XmlDocument doc)
            : base("command", URI.ADHOC_COMMANDS, doc)
        { }

        /// <summary>
        /// Create for inbound
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="qname"></param>
        /// <param name="doc"></param>
        public Command(string prefix, XmlQualifiedName qname, XmlDocument doc)
            : base(prefix, qname, doc)
        { }

        /// <summary>
        /// 
        /// </summary>
        public CommandActionType Action
        {
            get { return GetEnumAttr<CommandActionType>("action"); }
            set { SetEnumAttr("action", value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public String Node
        {
            get { return GetAttr("node"); }
            set { SetAttr("node", value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public String SessionId
        {
            get { return GetAttr("sessionid"); }
            set { SetAttr("sessionid", value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public CommandStatusType Status
        {
            get { return GetEnumAttr<CommandStatusType>("status"); }
            set { SetEnumAttr("status", value); }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [Dash]
    public enum CommandActionType
    {
        /// <summary>
        /// None specified
        /// </summary>
        UNSPECIFIED = -1,
        cancel,
        complete,
        execute,
        next,
        prev
    }

    /// <summary>
    /// 
    /// </summary>
    [Dash]
    public enum CommandStatusType
    {
        /// <summary>
        /// None specified
        /// </summary>
        UNSPECIFIED = -1,
        canceled,
        completed,
        executing,
    }
}