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
using System.Security.Cryptography.X509Certificates;
using System.Timers;
using Jabber.Stun.Attributes;

namespace Jabber.Stun.Turn
{
    public delegate void AllocateSuccessHandler(Object sender, TurnAllocation allocation, StunMessage sentMsg, StunMessage receivedMsg);

    public delegate void CreatePermissionSuccessHandler(Object sender, TurnAllocation allocation, TurnPermission permission, StunMessage sentMsg, StunMessage receivedMsg);

    public delegate void ChannelBindSuccessHandler(Object sender, TurnAllocation allocation, TurnChannel channel, StunMessage sentMsg, StunMessage receivedMsg);

    public delegate void ConnectionBindSuccessHandler(Object sender, Socket connectedSocket, StunMessage receivedMsg);

    /// <summary>
    /// TODO: Documentation Class
    /// </summary>
    public class TurnManager
    {
        #region MEMBERS
        /// <summary>
        /// TODO: Documentation Propery
        /// </summary>
        private StunClient StunClient { get; set; }
        /// <summary>
        /// TODO: Documentation Member
        /// </summary>
        private TurnManager TurnTcpManager { get; set; }
        /// <summary>
        /// TODO: Documentation Member
        /// </summary>
        private Timer RefreshTimer { get; set; }
        /// <summary>
        /// TODO: Documentation Member
        /// </summary>
        private Dictionary<byte[], KeyValuePair<StunMessage, Object>> PendingTransactions { get; set; }
        #endregion

        #region EVENTS
        /// <summary>
        /// TODO: Documentation Event
        /// </summary>
        public event AllocateSuccessHandler OnAllocateSucceed;
        /// <summary>
        /// TODO: Documentation Event
        /// </summary>
        public event CreatePermissionSuccessHandler OnCreatePermissionSucceed;
        /// <summary>
        /// TODO: Documentation Event
        /// </summary>
        public event ChannelBindSuccessHandler OnChannelBindSucceed;
        /// <summary>
        /// TODO: Documentation Event
        /// </summary>
        public event ConnectionBindSuccessHandler OnConnectionBindSucceed;
        /// <summary>
        /// TODO: Documentation Event
        /// </summary>
        public event MessageReceptionHandler OnConnectionAttemptReceived;
        /// <summary>
        /// TODO: Documentation Event
        /// </summary>
        public event MessageReceptionHandler OnDataReceived;
        #endregion

        #region PROPERTIES
        /// <summary>
        /// TODO: Documentation Properties
        /// </summary>
        public List<TurnAllocation> Allocations { get; set; }
        /// <summary>
        /// Contains the type of protocol (udp, tcp) used to communicate with the STUN Server
        /// </summary>
        public ProtocolType ProtocolType { get; set; }
        /// <summary>
        /// Contains the IPEndPoint of the STUN Server
        /// </summary>
        public IPEndPoint ServerEP { get; set; }
        /// <summary>
        /// TODO: Documentation Property
        /// </summary>
        public X509Certificate2 SslCertificate { get; set; }
        /// <summary>
        /// TODO: Documentation Property
        /// </summary>
        public RemoteCertificateValidationCallback RemoteCertificateValidationHandler { get; set; }
        /// <summary>
        /// Contains True if the current connected Socket must use TLS over TCP to communicate with the STUN Server
        /// </summary>
        public Boolean UseSsl
        {
            get { return this.SslCertificate != null; }
        }
        /// <summary>
        /// TODO: Documentation Property
        /// </summary>
        public Boolean Connected
        {
            get { return this.StunClient.Connected; }
        }
        #endregion

        #region CONSTRUCTORS & FINALIZERS
        /// <summary>
        /// TODO: Documentation Constructor
        /// </summary>
        /// <param name="turnServerEP"></param>
        /// <param name="protocolType"></param>
        /// <param name="remoteCertificateValidationHandler"></param>
        /// <param name="clientCertificate"></param>
        public TurnManager(IPEndPoint turnServerEP, ProtocolType protocolType, RemoteCertificateValidationCallback remoteCertificateValidationHandler, X509Certificate2 clientCertificate)
        {
            this.ServerEP = turnServerEP;
            this.ProtocolType = protocolType;

            this.RemoteCertificateValidationHandler = remoteCertificateValidationHandler;
            this.SslCertificate = clientCertificate;

            this.PendingTransactions = new Dictionary<byte[], KeyValuePair<StunMessage, Object>>(new ByteArrayComparer());
            this.Allocations = new List<TurnAllocation>();

            this.RefreshTimer = new Timer();
            this.RefreshTimer.Interval = 120000; // 2min
            this.RefreshTimer.AutoReset = true;
            this.RefreshTimer.Elapsed += new ElapsedEventHandler(refreshTimer_Elapsed);

            this.StunClient = new StunClient();
            this.StunClient.OnReceivedMessage += new MessageReceptionHandler(turnClient_OnReceivedMessage);
        }

