using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace Narrowcast
{
    public partial class NarrowcastWindow : Form
    {
        private bool invertColors = true;
        private string nick = null;
        private string windowId = null;
        public readonly int tabId = 0;
        private NarrowcastMonitor narrowcastMonitor = null;

        private List<string> inputBuffer = new List<string>();
        private int inputBufferSelected = 0;

        // for NarrowcastMonitor class to update the read textbox.
        public delegate void UpdateChatTextBoxReadDelegate(String str, Color normalColor, Color invertColor);
        public UpdateChatTextBoxReadDelegate updateChatTextBoxRead;
        public delegate void UpdateTitleQueueDelegate(int blocks);
        public UpdateTitleQueueDelegate updateTitleQueue;

        public NarrowcastWindow(int tabId, string nick, string channel, string domain, string server, int frequency, bool invertColors) {
            this.tabId = tabId;
            this.windowId = nick + "@" + frequency.ToString() + "." + channel + "." + domain + (server == null ? "" : " [dns=" + server + "]");
            this.nick = nick;
            this.invertColors = invertColors;
            this.updateChatTextBoxRead = new UpdateChatTextBoxReadDelegate(UpdateChatTextBoxReadInvoke);
            this.updateTitleQueue = new UpdateTitleQueueDelegate(UpdateTitleQueueInvoke);
            this.narrowcastMonitor = new NarrowcastMonitor(this, channel, domain, server, frequency);
            this.narrowcastMonitor.Send(null, (char)NarrowcastNative.ControlCodes.JOIN + this.nick);            
            InitializeComponent();
        }

        private void NarrowcastWindow_Load(object sender, EventArgs e) {
            // handle "invert" colors.
            Color foreColor = (this.invertColors ? System.Drawing.Color.White : System.Drawing.Color.Black);
            Color backColor = (this.invertColors ? System.Drawing.Color.Black : System.Drawing.Color.White);
            this.nwLayout.BackColor = this.nwRichTextBoxRead.BackColor = this.nwTextBoxWrite.BackColor = backColor;
            this.nwLayout.ForeColor = this.nwRichTextBoxRead.ForeColor = this.nwTextBoxWrite.ForeColor = foreColor;
            this.Icon = Narrowcast.Properties.Resources.ncIcon;
            this.Text = this.windowId;

            NarrowcastFormWindow nwc = (NarrowcastFormWindow)ParentForm;
            nwc.AddTab(this.windowId, this.tabId);

            // start monitor associated with this window.
            if (!narrowcastMonitor.Monitor(true)) {
                // destroy!
                narrowcastMonitor = null;
                return;
            }
        }

        private void NarrowcastWindow_FormClosing(object sender, FormClosingEventArgs e) {
            try {
                NarrowcastFormWindow nwc = (NarrowcastFormWindow)ParentForm;
                nwc.DelTab(this.tabId);
                if (this.narrowcastMonitor != null) {
                    this.narrowcastMonitor.Monitor(false);
                    this.narrowcastMonitor = null;
                }
            }
            catch { }
        }
        private void NarrowcastWindow_Resize(object sender, EventArgs e) {
            if (this.WindowState == FormWindowState.Minimized) {
                this.Hide();
                this.WindowState = FormWindowState.Normal;
            }
        }
        private void NarrowcastWindow_DragEnter(object sender, DragEventArgs e) {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void NarrowcastWindow_DragDrop(object sender, DragEventArgs e) {
            try {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length > 1) {
                    MessageBox.Show("Sorry, only one file can be dumped at a time.", "File Dump");
                    return;
                }
                if (new System.IO.FileInfo(files[0]).Length > NarrowcastNative.MAX_FILE_DUMP_SIZE) {
                    MessageBox.Show("Sorry, this file is too large. (" + NarrowcastNative.MAX_FILE_DUMP_SIZE + " byte limit)", "File Dump");
                    return;
                }
                this.narrowcastMonitor.Send(this.nick, "*** START FILE DUMP ***\n");
                this.narrowcastMonitor.Send(null, File.ReadAllText(files[0]));
                this.narrowcastMonitor.Send(this.nick, "*** END FILE DUMP ***\n");
            }
            catch { }
        }
        private void NW_TextBoxWrite_KeyDown(object sender, KeyEventArgs e) {
            switch (e.KeyData) {

                // send line/data out via narrowcastMonitor.
                case Keys.Enter: 
                    if (nwTextBoxWrite.Text.Length > 0) {
                        this.narrowcastMonitor.Send(this.nick, nwTextBoxWrite.Text + "\n");

                        // only save the input to the scrollback buffer if it's different from the last line.
                        if (this.inputBuffer.Count == 0 || !this.inputBuffer[this.inputBuffer.Count - 1].Equals(this.nwTextBoxWrite.Text))
                            this.inputBuffer.Add(nwTextBoxWrite.Text);

                        this.nwTextBoxWrite.Text = "";
                        this.inputBufferSelected = this.inputBuffer.Count; // reset scrollback location. (end of the list)
                    }
                    e.Handled = e.SuppressKeyPress = true;
                    break;

                // go up and down the scrollback input buffer to find lines we've already sent.
                case Keys.Up:
                    if (this.inputBufferSelected > 0) {
                        this.inputBufferSelected--;
                        this.nwTextBoxWrite.Text = this.inputBuffer[this.inputBufferSelected];
                        this.nwTextBoxWrite.SelectionStart = Text.Length;
                        this.nwTextBoxWrite.SelectionLength = 0;
                    }
                    e.Handled = e.SuppressKeyPress = true;
                    break;
                case Keys.Down:
                    if (this.inputBufferSelected < this.inputBuffer.Count - 1) {
                        this.inputBufferSelected++;
                        this.nwTextBoxWrite.Text = this.inputBuffer[this.inputBufferSelected];
                        this.nwTextBoxWrite.SelectionStart = Text.Length;
                        this.nwTextBoxWrite.SelectionLength = 0;
                    }
                    // all the way down to the bottom of the list, empty line and reset input buffer location.
                    else {
                        this.nwTextBoxWrite.Text = String.Empty;
                        this.inputBufferSelected = this.inputBuffer.Count;
                    }
                    e.Handled = e.SuppressKeyPress = true;
                    break;
            }
        }

        private void NW_RichTextBoxRead_ClearClick(object sender, EventArgs e) {
            nwRichTextBoxRead.Clear();
        }
        private void NW_RichTextBoxRead_CopyClick(object sender, EventArgs e) {
            try { Clipboard.SetText(Regex.Replace(nwRichTextBoxRead.Text, @"[\r\n]+", Environment.NewLine)); }
            catch { }
        }
        private void NW_RichTextBoxRead_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Right) {
                ContextMenu m = new ContextMenu();
                m.MenuItems.Add(new MenuItem("Copy All", new EventHandler(NW_RichTextBoxRead_CopyClick)));
                m.MenuItems.Add("-");
                m.MenuItems.Add(new MenuItem("Clear", new EventHandler(NW_RichTextBoxRead_ClearClick)));
                m.Show(nwRichTextBoxRead, e.Location);
            }
            nwTextBoxWrite.Focus();
        }

        // copy urls to the clipboard.
        private void NW_RichTextBoxRead_LinkClicked(object sender, LinkClickedEventArgs e) {
            try { Clipboard.SetText(e.LinkText); }
            catch { }
        }

        // called by NarrowcastMonitor to update the read chat window.
        void UpdateChatTextBoxReadInvoke(string str, Color normalColor, Color invertColor) {
            Color color = invertColors ? invertColor : normalColor;
            nwRichTextBoxRead.SelectionStart = nwRichTextBoxRead.TextLength;
            nwRichTextBoxRead.SelectionLength = 0;
            if (color != Color.Empty)
                nwRichTextBoxRead.SelectionColor = color;
            nwRichTextBoxRead.AppendText(str);
            nwRichTextBoxRead.SelectionColor = nwRichTextBoxRead.ForeColor;
            nwRichTextBoxRead.SelectionStart = nwRichTextBoxRead.Text.Length;
            nwRichTextBoxRead.ScrollToCaret();
        }

        // called by NarrowcastMonitor to update the number of blocks remaining to write.
        void UpdateTitleQueueInvoke(int blocks) {
            this.Text = this.windowId + (blocks > 0 ? " - ("+blocks+" " + (blocks==1?"block":"blocks") + " in output queue)" : "");
        }
    }
}
