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
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using Jabber.Stun.Attributes;

namespace Jabber.Stun
{
    public delegate void MessageReceptionHandler(Object sender, StunMessage receivedMsg, StunMessage sentMsg, Object transactionObject);

    public delegate void IndicationReceptionHandler(Object sender, StunMessage receivedMsg);

    /// <summary>
    /// Represents a STUN Client that can send request to a STUN Server
    /// and receive response from it (or not)
    /// </summary>
    public class StunClient
    {
        #region CONSTANTS
        /// <summary>
        /// IANA has assigned port number 3478 for the "stun"
        /// service, defined over TCP and UDP.
        /// </summary>
        public const Int32 DEFAULT_STUN_PORT = 3478;
        /// <summary>
        /// IANA has assigned port number 5349 for the "stuns" (stun-secured)
        /// service, defined over TCP and UDP.
        /// The UDP port is not currently defined. However, it is reserved for future use
        /// </summary>
        public const Int32 DEFAULT_STUNS_PORT = 5349;
        /// <summary>
        /// Socket receive buffer
        /// </summary>
        private const Int32 BUFFER_SIZE = 8192;
        #endregion

        #region EVENTS
        /// <summary>
        /// TODO: Documentation Event
        /// </summary>
        public event MessageReceptionHandler OnReceivedSuccessResponse;
        /// <summary>
        /// TODO: Documentation Event
        /// </summary>
        public event IndicationReceptionHandler OnReceivedIndication;
        /// <summary>
        /// TODO: Documentation Event
        /// </summary>
        public event MessageReceptionHandler OnReceivedError;
        #endregion

        #region PROPERTIES
        /// <summary>
        /// Contains the socket instance used to communicate with the STUN Server
        /// </summary>
        public Socket Socket { get; private set; }
        /// <summary>
        /// Contains the type of protocol (udp, tcp) used to communicate with the STUN Server
        /// </summary>
        public ProtocolType ProtocolType { get; private set; }
        /// <summary>
        /// Contains the type of socket (datagram, stream) used to communicate with the STUN Server
        /// </summary>
        public SocketType SocketType { get; private set; }
        /// <summary>
        /// Contains the IPEndPoint of the STUN Server
        /// </summary>
        public IPEndPoint ServerEP { get; private set; }
        /// <summary>
        /// Contains the IPEndPoint of the LocalEndPoint used by this client
        /// Unless Reset() method is fired every new connection to a STUN Server will be bound to this IPEndPoint
        /// </summary>
        public IPEndPoint StunningEP { get; private set; }
        /// <summary>
        /// TODO: Documentation Property
        /// </summary>
        public X509Certificate2 ClientCertificate { get; private set; }
        /// <summary>
        /// TODO: Documentation Property
        /// </summary>
        public RemoteCertificateValidationCallback RemoteCertificateValidationHandler { get; private set; }
        /// <summary>
        /// Contains the TcpClient handling TLS over TCP connection with the STUN Server
        /// </summary>
        private TcpClient SslClient { get; set; }
        /// <summary>
        /// Contains the SslStream handling TLS over TCP communication with the STUN Server
        /// </summary>
        private SslStream SslStream { get; set; }
        /// <summary>
        /// Contains True if the current connected Socket must use TLS over TCP to communicate with the STUN Server
        /// </summary>
        public Boolean UseSsl
        {
            get { return this.ClientCertificate != null; }
        }
        /// <summary>
        /// TODO: Documentation Property
        /// </summary>
        public Boolean Connected
        {
            get { return this.Socket != null ? this.Socket.Connected : false; }
        }
        /// <summary>
        /// TODO: Documentation Property
        /// </summary>
        private Dictionary<byte[], KeyValuePair<StunMessage, Object>> PendingTransactions { get; set; }
        #endregion

