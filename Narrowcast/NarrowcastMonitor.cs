using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Diagnostics;
using System.Threading;
using System.Collections;
using System.Drawing;
using System.Text.RegularExpressions;

namespace Narrowcast
{
    class NarrowcastMonitor
    {
        private Queue<string> writeQueue = new Queue<string>();    // queue to be sent to relaydns
        private Queue<string> writeQueueIn = new Queue<string>();  // holding queue to be moved to writeQueue (since locking on writeQueue can hold things up)
        private NarrowcastDns dns = null;
        private NarrowcastCrypt crypt = null;
        private NarrowcastWindow narrowcastWindow = null;
        private object waitLock = new Object(); // arbitrary object for Monitor (built-in sleep) lock onto.
        private string channel = null;
        private int channelFrequency = 0;
        private int channelTime = 0;
        private int channelInc = 0;
        private int timeOffset = 0;
        private bool monitorActive = false;
        private Thread monitorThread = null;

        public NarrowcastMonitor(NarrowcastWindow narrowcastWindow, string channel, string domain, string server, int frequency) {
            this.channel = channel;
            this.channelFrequency = frequency;
            this.dns = new NarrowcastDns(domain, server);
            this.crypt = new NarrowcastCrypt(NarrowcastCrypt.SHA256(this.channel));
            this.narrowcastWindow = narrowcastWindow;
        }

        ~NarrowcastMonitor() {
            this.Monitor(false);
            this.dns = null;
            this.crypt = null;
        }

        // called from the outside to queue data to write to the channel.
        public void Send(string nickname, string text) {
            try {
                if(nickname != null)
                    text = (char)NarrowcastNative.ControlCodes.SAY + nickname + (char)NarrowcastNative.ControlCodes.SAY + text;

                int size = NarrowcastNative.TEXT_BLOCK_SIZE, stringLength = text.Length;
                // break into 32 chunks. (max that can be sent per dns label w/AES+base64+dns encoding)
                lock (writeQueueIn) {
                    for (int i = 0; i < text.Length; i += size) {
                        if (i + size > text.Length)
                            size = text.Length - i;
                        writeQueueIn.Enqueue(text.Substring(i, size));
                    }
                }
                lock (waitLock) {
                    System.Threading.Monitor.Pulse(waitLock);
                }
            }
            catch { }
        }

        public byte[] CurrentId() {
            if (this.channelTime != this.CurrentSegmentTime()) {
                this.channelTime = this.CurrentSegmentTime();
                this.channelInc = 0;
            }
            string id = "";
            id += this.channelInc;
            id += this.CurrentSegmentTime();
            id += this.channelFrequency;
            id += this.channel;
            return(NarrowcastCrypt.SHA256(id));
        }        
        
