# GB Memory Binary Maker

![N|Solid](https://i.imgur.com/3PbCNq5.jpg)

##### This program's main purpose is to generate binaries to burn to a GB Memory Cartridge (DMG-MMSA-JPN)

 It is able to:
 - Generate a 1024kB binary including the standard GB Memory Menu(128kB) and as many smaller ROMs as the space allows
 (ROMs smaller than 128kB will be padded) + a fitting MAP file to go with the binary
 - Generate a MAP file to burn a single ROM without menu to your cartridge. This is helpful if you want to use your GB Memory cart with a ROM that is 1024kB on it's own. It does also work with ROMs smaller than that though.
 - Parse ROMs from existing 1024kB GBM binaries and then rip them to .gb/c ROMs or generate a new binary with them
 
Please keep in mind that to generate a binary with the Menu, the program requires the 128kB menu binary in the program folder!

![N|Solid](https://i.imgur.com/f7v7qtc.png)
