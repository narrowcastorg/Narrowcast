namespace Narrowcast
{
    partial class NarrowcastFormWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.nfwToolStrip = new System.Windows.Forms.ToolStrip();
            this.nfwStripNew = new System.Windows.Forms.ToolStripButton();
            this.nfwStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.nfwToolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // nfwToolStrip
            // 
            this.nfwToolStrip.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.nfwToolStrip.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nfwToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.nfwToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.nfwStripNew,
            this.nfwStripSeparator});
            this.nfwToolStrip.Location = new System.Drawing.Point(0, 458);
            this.nfwToolStrip.Name = "nfwToolStrip";
            this.nfwToolStrip.Padding = new System.Windows.Forms.Padding(0);
            this.nfwToolStrip.Size = new System.Drawing.Size(784, 25);
            this.nfwToolStrip.TabIndex = 2;
            this.nfwToolStrip.Text = "toolStrip";
            // 
            // nfwStripNew
            // 
            this.nfwStripNew.Image = global::Narrowcast.Properties.Resources.ncNew;
            this.nfwStripNew.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.nfwStripNew.Margin = new System.Windows.Forms.Padding(5, 1, 0, 2);
            this.nfwStripNew.Name = "nfwStripNew";
            this.nfwStripNew.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.nfwStripNew.Size = new System.Drawing.Size(52, 22);
            this.nfwStripNew.Text = "New";
            this.nfwStripNew.ToolTipText = "Join new channel";
            this.nfwStripNew.Click += new System.EventHandler(this.NFW_StripNew_Click);
            // 
            // nfwStripSeparator
            // 
            this.nfwStripSeparator.Margin = new System.Windows.Forms.Padding(0, 0, 10, 0);
            this.nfwStripSeparator.Name = "nfwStripSeparator";
            this.nfwStripSeparator.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.nfwStripSeparator.Padding = new System.Windows.Forms.Padding(0, 0, 10, 0);
            this.nfwStripSeparator.Size = new System.Drawing.Size(6, 25);
            // 
            // NarrowcastFormWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 483);
            this.Controls.Add(this.nfwToolStrip);
            this.IsMdiContainer = true;
            this.Name = "NarrowcastFormWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Narrowcast";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.NarrowcastFormWindow_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.NarrowcastFormWindow_FormClosed);
            this.Load += new System.EventHandler(this.NarrowcastFormWindow_Load);
            this.nfwToolStrip.ResumeLayout(false);
            this.nfwToolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip nfwToolStrip;
        private System.Windows.Forms.ToolStripButton nfwStripNew;
        private System.Windows.Forms.ToolStripSeparator nfwStripSeparator;



    }
}