        /// <summary>
        /// TODO: Documentation turnClient_OnReceivedMessage
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="receivedMsg"></param>
        private void turnClient_OnReceivedMessage(object sender, StunMessage receivedMsg)
        {
            KeyValuePair<StunMessage, Object> transactionObject;

            switch (receivedMsg.MethodClass)
            {
                case StunMethodClass.Error:
                    if (!this.IsPendingTransaction(receivedMsg.TransactionID, out transactionObject))
                        return;

                    switch (receivedMsg.MethodType)
                    {
                        case StunMethodType.Allocate:
                            if (receivedMsg.Stun.ErrorCode.ErrorType == ErrorCodeType.Unauthorized)
                            {
                                this.AllocateRetry(receivedMsg, (transactionObject.Value as String[])[0], (transactionObject.Value as String[])[1]);
                            }
                            break;
                    }
                    break;

                case StunMethodClass.SuccessResponse:
                    if (!this.IsPendingTransaction(receivedMsg.TransactionID, out transactionObject))
                        return;

                    switch (receivedMsg.MethodType)
                    {
                        case StunMethodType.Allocate:
                            TurnAllocation allocation = new TurnAllocation()
                            {
                                Username = transactionObject.Key.Stun.Username.ValueString,
                                Password = transactionObject.Value as String,
                                Realm = transactionObject.Key.Stun.Realm.ValueString,
                                Nonce = transactionObject.Key.Stun.Nonce.ValueString,
                                RelayedMappedAddress = receivedMsg.Turn.XorRelayedAddress,
                                MappedAddress = receivedMsg.Stun.XorMappedAddress,
                                StartTime = DateTime.Now,
                                LifeTime = StunUtilities.ReverseBytes(BitConverter.ToUInt32(receivedMsg.Turn.LifeTime.Value, 0))
                            };
                            this.Allocations.Add(allocation);

                            this.OnAllocateSucceed(this, allocation, transactionObject.Key, receivedMsg);
                            break;

                        case StunMethodType.CreatePermission:
                            TurnPermission permission = new TurnPermission()
                            {
                                PeerAddress = transactionObject.Key.Turn.XorPeerAddress,
                                StartTime = DateTime.Now,
                                LifeTime = 300
                            };
                            (transactionObject.Value as TurnAllocation).Permissions.Add(permission);

                            this.OnCreatePermissionSucceed(this, transactionObject.Value as TurnAllocation, permission, transactionObject.Key, receivedMsg);
                            break;

                        case StunMethodType.ChannelBind:
                            TurnChannel channel = new TurnChannel()
                            {
                                Channel = transactionObject.Key.Turn.ChannelNumber,
                                PeerAddress = transactionObject.Key.Turn.XorPeerAddress,
                                StartTime = DateTime.Now,
                                LifeTime = 600
                            };
                            (transactionObject.Value as TurnAllocation).Channels.Add(channel);

                            this.OnChannelBindSucceed(this, transactionObject.Value as TurnAllocation, channel, transactionObject.Key, receivedMsg);
                            break;

                        case StunMethodType.ConnectionBind:
                            this.OnConnectionBindSucceed(this, (transactionObject.Value as Socket), receivedMsg);
                            break;
                    }
                    break;

                case StunMethodClass.Indication:
                    switch (receivedMsg.MethodType)
                    {
                        case StunMethodType.ConnectionAttempt:
                            this.OnConnectionAttemptReceived(this, receivedMsg);
                            break;

                        case StunMethodType.Data:
                            this.OnDataReceived(this, receivedMsg);
                            break;
                    }
                    break;
            }
        }

