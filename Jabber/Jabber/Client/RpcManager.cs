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
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using Bedrock;
using CookComputing.XmlRpc;
using Jabber.Connection;
using Jabber.Protocol;
using Jabber.Protocol.Client;
using Jabber.Protocol.IQ;

namespace Jabber.Client
{
    /// <summary>
    /// Manages XML-RPC calls with peers and uses XML-RPC .NET as described in the XML-RPC .NET library documentation.
    /// 
    /// Server-Side 
    /// ===========
    /// Once you created your own XmlRpcServerProtocol inherited classes you must register them with this manager
    /// using the RegisterXmlRpcServiceType() method and can be
    /// 
    /// RpcManager rpcManager = new RpcManager();
    /// rpcManager.RegisterXmlRpcServiceType(typeof(JabberRpcServer));
    /// 
    /// 
    /// Client-Side 
    /// ===========
    /// An XML-RPC server instance is started by calling Initialize() method
    /// and listens on http://127.0.0.1:xxxxx/ where xxxxx is an unused port.
    /// A "to" parameter must be added the proxy URL to set the recipient of the XML-RPC call.
    /// 
    /// Once an IXmlRpcProxy is created the Proxy Url property must be set with the "to" parameter 
    /// each time a call is made in order to route it to the right peer
    /// 
    /// JabberRpcClient rpcProxy = XmlRpcProxyGen.Create<JabberRpcClient>();
    /// rpcProxy.Url = "http://127.0.0.1:xxxxx/?to=bollos@example.com/watercloset";
    /// var result = rpcProxy.Discover();
    /// </summary>
    public partial class RpcManager : StreamComponent
    {
        #region MEMBERS
        /// <summary>
        /// TODO: Documentation Member
        /// </summary>
        private RosterManager m_roster = null;
        /// <summary>
        /// TODO: Documentation Member
        /// </summary>
        private Thread methodCallSerializerThread;
        /// <summary>
        /// TODO: Documentation Member
        /// </summary>
        private Boolean methodCallSerializerIsRunning;
        /// <summary>
        /// TODO: Documentation Member
        /// </summary>
        private HttpListener methodCallSerializer = new HttpListener();
        /// <summary>
        /// TODO: Documentation Member
        /// </summary>
        private Int32 methodCallSerializerPort = 0;
        /// <summary>
        /// TODO: Documentation Member
        /// </summary>
        private List<Type> xmlRpcServiceTypes = new List<Type>();
        /// <summary>
        /// TODO: Documentation Member
        /// </summary>
        private Dictionary<String, HttpListenerContext> xmlRpcContexts = new Dictionary<String, HttpListenerContext>();
        #endregion

        #region PROPERTIES
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
        /// <summary>
        /// TODO: Documentation Property
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<JID> TrustedJIDs { get; set; }
        /// <summary>
        /// TODO: Documentation Property
        /// </summary>
        [DefaultValue(true)]
        public Boolean IsRosterTrusted { get; set; }
        /// <summary>
        /// TODO: Documentation Property
        /// </summary>
        [DefaultValue(true)]
        public Boolean CanSendRpcXml { get; set; }
        /// <summary>
        /// TODO: Documentation Property
        /// </summary>
        [DefaultValue(true)]
        public Boolean CanReceiveRpcXml { get; set; }
        /// <summary>
        /// TODO: Documentation Property
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public String MethodCallSerializerAddress
        {
            get
            {
                if (this.methodCallSerializerPort == 0) // If no port configured, find a free one
                {
                    TcpListener listener = new TcpListener(IPAddress.Loopback, 0);
                    listener.Start();
                    this.methodCallSerializerPort = ((IPEndPoint)listener.LocalEndpoint).Port;
                    listener.Stop();
                }

                return String.Format(CultureInfo.CurrentCulture,
                                     "http://127.0.0.1:{0}/",
                                     this.methodCallSerializerPort);
            }
        }
        #endregion