        #region CONSTRUCTORS & FINALIZERS
        /// <summary>
        /// Constructs a StunClient 
        /// </summary>
        /// <param name="stunServerEP">The IPEndPoint where the STUN Server can be reached</param>
        /// <param name="protocolType">The protocol type (UDP or TCP only) used to communicate with the STUN Server</param>
        /// <param name="clientCertificate">
        /// Client certificate used for mutual authentication. This certificate must be in PKCS #12 format and must contains its private key
        /// The simpler way to create a certificate of this type is to follow this makecert tutorial http://www.inventec.ch/chdh/notes/14.htm.
        /// Once your certificate is created : launch "mmc", CTRL+M, select "Certificates", add, choose "Local machine".
        /// Find your certificate under "Personal", it must have a little key in its icon, right click on it, choose "All tasks > Export...".
        /// Check the "Export key" checkbox, finish the process and then you have a valid X509Certificate2 with its private key in it
        /// </param>
        /// <param name="remoteCertificateValidationHandler">The callback handler which validate STUN Server TLS certificate</param>
        public StunClient(IPEndPoint stunServerEP, ProtocolType protocolType, X509Certificate2 clientCertificate, RemoteCertificateValidationCallback remoteCertificateValidationHandler)
            : this(null, stunServerEP, protocolType, clientCertificate, remoteCertificateValidationHandler)
        { }

        /// <summary>
        /// Constructs a StunClient using an existing IPEndPoint
        /// </summary>
        /// <param name="stunningEP">The IPEndPoint to which the socket will be bound to</param>
        /// <param name="stunServerEP">The IPEndPoint where the STUN Server can be reached</param>
        /// <param name="protocolType">The protocol type (UDP or TCP only) used to communicate with the STUN Server</param>
        /// <param name="clientCertificate">
        /// Client certificate used for mutual authentication. This certificate must be in PKCS #12 format and must contains its private key
        /// The simpler way to create a certificate of this type is to follow this makecert tutorial http://www.inventec.ch/chdh/notes/14.htm.
        /// Once your certificate is created : launch "mmc", CTRL+M, select "Certificates", add, choose "Local machine".
        /// Find your certificate under "Personal", it must have a little key in its icon, right click on it, choose "All tasks > Export...".
        /// Check the "Export key" checkbox, finish the process and then you have a valid X509Certificate2 with its private key in it
        /// </param>
        /// <param name="remoteCertificateValidationHandler">The callback handler which validate STUN Server TLS certificate</param>
        public StunClient(IPEndPoint stunningEP, IPEndPoint stunServerEP, ProtocolType protocolType, X509Certificate2 clientCertificate, RemoteCertificateValidationCallback remoteCertificateValidationHandler)
        {
            this.PendingTransactions = new Dictionary<byte[], KeyValuePair<StunMessage, Object>>(new ByteArrayComparer());

            this.StunningEP = stunningEP;

            this.ServerEP = stunServerEP;
            this.ProtocolType = protocolType;
            this.ClientCertificate = clientCertificate;

            if(this.UseSsl && remoteCertificateValidationHandler == null)
                    throw new ArgumentException("You must provide a valid RemoteCertificateValidationCallback", "remoteCertificateValidationHandler");

            this.RemoteCertificateValidationHandler = remoteCertificateValidationHandler;

            if (this.ProtocolType != ProtocolType.Tcp && this.ProtocolType != ProtocolType.Udp)
                throw new ArgumentException("Only UDP and TCP are acceptable values", "protocolType");

            if (this.UseSsl && this.ProtocolType != ProtocolType.Tcp)
                throw new ArgumentException("Only TCP can be used in with SSL", "protocolType");
        }

        /// <summary>
        /// Sample usage of this StunClient
        /// </summary>
        /// <param name="args">Unused</param>
        private static void Main(string[] args)
        {
            KeyValuePair<IPEndPoint, IPEndPoint> stunKeyValue = StunUtilities.GetMappedAddressFrom(null,
                                                                                                   new IPEndPoint(IPAddress.Parse("66.228.45.110"), StunClient.DEFAULT_STUN_PORT),
                                                                                                   ProtocolType.Udp);

            StunMessage msg = new StunMessage(StunMethodType.Binding, StunMethodClass.Request, StunUtilities.NewTransactionId);
            msg.Stun.Realm = new UTF8Attribute(StunAttributeType.Realm, "Hello World !");
            msg.Stun.Username = new UTF8Attribute(StunAttributeType.Username, "Bob");

            byte[] octets = msg;
            StunMessage msgCopy = octets;

            // Reuse of an existing local IPEndPoint makes the three requests returning 
            // the same MappedAddress IPEndPoint if this client is behind a Cone NAT
            StunClient cli1 = new StunClient(stunKeyValue.Key,
                                             new IPEndPoint(IPAddress.Parse("66.228.45.110"), StunClient.DEFAULT_STUNS_PORT),
                                             ProtocolType.Tcp, null,
                                             (sender, certificate, chain, sslPolicyErrors) => true);

            // Sample TLS over TCP working with ejabberd but may not work with the sample server IP given here
            cli1.Connect();
            StunMessage resp1 = cli1.SendMessage(msgCopy);
            cli1.Close();

            msgCopy.ClearAttributes();

            StunClient cli2 = new StunClient(stunKeyValue.Key,
                                             new IPEndPoint(IPAddress.Parse("132.177.123.13"), StunClient.DEFAULT_STUN_PORT),
                                             ProtocolType.Udp, null, null);

            cli2.Connect();
            StunMessage resp2 = cli2.SendMessage(msgCopy);
            cli2.Close();
        }
        #endregion

