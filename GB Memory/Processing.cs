using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GB_Memory
{
    public static class Processing
    {
        public static ROM ParseROM(MemoryStream Data, String File = null)
        {
            ROM Output = new ROM();
            Data.Position = 0x143;
            Byte tmp = (Byte)Data.ReadByte();
            if (tmp == 0x80 || tmp == 0xC0) Output.CGB = true;
            Data.Position = 0x147;
            Output.CartridgeType = (byte)Data.ReadByte();
            Output.ROMSize = (byte)Data.ReadByte();
            Output.RAMSize = (byte)Data.ReadByte();
            Data.Position = 0x134;
            Byte[] buffer = new Byte[0xF];
            Data.Read(buffer, 0, 0xF);
            Output.ASCIITitle = System.Text.ASCIIEncoding.ASCII.GetString(buffer);
            if (Output.ROMSizeKByte < 128)
            {
                Output.padded = true;
            }
            if (!String.IsNullOrEmpty(File))
            {
                Output.File = File;
            }
            Data.Dispose();
            return Output;
        }

        public static List<ROM> ParseGBMBinary(String ToImport)
        {
            List<ROM> ROMsToAdd;
            using (FileStream Reader = new FileStream(ToImport, FileMode.Open, FileAccess.Read))
            {
                Reader.Position = 0x134;
                Byte[] temp = new Byte[15], temp1 = new Byte[0x10];

                //ROM ASCII title
                Reader.Read(temp, 0, 15);

                //Info entry header for Menu
                Reader.Position = 0x1C000;
                Reader.Read(temp1, 0, 0x10);
                ROMsToAdd = new List<ROM>();
                if (Encoding.ASCII.GetString(temp) != "NP M-MENU  MENU" || !temp1.SequenceEqual(new Byte[] { 0x0, 0x0, 0x0, 0x1, 0x0, 0x0, 0x0, 0x44, 0x4D, 0x47, 0x20, 0x2D, 0x4D, 0x45, 0x4E, 0x55 }))
                {
                    MessageBox.Show(String.Format("File {1}{0}{1} is not a valid GBM binary", Path.GetFileName(ToImport), '"'), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return ROMsToAdd;
                }
                Reader.Position = 0x1C200;
                Byte NextGameIndex = (Byte)Reader.ReadByte();
                ROM Temp = new ROM();
                //TitleEntry ROMTitle;

                //Read info for each game written to the menu binary into ROM class and add all to the ROMsToAdd list until end is reached
                while (NextGameIndex != 0 && NextGameIndex != 0xFF)
                {
                    int Base = (Reader.ReadByte() - 1) * 128;

                    Reader.Position = 0x20000 + (Base * 1024) + 0x148;
                    int Size = (32 << Reader.ReadByte());
                    Reader.Position = 0x20000 + Base * 1024;
                    temp = new Byte[Size * 1024];
                    Reader.Read(temp, 0, Size * 1024);
                    using (MemoryStream Mem = new MemoryStream(temp))
                    {
                        Temp = ParseROM(Mem);
                    }
                    Temp.ROMPos = (uint)(0x20000 + Base * 1024);
                    Temp.embedded = true;
                    Temp.File = ToImport;

                    //ROMTitle = new TitleEntry(Temp.ASCIITitle);
                    //if (ROMTitle.ShowDialog() == DialogResult.OK)
                    //{
                    //    Temp.Title = ROMTitle.Title;
                    //}
                    ROMsToAdd.Add(Temp);

                    Reader.Position = 0x1C200 + 0x200 * ROMsToAdd.Count;
                    NextGameIndex = (Byte)Reader.ReadByte();
                    Temp = new ROM();
                }
            }
            return ROMsToAdd;
        }

        public static Byte[] GenerateStandaloneMAPForROM(ROM ROMToProcess)
        {
            Byte[] MAPBytes = new Byte[0x80];
            using (MemoryStream Mem = new MemoryStream(MAPBytes))
            {
                //Write MBC Type, ROM Size, and SRAM Size etc. for ROM
                BitArray Bits = new BitArray(16);

                //MBC Type
                if (ROMToProcess.CartridgeType >= 0x1 && ROMToProcess.CartridgeType <= 0x3)
                {
                    //MBC1
                    Bits[15] = false; Bits[14] = false; Bits[13] = true;
                }
                else if (ROMToProcess.CartridgeType >= 0x5 && ROMToProcess.CartridgeType <= 0x6)
                {
                    //MBC2
                    Bits[15] = false; Bits[14] = true; Bits[13] = false;
                }
                else if (ROMToProcess.CartridgeType >= 0xF && ROMToProcess.CartridgeType <= 0x13)
                {
                    //MBC3
                    Bits[15] = false; Bits[14] = true; Bits[13] = true;
                }
                else if (ROMToProcess.CartridgeType >= 0x19 && ROMToProcess.CartridgeType <= 0x1E)
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
                switch (ROMToProcess.ROMSizeKByte)
                {
                    case (64):
                        Bits[12] = false; Bits[11] = true; Bits[10] = false;
                        break;
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
                if (ROMToProcess.CartridgeType == 0x6)
                {
                    //MBC2+BATTERY
                    Bits[9] = false; Bits[8] = false; Bits[7] = true;
                }
                else if (ROMToProcess.RAMSizeKByte == 0)
                {
                    //No SRAM
                    Bits[9] = false; Bits[8] = false; Bits[7] = false;
                }
                else if (ROMToProcess.RAMSizeKByte == 8)
                {
                    //8KB
                    Bits[9] = false; Bits[8] = true; Bits[7] = false;
                }
                else if (ROMToProcess.RAMSizeKByte >= 32)
                {
                    //32KB or bigger (but we can use <= 4 blocks (32KB) of SRAM per one ROM)
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
                // Or ROM Startoffset (in this case always 0x0) together with last bit of SRAM Size
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
                if (!ROMToProcess.CGB)
                {
                    temp = System.Text.ASCIIEncoding.ASCII.GetBytes("DMG -A" + ROMToProcess.ASCIITitle.Substring(0, 2) + "J-  ");
                }
                else
                {
                    temp = System.Text.ASCIIEncoding.ASCII.GetBytes("CGB -A" + ROMToProcess.ASCIITitle.Substring(0, 2) + "J-  ");
                }
                Mem.Write(temp, 0, temp.Length);

                //Title SHIFT-JIS
                //temp = new Byte[] { 0x82, 0x50, 0x82, 0x54, 0x82, 0x61, 0x83, 0x7B, 0x83, 0x93, 0x83, 0x6F, 0x81, 0x5B, 0x83, 0x7D, 0x83, 0x93, 0x82, 0x66, 0x82, 0x61, 0x82, 0x52, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20 };
                Encoding ShiftJIS = Encoding.GetEncoding(932);
                String temp1 = "これを読めば、あなたはばかだ";
                temp1 = temp1.PadRight(30, ' ');
                temp = ShiftJIS.GetBytes(temp1);
                Mem.Write(temp, 0, temp.Length);

                //Date ASCII "MM/DD/YYYY" + Time ASCII"HH:MM:SS"
                temp1 = DateTime.Now.ToString(@"MM\/dd\/yyyyHH:mm:ss");
                temp = System.Text.ASCIIEncoding.ASCII.GetBytes(temp1);
                Mem.Write(temp, 0, temp.Length);

                //LAW ASCII  "LAWnnnnn"
                temp = System.Text.ASCIIEncoding.ASCII.GetBytes("LAW03347");
                Mem.Write(temp, 0, temp.Length);

                //?? No idea what this does
                temp = new Byte[] { 0x01, 0x00, 0x30, 0x25, 0x00, 0x03, 0x01, 0x00, 0x12, 0x57, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00 };
                Mem.Write(temp, 0, temp.Length);
            }
            return MAPBytes;
        }

        public static Byte[] GenerateMAPForMenuBinary(List<ROM> ROMList)
        {
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
                        //MBC1
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
                        case (64):
                            Bits[12] = false; Bits[11] = true; Bits[10] = false;
                            break;
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
                    if (ROMList[i].CartridgeType == 0x6)
                    {
                        //MBC2+BATTERY
                        Bits[9] = false; Bits[8] = false; Bits[7] = true;
                    }
                    else if (ROMList[i].RAMSizeKByte == 0)
                    {
                        //No SRAM
                        Bits[9] = false; Bits[8] = false; Bits[7] = false;
                    }
                    else if (ROMList[i].RAMSizeKByte == 8)
                    {
                        //8KB
                        Bits[9] = false; Bits[8] = true; Bits[7] = false;
                    }
                    else if (ROMList[i].RAMSizeKByte >= 32)
                    {
                        //32KB or bigger (but we can use <= 4 blocks (32KB) of SRAM per one ROM)
                        Bits[9] = false; Bits[8] = true; Bits[7] = true;
                    }
                    else
                    {
                        //8KB For stuff that has smaller than 8KB
                        Bits[9] = false; Bits[8] = true; Bits[7] = false;
                    }

                    //ROM Startoffset
                    Byte StartOffsetByte = (Byte)(StartOffset / 32);
                    StartOffset += ((ROMList[i].ROMSizeKByte < 128) ? 128 : ROMList[i].ROMSizeKByte);

                    temp = new Byte[2];
                    Bits.CopyTo(temp, 0);
                    Array.Reverse(temp);
                    // Or ROM Startoffset together with last bit of SRAM Size
                    temp[1] = (byte)(StartOffsetByte | temp[1]);
                    Mem.Write(temp, 0, temp.Length);

                    //RAM Offset
                    Mem.WriteByte((Byte)(RAMStartOffset / 2));
                    RAMStartOffset += ((ROMList[i].CartridgeType == 0x6 || ROMList[i].RAMSizeKByte < 8) ? 8 : ROMList[i].RAMSizeKByte);
                }

                while (Mem.Position < 0x6E)
                {
                    Mem.WriteByte(0xFF);
                }

                //?? No idea what this does
                temp = new Byte[] { 0x02, 0x00, 0x30, 0x12, 0x99, 0x11, 0x12, 0x20, 0x37, 0x57, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00 };
                Mem.Write(temp, 0, temp.Length);
            }
            return MAPBytes;
        }

        public static void CreateMenuBinary(List<ROM> ROMList, ref Byte[] Template)
        {
            using (MemoryStream Mem = new MemoryStream(Template))
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
                        Base += ((ROMList[b].ROMSizeKByte < 128) ? 128 : ROMList[b].ROMSizeKByte);
                    }
                    Mem.WriteByte((Byte)(Base / 128));

                    //Maybe SRAM base???
                    Mem.WriteByte(0x0);

                    //ROM size in 128Kbyte units (0001h..0007h = 128K..896K)
                    Mem.WriteByte((Byte)(((ROMList[i].ROMSizeKByte < 128) ? 128 : ROMList[i].ROMSizeKByte) / 128));
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
                    //temp = new Byte[] { 0x82, 0x50, 0x82, 0x54, 0x82, 0x61, 0x83, 0x7B, 0x83, 0x93, 0x83, 0x6F, 0x81, 0x5B, 0x83, 0x7D, 0x83, 0x93, 0x82, 0x66, 0x82, 0x61, 0x82, 0x52, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20 };
                    Encoding ShiftJIS = Encoding.GetEncoding(932);
                    String temp1 = "これを読めば、あなたはばかだ";
                    temp1 = temp1.PadRight(30,' ');
                    temp = ShiftJIS.GetBytes(temp1);
                    Mem.Write(temp, 0, temp.Length);

                    //Title Bitmap (128x8 pixels, 16 tiles at 2bpp)
                    temp = TitleImage.GB(TitleImage.CreateTitleBitmap(ROMList[i]));
                    Mem.Write(temp, 0, temp.Length);

                    //Zerofill
                    for (int b = 0; b < 0xC0; b++)
                    {
                        Mem.WriteByte(0x0);
                    }

                    //Date ASCII "MM/DD/YYYY" + Time ASCII"HH:MM:SS"
                    temp1 = DateTime.Now.ToString(@"MM\/dd\/yyyyHH:mm:ss");
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
                    Byte[] temp = new Byte[0];
                    if (!ROMList[i].embedded)
                    {
                        temp = File.ReadAllBytes(ROMList[i].File);
                    }
                    else
                    {
                        using (FileStream Reader = new FileStream(ROMList[i].File, FileMode.Open, FileAccess.Read))
                        {
                            Reader.Position = ROMList[i].ROMPos;
                            temp = new Byte[ROMList[i].ROMSizeKByte * 1024];
                            Reader.Read(temp, 0, ROMList[i].ROMSizeKByte * 1024);
                        }
                    }
                    Mem.Write(temp, 0, ROMList[i].ROMSizeKByte * 1024);
                    Mem.Position = pos + ((ROMList[i].ROMSizeKByte < 128) ? 128 : ROMList[i].ROMSizeKByte) * 1024;
                }
            }
        }

    }
}
