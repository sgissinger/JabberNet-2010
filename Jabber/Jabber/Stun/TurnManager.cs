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
using Jabber.Stun.Attributes;

namespace Jabber.Stun
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
        #region EVENTS
        /// <summary>
        /// TODO: Documentation Event
        /// </summary>
        public event AllocateSuccessHandler OnAllocateSucceed;
        /// <summary>
        /// TODO: Documentation Event
        /// </summary>
        public event MessageReceptionHandler OnAllocateFailed;
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
        public event IndicationReceptionHandler OnConnectionAttemptReceived;
        /// <summary>
        /// TODO: Documentation Event
        /// </summary>
        public event IndicationReceptionHandler OnDataReceived;
        #endregion

        #region PROPERTIES
        /// <summary>
        /// TODO: Documentation Property
        /// </summary>
        public Dictionary<XorMappedAddress, TurnAllocation> Allocations { get; set; }
        /// <summary>
        /// TODO: Documentation Property
        /// </summary>
        public Boolean Connected
        {
            get { return this.StunClient.Connected; }
        }
        /// <summary>
        /// TODO: Documentation Propery
        /// </summary>
        public IPEndPoint HostEP
        {
            get { return this.StunClient.HostEP; }
        }
        /// <summary>
        /// TODO: Documentation Propery
        /// </summary>
        private StunClient StunClient { get; set; }
        /// <summary>
        /// TODO: Documentation Property
        /// </summary>
        private TurnManager TurnTcpManager { get; set; }
        #endregion

        #region CONSTRUCTORS & FINALIZERS
        /// <summary>
        /// TODO: Documentation Constructor
        /// </summary>
        /// <param name="turnServerEP"></param>
        /// <param name="protocolType"></param>
        /// <param name="clientCertificate"></param>
        /// <param name="remoteCertificateValidationHandler"></param>
        public TurnManager(IPEndPoint turnServerEP, ProtocolType protocolType, X509Certificate2 clientCertificate, RemoteCertificateValidationCallback remoteCertificateValidationHandler)
        {
            this.Allocations = new Dictionary<XorMappedAddress, TurnAllocation>(new XorMappedAddressComparer());

            this.StunClient = new StunClient(turnServerEP, protocolType,
                                             clientCertificate, remoteCertificateValidationHandler);

            this.StunClient.OnReceivedError += new MessageReceptionHandler(StunClient_OnReceivedError);
            this.StunClient.OnReceivedIndication += new IndicationReceptionHandler(StunClient_OnReceivedIndication);
            this.StunClient.OnReceivedSuccessResponse += new MessageReceptionHandler(StunClient_OnReceivedSuccessResponse);
        }

        /// <summary>
        /// TODO: Documentation StunClient_OnReceivedSuccessResponse
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="receivedMsg"></param>
        /// <param name="sentMsg"></param>
        /// <param name="transactionObject"></param>
        private void StunClient_OnReceivedSuccessResponse(object sender, StunMessage receivedMsg, StunMessage sentMsg, object transactionObject)
        {
            switch (receivedMsg.MethodType)
            {
                case StunMethodType.Allocate:
                    TurnAllocation allocation = new TurnAllocation()
                    {
                        Username = sentMsg.Stun.Username.ValueString,
                        Password = transactionObject as String,
                        Realm = sentMsg.Stun.Realm.ValueString,
                        Nonce = sentMsg.Stun.Nonce.ValueString,
                        RelayedMappedAddress = receivedMsg.Turn.XorRelayedAddress,
                        MappedAddress = receivedMsg.Stun.XorMappedAddress,
                        StartTime = DateTime.Now,
                        LifeTime = StunUtilities.ReverseBytes(BitConverter.ToUInt32(receivedMsg.Turn.LifeTime.Value, 0))
                    };

                    if (this.Allocations.ContainsKey(receivedMsg.Turn.XorRelayedAddress))
                        this.Allocations[receivedMsg.Turn.XorRelayedAddress] = allocation;
                    else
                    {
                        this.Allocations.Add(receivedMsg.Turn.XorRelayedAddress, allocation);

                        if (this.OnAllocateSucceed != null)
                            this.OnAllocateSucceed(this, allocation, sentMsg, receivedMsg);
                    }
                    break;

                case StunMethodType.CreatePermission:
                    TurnPermission permission = new TurnPermission()
                    {
                        PeerAddress = sentMsg.Turn.XorPeerAddress,
                        StartTime = DateTime.Now,
                        LifeTime = 300
                    };
                    TurnAllocation permAllocation = transactionObject as TurnAllocation;

                    if (permAllocation.Permissions.ContainsKey(sentMsg.Turn.XorPeerAddress))
                        permAllocation.Permissions[sentMsg.Turn.XorPeerAddress] = permission;
                    else
                    {
                        permAllocation.Permissions.Add(sentMsg.Turn.XorPeerAddress, permission);

                        if (this.OnCreatePermissionSucceed != null)
                            this.OnCreatePermissionSucceed(this, permAllocation, permission, sentMsg, receivedMsg);
                    }

                    break;

                case StunMethodType.ChannelBind:
                    TurnChannel channel = new TurnChannel()
                    {
                        Channel = sentMsg.Turn.ChannelNumber,
                        PeerAddress = sentMsg.Turn.XorPeerAddress,
                        StartTime = DateTime.Now,
                        LifeTime = 600
                    };
                    TurnAllocation channelAllocation = transactionObject as TurnAllocation;

                    if (channelAllocation.Channels.ContainsKey(sentMsg.Turn.ChannelNumber))
                        channelAllocation.Channels[sentMsg.Turn.ChannelNumber] = channel;
                    else
                    {
                        channelAllocation.Channels.Add(sentMsg.Turn.ChannelNumber, channel);

                        if (this.OnChannelBindSucceed != null)
                            this.OnChannelBindSucceed(this, transactionObject as TurnAllocation, channel, sentMsg, receivedMsg);
                    }
                    break;

                case StunMethodType.ConnectionBind:
                    if (this.OnConnectionBindSucceed != null)
                        this.OnConnectionBindSucceed(this, transactionObject as Socket, receivedMsg);
                    break;
            }
        }

        /// <summary>
        /// TODO: Documentation StunClient_OnReceivedIndication
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="receivedMsg"></param>
        private void StunClient_OnReceivedIndication(object sender, StunMessage receivedMsg)
        {
            switch (receivedMsg.MethodType)
            {
                case StunMethodType.ConnectionAttempt:
                    if (this.OnConnectionAttemptReceived != null)
                        this.OnConnectionAttemptReceived(this, receivedMsg);
                    break;

                case StunMethodType.Data:
                    if (this.OnDataReceived != null)
                        this.OnDataReceived(this, receivedMsg);
                    break;
            }
        }

        /// <summary>
        /// TODO: Documentation StunClient_OnReceivedError
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="receivedMsg"></param>
        /// <param name="sentMsg"></param>
        /// <param name="transactionObject"></param>
        private void StunClient_OnReceivedError(object sender, StunMessage receivedMsg, StunMessage sentMsg, object transactionObject)
        {
            switch (receivedMsg.MethodType)
            {
                case StunMethodType.Allocate:
                    if (receivedMsg.Stun.ErrorCode.ErrorType == ErrorCodeType.Unauthorized)
                    {
                        String[] credentials = transactionObject as String[];

                        if (credentials != null)
                            this.AllocateRetry(receivedMsg, credentials[0], credentials[1]);
                        else
                            if (this.OnAllocateFailed != null)
                                this.OnAllocateFailed(this, receivedMsg, sentMsg, transactionObject);
                    }
                    break;
            }
        }
        #endregion

        #region METHODS
        /// <summary>
        /// TODO: Documentation Close
        /// </summary>
        public void Connect()
        {
            this.StunClient.Connect();
        }

        /// <summary>
        /// TODO: Documentation Disconnect
        /// </summary>
        public void Disconnect()
        {
            foreach (var allocation in this.Allocations)
            {
                this.RefreshAllocation(allocation.Value, (UInt32)0);
            }

            this.StunClient.Disconnect();
            this.StunClient = null;
        }

        /// <summary>
        /// TODO: Documentation Allocate
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public void Allocate(String username, String password)
        {
            StunMessage msg = new StunMessage(StunMethodType.Allocate, StunMethodClass.Request, StunUtilities.NewTransactionId);

            this.StunClient.BeginSendMessage(msg, new String[] { username, password });
        }

        /// <summary>
        /// TODO: Documentation AllocateRetry
        /// </summary>
        /// <param name="receivedMsg"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        private void AllocateRetry(StunMessage receivedMsg, String username, String password)
        {
            StunMessage msg = new StunMessage(StunMethodType.Allocate, StunMethodClass.Request, StunUtilities.NewTransactionId);

            msg.Turn.RequestedTransport = new StunAttribute(StunAttributeType.RequestedTransport, BitConverter.GetBytes(StunMessage.CODE_POINT_TCP));
            msg.Stun.Username = new UTF8Attribute(StunAttributeType.Username, username);
            msg.Stun.Realm = receivedMsg.Stun.Realm;
            msg.Stun.Nonce = receivedMsg.Stun.Nonce;

            msg.AddMessageIntegrity(password, true);

            this.StunClient.BeginSendMessage(msg, password);
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
            msg.Turn.LifeTime = new StunAttribute(StunAttributeType.LifeTime, BitConverter.GetBytes(StunUtilities.ReverseBytes(lifeTime)));

            msg.AddMessageIntegrity(allocation.Password, true);

            this.StunClient.BeginSendMessage(msg, null);
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

            this.StunClient.BeginSendMessage(msg, allocation);
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

            this.StunClient.BeginSendMessage(msg, allocation);
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

            this.StunClient.BeginSendMessage(msg, null);
        }

        /// <summary>
        /// TODO: Documentation ConnectionBind
        /// </summary>
        /// <param name="connectionId"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public void ConnectionBind(StunAttribute connectionId, String username, String password)
        {
            this.TurnTcpManager = new TurnManager(this.StunClient.ServerEP,
                                                  this.StunClient.ProtocolType,
                                                  this.StunClient.ClientCertificate,
                                                  this.StunClient.RemoteCertificateValidationHandler);

            this.TurnTcpManager.OnAllocateSucceed += (object sender, TurnAllocation allocation, StunMessage sentMsg, StunMessage receivedMsg) =>
                {
                    StunMessage msg = new StunMessage(StunMethodType.ConnectionBind, StunMethodClass.Request, StunUtilities.NewTransactionId);

                    msg.Turn.ConnectionId = connectionId;
                    msg.Stun.Username = new UTF8Attribute(StunAttributeType.Username, allocation.Username);
                    msg.Stun.Realm = new UTF8Attribute(StunAttributeType.Realm, allocation.Realm);
                    msg.Stun.Nonce = new UTF8Attribute(StunAttributeType.Nonce, allocation.Nonce);

                    msg.AddMessageIntegrity(allocation.Password, true);

                    this.TurnTcpManager.StunClient.BeginSendMessage(msg, this.TurnTcpManager.StunClient.Socket);
                };

            this.TurnTcpManager.OnConnectionBindSucceed += (object sender, Socket connectedSocket, StunMessage receivedMsg) =>
                {
                    this.TurnTcpManager.Allocations.Clear();
                    this.TurnTcpManager.StunClient.Cancel = true;

                    if (this.OnConnectionBindSucceed != null)
                        this.OnConnectionBindSucceed(sender, connectedSocket, receivedMsg);
                };

            this.TurnTcpManager.Connect();
            this.TurnTcpManager.Allocate(username, password);
        }
        #endregion
    }
}