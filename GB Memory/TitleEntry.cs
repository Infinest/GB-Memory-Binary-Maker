using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GB_Memory
{
    public partial class TitleEntry : Form
    {
        private String ASCII;
        public String Title
        {
            get
            {
                if (checkBox2.Checked)
                {
                    return Regex.Replace(textBox1.Text, "[!-~]", p => ((char)(p.Value[0] - '!' + '\uff01')).ToString()); // '\uff01' == '！'
                }

                return textBox1.Text;
            }
        }
        public bool UseTrueTypeFont
        {
            get
            {
                return checkBox1.Checked;
            }
        }
        public TitleEntry(string title = "")
        {
            InitializeComponent();
            ASCII = title;
            if(title != "")
            {
                this.Height += 10;
                foreach (Control o in this.Controls)
                {
                    o.Location = new Point(o.Location.X, o.Location.Y + 10); 
                }
                Label DisplayASCIITitle = new Label();
                DisplayASCIITitle.AutoSize = true;
                DisplayASCIITitle.Text = String.Format("Enter title for {0}",title);
                DisplayASCIITitle.Location = new Point(5, 5);
                this.Controls.Add(DisplayASCIITitle);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            checkBox2.Enabled = checkBox1.Checked;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length == 0)
            {
                textBox1.Text = ASCII;
            }
            this.DialogResult = DialogResult.OK;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }


    }
}
