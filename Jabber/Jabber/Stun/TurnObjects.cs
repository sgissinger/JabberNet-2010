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
using System.Timers;
using Jabber.Stun.Attributes;

namespace Jabber.Stun
{
    /// <summary>
    /// TODO: Documentation Class
    /// </summary>
    public class TurnAllocation
    {
        #region MEMBERS
        /// <summary>
        /// TODO: Documentation Members
        /// </summary>
        private Timer refreshTimer = null;
        #endregion

        #region PROPERTIES
        /// <summary>
        /// TODO: Documentation Property
        /// </summary>
        public String Username { get; set; }
        /// <summary>
        /// TODO: Documentation Property
        /// </summary>
        public String Password { get; set; }
        /// <summary>
        /// TODO: Documentation Property
        /// </summary>
        public String Realm { get; set; }
        /// <summary>
        /// TODO: Documentation Property
        /// </summary>
        public String Nonce { get; set; }
        /// <summary>
        /// TODO: Documentation Property
        /// </summary>
        public XorMappedAddress MappedAddress { get; set; }
        /// <summary>
        /// TODO: Documentation Property
        /// </summary>
        public XorMappedAddress RelayedMappedAddress { get; set; }
        /// <summary>
        /// TODO: Documentation Property
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// TODO: Documentation Property
        /// </summary>
        public UInt32 LifeTime { get; set; }
        /// <summary>
        /// TODO: Documentation Property
        /// </summary>
        public Dictionary<XorMappedAddress, TurnPermission> Permissions { get; set; }
        /// <summary>
        /// TODO: Documentation Property
        /// </summary>
        public Dictionary<byte[], TurnChannel> Channels { get; set; }
        #endregion

        #region CONSTRUCTORS & FINALIZERS
        /// <summary>
        /// TODO: Documentation Constructor
        /// </summary>
        public TurnAllocation()
        {
            this.Permissions = new Dictionary<XorMappedAddress, TurnPermission>(new XorMappedAddressComparer());
            this.Channels = new Dictionary<byte[], TurnChannel>(new ByteArrayComparer());
        }

        /// <summary>
        /// TODO: Documentation StartAutoRefresh
        /// </summary>
        public void StartAutoRefresh(TurnManager turnManager)
        {
            this.refreshTimer = new Timer();
            this.refreshTimer.AutoReset = true;

            this.refreshTimer.Interval = (this.LifeTime * 1000) - 60000; // lifetime - 1min
            this.refreshTimer.Elapsed += new ElapsedEventHandler((sender, e) => { this.AutoRefresh(turnManager); });
            this.refreshTimer.Start();
        }

        /// <summary>
        /// TODO: Documentation StopAutoRefresh
        /// </summary>
        public void StopAutoRefresh()
        {
            if (this.refreshTimer != null)
                this.refreshTimer.Stop();
        }

        /// <summary>
        /// TODO: Documentation AutoRefresh
        /// </summary>
        /// <param name="turnManager"></param>
        private void AutoRefresh(TurnManager turnManager)
        {
            turnManager.RefreshAllocation(this, this.LifeTime);

            this.refreshTimer.Interval = (this.LifeTime * 1000) - 60000; // lifetime - 1min
        }
        #endregion
    }

    /// <summary>
    /// TODO: Documentation Class
    /// </summary>
    public class TurnPermission
    {
        /// <summary>
        /// TODO: Documentation Property
        /// </summary>
        public XorMappedAddress PeerAddress { get; set; }
        /// <summary>
        /// TODO: Documentation Property
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// TODO: Documentation Property
        /// </summary>
        public UInt32 LifeTime { get; set; }
    }

    /// <summary>
    /// TODO: Documentation Class
    /// </summary>
    public class TurnChannel
    {
        /// <summary>
        /// TODO: Documentation Property
        /// </summary>
        public XorMappedAddress PeerAddress { get; set; }
        /// <summary>
        /// TODO: Documentation Property
        /// </summary>
        public byte[] Channel { get; set; }
        /// <summary>
        /// TODO: Documentation Property
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// TODO: Documentation Property
        /// </summary>
        public UInt32 LifeTime { get; set; }
    }
}
