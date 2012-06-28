/* --------------------------------------------------------------------------
 * Copyrights
 *
 * Portions created by or assigned to Cursive Systems, Inc. are
 * Copyright (c) 2002-2008 Cursive Systems, Inc.  All Rights Reserved.  Contact
 * information for Cursive Systems, Inc. is available at
 * http://www.cursive.net/.
 *
 * License
 *
 * Jabber-Net is licensed under the LGPL.
 * See LICENSE.txt for details.
 * --------------------------------------------------------------------------*/



namespace Jabber.Protocol.IQ
{
    /// <summary>
    /// ElementFactory for all currently supported IQ namespaces.
    /// </summary>
    public class Factory : IPacketTypes
    {
        private static QnameType[] s_qnt = new QnameType[]
        {
            new QnameType("query", URI.AUTH,     typeof(Jabber.Protocol.IQ.Auth)),
            new QnameType("query", URI.REGISTER, typeof(Jabber.Protocol.IQ.Register)),
            new QnameType("query", URI.ROSTER,   typeof(Jabber.Protocol.IQ.Roster)),
            new QnameType("item",  URI.ROSTER,   typeof(Jabber.Protocol.IQ.Item)),
            new QnameType("group", URI.ROSTER,   typeof(Jabber.Protocol.IQ.Group)),
            new QnameType("query", URI.AGENTS,   typeof(Jabber.Protocol.IQ.AgentsQuery)),
            new QnameType("agent", URI.AGENTS,   typeof(Jabber.Protocol.IQ.Agent)),
            new QnameType("query", URI.OOB,      typeof(Jabber.Protocol.IQ.OOB)),
            new QnameType("query", URI.TIME,     typeof(Jabber.Protocol.IQ.Time)),
            new QnameType("query", URI.VERSION,  typeof(Jabber.Protocol.IQ.Version)),
            new QnameType("query", URI.LAST,     typeof(Jabber.Protocol.IQ.Last)),
            new QnameType("item",  URI.BROWSE,   typeof(Jabber.Protocol.IQ.Browse)),
            new QnameType("ping",  URI.PING,     typeof(Jabber.Protocol.IQ.Ping)),
            new QnameType("geoloc",URI.GEOLOC,   typeof(Jabber.Protocol.IQ.GeoLoc)),

            new QnameType("query",      URI.PRIVATE,   typeof(Jabber.Protocol.IQ.Private)),
            new QnameType("storage",    URI.BOOKMARKS, typeof(Jabber.Protocol.IQ.Bookmarks)),
            new QnameType("url",        URI.BOOKMARKS, typeof(Jabber.Protocol.IQ.BookmarkURL)),
            new QnameType("conference", URI.BOOKMARKS, typeof(Jabber.Protocol.IQ.BookmarkConference)),
            new QnameType("note",       URI.BOOKMARKS, typeof(Jabber.Protocol.IQ.BookmarkNote)),

            // Jingle
            new QnameType("jingle",  URI.JINGLE, typeof(Jabber.Protocol.IQ.Jingle)),
            new QnameType("reason",  URI.JINGLE, typeof(Jabber.Protocol.IQ.JingleReason)),
            new QnameType("content", URI.JINGLE, typeof(Jabber.Protocol.IQ.JingleContent)),

            // Jingle Reasons
            new QnameType("alternative-session",      URI.JINGLE, typeof(Jabber.Protocol.IQ.AlternativeSession)),
            new QnameType("busy",                     URI.JINGLE, typeof(Jabber.Protocol.IQ.Busy)),
            new QnameType("cancel",                   URI.JINGLE, typeof(Jabber.Protocol.IQ.Cancel)),
            new QnameType("connectivity-error",       URI.JINGLE, typeof(Jabber.Protocol.IQ.ConnectivityError)),
            new QnameType("decline",                  URI.JINGLE, typeof(Jabber.Protocol.IQ.JingleDecline)),
            new QnameType("expired",                  URI.JINGLE, typeof(Jabber.Protocol.IQ.Expired)),
            new QnameType("failed-application",       URI.JINGLE, typeof(Jabber.Protocol.IQ.FailedApplication)),
            new QnameType("failed-transport",         URI.JINGLE, typeof(Jabber.Protocol.IQ.FailedTransport)),
            new QnameType("general-error",            URI.JINGLE, typeof(Jabber.Protocol.IQ.GeneralError)),
            new QnameType("gone",                     URI.JINGLE, typeof(Jabber.Protocol.IQ.Gone)),
            new QnameType("incompatible-parameters",  URI.JINGLE, typeof(Jabber.Protocol.IQ.IncompatibleParameters)),
            new QnameType("media-error",              URI.JINGLE, typeof(Jabber.Protocol.IQ.MediaError)),
            new QnameType("security-error",           URI.JINGLE, typeof(Jabber.Protocol.IQ.SecurityError)),
            new QnameType("success",                  URI.JINGLE, typeof(Jabber.Protocol.IQ.Success)),
            new QnameType("timeout",                  URI.JINGLE, typeof(Jabber.Protocol.IQ.Timeout)),
            new QnameType("unsupported-applications", URI.JINGLE, typeof(Jabber.Protocol.IQ.UnsupportedApplications)),
            new QnameType("unsupported-transports",   URI.JINGLE, typeof(Jabber.Protocol.IQ.UnsupportedTransports)),

            // Jingle Errors
            new QnameType("out-of-order",      URI.JINGLE_ERRORS, typeof(Jabber.Protocol.IQ.OutOfOrder)),
            new QnameType("tie-break",         URI.JINGLE_ERRORS, typeof(Jabber.Protocol.IQ.TieBreak)),
            new QnameType("unknown-session",   URI.JINGLE_ERRORS, typeof(Jabber.Protocol.IQ.UnknownSession)),
            new QnameType("unsupported-info",  URI.JINGLE_ERRORS, typeof(Jabber.Protocol.IQ.UnsupportedInfo)),
            new QnameType("security-required", URI.JINGLE_ERRORS, typeof(Jabber.Protocol.IQ.SecurityRequired)),

            // Jingle ICE
            new QnameType("transport",        URI.JINGLE_ICE, typeof(Jabber.Protocol.IQ.JingleIce)),
            new QnameType("candidate",        URI.JINGLE_ICE, typeof(Jabber.Protocol.IQ.JingleIceCandidate)),
            new QnameType("remote-candidate", URI.JINGLE_ICE, typeof(Jabber.Protocol.IQ.JingleIceRemoteCandidate)),

            // VCard
            new QnameType("vCard", URI.VCARD, typeof(Jabber.Protocol.IQ.VCard)),
            new QnameType("N",     URI.VCARD, typeof(Jabber.Protocol.IQ.VCard.VName)),
            new QnameType("ORG",   URI.VCARD, typeof(Jabber.Protocol.IQ.VCard.VOrganization)),
            new QnameType("TEL",   URI.VCARD, typeof(Jabber.Protocol.IQ.VCard.VTelephone)),
            new QnameType("EMAIL", URI.VCARD, typeof(Jabber.Protocol.IQ.VCard.VEmail)),
            new QnameType("GEO",   URI.VCARD, typeof(Jabber.Protocol.IQ.VCard.VGeo)),
            new QnameType("PHOTO", URI.VCARD, typeof(Jabber.Protocol.IQ.VCard.VPhoto)),
            new QnameType("ADR",   URI.VCARD, typeof(Jabber.Protocol.IQ.VCard.VAddress)),

            // Disco
            new QnameType("query",    URI.DISCO_ITEMS, typeof(Jabber.Protocol.IQ.DiscoItems)),
            new QnameType("item",     URI.DISCO_ITEMS, typeof(Jabber.Protocol.IQ.DiscoItem)),
            new QnameType("query",    URI.DISCO_INFO, typeof(Jabber.Protocol.IQ.DiscoInfo)),
            new QnameType("identity", URI.DISCO_INFO, typeof(Jabber.Protocol.IQ.DiscoIdentity)),
            new QnameType("feature",  URI.DISCO_INFO, typeof(Jabber.Protocol.IQ.DiscoFeature)),

            // PubSub
            new QnameType("pubsub",        URI.PUBSUB, typeof(Jabber.Protocol.IQ.PubSub)),
            new QnameType("affiliations",  URI.PUBSUB, typeof(Jabber.Protocol.IQ.Affiliations)),
            new QnameType("create",        URI.PUBSUB, typeof(Jabber.Protocol.IQ.Create)),
            new QnameType("items",         URI.PUBSUB, typeof(Jabber.Protocol.IQ.Items)),
            new QnameType("publish",       URI.PUBSUB, typeof(Jabber.Protocol.IQ.Publish)),
            new QnameType("retract",       URI.PUBSUB, typeof(Jabber.Protocol.IQ.Retract)),
            new QnameType("subscribe",     URI.PUBSUB, typeof(Jabber.Protocol.IQ.Subscribe)),
            new QnameType("subscriptions", URI.PUBSUB, typeof(Jabber.Protocol.IQ.Subscriptions)),
            new QnameType("unsubscribe",   URI.PUBSUB, typeof(Jabber.Protocol.IQ.Unsubscribe)),

            new QnameType("configure",     URI.PUBSUB, typeof(Jabber.Protocol.IQ.Configure)),
            new QnameType("options",       URI.PUBSUB, typeof(Jabber.Protocol.IQ.PubSubOptions)),
            new QnameType("affiliation",   URI.PUBSUB, typeof(Jabber.Protocol.IQ.Affiliation)),
            new QnameType("item",          URI.PUBSUB, typeof(Jabber.Protocol.IQ.PubSubItem)),
            new QnameType("subscription",  URI.PUBSUB, typeof(Jabber.Protocol.IQ.PubSubSubscription)),

            // Pubsub event notifications
            new QnameType("event",         URI.PUBSUB_EVENT, typeof(Jabber.Protocol.IQ.PubSubEvent)),
            new QnameType("associate",     URI.PUBSUB_EVENT, typeof(Jabber.Protocol.IQ.EventAssociate)),
            new QnameType("collection",    URI.PUBSUB_EVENT, typeof(Jabber.Protocol.IQ.EventCollection)),
            new QnameType("configuration", URI.PUBSUB_EVENT, typeof(Jabber.Protocol.IQ.EventConfiguration)),
            new QnameType("disassociate",  URI.PUBSUB_EVENT, typeof(Jabber.Protocol.IQ.EventDisassociate)),
            new QnameType("items",         URI.PUBSUB_EVENT, typeof(Jabber.Protocol.IQ.EventItems)),
            new QnameType("item",          URI.PUBSUB_EVENT, typeof(Jabber.Protocol.IQ.PubSubItem)),
            new QnameType("purge",         URI.PUBSUB_EVENT, typeof(Jabber.Protocol.IQ.EventPurge)),
            new QnameType("retract",       URI.PUBSUB_EVENT, typeof(Jabber.Protocol.IQ.EventRetract)),
            new QnameType("subscription",  URI.PUBSUB_EVENT, typeof(Jabber.Protocol.IQ.EventSubscription)),

            // Pubsub owner use cases
            new QnameType("pubsub",        URI.PUBSUB_OWNER, typeof(Jabber.Protocol.IQ.PubSubOwner)),
            new QnameType("affiliations",  URI.PUBSUB_OWNER, typeof(Jabber.Protocol.IQ.OwnerAffiliations)),
            new QnameType("affiliation",   URI.PUBSUB_OWNER, typeof(Jabber.Protocol.IQ.OwnerAffiliation)),
            new QnameType("configure",     URI.PUBSUB_OWNER, typeof(Jabber.Protocol.IQ.OwnerConfigure)),
            new QnameType("default",       URI.PUBSUB_OWNER, typeof(Jabber.Protocol.IQ.OwnerDefault)),
            new QnameType("delete",        URI.PUBSUB_OWNER, typeof(Jabber.Protocol.IQ.OwnerDelete)),
            new QnameType("purge",         URI.PUBSUB_OWNER, typeof(Jabber.Protocol.IQ.OwnerPurge)),
            new QnameType("subscriptions", URI.PUBSUB_OWNER, typeof(Jabber.Protocol.IQ.OwnerSubscriptions)),
            new QnameType("subscription",  URI.PUBSUB_OWNER, typeof(Jabber.Protocol.IQ.PubSubSubscription)),

            // Pubsub errors
            new QnameType("closed-node",                    URI.PUBSUB_ERRORS, typeof(Jabber.Protocol.IQ.ClosedNode)),
            new QnameType("configuration-required",         URI.PUBSUB_ERRORS, typeof(Jabber.Protocol.IQ.ConfigurationRequired)),
            new QnameType("invalid-jid",                    URI.PUBSUB_ERRORS, typeof(Jabber.Protocol.IQ.InvalidJID)),
            new QnameType("invalid-options",                URI.PUBSUB_ERRORS, typeof(Jabber.Protocol.IQ.InvalidOptions)),
            new QnameType("invalid-payload",                URI.PUBSUB_ERRORS, typeof(Jabber.Protocol.IQ.InvalidPayload)),
            new QnameType("invalid-subid",                  URI.PUBSUB_ERRORS, typeof(Jabber.Protocol.IQ.InvalidSubid)),
            new QnameType("item-forbidden",                 URI.PUBSUB_ERRORS, typeof(Jabber.Protocol.IQ.ItemForbidden)),
            new QnameType("item-required",                  URI.PUBSUB_ERRORS, typeof(Jabber.Protocol.IQ.ItemRequired)),
            new QnameType("jid-required",                   URI.PUBSUB_ERRORS, typeof(Jabber.Protocol.IQ.JIDRequired)),
            new QnameType("max-items-exceeded",             URI.PUBSUB_ERRORS, typeof(Jabber.Protocol.IQ.MaxItemsExceeded)),
            new QnameType("max-nodes-exceeded",             URI.PUBSUB_ERRORS, typeof(Jabber.Protocol.IQ.MaxNodesExceeded)),
            new QnameType("nodeid-required",                URI.PUBSUB_ERRORS, typeof(Jabber.Protocol.IQ.NodeIDRequired)),
            new QnameType("not-in-roster-group",            URI.PUBSUB_ERRORS, typeof(Jabber.Protocol.IQ.NotInRosterGroup)),
            new QnameType("not-subscribed",                 URI.PUBSUB_ERRORS, typeof(Jabber.Protocol.IQ.NotSubscribed)),
            new QnameType("payload-too-big",                URI.PUBSUB_ERRORS, typeof(Jabber.Protocol.IQ.PayloadTooBig)),
            new QnameType("payload-required",               URI.PUBSUB_ERRORS, typeof(Jabber.Protocol.IQ.PayloadRequired)),
            new QnameType("pending-subscription",           URI.PUBSUB_ERRORS, typeof(Jabber.Protocol.IQ.PendingSubscription)),
            new QnameType("presence-subscription-required", URI.PUBSUB_ERRORS, typeof(Jabber.Protocol.IQ.PresenceSubscriptionRequired)),
            new QnameType("subid-required",                 URI.PUBSUB_ERRORS, typeof(Jabber.Protocol.IQ.SubidRequired)),
            new QnameType("unsupported",                    URI.PUBSUB_ERRORS, typeof(Jabber.Protocol.IQ.Unsupported)),
            new QnameType("unsupported-access-model",       URI.PUBSUB_ERRORS, typeof(Jabber.Protocol.IQ.UnsupportedAccessModel)),

            // Multi-user chat
            new QnameType("x",       URI.MUC, typeof(Jabber.Protocol.IQ.RoomX)),
            new QnameType("history", URI.MUC, typeof(Jabber.Protocol.IQ.History)),

            new QnameType("x",       URI.MUC_USER, typeof(Jabber.Protocol.IQ.UserX)),
            new QnameType("decline", URI.MUC_USER, typeof(Jabber.Protocol.IQ.Decline)),
            new QnameType("invite",  URI.MUC_USER, typeof(Jabber.Protocol.IQ.Invite)),
            new QnameType("destroy", URI.MUC_USER, typeof(Jabber.Protocol.IQ.Destroy)),
            new QnameType("item",    URI.MUC_USER, typeof(Jabber.Protocol.IQ.RoomItem)),
            new QnameType("actor",   URI.MUC_USER, typeof(Jabber.Protocol.IQ.RoomActor)),

            new QnameType("query",   URI.MUC_ADMIN, typeof(Jabber.Protocol.IQ.AdminQuery)),
            new QnameType("item",    URI.MUC_ADMIN, typeof(Jabber.Protocol.IQ.AdminItem)),

            new QnameType("query",   URI.MUC_OWNER, typeof(Jabber.Protocol.IQ.OwnerQuery)),
            new QnameType("destroy", URI.MUC_OWNER, typeof(Jabber.Protocol.IQ.OwnerDestroy)),
        };

        QnameType[] IPacketTypes.Types { get { return s_qnt; } }
    }
}
