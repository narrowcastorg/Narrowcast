using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using DnDns.Enums;
using DnDns.Query;
using DnDns.Records;
using DnDns.Security;

namespace Narrowcast
{
    class NarrowcastDns
    {
        public const string setKeyIpSuccess = "0.0.0.0";
        public const string setKeyIpFailed = "255.255.255.255";
        private string domain = null;
        private string server = null;
        public enum KeyMode { Set, InUse, None };

        public NarrowcastDns(string domain, string server) {
            this.domain = domain;
            try {
                IPAddress.Parse(server); // just for validation. (DnDns just uses the string)
                this.server = server;
            } 
            catch { this.server = null; }
        }

        // get VALUE from relaydns server. (ie. CNAME or TEXT record for "KEY.relaydns.com")
        public string GetKey(string key) {
            try {
                string getKeyDomain = key + "." + this.domain;
                DnsQueryResponse response = this.server == null
                    ? new DnsQueryRequest().Resolve(getKeyDomain, NsType.TXT, NsClass.INET, ProtocolType.Udp, null)
                    : new DnsQueryRequest().Resolve(this.server, getKeyDomain, NsType.TXT, NsClass.INET, ProtocolType.Udp, null);
                if (response.Answers.Length > 0) {
                    TxtRecord txtRecord = (TxtRecord)response.Answers[0];
                    return (txtRecord.Text);
                }
            }
            catch { }
            return(null);
        }

        // set VALUE for KEY on relaydns server. (ie. A or AAAA record for "VALUE.KEY.relaydns.com")
        public int SetKey(string key, string data)
        {
            try {
                string setKeyDomain = data + "." + key + "." + this.domain;
                DnsQueryResponse response = this.server == null
                    ? new DnsQueryRequest().Resolve(setKeyDomain, NsType.A, NsClass.INET, ProtocolType.Udp, null)
                    : new DnsQueryRequest().Resolve(this.server, setKeyDomain, NsType.A, NsClass.INET, ProtocolType.Udp, null);                    
                if (response.Answers.Length > 0) {
                    ARecord aRecord = (ARecord)response.Answers[0];
                    if (aRecord.HostAddress.Equals(NarrowcastDns.setKeyIpSuccess))
                        return ((int)KeyMode.Set);
                    else if (aRecord.HostAddress.Equals(NarrowcastDns.setKeyIpFailed))
                        return ((int)KeyMode.InUse);
                    else // unexpected, should always be 0.0.0.0 or 255.255.255.255
                        return ((int)KeyMode.None);
                }
            }
            catch { }
            return ((int)KeyMode.None); // overall resolution failure?
        }

        // get remote unix timestamp from relaydns server, to synchronize with the local time of this machine.
        public int GetRemoteTime() {
            try {
                string getRemoteTimeDomain = Guid.NewGuid().ToString("n") + ".time." + this.domain;
                int remoteTime = 0;
                DnsQueryResponse response = this.server == null
                    ? new DnsQueryRequest().Resolve(getRemoteTimeDomain, NsType.A, NsClass.INET, ProtocolType.Udp, null)
                    : new DnsQueryRequest().Resolve(this.server, getRemoteTimeDomain, NsType.A, NsClass.INET, ProtocolType.Udp, null);
                if (response.Answers.Length > 0) {
                    ARecord aRecord = (ARecord)response.Answers[0];
                    remoteTime = BitConverter.ToInt32(IPAddress.Parse(aRecord.HostAddress).GetAddressBytes(), 0);
                    return (remoteTime);
                }
            }
            catch { }

            // if we reached this point we never got the real time.
            throw new Exception("Failed to get the remote time from the relaydns server.");
        }
    
    }
}
