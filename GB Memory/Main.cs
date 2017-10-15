using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GB_Memory
{
    public partial class Main : Form
    {
        const int ROMSpace = 896;
        int FreeROMSpace = 896;
        List<ROM> ROMList = new List<ROM>();
        public Main()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            String ToAdd = "";
            using (OpenFileDialog AddRom = new OpenFileDialog())
            {
                AddRom.Filter = "All GB/C ROMs|*.gb;*.gbc";
                if (AddRom.ShowDialog() == DialogResult.OK && File.Exists(AddRom.FileName))
                {
                    ToAdd = AddRom.FileName;
                }
            }
            if (ToAdd == "") return;
            ROM ROMToAdd = new ROM();
            TitleEntry ROMTitle = new TitleEntry();
            using (FileStream FileToAdd = new FileStream(ToAdd, FileMode.Open, FileAccess.Read))
            {
                FileToAdd.Position = 0x143;
                Byte tmp = (Byte)FileToAdd.ReadByte();
                if (tmp == 0x80 || tmp == 0xC0) ROMToAdd.CGB = true;
                FileToAdd.Position = 0x147;
                ROMToAdd.CartridgeType = (byte)FileToAdd.ReadByte();
                ROMToAdd.ROMSize = (byte)FileToAdd.ReadByte();
                ROMToAdd.RAMSize = (byte)FileToAdd.ReadByte();
                ROMToAdd.File = ToAdd;
                FileToAdd.Position = 0x134;
                Byte[] buffer = new Byte[0xF];
                FileToAdd.Read(buffer, 0, 0xF);
                ROMToAdd.ASCIITitle = System.Text.ASCIIEncoding.ASCII.GetString(buffer);
            }

            if (ROMToAdd.ROMSizeKByte < 128)
            {
                ROMToAdd.padded = true;
                ROMToAdd.ROMSize = 0x2;
            }

            if (ROMToAdd.ROMSizeKByte > FreeROMSpace)
            {
                MessageBox.Show("ROM size exceeds the free space left.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (ROMTitle.ShowDialog() == DialogResult.OK)
            {
                ROMToAdd.Title = ROMTitle.Title;
            }
            else return;

            Panel ROMPanel = new Panel();
            ROMPanel.Width = panel1.Width - 22;
            ROMPanel.Height = 90;
            ROMPanel.Location = new Point(1, 1 + panel1.Controls.Count * 91);
            ROMPanel.BorderStyle = BorderStyle.FixedSingle;

            Label T = new Label();
            T.Visible = false;
            T.Text = ROMToAdd.Title;
            ROMPanel.Controls.Add(T);

            Label AT = new Label();
            AT.Visible = false;
            AT.Text = ROMToAdd.ASCIITitle;
            ROMPanel.Controls.Add(AT);

            Label ASCIITitleDisplay = new Label();
            ASCIITitleDisplay.AutoSize = true;
            ASCIITitleDisplay.Text = "ASCII-Title: " + ROMToAdd.ASCIITitle;
            ASCIITitleDisplay.Location = new Point(5, 4);
            ROMPanel.Controls.Add(ASCIITitleDisplay);

            Label CartTypeDisplay = new Label();
            CartTypeDisplay.AutoSize = true;
            CartTypeDisplay.Text = "Cart-Type: " + ROMToAdd.CartridgeTypeString;
            CartTypeDisplay.Location = new Point(5, 36);
            ROMPanel.Controls.Add(CartTypeDisplay);

            Label TitleIMDisplay = new Label();
            TitleIMDisplay.AutoSize = true;
            TitleIMDisplay.Text = "Title:";
            TitleIMDisplay.Location = new Point(5, 19);
            ROMPanel.Controls.Add(TitleIMDisplay);

            PictureBox TitleIMG = new PictureBox();
            TitleIMG.Image = TitleImage.CreateTitleBitmap(ROMToAdd.Title);
            TitleIMG.SizeMode = PictureBoxSizeMode.AutoSize;
            TitleIMG.Location = new Point(35, 21);
            TitleIMG.BorderStyle = BorderStyle.FixedSingle;
            ROMPanel.Controls.Add(TitleIMG);

            Label ROMSizeDisplay = new Label();
            ROMSizeDisplay.AutoSize = true;
            ROMSizeDisplay.Text = "ROMSize: " + ROMToAdd.ROMSizeKByte + "kByte" + (ROMToAdd.padded ? " (padded)" : "");
            ROMSizeDisplay.Location = new Point(5, 53);
            ROMPanel.Controls.Add(ROMSizeDisplay);

            Label RAMSizeDisplay = new Label();
            RAMSizeDisplay.AutoSize = true;
            RAMSizeDisplay.Text = "RAMSize: " + ROMToAdd.RAMSizeKByte + "kByte";
            RAMSizeDisplay.Location = new Point(5, 70);
            ROMPanel.Controls.Add(RAMSizeDisplay);

            Button Remove = new Button();
            Remove.Text = "Remove";
            Remove.Location = new Point(300, 60);
            Remove.Click += (s, ev) =>
            {
                foreach (ROM R in ROMList)
                {
                    if (R.Title == ((Button)s).Parent.Controls[0].Text && R.ASCIITitle == ((Button)s).Parent.Controls[1].Text)
                    {
                        ROMList.Remove(R);
                        break;
                    }
                }
                panel1.Controls.Remove(((Button)s).Parent);
                updateROMSpace();
            };
            ROMPanel.Controls.Add(Remove);

            Button Edit = new Button();
            Edit.Text = "Edit Title";
            Edit.Location = new Point(300, 30);
            Edit.Click += (s, ev) =>
            {
                foreach (ROM R in ROMList)
                {
                    if (R.Title == ((Button)s).Parent.Controls[0].Text && R.ASCIITitle == ((Button)s).Parent.Controls[1].Text)
                    {
                        TitleEntry ROMEditTitle = new TitleEntry();
                        if (ROMEditTitle.ShowDialog() == DialogResult.OK)
                        {
                            R.Title = ROMEditTitle.Title;
                            ((PictureBox)(((Button)s).Parent.Controls[5])).Image = TitleImage.CreateTitleBitmap(R.Title);
                            (((Button)s).Parent.Controls[0]).Text = R.Title;
                        }
                    }
                }
            };
            ROMPanel.Controls.Add(Edit);

            panel1.Controls.Add(ROMPanel);
            ROMList.Add(ROMToAdd);
            updateROMSpace();
        }

        private void updateROMSpace()
        {
            int TakenSpace = 0;
            foreach (ROM R in ROMList)
            {
                TakenSpace += R.ROMSizeKByte;
            }
            FreeROMSpace = ROMSpace - TakenSpace;
            SpaceLabel.Text = String.Format("Free Space: {0}kByte", FreeROMSpace);
        }

        private void panel1_ControlRemoved(Object sender, ControlEventArgs e)
        {
            for (int i = 0; i < ((Panel)sender).Controls.Count; i++)
            {
                ((Panel)sender).Controls[i].Location = new Point(0, i * 91);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            updateROMSpace();
        }

        public bool CheckFolderPermissions(string folderPath)
        {
            var permissionSet = new PermissionSet(PermissionState.None);
            var writePermission = new FileIOPermission(FileIOPermissionAccess.Write, folderPath);
            permissionSet.AddPermission(writePermission);

            if (permissionSet.IsSubsetOf(AppDomain.CurrentDomain.PermissionSet))
                return true;
            else
                return false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (ROMList.Count < 1) return;
            Byte[] MenuBuffer;
            if (File.Exists(Application.StartupPath + @"\Menu.gbc"))
            {
                MenuBuffer = File.ReadAllBytes(Application.StartupPath + @"\Menu.gbc");
            }
            else if ((File.Exists(Application.StartupPath + @"\Menu.gb")))
            {
                MenuBuffer = File.ReadAllBytes(Application.StartupPath + @"\Menu.gb");
            }
            else
            {
                MessageBox.Show("Couldn't find Menu.gb or Menu.gbc", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Array.Resize(ref MenuBuffer, 1024 * 1024);

            String SavePath;
            using (SaveFileDialog ToSave = new SaveFileDialog())
            {
                //ToSave.Filter = "Binaries";
                if (ToSave.ShowDialog() != DialogResult.OK) return;
                if (!Directory.Exists(Path.GetDirectoryName(ToSave.FileName))) return;
                if (!CheckFolderPermissions(Path.GetDirectoryName(ToSave.FileName))) return;
                SavePath = ToSave.FileName;
            }

            using (MemoryStream Mem = new MemoryStream(MenuBuffer))
            {
                Mem.Position = 0x1C200;
                for (int i = 0; i < ROMList.Count; i++)
                {
                    //ROM Index
                    Mem.WriteByte((Byte)(i + 1));

                    //ROM Base (in 128K units)
                    int Base = 128;
                    for (int b = i - 1; b >= 0; b--)
                    {
                        Base += ROMList[b].ROMSizeKByte;
                    }
                    Mem.WriteByte((Byte)(Base / 128));

                    //Maybe SRAM base??? MUSS ÄNDERN!!!!!!!!!!
                    Mem.WriteByte(0x0);

                    //ROM size in 128Kbyte units (0001h..0007h = 128K..896K)
                    Mem.WriteByte((Byte)(ROMList[i].ROMSizeKByte / 128));
                    Mem.WriteByte(0x0);

                    //SRAM size in 32-byte units (0000h,00xxh,01xxh,xxxxh=0,MBC2,8K,32K)
                    Mem.WriteByte(0x0);
                    Mem.WriteByte(0x0);

                    //Title Title ASCII "DMG -xxxx-  "
                    Byte[] temp = new Byte[0];
                    if (!ROMList[i].CGB)
                    {
                        temp = System.Text.ASCIIEncoding.ASCII.GetBytes("DMG -A" + ROMList[i].ASCIITitle.Substring(0, 2) + "J-  ");
                    }
                    else
                    {
                        temp = System.Text.ASCIIEncoding.ASCII.GetBytes("CGB -A" + ROMList[i].ASCIITitle.Substring(0, 2) + "J-  ");
                    }

                    Mem.Write(temp, 0, temp.Length);



                    //Title SHIFT-JIS
                    temp = new Byte[] { 0x82, 0x50, 0x82, 0x54, 0x82, 0x61, 0x83, 0x7B, 0x83, 0x93, 0x83, 0x6F, 0x81, 0x5B, 0x83, 0x7D, 0x83, 0x93, 0x82, 0x66, 0x82, 0x61, 0x82, 0x52, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20 };
                    Mem.Write(temp, 0, temp.Length);

                    //Title Bitmap (128x8 pixels, 16 tiles at 2bpp)
                    temp = TitleImage.GB(TitleImage.CreateTitleBitmap(ROMList[i].Title));
                    Mem.Write(temp, 0, temp.Length);

                    //Zerofill
                    for (int b = 0; b < 0xC0; b++)
                    {
                        Mem.WriteByte(0x0);
                    }

                    //Date ASCII "MM/DD/YYYY" + Time ASCII"HH:MM:SS"
                    String temp1 = DateTime.Now.ToString(@"MM\/dd\/yyyyHH:mm:ss");
                    temp = System.Text.ASCIIEncoding.ASCII.GetBytes(temp1);
                    Mem.Write(temp, 0, temp.Length);

                    //LAW ASCII  "LAWnnnnn"
                    temp = System.Text.ASCIIEncoding.ASCII.GetBytes("LAW01780");
                    Mem.Write(temp, 0, temp.Length);

                    //Unused (FFh-filled)
                    for (int b = 0; b < 0x17; b++)
                    {
                        Mem.WriteByte(0xFF);
                    }

                    //Unused (FFh-filled)(game entries) or "MULTICARTRIDGE 8"(menu entry)
                    for (int b = 0; b < 0x10; b++)
                    {
                        Mem.WriteByte(0xFF);
                    }
                }

                //Write Game ROMs to Binary
                Mem.Position = 0x20000;
                long pos = 0;
                for (int i = 0; i < ROMList.Count; i++)
                {
                    pos = Mem.Position;
                    Byte[] temp = File.ReadAllBytes(ROMList[i].File);
                    Mem.Write(temp, 0, temp.Length);
                    Mem.Position = pos + ROMList[i].ROMSizeKByte * 1024;
                }
            }

            //Do MAP File
            Byte[] MAPBytes = new Byte[0x80];
            using (MemoryStream Mem = new MemoryStream(MAPBytes))
            {
                //Write MBC Type, ROM Size, and SRAM Size for Menu
                Byte[] temp = new Byte[] { 0xA8, 0x00, 0x00 };
                Mem.Write(temp, 0, temp.Length);

                int StartOffset = 128;
                int RAMStartOffset = 0;
                //Write MBC Type, ROM Size, and SRAM Size etc. for all ROMs
                for (int i = 0; i < ROMList.Count; i++)
                {
                    BitArray Bits = new BitArray(16);

                    //MBC Type
                    if (ROMList[i].CartridgeType >= 0x1 && ROMList[i].CartridgeType <= 0x3)
                    {
                        //MBC2
                        Bits[15] = false; Bits[14] = false; Bits[13] = true;
                    }
                    else if (ROMList[i].CartridgeType >= 0x5 && ROMList[i].CartridgeType <= 0x6)
                    {
                        //MBC2
                        Bits[15] = false; Bits[14] = true; Bits[13] = false;
                    }
                    else if (ROMList[i].CartridgeType >= 0xF && ROMList[i].CartridgeType <= 0x13)
                    {
                        //MBC3
                        Bits[15] = false; Bits[14] = true; Bits[13] = true;
                    }
                    else if (ROMList[i].CartridgeType >= 0x19 && ROMList[i].CartridgeType <= 0x1E)
                    {
                        //MBC5
                        Bits[15] = true; Bits[14] = false; Bits[13] = true;
                    }
                    else
                    {
                        //MBC0
                        Bits[15] = false; Bits[14] = false; Bits[13] = false;
                    }

                    //ROM Size
                    switch (ROMList[i].ROMSizeKByte)
                    {
                        case (128):
                            Bits[12] = false; Bits[11] = true; Bits[10] = false;
                            break;
                        case (256):
                            Bits[12] = false; Bits[11] = true; Bits[10] = true;
                            break;
                        case (512):
                            Bits[12] = true; Bits[11] = false; Bits[10] = false;
                            break;
                        case (1024):
                            Bits[12] = true; Bits[11] = false; Bits[10] = true;
                            break;
                    }

                    //SRAM Size
                    if (ROMList[i].RAMSizeKByte == 0)
                    {
                        //No SRAM
                        Bits[9] = false; Bits[8] = false; Bits[7] = false;
                    }
                    else if (ROMList[i].CartridgeType >= 0x5 && ROMList[i].CartridgeType <= 0x6)
                    {
                        //MBC2
                        Bits[9] = false; Bits[8] = false; Bits[7] = true;
                    }
                    else if (ROMList[i].RAMSizeKByte == 8)
                    {
                        //8KB
                        Bits[9] = false; Bits[8] = true; Bits[7] = false;
                    }
                    else if (ROMList[i].RAMSizeKByte == 32)
                    {
                        //32KB
                        Bits[9] = false; Bits[8] = true; Bits[7] = true;
                    }
                    else
                    {
                        //8KB For stuff that has smaller than 8KB
                        Bits[9] = false; Bits[8] = true; Bits[7] = false;
                    }

                    //ROM Startoffset
                    Byte StartOffsetByte = (Byte)(StartOffset / 32);
                    StartOffset += ROMList[i].ROMSizeKByte;

                    temp = new Byte[2];
                    Bits.CopyTo(temp, 0);
                    Array.Reverse(temp);
                    // Or together ROM Startoffset together with last bit of SRAM Size
                    temp[1] = (byte)(StartOffsetByte | temp[1]);
                    Mem.Write(temp, 0, temp.Length);

                    //RAM Offset
                    Mem.WriteByte((Byte)(RAMStartOffset / 2));
                    RAMStartOffset += ROMList[i].RAMSizeKByte;
                }

                while (Mem.Position < 0x6E)
                {
                    Mem.WriteByte(0xFF);
                }

                //?? No idea what this does
                temp = new Byte[] { 0x02, 0x00, 0x30, 0x12, 0x99, 0x11, 0x12, 0x20, 0x37, 0x57, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00 };
                Mem.Write(temp, 0, temp.Length);
            }

            String[] FilesToCreate = new String[2];
            FilesToCreate[0] = Path.GetDirectoryName(SavePath) + @"\" + Path.GetFileName(SavePath) + ".gb";
            FilesToCreate[1] = Path.GetDirectoryName(SavePath) + @"\" + Path.GetFileName(SavePath) + ".map";
            File.WriteAllBytes(FilesToCreate[0], MenuBuffer);
            File.WriteAllBytes(FilesToCreate[1], MAPBytes);

            MessageBox.Show("Created:" + Environment.NewLine + FilesToCreate[0] + Environment.NewLine + FilesToCreate[1]);

            //File.WriteAllBytes(Application.StartupPath + @"\output.gb", MenuBuffer);
            //File.WriteAllBytes(Application.StartupPath + @"\output.map", MAPBytes);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            String ToAdd = "";
            using (OpenFileDialog AddRom = new OpenFileDialog())
            {
                AddRom.Filter = "All GB/C ROMs|*.gb;*.gbc";
                if (AddRom.ShowDialog() == DialogResult.OK && File.Exists(AddRom.FileName))
                {
                    ToAdd = AddRom.FileName;
                }
            }
            if (ToAdd == "") return;
            if ((new FileInfo(ToAdd).Length / 1024) > 1024)
            {
                MessageBox.Show("ROM size exceeds the maximum of 1024kByte.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ROM ROMToAdd = new ROM();
            using (FileStream FileToAdd = new FileStream(ToAdd, FileMode.Open, FileAccess.Read))
            {
                FileToAdd.Position = 0x143;
                Byte tmp = (Byte)FileToAdd.ReadByte();
                if (tmp == 0x80 || tmp == 0xC0) ROMToAdd.CGB = true;
                FileToAdd.Position = 0x147;
                ROMToAdd.CartridgeType = (byte)FileToAdd.ReadByte();
                ROMToAdd.ROMSize = (byte)FileToAdd.ReadByte();
                ROMToAdd.RAMSize = (byte)FileToAdd.ReadByte();
                ROMToAdd.File = ToAdd;
                FileToAdd.Position = 0x134;
                Byte[] buffer = new Byte[0xF];
                FileToAdd.Read(buffer, 0, 0xF);
                ROMToAdd.ASCIITitle = System.Text.ASCIIEncoding.ASCII.GetString(buffer);
            }

            Byte[] MAPBytes = new Byte[0x80];
            using (MemoryStream Mem = new MemoryStream(MAPBytes))
            {
                //Write MBC Type, ROM Size, and SRAM Size etc. for ROM
                BitArray Bits = new BitArray(16);

                //MBC Type
                if (ROMToAdd.CartridgeType >= 0x1 && ROMToAdd.CartridgeType <= 0x3)
                {
                    //MBC2
                    Bits[15] = false; Bits[14] = false; Bits[13] = true;
                }
                else if (ROMToAdd.CartridgeType >= 0x5 && ROMToAdd.CartridgeType <= 0x6)
                {
                    //MBC2
                    Bits[15] = false; Bits[14] = true; Bits[13] = false;
                }
                else if (ROMToAdd.CartridgeType >= 0xF && ROMToAdd.CartridgeType <= 0x13)
                {
                    //MBC3
                    Bits[15] = false; Bits[14] = true; Bits[13] = true;
                }
                else if (ROMToAdd.CartridgeType >= 0x19 && ROMToAdd.CartridgeType <= 0x1E)
                {
                    //MBC5
                    Bits[15] = true; Bits[14] = false; Bits[13] = true;
                }
                else
                {
                    //MBC0
                    Bits[15] = false; Bits[14] = false; Bits[13] = false;
                }

                //ROM Size
                switch (ROMToAdd.ROMSizeKByte)
                {
                    case (128):
                        Bits[12] = false; Bits[11] = true; Bits[10] = false;
                        break;
                    case (256):
                        Bits[12] = false; Bits[11] = true; Bits[10] = true;
                        break;
                    case (512):
                        Bits[12] = true; Bits[11] = false; Bits[10] = false;
                        break;
                    case (1024):
                        Bits[12] = true; Bits[11] = false; Bits[10] = true;
                        break;
                }

                //SRAM Size
                if (ROMToAdd.RAMSizeKByte == 0)
                {
                    //No SRAM
                    Bits[9] = false; Bits[8] = false; Bits[7] = false;
                }
                else if (ROMToAdd.CartridgeType >= 0x5 && ROMToAdd.CartridgeType <= 0x6)
                {
                    //MBC2
                    Bits[9] = false; Bits[8] = false; Bits[7] = true;
                }
                else if (ROMToAdd.RAMSizeKByte == 8)
                {
                    //8KB
                    Bits[9] = false; Bits[8] = true; Bits[7] = false;
                }
                else if (ROMToAdd.RAMSizeKByte == 32)
                {
                    //32KB
                    Bits[9] = false; Bits[8] = true; Bits[7] = true;
                }
                else
                {
                    //8KB For stuff that has smaller than 8KB
                    Bits[9] = false; Bits[8] = true; Bits[7] = false;
                }

                Byte[] temp = new Byte[2];
                Bits.CopyTo(temp, 0);
                Array.Reverse(temp);
                // Or together ROM Startoffset (in this case always 0x0) together with last bit of SRAM Size
                temp[1] = (byte)(0x0 | temp[1]);
                Mem.Write(temp, 0, temp.Length);

                //RAM Offset (In this case always 0x0)
                Mem.WriteByte(0x0);

                //FF Fill
                while (Mem.Position < 0x18)
                {
                    Mem.WriteByte(0xFF);
                }

                //?? Not sure what this is
                temp = new Byte[] { 0x08, 0x00, 0x40, 0x00 };
                Mem.Write(temp, 0, temp.Length);

                //Title Title ASCII "DMG -xxxx-  "
                if (!ROMToAdd.CGB)
                {
                    temp = System.Text.ASCIIEncoding.ASCII.GetBytes("DMG -A" + ROMToAdd.ASCIITitle.Substring(0, 2) + "J-  ");
                }
                else
                {
                    temp = System.Text.ASCIIEncoding.ASCII.GetBytes("CGB -A" + ROMToAdd.ASCIITitle.Substring(0, 2) + "J-  ");
                }
                Mem.Write(temp, 0, temp.Length);

                //Title SHIFT-JIS
                temp = new Byte[] { 0x82, 0x4F,0x82,0x57,0x82,0x60,0x83,0x58,0x81,0x5B,0x83,0x70,0x81,0x5B,0x83,0x7D,0x83,0x8A,0x83,0x49,0x83,0x75,0x83,0x89,0x83,0x55,0x81,0x5B,0x83,0x59,0x83,0x66,0x83,0x89,0x83,0x62,0x83,0x4E,0x83,0x58,0x20,0x20,0x20,0x20};
                Mem.Write(temp, 0, temp.Length);

                //Date ASCII "MM/DD/YYYY" + Time ASCII"HH:MM:SS"
                String temp1 = DateTime.Now.ToString(@"MM\/dd\/yyyyHH:mm:ss");
                temp = System.Text.ASCIIEncoding.ASCII.GetBytes(temp1);
                Mem.Write(temp, 0, temp.Length);

                //LAW ASCII  "LAWnnnnn"
                temp = System.Text.ASCIIEncoding.ASCII.GetBytes("LAW03347");
                Mem.Write(temp, 0, temp.Length);

                //?? No idea what this does
                temp = new Byte[] { 0x01, 0x00, 0x30, 0x25, 0x00, 0x03, 0x01, 0x00, 0x12, 0x57, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00 };
                Mem.Write(temp, 0, temp.Length);
            }
            if (!CheckFolderPermissions(Path.GetDirectoryName(ROMToAdd.File))) return;
            String FileToCreate = Path.GetDirectoryName(ROMToAdd.File) + @"\" + Path.GetFileNameWithoutExtension(ROMToAdd.File) + ".map";
            File.WriteAllBytes(FileToCreate, MAPBytes);
            MessageBox.Show("Created:" + Environment.NewLine + FileToCreate);
        }
    }
}
