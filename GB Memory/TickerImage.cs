using System;
using System.Collections;
using System.Collections.Generic;
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
    public class TickerImage
    {
        public Bitmap Bitmap
        {
            get { return this.tickerBitmap; }
        }

        private Bitmap tickerBitmap = null;

        public TickerImage(string text)
        {
            Bitmap Output = new Bitmap(2048, 16);
            using (Graphics g = Graphics.FromImage(Output))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                g.Clear(Color.White);
                if (!File.Exists(Application.StartupPath + @"\fonts\ticker\PixelMplus12-Regular.ttf"))
                {
                    MessageBox.Show("Font file is missing!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                PrivateFontCollection pfc = new PrivateFontCollection();
                pfc.AddFontFile(Application.StartupPath + @"\fonts\ticker\PixelMplus12-Regular.ttf");
                Font f = new Font(pfc.Families[0], 12, FontStyle.Regular, GraphicsUnit.Pixel);

                g.TextRenderingHint = TextRenderingHint.SingleBitPerPixel;

                int x = 0;
                for (int i = 0; i < text.Length; i++)
                {
                    string chr = text[i].ToString();

                    SizeF s = g.MeasureString(chr, f, Output.Width);
                    int w = (s.Width > 15 ? 16 : 8);

                    g.FillRectangle(Brushes.Black, new Rectangle(x, 0, w, Output.Height));
                    g.DrawString(chr, f, Brushes.White, new PointF(x, 3));

                    x += w;
                }
                if (x % 16 != 0)
                {
                    g.FillRectangle(Brushes.Black, new Rectangle(x, 0, 8, Output.Height));
                }

                f.Dispose();
                pfc.Dispose();
            }
            tickerBitmap = Output.Clone(new Rectangle(0, 0, Output.Width, Output.Height), PixelFormat.Format8bppIndexed);
        }

        public Byte[] GB()
        {
            BitArray Pixels;
            List<Byte> GB2bpp = new List<byte>();
            for (int i = 0; i < 512; i++)
            {
                int tx = (i / 2) * 8;
                int ty = (i % 2 == 0) ? 0 : 8;
                Pixels = new BitArray(128);
                Bitmap Tile = tickerBitmap.Clone(new Rectangle(tx, ty, 8, 8), PixelFormat.Format8bppIndexed);
                Tile.RotateFlip(RotateFlipType.RotateNoneFlipX);
                for (int y = 0; y < 8; y++)
                {
                    for (int x = 0; x < 8; x++)
                    {
                        Color Test = Tile.GetPixel(x, y);
                        if (Test.R == 0 && Test.G == 0 && Test.B == 0)
                        {
                            Pixels[y * 16 + x] = true;
                            Pixels[y * 16 + x + 8] = true;
                        }
                    }
                }
                Byte[] tmp = new Byte[16];
                Pixels.CopyTo(tmp, 0);
                GB2bpp.AddRange(tmp);
            }
            return GB2bpp.ToArray();
        }
    }
}