        #region METHODS
        /// <summary>
        /// Opens a socket connection to a STUN Server
        /// </summary>
        public void Connect()
        {
            if (this.Socket != null)
                throw new ArgumentException("StunClient socket is not null, you must close it before doing any new connection", "this.Socket");


            if (this.ProtocolType == ProtocolType.Tcp)
                this.SocketType = SocketType.Stream;
            else
                this.SocketType = SocketType.Dgram;


            this.Socket = new Socket(this.ServerEP.AddressFamily, this.SocketType, this.ProtocolType);
            this.Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);


            if (this.StunningEP != null)
                this.Socket.Bind(this.StunningEP);


            if (this.UseSsl)
            {
                this.SslClient = new TcpClient();
                this.SslClient.Client = this.Socket;
                this.SslClient.Connect(this.ServerEP);

                LocalCertificateSelectionCallback localCertificateSelectionHandler = null;
                localCertificateSelectionHandler = (sender, targetHost, localCertificates, remoteCertificate, acceptableIssuers) => localCertificates[0];

                X509Certificate2Collection clientCertificates = new X509Certificate2Collection();
                clientCertificates.Add(this.ClientCertificate);

                this.SslStream = new SslStream(this.SslClient.GetStream(), false,
                                               this.RemoteCertificateValidationHandler,
                                               localCertificateSelectionHandler);

                IAsyncResult ar = this.SslStream.BeginAuthenticateAsClient(this.ServerEP.Address.ToString(), clientCertificates, SslProtocols.Tls, true, null, null);

                ar.AsyncWaitHandle.WaitOne();

                this.SslStream.EndAuthenticateAsClient(ar);

                byte[] result = new byte[StunClient.BUFFER_SIZE];

                this.SslStream.BeginRead(result, 0, result.Length, new AsyncCallback(this.ReceiveCallback), result);
            }
            else
            {
                this.Socket.Connect(this.ServerEP);

                byte[] result = new byte[StunClient.BUFFER_SIZE];

                this.Socket.BeginReceive(result, 0, result.Length, SocketFlags.None, new AsyncCallback(this.ReceiveCallback), result);
            }

            if (this.StunningEP == null)
                this.StunningEP = (IPEndPoint)this.Socket.LocalEndPoint;
        }

        /// <summary>
        /// Closes the socket but do not reset the StunningEP to make the client able
        /// to send requests to a STUN Server using the same StunningEP
        /// </summary>
        public void Close()
        {
            if (this.Socket.Connected)
                this.Socket.Shutdown(SocketShutdown.Both);

            this.Socket.Close();

            if (this.UseSsl)
            {
                this.SslStream.Close();
                this.SslClient.Close();
            }

            this.Socket = null;
            this.SslStream = null;
            this.SslClient = null;
            this.ClientCertificate = null;
        }

        /// <summary>
        /// Resets everything and make this client able to connect to a STUN Server using a new StunningEP
        /// </summary>
        public void Reset()
        {
            if (this.Socket != null)
                this.Close();

            this.ServerEP = null;
            this.ProtocolType = ProtocolType.Unknown;
            this.SocketType = SocketType.Unknown;
            this.StunningEP = null;
        }

