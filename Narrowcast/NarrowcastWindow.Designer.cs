namespace Narrowcast
{
    partial class NarrowcastWindow
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
            this.nwTextBoxWrite = new System.Windows.Forms.TextBox();
            this.nwLayout = new System.Windows.Forms.TableLayoutPanel();
            this.nwRichTextBoxRead = new System.Windows.Forms.RichTextBox();
            this.nwLayout.SuspendLayout();
            this.SuspendLayout();
            // 
            // nwTextBoxWrite
            // 
            this.nwTextBoxWrite.BackColor = System.Drawing.SystemColors.Control;
            this.nwTextBoxWrite.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.nwTextBoxWrite.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.nwTextBoxWrite.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nwTextBoxWrite.ForeColor = System.Drawing.SystemColors.ControlText;
            this.nwTextBoxWrite.Location = new System.Drawing.Point(2, 316);
            this.nwTextBoxWrite.Margin = new System.Windows.Forms.Padding(2);
            this.nwTextBoxWrite.Name = "nwTextBoxWrite";
            this.nwTextBoxWrite.Size = new System.Drawing.Size(602, 16);
            this.nwTextBoxWrite.TabIndex = 0;
            this.nwTextBoxWrite.KeyDown += new System.Windows.Forms.KeyEventHandler(this.NW_TextBoxWrite_KeyDown);
            // 
            // nwLayout
            // 
            this.nwLayout.BackColor = System.Drawing.SystemColors.Control;
            this.nwLayout.ColumnCount = 1;
            this.nwLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.nwLayout.Controls.Add(this.nwTextBoxWrite, 0, 1);
            this.nwLayout.Controls.Add(this.nwRichTextBoxRead, 0, 0);
            this.nwLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.nwLayout.ForeColor = System.Drawing.SystemColors.ControlText;
            this.nwLayout.Location = new System.Drawing.Point(0, 0);
            this.nwLayout.Margin = new System.Windows.Forms.Padding(0);
            this.nwLayout.Name = "nwLayout";
            this.nwLayout.RowCount = 2;
            this.nwLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.nwLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.nwLayout.Size = new System.Drawing.Size(606, 334);
            this.nwLayout.TabIndex = 0;
            // 
            // nwRichTextBoxRead
            // 
            this.nwRichTextBoxRead.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.nwRichTextBoxRead.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.nwRichTextBoxRead.Dock = System.Windows.Forms.DockStyle.Fill;
            this.nwRichTextBoxRead.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nwRichTextBoxRead.Location = new System.Drawing.Point(0, 0);
            this.nwRichTextBoxRead.Margin = new System.Windows.Forms.Padding(0);
            this.nwRichTextBoxRead.Name = "nwRichTextBoxRead";
            this.nwRichTextBoxRead.ReadOnly = true;
            this.nwRichTextBoxRead.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.nwRichTextBoxRead.Size = new System.Drawing.Size(606, 314);
            this.nwRichTextBoxRead.TabIndex = 1;
            this.nwRichTextBoxRead.Text = "";
            this.nwRichTextBoxRead.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.NW_RichTextBoxRead_LinkClicked);
            this.nwRichTextBoxRead.MouseDown += new System.Windows.Forms.MouseEventHandler(this.NW_RichTextBoxRead_MouseDown);
            // 
            // NarrowcastWindow
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(606, 334);
            this.Controls.Add(this.nwLayout);
            this.Name = "NarrowcastWindow";
            this.ShowIcon = false;
            this.Text = "NarrowcastWindow";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.NarrowcastWindow_FormClosing);
            this.Load += new System.EventHandler(this.NarrowcastWindow_Load);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.NarrowcastWindow_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.NarrowcastWindow_DragEnter);
            this.Resize += new System.EventHandler(this.NarrowcastWindow_Resize);
            this.nwLayout.ResumeLayout(false);
            this.nwLayout.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox nwTextBoxWrite;
        private System.Windows.Forms.TableLayoutPanel nwLayout;
        private System.Windows.Forms.RichTextBox nwRichTextBoxRead;


    }
}