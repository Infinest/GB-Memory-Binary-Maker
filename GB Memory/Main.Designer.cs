namespace GB_Memory
{
    partial class Main
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.SpaceLabel = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.MenuSpace = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Add";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Location = new System.Drawing.Point(12, 41);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(423, 393);
            this.panel1.TabIndex = 1;
            this.panel1.ControlRemoved += new System.Windows.Forms.ControlEventHandler(this.panel1_ControlRemoved);
            // 
            // SpaceLabel
            // 
            this.SpaceLabel.AutoSize = true;
            this.SpaceLabel.Location = new System.Drawing.Point(322, 22);
            this.SpaceLabel.Name = "SpaceLabel";
            this.SpaceLabel.Size = new System.Drawing.Size(113, 13);
            this.SpaceLabel.TabIndex = 3;
            this.SpaceLabel.Text = "Free Space: 896kByte";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(93, 12);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(88, 23);
            this.button3.TabIndex = 4;
            this.button3.Text = "Create binaries";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // MenuSpace
            // 
            this.MenuSpace.AutoSize = true;
            this.MenuSpace.Location = new System.Drawing.Point(180, 22);
            this.MenuSpace.Name = "MenuSpace";
            this.MenuSpace.Size = new System.Drawing.Size(146, 13);
            this.MenuSpace.TabIndex = 5;
            this.MenuSpace.Text = "128kByte reserved for menu -";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(12, 440);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(423, 23);
            this.button2.TabIndex = 6;
            this.button2.Text = "Create MAP data for standalone ROM (Up to 1024kByte)";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(447, 467);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.MenuSpace);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.SpaceLabel);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "GB Memory Binary Maker";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label SpaceLabel;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Label MenuSpace;
        private System.Windows.Forms.Button button2;
    }
}

