This project is based on [ffailla-jabber-net] VS2010 fork of the original [jabber-net] library

## .NET Framework

* Namespace naming follows C# coding guidelines
* .NET Framework 2.0 is still the base support except for Tests project which requires .NET Framework 4

## Visual Studio 2010

* Projects were reorganized to follow Visual Studio 2010 Solution guidelines
* Each components and forms classes of JabberNet, Muzzle and SampleWinFormClient projects were added a .Designer.cs file where every stuff belonging to VS Designer has been moved
* Component designer icons were refreshed and added where missing
* Tests project uses Visual Studio Testing Framework instead of NUnit Framework
* NuGet 2.0 package manager is used to manage zlib and Rhino.Mocks libraries

## XMPP Additions

* [XEP-0199]: XMPP Ping 
* [XEP-0166]: Jingle coupled to a Jingle Manager for session management only
* [XEP-0176]: Jingle ICE-UDP Transport Method
* [RFC 5389]: Session Traversal Utilities for NAT (STUN). Can parse STUN messages and attributes coupled to a minimal client that can request a STUN server for its reflexive address. IPv6 is not handled.
* [RFC 5766]: Traversal Using Relays around NAT (TURN). Can parse TURN messages and attributes
* [RFC 3489]: STUN Classic deprecated in favor of RFC 5389. Can parse STUN Classic messages and attributes for debugging purposes only. Code elements of this RFC are annotated as deprecated.

## ATTENTION Support dropped

* Visual Studio below 2010 and Mono support is dropped because I do not have time and resources to support them
* Specific Mono compiler directives remains in place for people needing it
* Project using the original ones won't be compatible with this project mainly because of the namespace refactoring

[XEP-0199]: http://xmpp.org/extensions/xep-0199.html
[XEP-0166]: http://xmpp.org/extensions/xep-0166.html
[XEP-0176]: http://xmpp.org/extensions/xep-0176.html
[RFC 5389]: http://tools.ietf.org/html/rfc5389
[RFC 5766]: http://tools.ietf.org/html/rfc5766
[RFC 3489]: http://tools.ietf.org/html/rfc3489
[jabber-net]: http://code.google.com/p/jabber-net/
[ffailla-jabber-net]: https://github.com/ffailla/jabber-net