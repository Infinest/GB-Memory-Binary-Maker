using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GB_Memory
{
    public class ROM
    {
        public String Title,ASCIITitle,File;
        public Byte CartridgeType, ROMSize, RAMSize;
        public bool padded,CGB,embedded = false;
        public uint ROMPos = 0;

        public int ROMSizeKByte
        {
            get
            {
                return (32 << ROMSize);
            }
        }
        public int RAMSizeKByte
        {
            get
            {
                return (2 * (int)Math.Pow(4,RAMSize-1));
            }
        }
        public String CartridgeTypeString
        {
            get
            {
                switch(CartridgeType)
                {
                    case (0x0):
                        return "ROM Only";
                    case (0x1):
                        return "MBC1";
                    case (0x2):
                        return "MBC1+RAM";
                    case (0x3):
                        return "MBC1+RAM+BATTERY";
                    case (0x5):
                        return "MBC2";
                    case (0x6):
                        return "MBC2+BATTERY";
                    case (0x8):
                        return "ROM+RAM";
                    case (0x9):
                        return "ROM+RAM+BATTERY";
                    case (0xF):
                        return "MBC3+TIMER+BATTERY";
                    case (0x10):
                        return "MBC3+TIMER+RAM+BATTERY";
                    case (0x11):
                        return "MBC3";
                    case (0x12):
                        return "MBC3+RAM";
                    case (0x13):
                        return "MBC3+RAM+BATTERY";
                    case (0x19):
                        return "MBC5";
                    case (0x1A):
                        return "MBC5+RAM";
                    case (0x1B):
                        return "MBC5+RAM+BATTERY";
                    case (0x1C):
                        return "MBC5+RUMBLE";
                    case (0x1D):
                        return "MBC5+RUMBLE+RAM";
                    case (0x1E):
                        return "MBC5+RUMBLE+RAM+BATTERY";
                }
                return "Unsupported?";
            }
        }
    }
}
