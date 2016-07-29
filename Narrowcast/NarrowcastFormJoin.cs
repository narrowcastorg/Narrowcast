using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;


namespace Narrowcast
{
    public partial class NarrowcastFormJoin : Form
    {
        private int narrowcastWindowCount = 0;
        private NarrowcastFormWindow narrowcastWindowContainer = null;

        public NarrowcastFormJoin() {
            InitializeComponent();
        }

        private void NarrowcastFormJoin_Load(object sender, EventArgs e) {
            this.Icon = Narrowcast.Properties.Resources.ncIcon;

            // set nickname to random (CNNNNNNN)
            Random r = new Random();
            nfjNickTextBox.Text = ((char)('a' + r.Next(0, 26))).ToString() + r.Next(1000000, 9999999).ToString();
        }

        private void NarrowcastFormJoin_FormClosing(object sender, FormClosingEventArgs e) {
            if (narrowcastWindowCount > 0) {
                e.Cancel = true;
                this.Hide();
            }
        }

        private void NarrowcastFormJoin_Deactivate(object sender, EventArgs e) {
            if (this.narrowcastWindowContainer != null)
                this.Hide();
        }

        // listen to child form (NarrowcastWindow) for closures, decrement the overall count and trigger program exit if none are left (as long as it's hidden).
        // NOTE: this is to be passed to "NarrowcastWindow", NOT RELATED TO "NarrowcastFormJoin" (this form).
        private void NarrowcastWindow_FormClosed(object sender, FormClosedEventArgs e) {
            narrowcastWindowCount--;
            if (narrowcastWindowCount <= 0 && !this.Visible)
                this.Close();
        }

        private void NFJ_JoinButton_Click(object sender, EventArgs e) {
            // validate.
            if (nfjNickTextBox.Text.Length == 0)
                MessageBox.Show("No nickname specified to use.", "Nickname Error");
            else if (nfjChannelTextBox.Text.Length == 0)
                MessageBox.Show("No channel specified to join.", "Channel Error");
            else if (nfjDomainTextBox.Text.Length == 0)
                MessageBox.Show("No (relaydns) domain specified to use.", "Domain Error");
            else if (nfjDomainTldTextBox.Text.Length == 0)
                MessageBox.Show("No (relaydns) domain TLD specified to use.", "Domain Error");
            // validated, create window and hide the main form.
            else {
                NarrowcastWindow narrowcastWindow = new NarrowcastWindow(
                    narrowcastWindowCount,
                    nfjNickTextBox.Text,
                    nfjChannelTextBox.Text,
                    nfjDomainTextBox.Text + "." + nfjDomainTldTextBox.Text,
                    this.GenerateDnsServerIp(),
                    (int)nfjFrequencyNumeric.Value,
                    nfjInvertCheckBox.Checked
                );

                // tell the window to report back to this class (main form) when it's closed.
                narrowcastWindow.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.NarrowcastWindow_FormClosed);

                if (this.narrowcastWindowContainer == null)
                    this.narrowcastWindowContainer = new NarrowcastFormWindow(this);
                narrowcastWindow.MdiParent = this.narrowcastWindowContainer;

                // up our total window count and show it.
                narrowcastWindowCount++;
                narrowcastWindow.Show();

                narrowcastWindowContainer.Show();

                // hide the main form until requested upon.
                this.Hide();
            }
        }

        private void NFJ_ArbitraryTextBox_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyData == Keys.Enter) {
                e.SuppressKeyPress = true;
                SelectNextControl(ActiveControl, true, true, true, true);
            }

        }

        private void NFJ_ArbitraryTextBox_KeyPress(object sender, KeyPressEventArgs e) {
            e.Handled = !(char.IsLetterOrDigit(e.KeyChar) || e.KeyChar == (char)Keys.Back || e.KeyChar == '-');
        }

        // remove extra dashes from start/end of the domain string, since they are prohibited.
        private void NFJ_DomainTextBox_Leave(object sender, EventArgs e) {
            TextBox textBox = (TextBox)sender;
            textBox.Text = textBox.Text.Trim(new Char[] { '-' });
        }

        private void NFJ_IpTextBox_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.OemPeriod)
            {
                e.SuppressKeyPress = true;
                SelectNextControl(ActiveControl, true, true, true, true);
            }
        }

        private void NFJ_IpTextBox_KeyPress(object sender, KeyPressEventArgs e) {
            e.Handled = !(char.IsNumber(e.KeyChar) || e.KeyChar == (char)Keys.Back);
        }

        private void NFJ_IpTextBox_Leave(object sender, EventArgs e) {
            TextBox textBox = (TextBox)sender;
            int number;
            bool result = Int32.TryParse(textBox.Text, out number);
            if (result)
                textBox.Text = number > 255 ? "0" : number.ToString();
            else
                textBox.Text = "0";
        }

        private string GenerateDnsServerIp() {
            try {
                string ip = nfjServerTextBox1.Text + "." + nfjServerTextBox2.Text + "." + nfjServerTextBox3.Text + "." + nfjServerTextBox4.Text;
                if (ip.Equals("0.0.0.0"))
                    return (null);
                else
                    return (ip);
            }
            catch { return (null); }
        }

        protected override void WndProc(ref Message m) {
            if (m.Msg == NarrowcastNative.WM_SHOWNARROWCAST) {
                this.Show();
                TopMost = true;
                TopMost = false;
            }
            base.WndProc(ref m);
        }

    }
}
