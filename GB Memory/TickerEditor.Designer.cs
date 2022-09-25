namespace GB_Memory
{
    partial class TickerEditor
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
            this.convertButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.tickerTextBox = new System.Windows.Forms.TextBox();
            this.tickerTextLabel = new System.Windows.Forms.Label();
            this.tickerImageLabel = new System.Windows.Forms.Label();
            this.tickerPictureBox = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.tickerPictureBox)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // convertButton
            // 
            this.convertButton.Location = new System.Drawing.Point(12, 157);
            this.convertButton.Name = "convertButton";
            this.convertButton.Size = new System.Drawing.Size(323, 31);
            this.convertButton.TabIndex = 0;
            this.convertButton.Text = "Convert to Image";
            this.convertButton.UseVisualStyleBackColor = true;
            this.convertButton.Click += new System.EventHandler(this.convertButton_Click);
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(12, 282);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(159, 35);
            this.okButton.TabIndex = 1;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(177, 282);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(158, 35);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // tickerTextBox
            // 
            this.tickerTextBox.Location = new System.Drawing.Point(12, 27);
            this.tickerTextBox.Multiline = true;
            this.tickerTextBox.Name = "tickerTextBox";
            this.tickerTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tickerTextBox.Size = new System.Drawing.Size(323, 124);
            this.tickerTextBox.TabIndex = 3;
            // 
            // tickerTextLabel
            // 
            this.tickerTextLabel.AutoSize = true;
            this.tickerTextLabel.Location = new System.Drawing.Point(12, 9);
            this.tickerTextLabel.Name = "tickerTextLabel";
            this.tickerTextLabel.Size = new System.Drawing.Size(246, 15);
            this.tickerTextLabel.TabIndex = 4;
            this.tickerTextLabel.Text = "Ticker Text (line-breaks are ignored):";
            // 
            // tickerImageLabel
            // 
            this.tickerImageLabel.AutoSize = true;
            this.tickerImageLabel.Location = new System.Drawing.Point(12, 191);
            this.tickerImageLabel.Name = "tickerImageLabel";
            this.tickerImageLabel.Size = new System.Drawing.Size(92, 15);
            this.tickerImageLabel.TabIndex = 5;
            this.tickerImageLabel.Text = "Ticker Image:";
            // 
            // tickerPictureBox
            // 
            this.tickerPictureBox.BackColor = System.Drawing.Color.Black;
            this.tickerPictureBox.Location = new System.Drawing.Point(0, 0);
            this.tickerPictureBox.Name = "tickerPictureBox";
            this.tickerPictureBox.Size = new System.Drawing.Size(2048, 16);
            this.tickerPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.tickerPictureBox.TabIndex = 6;
            this.tickerPictureBox.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.tickerPictureBox);
            this.panel1.Location = new System.Drawing.Point(15, 209);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(320, 55);
            this.panel1.TabIndex = 7;
            // 
            // TickerEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(351, 330);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.tickerImageLabel);
            this.Controls.Add(this.tickerTextLabel);
            this.Controls.Add(this.tickerTextBox);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.convertButton);
            this.Name = "TickerEditor";
            this.Text = "Edit Ticker";
            ((System.ComponentModel.ISupportInitialize)(this.tickerPictureBox)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button convertButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.TextBox tickerTextBox;
        private System.Windows.Forms.Label tickerTextLabel;
        private System.Windows.Forms.Label tickerImageLabel;
        private System.Windows.Forms.PictureBox tickerPictureBox;
        private System.Windows.Forms.Panel panel1;
    }
}