        public int CurrentSegmentTime() {
            return (((int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds + this.timeOffset) / this.channelFrequency);
        }

        public int CurrentSegmentTimeRemaining() {
            return (this.channelFrequency - ((int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds + this.timeOffset) % this.channelFrequency);
        }

        // query <anything>.time.relaydns.com (or other relaydns server) for the server's time, then determine an offset against the machine's local time.
        public bool UpdateTimeOffset() {
            try {
                int remoteTime = this.dns.GetRemoteTime(); // throws exception if fails.
                this.timeOffset = remoteTime - (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                return(true);
            }
            catch { }
            return(false);
        }

        private string ParseControlCodes(string text)
        {
            try {
                string modText = text.Replace("\0", string.Empty);
                int controlCode = text[0];
                modText = modText.Remove(0, 1);
                switch (controlCode) {
                    case (int)NarrowcastNative.ControlCodes.JOIN:
                        modText = NarrowcastNative.STATUS_PREFIX + "JOIN: \"" + modText + "\" has joined the channel.\n";
                        break;
                    case (int)NarrowcastNative.ControlCodes.PART:
                        modText = NarrowcastNative.STATUS_PREFIX + "PART: \"" + modText + "\" has joined the channel.\n";
                        break;
                    case (int)NarrowcastNative.ControlCodes.QUIT:
                        modText = NarrowcastNative.STATUS_PREFIX + "QUIT: \"" + modText + "\" has joined the channel.\n";
                        break;
                    case (int)NarrowcastNative.ControlCodes.SAY:
                        int nickOffset = modText.IndexOf((char)NarrowcastNative.ControlCodes.SAY);
                        modText = "<" + modText.Substring(0, nickOffset) + "> " + modText.Substring(nickOffset + 1);
                        break;
                    default:
                        modText = text;
                        break;
                }
                return (modText);
            }
            catch { return (text); } 
        }

        private void PushToReadWindow(string text, Color normalColor, Color invertColor) {
            try { this.narrowcastWindow.Invoke(this.narrowcastWindow.updateChatTextBoxRead, Regex.Replace(this.ParseControlCodes(text), @"[\r\n]+", Environment.NewLine), normalColor, invertColor); }
            catch { }
        }

        private void UpdateWindowTitle() {
            try { this.narrowcastWindow.Invoke(this.narrowcastWindow.updateTitleQueue, writeQueue.Count + writeQueueIn.Count); }
            catch { }
        }

        public bool Monitor(bool start) {
            if (this.monitorThread != null) {
                try {
                    this.monitorActive = false;
                    this.monitorThread.Interrupt();
                    this.monitorThread = null;
                }
                catch { }
            }
            if (start) {
                try {
                    monitorActive = true;
                    this.monitorThread = new Thread(new ThreadStart(this.MonitorThread));
                    this.monitorThread.Start();
                }
                catch { return (false); }
            }
            return (true);
        }

        private void MonitorThread() {
            try {
                bool sleep = true;
                int timeUpdate = 0, currentTime = 0;
                string nextKey = null, nextData = null, txt = null, txtEncrypted = null;

                // THREAD LOOP: monitor all traffic to this channel.
                while (this.monitorThread.IsAlive && this.monitorActive) {
                    sleep = true;

                    // SYNC: get the time offset from the relaydns server, so all clients can be relatively synced without ntpd.  (only sync'd once at the beginning for now)
                    currentTime = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                    if (currentTime > timeUpdate) {
                        while (this.monitorThread.IsAlive && this.monitorActive && !this.UpdateTimeOffset()) {
                            PushToReadWindow(NarrowcastNative.STATUS_PREFIX + "ERROR: Time sync failed with dns server. (non-relaydns domain or bad dns server?)\n", Color.Red, Color.LightSalmon);
                            Thread.Sleep(1000);
                        }
                        timeUpdate = currentTime + NarrowcastNative.SERVER_TIME_SYNC_SECONDS;
                    }

                    // READ: find new dats submitted to this channel/frequency. (including our own)
                    try {
                        nextKey = this.crypt.Encrypt(this.CurrentId());
                        if ((txtEncrypted = this.dns.GetKey(nextKey)) != null) {
                            try {
                                txt = this.crypt.Decrypt(txtEncrypted);
                                PushToReadWindow(txt, Color.Empty, Color.Empty);
                            }
                            catch { }
                            this.UpdateWindowTitle();

                            // on to the next key, and skip the sleep() (and WRITE queue) to see if we already have another record waiting.
                            this.channelInc++;
                            continue; // skip the sleep since we found a result.
                        }
                    }
                    catch { }

                    // WRITE: immediately after most current READ so the key is likely current, now submit our data. (if we have any)
                    try {
                        lock (writeQueue) {

                            // see if we have anything new to add to the "real" write queue. (2 different queues due to minimize lock time)
                            lock (writeQueueIn) {
                                while (writeQueueIn.Count > 0)
                                    writeQueue.Enqueue(writeQueueIn.Dequeue());
                            }

                            bool abortWriteQueue = false;
                            // only write if: there's something to write AND we're not told to abort the loop AND we're not in the last seconds of the current window.
                            while (this.monitorThread.IsAlive && this.monitorActive && writeQueue.Count > 0 && !abortWriteQueue && this.CurrentSegmentTimeRemaining() > NarrowcastNative.SKIP_WINDOW_FINAL_SECONDS) {
                                txt = writeQueue.Peek();
                                nextKey = this.crypt.Encrypt(this.CurrentId());
                                nextData = this.crypt.Encrypt(txt);
                                switch (this.dns.SetKey(nextKey, nextData)) {
                                    // successfully set; remove from the queue, increment, and move on to the next (if any are left)
                                    case (int)NarrowcastDns.KeyMode.Set:
                                        writeQueue.Dequeue();
                                        PushToReadWindow(txt, Color.Green, Color.LightGreen);
                                        this.channelInc++;
                                        this.UpdateWindowTitle();
                                        break;
                                    // someone bet us to the punch, give up for now and go back to reading (in the next iteration of the main loop).
                                    case (int)NarrowcastDns.KeyMode.InUse:
                                        sleep = false;
                                        abortWriteQueue = true;
                                        break;
                                    // unexpected/dns resolution failure, behave like it's "in use" and go back to reading.
                                    case (int)NarrowcastDns.KeyMode.None:
                                        sleep = false; // might make sense to sleep a moment if this happens, but this should be rare.
                                        abortWriteQueue = true;
                                        break;
                                }
                            }
                        }
                        this.UpdateWindowTitle();
                    }
                    catch { }

                    // SLEEP: 2.5 seconds (or if we're pulsed after a Send()).
                    try {
                        if (sleep) {
                            lock (waitLock) {
                                System.Threading.Monitor.Wait(waitLock, 2500);
                            }
                        }
                    }
                    catch { }
                }
            }
            // really just care about ThreadAbortException, but this is a catchall.
            catch { }
        }
    }
}