        #region CONSTRUCTORS & FINALIZERS
        /// <summary>
        /// Creates a new RPC Manager inside a container
        /// </summary>
        /// <param name="container">Parent container</param>
        public RpcManager(IContainer container)
            : this()
        {
            container.Add(this);
        }

        /// <summary>
        /// Creates a new RPC Manager
        /// </summary>
        public RpcManager()
        {
            InitializeComponent();

            this.TrustedJIDs = new List<JID>();
            this.IsRosterTrusted = true;
            this.CanReceiveRpcXml = true;
            this.CanSendRpcXml = true;

            this.OnStreamChanged += new ObjectHandler(this.RpcManager_OnStreamChanged);
            this.Disposed += new EventHandler(this.RpcManager_Disposed);
        }

        /// <summary>
        /// TODO: Documentation 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RpcManager_Disposed(object sender, EventArgs e)
        {
            this.Stop();
        }
        #endregion

        #region XMPP EVENTS
        /// <summary>
        /// Entry point for XMPP stream events
        /// </summary>
        /// <param name="sender"></param>
        private void RpcManager_OnStreamChanged(object sender)
        {
            JabberClient cli = m_stream as JabberClient;

            if (cli == null)
                return;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="iq"></param>
        /// <param name="state"></param>
        private void GotRpcCall(object sender, IQ iq, object state)
        {
            if (!this.IsTrusted(iq.From))
                return;

            String rpcXmlString = Regex.Replace(iq.Query.FirstChild.OuterXml, " xmlns(.+?)>", ">");

            switch (iq.Type)
            {
                case IQType.set:
                    if (this.CanReceiveRpcXml)
                    {
                        RpcIQ methodResponse = new RpcIQ(new XmlDocument());
                        methodResponse.From = this.Stream.JID;
                        methodResponse.To = iq.From;
                        methodResponse.Type = IQType.result;
                        methodResponse.ID = iq.ID; // Make IQ ID behaving like a transaction ID
                        methodResponse.Instruction.SetXmlRpcPayload(this.InvokeXmlRpc(rpcXmlString));

                        this.BeginIQ(methodResponse, new IqCB(this.GotRpcCall), null);

                        iq.Handled = true;
                    }
                    break;

                case IQType.result:
                    if (this.CanSendRpcXml && this.xmlRpcContexts.ContainsKey(iq.ID))
                    {
                        byte[] outputByteArray = Encoding.UTF8.GetBytes(rpcXmlString);
                        MemoryStream outputStream = new MemoryStream(outputByteArray);

                        HttpListenerResponse response = this.xmlRpcContexts[iq.ID].Response;

                        response.ContentLength64 = outputStream.Length;

                        Util.CopyStream(outputStream, response.OutputStream);

                        response.OutputStream.Flush();

                        this.xmlRpcContexts.Remove(iq.ID);

                        iq.Handled = true;
                    }
                    break;
            }
        }
        #endregion

        #region METHODS
        /// <summary>
        /// TODO: Documentation Initialize
        /// </summary>
        public void Initialize()
        {
            if (this.CanSendRpcXml)
            {
                this.methodCallSerializerIsRunning = true;
                this.methodCallSerializerThread = new Thread(new ThreadStart(this.MethodCallSerializerThreadStart));
                this.methodCallSerializerThread.IsBackground = true;
                this.methodCallSerializerThread.Start();
            }
        }

        /// <summary>
        /// TODO: Documentation RegisterXmlRpcServiceType
        /// </summary>
        /// <param name="type"></param>
        public void RegisterXmlRpcServerType(Type type)
        {
            if (!this.xmlRpcServiceTypes.Contains(type))
                this.xmlRpcServiceTypes.Add(type);
        }

        /// <summary>
        /// TODO: Documentation IsTrusted
        /// </summary>
        /// <param name="jid"></param>
        /// <returns></returns>
        private Boolean IsTrusted(JID jid)
        {
            if (jid.BareJID == this.Stream.JID.BareJID)
                return true;

            if (this.IsRosterTrusted && this.RosterManager[jid.BareJID] != null)
                return true;

            if (this.TrustedJIDs.Contains(jid.BareJID))
                return true;

            return false;
        }

        /// <summary>
        /// TODO: Documentation MethodCallSerializerThreadStart
        /// </summary>
        private void MethodCallSerializerThreadStart()
        {

            //IPAddress.Loopback, 0
            this.methodCallSerializer.Prefixes.Add(this.MethodCallSerializerAddress);
            this.methodCallSerializer.Start();

            while (this.methodCallSerializerIsRunning)
            {
                try
                {
                    HttpListenerContext context = this.methodCallSerializer.GetContext();

                    MatchCollection matches = Regex.Matches(context.Request.Url.Query, @"(\&|\?)(?:(?<key>.[^\=\&]*)\=(?<val>.[^\=\&]*))");
                    String to = null;

                    foreach (Match match in matches)
                    {
                        switch (match.Groups["key"].Value)
                        {
                            case "to":
                                to = Uri.UnescapeDataString(match.Groups["val"].Value);
                                break;
                        }

                        if (!String.IsNullOrEmpty(to))
                            break;
                    }

                    StreamReader inputReader = new StreamReader(context.Request.InputStream);
                    String inputString = inputReader.ReadToEnd();

                    RpcIQ methodCall = new RpcIQ(new XmlDocument());

                    methodCall.From = this.Stream.JID;
                    methodCall.To = to;
                    methodCall.Type = IQType.set;
                    methodCall.Instruction.SetXmlRpcPayload(inputString);

                    this.xmlRpcContexts.Add(methodCall.ID, context);

                    this.BeginIQ(methodCall, new IqCB(this.GotRpcCall), null);
                }
                catch (HttpListenerException)
                {
                    this.Stop();
                }
            }
        }

        /// <summary>
        /// TODO: Documentation GetMatchingXmlRpcType
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        private Type GetMatchingXmlRpcType(String inputString)
        {
            Match match = Regex.Match(inputString, "<methodName>(?<methodName>.+?)</methodName>");
            String methodName = null;

            if (match.Success)
                methodName = match.Groups["methodName"].Value;

            foreach (Type type in this.xmlRpcServiceTypes)
            {
                foreach (MethodInfo method in type.GetMethods())
                {
                    Object[] attributes = method.GetCustomAttributes(typeof(XmlRpcMethodAttribute), false);
                    XmlRpcMethodAttribute methodAttribute = null;

                    if (attributes.Length > 0)
                        methodAttribute = attributes[0] as XmlRpcMethodAttribute;

                    if (methodAttribute != null && methodAttribute.Method.Equals(methodName))
                        return type;

                    if (method.Name.Equals(methodName))
                        return type;
                }
            }
            return null;
        }

        /// <summary>
        /// TODO: Documentation InvokeXmlRpc
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        private String InvokeXmlRpc(String inputString)
        {
            Type matchingType = this.GetMatchingXmlRpcType(inputString);

            // Parse and execute input XML-RPC
            XmlRpcServerProtocol svc = Activator.CreateInstance(matchingType) as XmlRpcServerProtocol;
            
            byte[] inputByteArray = Encoding.UTF8.GetBytes(inputString);
            MemoryStream inputStream = new MemoryStream(inputByteArray);

            StreamReader outputReader = new StreamReader(svc.Invoke(inputStream));
            
            return outputReader.ReadToEnd();
        }

        /// <summary>
        /// TODO: Documentation Stop
        /// </summary>
        private void Stop()
        {
            if (this.methodCallSerializerIsRunning)
            {
                this.methodCallSerializerIsRunning = false;
                this.xmlRpcContexts.Clear();
                this.methodCallSerializer.Close();
            }
        }
        #endregion
    }
}