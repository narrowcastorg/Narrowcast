using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace Narrowcast
{
    public partial class NarrowcastFormWindow : Form
    {
        private static string tabPrefix = "ncTab";
        private NarrowcastFormJoin narrowcastForm = null;

        public NarrowcastFormWindow(NarrowcastFormJoin narrowcastForm) {
            this.narrowcastForm = narrowcastForm;
            InitializeComponent();
        }

        private void NarrowcastFormWindow_Load(object sender, EventArgs e) {
            this.Icon = Narrowcast.Properties.Resources.ncIcon;
        }

        private void NarrowcastFormWindow_FormClosing(object sender, FormClosingEventArgs e) {
            foreach (NarrowcastWindow window in this.MdiChildren) {
                window.Show();
                window.Close();
            }
        }

        private void NarrowcastFormWindow_FormClosed(object sender, FormClosedEventArgs e) {
            Application.Exit();
        }

        private void NFW_StripNew_Click(object sender, EventArgs e) {
            narrowcastForm.Show();
        }

        private void NFW_TabClicked(object sender, EventArgs args) {
            try {
                ToolStripItem item = (ToolStripItem)sender;
                string tabId = item.Name;
                foreach (NarrowcastWindow window in this.MdiChildren) {
                    if (tabId.Equals(NarrowcastFormWindow.tabPrefix + window.tabId)) {
                        if (!window.Visible)
                            window.Show();
                        window.BringToFront();
                    }
                }
            }
            catch { }
        }

        public void AddTab(string tabName, int tabId) {
            ToolStripItem item = new ToolStripMenuItem();
            item.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            item.Text = tabName;
            item.Name = NarrowcastFormWindow.tabPrefix + tabId.ToString();
            item.Image = Narrowcast.Properties.Resources.ncWindow;
            item.BackColor = System.Drawing.SystemColors.ControlLight;
            item.Overflow = ToolStripItemOverflow.AsNeeded;
            item.Click += new EventHandler(NFW_TabClicked);
            item.Margin = new Padding(0, 0, 2, 0);
            this.nfwToolStrip.Items.Add(item);
        }

        public void DelTab(int tabId) {
            this.nfwToolStrip.Items.RemoveByKey(NarrowcastFormWindow.tabPrefix + tabId.ToString());
        }

    }
}
