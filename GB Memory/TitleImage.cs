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

    class TitleImage
    {

        public static Bitmap CreateTitleBitmapWithOriginalFont(string Input)
        {
            Bitmap Output = new Bitmap(128, 8);
            using (Graphics g = Graphics.FromImage(Output))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                g.Clear(Color.Black);
                if (!File.Exists(Application.StartupPath + @"\font.bmp"))
                {
                    MessageBox.Show("Font file is missing!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return Output;
                }
                int pos = 0;
                Bitmap Font = (Bitmap)Bitmap.FromFile(Application.StartupPath + @"\font.bmp");
                foreach (Char C in Input)
                {
                    g.DrawImage(Font.Clone(Letter(C), PixelFormat.Format8bppIndexed), pos, 0, Letter(C).Width, Letter(C).Height);
                    pos += Letter(C).Width;
                }
                Font.Dispose();
            }
            return Output.Clone(new Rectangle(0, 0, Output.Width, Output.Height), PixelFormat.Format8bppIndexed);
        }

        public static Bitmap CreateTitleBitmapWithTrueTypeFont(string Input)
        {
            Bitmap Output = new Bitmap(128, 8);
            using (Graphics g = Graphics.FromImage(Output))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                g.Clear(Color.Black);
                if (!File.Exists(Application.StartupPath + @"\fonts\title\misaki_gothic.ttf"))
                {
                    MessageBox.Show("Font file is missing!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return Output;
                }
                PrivateFontCollection pfc = new PrivateFontCollection();
                pfc.AddFontFile(Application.StartupPath + @"\fonts\title\misaki_gothic.ttf");
                Font f = new Font(pfc.Families[0], 8, FontStyle.Regular, GraphicsUnit.Pixel);
                g.TextRenderingHint = TextRenderingHint.SingleBitPerPixel;
                int pos = 0;
                foreach (Char C in Input)
                {
                    g.DrawString(C.ToString(), f, Brushes.White, new PointF(pos, 1));
                    pos += 8;
                }
                f.Dispose();
                pfc.Dispose();
            }
            return Output.Clone(new Rectangle(0, 0, Output.Width, Output.Height), PixelFormat.Format8bppIndexed);
        }

        public static Bitmap CreateTitleBitmap(ROM R)
        {
            if (R.UseTrueTypeFontForTitleImage)
            {
                return CreateTitleBitmapWithTrueTypeFont(R.Title);
            }

            return CreateTitleBitmapWithOriginalFont(R.Title);
        }

        public static Byte[] GB(Bitmap Input)
        {
            BitArray Pixels;
            List<Byte> GB2bpp = new List<byte>();
            for (int i = 0; i < 16; i++)
            {
                Pixels = new BitArray(128);
                Bitmap Tile = Input.Clone(new Rectangle(i * 8, 0, 8, 8), PixelFormat.Format8bppIndexed);
                Tile.RotateFlip(RotateFlipType.RotateNoneFlipX);
                for (int y = 0; y < 8; y++)
                {
                    for(int x = 0;x < 8; x++)
                    {
                        Color Test = Tile.GetPixel(x, y);
                        if (Test.R > 0 || Test.G > 0 || Test.B > 0)
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

        public static Rectangle Letter2(Char Input)
        {
            switch (Input)
            {
                //First Row
                case ('°'):
                    return new Rectangle(16, 0, 8, 8);
                case ('´'):
                    return new Rectangle(24, 0, 8, 8);
                case ('$'):
                    return new Rectangle(32, 0, 8, 8);
                case ('"'):
                    return new Rectangle(40, 0, 8, 8);
                case ('&'):
                    return new Rectangle(48, 0, 8, 8);
                case ('('):
                    return new Rectangle(64, 0, 8, 8);
                case (')'):
                    return new Rectangle(72, 0, 8, 8);
                case ('*'):
                    return new Rectangle(80, 0, 8, 8);
                case ('+'):
                    return new Rectangle(88, 0, 8, 8);
                case (','):
                    return new Rectangle(96, 0, 8, 8);
                case ('-'):
                    return new Rectangle(104, 0, 8, 8);
                case ('.'):
                    return new Rectangle(112, 0, 8, 8);
                case ('/'):
                    return new Rectangle(120, 0, 8, 8);

                //Second Row
                case ('0'):
                    return new Rectangle(0, 8, 8, 8);
                case ('1'):
                    return new Rectangle(8, 8, 8, 8);
                case ('2'):
                    return new Rectangle(16, 8, 8, 8);
                case ('3'):
                    return new Rectangle(24, 8, 8, 8);
                case ('4'):
                    return new Rectangle(32, 8, 8, 8);
                case ('5'):
                    return new Rectangle(40, 8, 8, 8);
                case ('6'):
                    return new Rectangle(48, 8, 8, 8);
                case ('7'):
                    return new Rectangle(56, 8, 8, 8);
                case ('8'):
                    return new Rectangle(64, 8, 8, 8);
                case ('9'):
                    return new Rectangle(72, 8, 8, 8);
                case (':'):
                    return new Rectangle(80, 8, 8, 8);
                case (';'):
                    return new Rectangle(88, 8, 8, 8);
                case ('<'):
                    return new Rectangle(96, 8, 8, 8);
                case ('='):
                    return new Rectangle(104, 8, 8, 8);
                case ('>'):
                    return new Rectangle(112, 8, 8, 8);
                case ('?'):
                    return new Rectangle(120, 8, 8, 8);

                //Third Row
                case ('A'):
                    return new Rectangle(8, 16, 8, 8);
                case ('B'):
                    return new Rectangle(16, 16, 8, 8);
                case ('C'):
                    return new Rectangle(24, 16, 8, 8);
                case ('D'):
                    return new Rectangle(32, 16, 8, 8);
                case ('E'):
                    return new Rectangle(40, 16, 8, 8);
                case ('F'):
                    return new Rectangle(48, 16, 8, 8);
                case ('G'):
                    return new Rectangle(56, 16, 8, 8);
                case ('H'):
                    return new Rectangle(64, 16, 8, 8);
                case ('I'):
                    return new Rectangle(72, 16, 8, 8);
                case ('J'):
                    return new Rectangle(80, 16, 8, 8);
                case ('K'):
                    return new Rectangle(88, 16, 8, 8);
                case ('L'):
                    return new Rectangle(96, 16, 8, 8);
                case ('M'):
                    return new Rectangle(104, 16, 8, 8);
                case ('N'):
                    return new Rectangle(112, 16, 8, 8);
                case ('O'):
                    return new Rectangle(120, 16, 8, 8);

                //Fourth Row
                case ('P'):
                    return new Rectangle(0, 24, 8, 8);
                case ('Q'):
                    return new Rectangle(8, 24, 8, 8);
                case ('R'):
                    return new Rectangle(16, 24, 8, 8);
                case ('S'):
                    return new Rectangle(24, 24, 8, 8);
                case ('T'):
                    return new Rectangle(32, 24, 8, 8);
                case ('U'):
                    return new Rectangle(40, 24, 8, 8);
                case ('V'):
                    return new Rectangle(48, 24, 8, 8);
                case ('W'):
                    return new Rectangle(56, 24, 8, 8);
                case ('X'):
                    return new Rectangle(64, 24, 8, 8);
                case ('Y'):
                    return new Rectangle(72, 24, 8, 8);
                case ('Z'):
                    return new Rectangle(80, 24, 8, 8);
                case ('['):
                    return new Rectangle(88, 24, 8, 8);
                case (']'):
                    return new Rectangle(104, 24, 8, 8);
                case ('^'):
                    return new Rectangle(112, 24, 8, 8);
                case ('_'):
                    return new Rectangle(120, 24, 8, 8);

                //Fifth Row
                case ('a'):
                    return new Rectangle(8, 32, 8, 8);
                case ('b'):
                    return new Rectangle(16, 32, 8, 8);
                case ('c'):
                    return new Rectangle(24, 32, 8, 8);
                case ('d'):
                    return new Rectangle(32, 32, 8, 8);
                case ('e'):
                    return new Rectangle(40, 32, 8, 8);
                case ('f'):
                    return new Rectangle(48, 32, 8, 8);
                case ('g'):
                    return new Rectangle(56, 32, 8, 8);
                case ('h'):
                    return new Rectangle(64, 32, 8, 8);
                case ('i'):
                    return new Rectangle(72, 32, 8, 8);
                case ('j'):
                    return new Rectangle(80, 32, 8, 8);
                case ('k'):
                    return new Rectangle(88, 32, 8, 8);
                case ('l'):
                    return new Rectangle(96, 32, 8, 8);
                case ('m'):
                    return new Rectangle(104, 32, 8, 8);
                case ('n'):
                    return new Rectangle(112, 32, 8, 8);
                case ('o'):
                    return new Rectangle(120, 32, 8, 8);

                //Sixth Row
                case ('p'):
                    return new Rectangle(0, 40, 8, 8);
                case ('q'):
                    return new Rectangle(8, 40, 8, 8);
                case ('r'):
                    return new Rectangle(16, 40, 8, 8);
                case ('s'):
                    return new Rectangle(24, 40, 8, 8);
                case ('t'):
                    return new Rectangle(32, 40, 8, 8);
                case ('u'):
                    return new Rectangle(40, 40, 8, 8);
                case ('v'):
                    return new Rectangle(48, 40, 8, 8);
                case ('w'):
                    return new Rectangle(56, 40, 8, 8);
                case ('x'):
                    return new Rectangle(64, 40, 8, 8);
                case ('y'):
                    return new Rectangle(72, 40, 8, 8);
                case ('z'):
                    return new Rectangle(80, 40, 8, 8);
                case ('―'):
                    return new Rectangle(104, 40, 8, 8);
                case ('~'):
                    return new Rectangle(112, 40, 8, 8);
                case ('|'):
                    return new Rectangle(120, 40, 8, 8);
            }
            return new Rectangle(0, 0, 8, 7);
        }
        public static Rectangle Letter(Char Input)
        {
            switch (Input)
            {
                case '0':
                    return new Rectangle(0, 0, 7, 8);
                case '1':
                    return new Rectangle(9, 0, 5, 8);
                case '2':
                    return new Rectangle(16, 0, 7, 8);
                case '3':
                    return new Rectangle(24, 0, 7, 8);
                case '4':
                    return new Rectangle(32, 0, 8, 8);
                case '5':
                    return new Rectangle(40, 0, 7, 8);
                case '6':
                    return new Rectangle(48, 0, 7, 8);
                case '7':
                    return new Rectangle(56, 0, 7, 8);
                case '8':
                    return new Rectangle(64, 0, 7, 8);
                case '9':
                    return new Rectangle(72, 0, 7, 8);
                case 'A':
                    return new Rectangle(80, 0, 7, 8);
                case 'B':
                    return new Rectangle(88, 0, 7, 8);
                case 'C':
                    return new Rectangle(96, 0, 7, 8);
                case 'D':
                    return new Rectangle(104, 0, 7, 8);
                case 'E':
                    return new Rectangle(112, 0, 7, 8);
                case 'F':
                    return new Rectangle(120, 0, 7, 8);
                case 'G':
                    return new Rectangle(0, 8, 7, 8);
                case 'H':
                    return new Rectangle(8, 8, 7, 8);
                case 'I':
                    return new Rectangle(17, 8, 5, 8);
                case 'J':
                    return new Rectangle(24, 8, 7, 8);
                case 'K':
                    return new Rectangle(32, 8, 7, 8);
                case 'L':
                    return new Rectangle(40, 8, 7, 8);
                case 'M':
                    return new Rectangle(48, 8, 7, 8);
                case 'N':
                    return new Rectangle(56, 8, 7, 8);
                case 'O':
                    return new Rectangle(64, 8, 7, 8);
                case 'P':
                    return new Rectangle(72, 8, 7, 8);
                case 'Q':
                    return new Rectangle(80, 8, 7, 8);
                case 'R':
                    return new Rectangle(88, 8, 7, 8);
                case 'S':
                    return new Rectangle(96, 8, 7, 8);
                case 'T':
                    return new Rectangle(104, 8, 7, 8);
                case 'U':
                    return new Rectangle(112, 8, 7, 8);
                case 'V':
                    return new Rectangle(120, 8, 7, 8);
                case 'W':
                    return new Rectangle(0, 16, 7, 8);
                case 'X':
                    return new Rectangle(8, 16, 7, 8);
                case 'Y':
                    return new Rectangle(16, 16, 7, 8);
                case 'Z':
                    return new Rectangle(24, 16, 7, 8);
                case 'a':
                    return new Rectangle(32, 16, 7, 8);
                case 'b':
                    return new Rectangle(40, 16, 7, 8);
                case 'c':
                    return new Rectangle(49, 16, 6, 8);
                case 'd':
                    return new Rectangle(56, 16, 6, 8);
                case 'e':
                    return new Rectangle(65, 16, 6, 8);
                case 'f':
                    return new Rectangle(73, 16, 5, 8);
                case 'g':
                    return new Rectangle(80, 16, 7, 8);
                case 'h':
                    return new Rectangle(88, 16, 7, 8);
                case 'i':
                    return new Rectangle(98, 16, 4, 8);
                case 'j':
                    return new Rectangle(105, 16, 4, 8);
                case 'k':
                    return new Rectangle(112, 16, 7, 8);
                case 'l':
                    return new Rectangle(122, 16, 4, 8);

                case 'm':
                    return new Rectangle(0, 24, 7, 8);
                case 'n':
                    return new Rectangle(8, 24, 7, 8);
                case 'o':
                    return new Rectangle(17, 24, 6, 8);
                case 'p':
                    return new Rectangle(24, 24, 7, 8);
                case 'q':
                    return new Rectangle(32, 24, 7, 8);
                case 'r':
                    return new Rectangle(41, 24, 6, 8);
                case 's':
                    return new Rectangle(49, 24, 6, 8);
                case 't':
                    return new Rectangle(57, 24, 5, 8);
                case 'u':
                    return new Rectangle(64, 24, 7, 8);
                case 'v':
                    return new Rectangle(72, 24, 7, 8);
                case 'w':
                    return new Rectangle(80, 24, 7, 8);
                case 'x':
                    return new Rectangle(88, 24, 7, 8);
                case 'y':
                    return new Rectangle(97, 24, 6, 8);
                case 'z':
                    return new Rectangle(105, 24, 6, 8);
                case '.':
                    return new Rectangle(112, 24, 7, 8);
                case ',':
                    return new Rectangle(120, 24, 7, 8);

                case '\'':
                    return new Rectangle(0, 32, 3, 8);
                case '!':
                    return new Rectangle(8, 32, 4, 8);
                case '?':
                    return new Rectangle(16, 32, 6, 8);
            }
            return new Rectangle(120, 32, 4,8);
        }
    }
}
