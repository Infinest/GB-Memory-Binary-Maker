using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GB_Memory
{
    public partial class TickerEditor : Form
    {
        public TickerEditor()
        {
            InitializeComponent();
        }

        public TickerImage TickerImage
        {
            get { return tickerImage; }
        }

        private TickerImage tickerImage = null;

        private void convertButton_Click(object sender, EventArgs e)
        {
            string text = this.tickerTextBox.Text.Replace("\r", "").Replace("\n", "");
            tickerImage = new TickerImage(text);
            this.tickerPictureBox.Image = tickerImage.Bitmap;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