        /// <summary>
        /// TODO: Documentation refreshTimer_Elapsed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void refreshTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            foreach (TurnAllocation allocation in this.Allocations)
            {
                this.RefreshAllocation(allocation, allocation.LifeTime);

                foreach (TurnPermission permission in allocation.Permissions)
                    this.CreatePermission(permission.PeerAddress, allocation);

                foreach (TurnChannel channel in allocation.Channels)
                    this.BindChannel(channel.Channel, channel.PeerAddress, allocation);
            }
        }
        #endregion

        #region METHODS
        /// <summary>
        /// TODO: Documentation Close
        /// </summary>
        public void Connect()
        {
            this.StunClient.Connect(this.ServerEP, this.ProtocolType, this.RemoteCertificateValidationHandler, this.SslCertificate);
        }

        /// <summary>
        /// TODO: Documentation Close
        /// </summary>
        public void Close()
        {
            foreach (TurnAllocation allocation in this.Allocations)
            {
                this.RefreshAllocation(allocation, (UInt32)0);
            }

            this.StunClient.Reset();
            this.StunClient = null;
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
        /// TODO: Documentation Allocate
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public void Allocate(String username, String password)
        {
            StunMessage msg = new StunMessage(StunMethodType.Allocate, StunMethodClass.Request, StunUtilities.NewTransactionId);

            this.PendingTransactions.Add(msg.TransactionID, new KeyValuePair<StunMessage, Object>(msg, new String[] { username, password }));

            this.StunClient.BeginSendMessage(msg);
        }

        /// <summary>
        /// TODO: Documentation AllocateRetry
        /// </summary>
        /// <param name="receivedMsg"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        private void AllocateRetry(StunMessage receivedMsg, String username, String password)
        {
            StunMessage msgAllocate = new StunMessage(StunMethodType.Allocate, StunMethodClass.Request, StunUtilities.NewTransactionId);

            msgAllocate.Stun.Username = new UTF8Attribute(StunAttributeType.Username, username);
            msgAllocate.Turn.RequestedTransport = new StunAttribute(StunAttributeType.RequestedTransport, BitConverter.GetBytes(StunMessage.CODE_POINT_TCP));
            msgAllocate.Stun.Realm = receivedMsg.Stun.Realm;
            msgAllocate.Stun.Nonce = receivedMsg.Stun.Nonce;

            msgAllocate.AddMessageIntegrity(password, true);

            this.PendingTransactions.Add(msgAllocate.TransactionID, new KeyValuePair<StunMessage, Object>(msgAllocate, password));

            this.StunClient.BeginSendMessage(msgAllocate);
        }

        /// <summary>
        /// TODO: Documentation RefreshAllocation
        /// </summary>
        /// <param name="allocation"></param>
        /// <param name="lifeTime"></param>
        public void RefreshAllocation(TurnAllocation allocation, UInt32 lifeTime)
        {
            StunMessage msg = new StunMessage(StunMethodType.Refresh, StunMethodClass.Request, StunUtilities.NewTransactionId);

            msg.Stun.Username = new UTF8Attribute(StunAttributeType.Username, allocation.Username);
            msg.Stun.Realm = new UTF8Attribute(StunAttributeType.Realm, allocation.Realm);
            msg.Stun.Nonce = new UTF8Attribute(StunAttributeType.Nonce, allocation.Nonce);
            msg.Turn.LifeTime = new StunAttribute(StunAttributeType.LifeTime, BitConverter.GetBytes(lifeTime));

            msg.AddMessageIntegrity(allocation.Password, true);

            this.StunClient.BeginSendMessage(msg);
        }

        /// <summary>
        /// TODO: Documentation CreatePermission
        /// </summary>
        /// <param name="xorPeerAddress"></param>
        /// <param name="allocation"></param>
        public void CreatePermission(XorMappedAddress xorPeerAddress, TurnAllocation allocation)
        {
            StunMessage msg = new StunMessage(StunMethodType.CreatePermission, StunMethodClass.Request, StunUtilities.NewTransactionId);

            msg.Turn.XorPeerAddress = xorPeerAddress;
            msg.Stun.Username = new UTF8Attribute(StunAttributeType.Username, allocation.Username);
            msg.Stun.Realm = new UTF8Attribute(StunAttributeType.Realm, allocation.Realm);
            msg.Stun.Nonce = new UTF8Attribute(StunAttributeType.Nonce, allocation.Nonce);

            msg.AddMessageIntegrity(allocation.Password, true);

            this.PendingTransactions.Add(msg.TransactionID, new KeyValuePair<StunMessage, Object>(msg, allocation));

            this.StunClient.BeginSendMessage(msg);
        }

        /// <summary>
        /// TODO: Documentation BindChannel
        /// </summary>
        /// <param name="channelNumber"></param>
        /// <param name="xorPeerAddress"></param>
        /// <param name="allocation"></param>
        public void BindChannel(byte[] channelNumber, XorMappedAddress xorPeerAddress, TurnAllocation allocation)
        {
            StunMessage msg = new StunMessage(StunMethodType.ChannelBind, StunMethodClass.Request, StunUtilities.NewTransactionId);

            msg.Turn.ChannelNumber = new StunAttribute(StunAttributeType.ChannelNumber, channelNumber);
            msg.Turn.XorPeerAddress = xorPeerAddress;
            msg.Stun.Username = new UTF8Attribute(StunAttributeType.Username, allocation.Username);
            msg.Stun.Realm = new UTF8Attribute(StunAttributeType.Realm, allocation.Realm);
            msg.Stun.Nonce = new UTF8Attribute(StunAttributeType.Nonce, allocation.Nonce);

            msg.AddMessageIntegrity(allocation.Password, true);

            this.PendingTransactions.Add(msg.TransactionID, new KeyValuePair<StunMessage, Object>(msg, allocation));

            this.StunClient.BeginSendMessage(msg);
        }

        /// <summary>
        /// TODO: Documentation SendIndication
        /// </summary>
        /// <param name="xorPeerAddress"></param>
        /// <param name="data"></param>
        /// <param name="allocation"></param>
        public void SendIndication(XorMappedAddress xorPeerAddress, byte[] data, TurnAllocation allocation)
        {
            StunMessage msg = new StunMessage(StunMethodType.Send, StunMethodClass.Indication, StunUtilities.NewTransactionId);
 
            msg.Turn.XorPeerAddress = xorPeerAddress;
            msg.Turn.Data = new StunAttribute(StunAttributeType.Data, data);
            msg.Stun.Username = new UTF8Attribute(StunAttributeType.Username, allocation.Username);
            msg.Stun.Realm = new UTF8Attribute(StunAttributeType.Realm, allocation.Realm);
            msg.Stun.Nonce = new UTF8Attribute(StunAttributeType.Nonce, allocation.Nonce);

            msg.AddMessageIntegrity(allocation.Password, true);

            this.StunClient.BeginSendMessage(msg);
        }

        /// <summary>
        /// TODO: Documentation ConnectionBind
        /// </summary>
        /// <param name="connectionId"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public void ConnectionBind(StunAttribute connectionId, String username, String password)
        {
            this.TurnTcpManager = new TurnManager(this.ServerEP, this.ProtocolType, this.RemoteCertificateValidationHandler, this.SslCertificate);

            this.TurnTcpManager.OnAllocateSucceed += (object sender, TurnAllocation allocation, StunMessage sentMsg, StunMessage receivedMsg) =>
                {
                    StunMessage msg = new StunMessage(StunMethodType.ConnectionBind, StunMethodClass.Request, StunUtilities.NewTransactionId);

                    msg.Turn.ConnectionId = connectionId;
                    msg.Stun.Username = new UTF8Attribute(StunAttributeType.Username, allocation.Username);
                    msg.Stun.Realm = new UTF8Attribute(StunAttributeType.Realm, allocation.Realm);
                    msg.Stun.Nonce = new UTF8Attribute(StunAttributeType.Nonce, allocation.Nonce);

                    msg.AddMessageIntegrity(allocation.Password, true);

                    this.TurnTcpManager.PendingTransactions.Add(msg.TransactionID, new KeyValuePair<StunMessage, Object>(msg, this.TurnTcpManager.StunClient.Socket));

                    this.TurnTcpManager.StunClient.BeginSendMessage(msg);
                };

            this.TurnTcpManager.OnConnectionBindSucceed += (object sender, Socket connectedSocket, StunMessage receivedMsg) =>
                {
                    this.OnConnectionBindSucceed(sender, connectedSocket, receivedMsg);
                };

            this.TurnTcpManager.Connect();
            this.TurnTcpManager.Allocate(username, password);
        }
        #endregion
    }
}