        /// <summary>
        /// Sends a StunMessage to the connected STUN Server.
        /// </summary>
        /// <param name="msgToSend">The StunMessage to send to the connected STUN Server</param>
        /// <returns>The STUN Server response or null if msgToSend parameter is not a Request</returns>
        public StunMessage SendMessage(StunMessage msgToSend)
        {
            Boolean waitForResponse = msgToSend.MethodClass == StunMethodClass.Request;

            byte[] msgReceived = new byte[StunClient.BUFFER_SIZE];

            if (this.UseSsl)
            {
                this.SslStream.Write(msgToSend, 0, msgToSend.Bytes.Length);

                if (waitForResponse)
                    this.SslStream.Read(msgReceived, 0, StunClient.BUFFER_SIZE);
            }
            else
            {
                this.Socket.Send(msgToSend, 0, msgToSend.Bytes.Length, SocketFlags.None);

                if (waitForResponse)
                    this.Socket.Receive(msgReceived, 0, StunClient.BUFFER_SIZE, SocketFlags.None);
            }

            if (waitForResponse &&
                new ByteArrayComparer().Equals(((StunMessage)msgReceived).TransactionID, msgToSend.TransactionID))
            {
                return msgReceived;
            }

            return null;
        }

        /// <summary>
        /// Sends a StunMessage to the connected STUN Server.
        /// </summary>
        /// <param name="msgToSend">The StunMessage to send to the connected STUN Server</param>
        /// <param name="transactionObject"></param>
        public void BeginSendMessage(StunMessage msgToSend, Object transactionObject)
        {
            if(msgToSend.MethodClass == StunMethodClass.Request)
                this.PendingTransactions.Add(msgToSend.TransactionID, new KeyValuePair<StunMessage, Object>(msgToSend, transactionObject));

            if (this.UseSsl)
                this.SslStream.BeginWrite(msgToSend, 0, msgToSend.Bytes.Length, new AsyncCallback(this.SendCallback), null);
            else
                this.Socket.BeginSend(msgToSend, 0, msgToSend.Bytes.Length, SocketFlags.None, new AsyncCallback(this.SendCallback), null);
        }

        /// <summary>
        /// TODO: Documentation IsPendingTransaction
        /// </summary>
        /// <param name="transactionID"></param>
        /// <param name="transactionObject"></param>
        /// <returns></returns>
        private Boolean IsPendingTransaction(byte[] transactionID, out KeyValuePair<StunMessage, Object> transactionObject)
        {
            if (!this.PendingTransactions.ContainsKey(transactionID))
            {
                transactionObject = new KeyValuePair<StunMessage, Object>();
                return false;
            }

            transactionObject = this.PendingTransactions[transactionID];

            this.PendingTransactions.Remove(transactionID);

            return true;
        }

        /// <summary>
        /// TODO: Documentation SendCallback
        /// </summary>
        /// <param name="ar"></param>
        private void SendCallback(IAsyncResult ar)
        {
            if (this.UseSsl)
                this.SslStream.EndWrite(ar);
            else
                this.Socket.EndSend(ar);
        }

        /// <summary>
        /// TODO: Documentation ReceiveCallback
        /// </summary>
        /// <param name="ar"></param>
        private void ReceiveCallback(IAsyncResult ar)
        {
            byte[] state = (byte[])ar.AsyncState;

            Int32 bytesReceived;

            if (this.UseSsl)
                bytesReceived = this.SslStream.EndRead(ar);
            else
                bytesReceived = this.Socket.EndReceive(ar);


            if (bytesReceived > 0)
            {
                StunMessage receivedMsg = state;

                KeyValuePair<StunMessage, Object> transactionObject;

                switch(receivedMsg.MethodClass)
                {
                    case StunMethodClass.SuccessResponse:
                        if (!this.IsPendingTransaction(receivedMsg.TransactionID, out transactionObject))
                            return;

                        this.OnReceivedSuccessResponse(this, receivedMsg, transactionObject.Key, transactionObject.Value);
                        break;

                    case StunMethodClass.Indication:
                        this.OnReceivedIndication(this, receivedMsg);
                        break;

                    case StunMethodClass.Error:
                        if (!this.IsPendingTransaction(receivedMsg.TransactionID, out transactionObject))
                            return;

                        this.OnReceivedError(this, receivedMsg, transactionObject.Key, transactionObject.Value);
                        break;
                }

                byte[] result = new byte[StunClient.BUFFER_SIZE];

                if (this.Socket != null)
                {
                    if (this.UseSsl)
                        this.SslStream.BeginRead(result, 0, result.Length, new AsyncCallback(this.ReceiveCallback), result);
                    else
                        this.Socket.BeginReceive(result, 0, result.Length, SocketFlags.None, new AsyncCallback(this.ReceiveCallback), result);
                }
            }
        }
        #endregion
    }
}