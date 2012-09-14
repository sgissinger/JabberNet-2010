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
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace Jabber.Stun
{
    /// <summary>
    /// TODO: Documentation Class
    /// </summary>
    public class HolePuncher
    {
        #region MEMBERS
        /// <summary>
        /// TODO: Documentation Member
        /// </summary>
        private SortedDictionary<UInt32, IPEndPoint> peersToPunch = new SortedDictionary<UInt32, IPEndPoint>(new DescendingComparer<UInt32>());
        #endregion

        #region PROPERTIES
        /// <summary>
        /// TODO: Documentation Property
        /// </summary>
        private Thread PunchThread { get; set; }
        /// <summary>
        /// TODO: Documentation Property
        /// </summary>
        private Control Invoker { get; set; }
        /// <summary>
        /// TODO: Documentation Property
        /// </summary>
        private Object Data { get; set; }
        /// <summary>
        /// TODO: Documentation Property
        /// </summary>
        private IPEndPoint HostEP { get; set; }
        /// <summary>
        /// TODO: Documentation Property
        /// </summary>
        public IPEndPoint[] PeersEP
        {
            get
            {
                IPEndPoint[] peers = new IPEndPoint[this.peersToPunch.Values.Count];

                this.peersToPunch.Values.CopyTo(peers, 0); 

                return peers;
            }
        }
        /// <summary>
        /// TODO: Documentation Property
        /// </summary>
        public Boolean CanStart
        {
            get { return this.peersToPunch.Count > 0; }
        }
        #endregion

        #region CONSTRUCTORS & FINALIZERS
        /// <summary>
        /// TODO: Documentation Constructor
        /// </summary>
        /// <param name="hostEP"></param>
        /// <param name="data"></param>
        public HolePuncher(IPEndPoint hostEP, Object data)
            : this(null, hostEP, data)
        { }

        /// <summary>
        /// TODO: Documentation Constructor
        /// </summary>
        /// <param name="invoker"></param>
        /// <param name="hostEP"></param>
        /// <param name="data"></param>
        public HolePuncher(Control invoker, IPEndPoint hostEP, Object data)
        {
            this.Invoker = invoker;
            this.HostEP = hostEP;
            this.Data = data;
        }
        #endregion

        #region METHODS
        /// <summary>
        /// TODO: Documentation AddEP
        /// </summary>
        /// <param name="priority"></param>
        /// <param name="peerEP"></param>
        public void AddEP(UInt32 priority, IPEndPoint peerEP)
        {
            this.peersToPunch.Add(priority, peerEP);
        }

        /// <summary>
        /// TODO: Documentation RemoveEP
        /// </summary>
        /// <param name="peerEP"></param>
        public void RemoveEP(IPEndPoint peerEP)
        {
            IPEndPointComparer comparer = new IPEndPointComparer();

            foreach (var item in this.peersToPunch)
            {
                if (comparer.Equals(item.Value, peerEP))
                {
                    this.peersToPunch.Remove(item.Key);
                }
            }
        }

        /// <summary>
        /// TODO: Documentation ClearEPs
        /// </summary>
        public void ClearEPs()
        {
            this.peersToPunch.Clear();
        }

        /// <summary>
        /// TODO: Documentation StartTcpPunch
        /// </summary>
        /// <param name="successCallback"></param>
        /// <param name="failureCallback"></param>
        public void StartTcpPunch(HolePunchSuccessHandler successCallback, HolePunchFailureHandler failureCallback)
        {
            this.PunchThread = new Thread(() =>
            {
                this.TcpPunchThreadStart(successCallback, failureCallback);
            });
            this.PunchThread.Start();
        }

        /// <summary>
        /// TODO: Documentation TcpPunchThreadStart
        /// </summary>
        /// <param name="successCallback"></param>
        /// <param name="failureCallback"></param>
        private void TcpPunchThreadStart(HolePunchSuccessHandler successCallback, HolePunchFailureHandler failureCallback)
        {
            TcpClient client = null;

            foreach (IPEndPoint peerEP in this.PeersEP)
            {
                Int32 nbTries = 0;
                Boolean quit = false;

                while (true)
                {
                    nbTries++;

                    client = new TcpClient();
                    client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
                    
                    if (this.HostEP != null)
                        client.Client.Bind(this.HostEP);

                    IAsyncResult result = client.Client.BeginConnect(peerEP.Address, peerEP.Port, null, null);

                    result.AsyncWaitHandle.WaitOne(3000, true);

                    if (nbTries == 3 || client.Client.Connected)
                    {
                        quit = true;
                        break;
                    }

                    client.Client.Close();
                }

                if (quit)
                    break;
            }


            if (client != null)
            {
                if (client.Client.Connected)
                {
                    if (successCallback != null)
                    {
                        if (this.Invoker == null)
                            successCallback(this, client.Client, this.Data);
                        else
                            this.Invoker.Invoke(new MethodInvoker(() => { successCallback(this, client.Client, this.Data); }));
                    }
                }
                else
                {
                    client.Client.Close();

                    if (failureCallback != null)
                    {
                        if (this.Invoker == null)
                            failureCallback(this, this.Data);
                        else
                            this.Invoker.Invoke(new MethodInvoker(() => { failureCallback(this, this.Data); }));
                    }
                }
            }
            this.ClearEPs();
        }
        #endregion
    }
}
