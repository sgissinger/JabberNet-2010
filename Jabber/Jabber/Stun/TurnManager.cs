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
    /// <summary>
    /// TODO: Documentation Class
    /// </summary>
    public class TurnManager
    {
        #region MEMBERS
        /// <summary>
        /// TODO: Documentation Member
        /// </summary>
        private StunClient turnClient = null;
        /// <summary>
        /// TODO: Documentation Properties
        /// </summary>
        private List<TurnAllocation> allocations = null;
        /// <summary>
        /// TODO: Documentation Properties
        /// </summary>
        private List<TurnPermission> permissions = null;
        #endregion

        #region PROPERTIES
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
        public TurnAllocation[] Allocations 
        {
            get
            {
                TurnAllocation[] allocs = new TurnAllocation[this.allocations.Count];

                this.allocations.CopyTo(allocs, 0);

                return allocs;
            } 
        }
        /// <summary>
        /// TODO: Documentation Property
        /// </summary>
        public TurnPermission[] Permissions
        {
            get
            {
                TurnPermission[] perms = new TurnPermission[this.permissions.Count];

                this.permissions.CopyTo(perms, 0);

                return perms;
            } 
        }
        #endregion

        #region CONSTRUCTORS & FINALIZERS
        /// <summary>
        /// TODO: Documentation Constructor
        /// </summary>
        /// <param name="turnServerIp"></param>
        /// <param name="protocolType"></param>
        public TurnManager(String turnServerIp, ProtocolType protocolType)
            : this(turnServerIp, StunClient.DEFAULT_STUN_PORT, protocolType)
        { }

        /// <summary>
        /// TODO: Documentation Constructor
        /// </summary>
        /// <param name="turnServerIp"></param>
        /// <param name="turnServerPort"></param>
        /// <param name="protocolType"></param>
        public TurnManager(String turnServerIp, Int32 turnServerPort, ProtocolType protocolType)
            : this(new IPEndPoint(IPAddress.Parse(turnServerIp), turnServerPort), protocolType, null, null)
        { }

        /// <summary>
        /// TODO: Documentation Constructor
        /// </summary>
        /// <param name="turnServerIp"></param>
        /// <param name="remoteCertificateValidationHandler"></param>
        /// <param name="clientCertificate"></param>
        public TurnManager(String turnServerIp, RemoteCertificateValidationCallback remoteCertificateValidationHandler, X509Certificate2 clientCertificate)
            : this(turnServerIp, StunClient.DEFAULT_STUNS_PORT, remoteCertificateValidationHandler, clientCertificate)
        { }

        /// <summary>
        /// TODO: Documentation Constructor
        /// </summary>
        /// <param name="turnServerIp"></param>
        /// <param name="turnServerPort"></param>
        /// <param name="remoteCertificateValidationHandler"></param>
        /// <param name="clientCertificate"></param>
        public TurnManager(String turnServerIp, Int32 turnServerPort, RemoteCertificateValidationCallback remoteCertificateValidationHandler, X509Certificate2 clientCertificate)
            : this(new IPEndPoint(IPAddress.Parse(turnServerIp), turnServerPort), ProtocolType.Tcp, remoteCertificateValidationHandler, clientCertificate)
        { }

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

            this.allocations = new List<TurnAllocation>();
            this.permissions = new List<TurnPermission>();
        }
        #endregion

        #region METHODS
        /// <summary>
        /// TODO: Documentation Close
        /// </summary>
        public void Connect()
        {
            if (this.turnClient == null)
                this.turnClient = new StunClient();

            this.turnClient.Connect(this.ServerEP, this.ProtocolType, this.RemoteCertificateValidationHandler, this.SslCertificate);
        }

        /// <summary>
        /// TODO: Documentation Close
        /// </summary>
        public void Close()
        {
            foreach (TurnAllocation allocation in this.allocations)
            {
                StunMessage msgEndAllocation = new StunMessage(StunMethodType.Refresh, StunMethodClass.Request, StunUtilities.NewTransactionId);

                msgEndAllocation.Stun.Username = new UTF8Attribute(StunAttributeType.Username, allocation.Username);
                msgEndAllocation.Stun.Realm = new UTF8Attribute(StunAttributeType.Realm, allocation.Realm);
                msgEndAllocation.Stun.Nonce = new UTF8Attribute(StunAttributeType.Nonce, allocation.Nonce);
                msgEndAllocation.Turn.LifeTime = new StunAttribute(StunAttributeType.LifeTime, BitConverter.GetBytes((UInt32)0));

                msgEndAllocation.AddMessageIntegrity(allocation.Password, true);

                this.turnClient.SendMessage(msgEndAllocation);
            }

            this.turnClient.Reset();
            this.turnClient = null;
        }

        /// <summary>
        /// TODO: Documentation RequestAllocation
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public TurnAllocation RequestAllocation(String username, String password)
        {
            TurnAllocation result = null;

            StunMessage msgAllocate1 = new StunMessage(StunMethodType.Allocate, StunMethodClass.Request, StunUtilities.NewTransactionId);
            StunMessage respAllocate1 = this.turnClient.SendMessage(msgAllocate1);

            if (respAllocate1.Stun.ErrorCode != null && respAllocate1.Stun.Realm != null && respAllocate1.Stun.Nonce != null &&
                respAllocate1.Stun.ErrorCode.ErrorType == ErrorCodeType.Unauthorized)
            {
                StunMessage msgAllocate2 = new StunMessage(StunMethodType.Allocate, StunMethodClass.Request, StunUtilities.NewTransactionId);

                msgAllocate2.Stun.Username = new UTF8Attribute(StunAttributeType.Username, username);
                msgAllocate2.Turn.RequestedTransport = new StunAttribute(StunAttributeType.RequestedTransport, BitConverter.GetBytes(StunMessage.CODE_POINT_TCP));
                msgAllocate2.Stun.Realm = respAllocate1.Stun.Realm;
                msgAllocate2.Stun.Nonce = respAllocate1.Stun.Nonce;

                msgAllocate2.AddMessageIntegrity(password, true);

                StunMessage respAllocate2 = this.turnClient.SendMessage(msgAllocate2);

                if (respAllocate2.MethodType == StunMethodType.Allocate &&
                    respAllocate2.MethodClass == StunMethodClass.SuccessResponse)
                {
                    result = new TurnAllocation()
                    {
                        Username = username,
                        Password = password,
                        Realm = respAllocate1.Stun.Realm.ValueString,
                        Nonce = respAllocate1.Stun.Nonce.ValueString,
                        RelayedMappedAddress = respAllocate2.Turn.XorRelayedAddress,
                        MappedAddress = respAllocate2.Stun.XorMappedAddress,
                        StartTime = DateTime.Now,
                        LifeTime = StunUtilities.ReverseBytes(BitConverter.ToUInt32(respAllocate2.Turn.LifeTime.Value, 0))
                    };
                    this.allocations.Add(result);
                }
            }
            return result;
        }

        /// <summary>
        /// TODO: Documentation CreatePermission
        /// </summary>
        /// <param name="xorPeerAddress"></param>
        /// <param name="allocation"></param>
        /// <returns></returns>
        public TurnPermission CreatePermission(XorMappedAddress xorPeerAddress, TurnAllocation allocation)
        {
            TurnPermission result = null;

            StunMessage msgPermission = new StunMessage(StunMethodType.CreatePermission, StunMethodClass.Request, StunUtilities.NewTransactionId);

            msgPermission.Turn.XorPeerAddress = xorPeerAddress;
            msgPermission.Stun.Username = new UTF8Attribute(StunAttributeType.Username, allocation.Username);
            msgPermission.Stun.Realm = new UTF8Attribute(StunAttributeType.Realm, allocation.Realm);
            msgPermission.Stun.Nonce = new UTF8Attribute(StunAttributeType.Nonce, allocation.Nonce);

            msgPermission.AddMessageIntegrity(allocation.Password, true);

            StunMessage respPermission = this.turnClient.SendMessage(msgPermission);

            if (respPermission.MethodType == StunMethodType.CreatePermission &&
                respPermission.MethodClass == StunMethodClass.SuccessResponse)
            {
                result = new TurnPermission()
                {
                    StartTime = DateTime.Now,
                    LifeTime = 300,
                    PeerAddress = xorPeerAddress
                };
                this.permissions.Add(result);
            }

            return result;
        }
        #endregion
    }

    /// <summary>
    /// TODO: Documentation Class
    /// </summary>
    public class TurnAllocation
    {
        /// <summary>
        /// TODO: Documentation Class
        /// </summary>
        public String Username { get; set; }
        /// <summary>
        /// TODO: Documentation Class
        /// </summary>
        public String Password { get; set; }
        /// <summary>
        /// TODO: Documentation Class
        /// </summary>
        public String Realm { get; set; }
        /// <summary>
        /// TODO: Documentation Class
        /// </summary>
        public String Nonce { get; set; }
        /// <summary>
        /// TODO: Documentation Class
        /// </summary>
        public XorMappedAddress MappedAddress { get; set; }
        /// <summary>
        /// TODO: Documentation Class
        /// </summary>
        public XorMappedAddress RelayedMappedAddress { get; set; }
        /// <summary>
        /// TODO: Documentation Class
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// TODO: Documentation Class
        /// </summary>
        public UInt32 LifeTime { get; set; }
    }

    /// <summary>
    /// TODO: Documentation Class
    /// </summary>
    public class TurnPermission
    {
        /// <summary>
        /// TODO: Documentation Class
        /// </summary>
        public XorMappedAddress PeerAddress { get; set; }
        /// <summary>
        /// TODO: Documentation Class
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// TODO: Documentation Class
        /// </summary>
        public UInt32 LifeTime { get; set; }
    }
}