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
using System.Net;
using System.Net.Sockets;
using System.Xml;
using Jabber.Protocol;
using Jabber.Protocol.IQ;

namespace Jabber.Stun
{
    /// Used in JingleManager class
    public delegate void JingleSessionIdHandler(object sender, String sid);

    /// Used in JingleIceManager class
    public delegate void JingleDescriptionGatheringHandler(object sender, XmlDocument ownerDoc, String sid, ref Element jingleDescription, ref String contentName);

    public delegate void JingleIceCandidatesGatheringHandler(object sender, JingleIce jingleIce, IPEndPoint hostEP, TurnAllocation allocation);

    public delegate void JingleHolePunchSucceedHandler(object sender, Socket connectedSocket, Jingle jingle);

    public delegate void JingleTurnStartHandler(object sender, IPEndPoint peerEP, String sid);

    public delegate void JingleTurnConnectionBindSucceedHandler(object sender, Socket connectedSocket, String sid, JID recipient);

    /// Used in HolePuncher class
    public delegate void HolePunchSuccessHandler(object sender, Socket connectedSocket, object punchData);

    public delegate void HolePunchFailureHandler(object sender, object punchData);

    /// Used in StunClient class
    public delegate void StunMessageReceptionHandler(object sender, StunMessage receivedMsg, StunMessage sentMsg, object transactionObject);

    public delegate void StunIndicationReceptionHandler(object sender, StunMessage receivedMsg);

    /// Used in TurnManager class
    public delegate void TurnAllocateSuccessHandler(object sender, TurnAllocation allocation, StunMessage sentMsg, StunMessage receivedMsg);

    public delegate void TurnCreatePermissionSuccessHandler(object sender, TurnAllocation allocation, TurnPermission permission, StunMessage sentMsg, StunMessage receivedMsg);

    public delegate void TurnChannelBindSuccessHandler(object sender, TurnAllocation allocation, TurnChannel channel, StunMessage sentMsg, StunMessage receivedMsg);

    public delegate void TurnConnectionBindSuccessHandler(object sender, Socket connectedSocket, StunMessage receivedMsg);
}
