using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Windows.Forms;

namespace GB_Memory
{
    public partial class Main : Form
    {
        const int ROMSpace = 896;
        int FreeROMSpace = ROMSpace;
        List<ROM> ROMList = new List<ROM>();
        TickerImage Ticker = null;
        public Main()
        {
            InitializeComponent();
        }



        private void AddROMToInterface(ROM ROMToAdd)
        {

            //Create new sub-panel for ROM
            Panel ROMPanel = new Panel();
            ROMPanel.Width = ROMListPanel.Width - 22;
            ROMPanel.Height = 90;
            ROMPanel.Location = new Point(1, 1 + ROMListPanel.Controls.Count * 91);
            ROMPanel.BorderStyle = BorderStyle.FixedSingle;

            //Invisible that contains the entered title in String format for later
            Label T = new Label();
            T.Visible = false;
            T.Text = ROMToAdd.Title;
            ROMPanel.Controls.Add(T);

            //Invisible that contains just the ASCII-title in String format for later
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
            TitleIMG.Image = TitleImage.CreateTitleBitmap(ROMToAdd);
            TitleIMG.SizeMode = PictureBoxSizeMode.AutoSize;
            TitleIMG.Location = new Point(35, 21);
            TitleIMG.BorderStyle = BorderStyle.FixedSingle;
            ROMPanel.Controls.Add(TitleIMG);

            Label ROMSizeDisplay = new Label();
            ROMSizeDisplay.AutoSize = true;
            ROMSizeDisplay.Text = "ROMSize: " + ROMToAdd.ROMSizeKByte + "kByte" + (ROMToAdd.padded ? " (padded to 128kByte)" : "");
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
                ROMListPanel.Controls.Remove(((Button)s).Parent);
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
                            R.UseTrueTypeFontForTitleImage = ROMEditTitle.UseTrueTypeFont;
                            ((PictureBox)(((Button)s).Parent.Controls[5])).Image = TitleImage.CreateTitleBitmap(R);
                            (((Button)s).Parent.Controls[0]).Text = R.Title;
                        }
                        break;
                    }
                }
            };
            ROMPanel.Controls.Add(Edit);

            if (ROMToAdd.embedded)
            {
                PictureBox Rip = new PictureBox();
                Rip.Location = new Point(382, 1);
                Rip.SizeMode = PictureBoxSizeMode.AutoSize;
                Rip.Cursor = Cursors.Hand;
                Rip.Image = GB_Memory.Properties.Resources.icon_save;
                Rip.Click += (s, ev) =>
                {
                    if (MessageBox.Show("Rip ROM from binary?", "Rip ROM", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        foreach (ROM R in ROMList)
                        {
                            if (R.Title == ((PictureBox)s).Parent.Controls[0].Text && R.ASCIITitle == ((PictureBox)s).Parent.Controls[1].Text && R.embedded)
                            {
                                if (File.Exists(R.File))
                                {
                                    Byte[] buffer = new Byte[R.ROMSizeKByte * 1024];
                                    using (FileStream Reader = new FileStream(R.File, FileMode.Open, FileAccess.Read))
                                    {
                                        Reader.Position = R.ROMPos;
                                        Reader.Read(buffer, 0, R.ROMSizeKByte * 1024);
                                    }
                                    using (SaveFileDialog ToSave = new SaveFileDialog())
                                    {
                                        ToSave.FileName = R.ASCIITitle;
                                        ToSave.Filter = R.CGB ? "GBC ROM|*.gbc" : "GB ROM|*.gb";
                                        if (ToSave.ShowDialog() != DialogResult.OK) return;
                                        if (!Directory.Exists(Path.GetDirectoryName(ToSave.FileName))) return;
                                        if (!CheckFolderPermissions(Path.GetDirectoryName(ToSave.FileName))) return;
                                        File.WriteAllBytes(ToSave.FileName, buffer);
                                    }
                                }
                                break;
                            }
                        }
                    }
                };
                ROMPanel.Controls.Add(Rip);
            }

            ROMListPanel.Controls.Add(ROMPanel);
            ROMList.Add(ROMToAdd);
            updateROMSpace();
        }

        private void AddButtonClick(object sender, EventArgs e)
        {
            using (OpenFileDialog AddRom = new OpenFileDialog())
            {
                AddRom.Filter = "All GB/C ROMs|*.gb;*.gbc";
                AddRom.Multiselect = true;
                if (AddRom.ShowDialog() == DialogResult.OK)
                {
                    List<ROM> ROMsToAdd = new List<ROM>();
                    foreach (String BinaryFile in AddRom.FileNames)
                    {
                        if (!File.Exists(BinaryFile))
                        {
                            continue;
                        }

                        using (FileStream FileToAdd = new FileStream(BinaryFile, FileMode.Open, FileAccess.Read))
                        {
                            using (MemoryStream Mem = new MemoryStream())
                            {
                                FileToAdd.CopyTo(Mem);
                                ROMsToAdd.Add(Processing.ParseROM(Mem, BinaryFile));
                            }
                        }
                    }
                    foreach (ROM R in ROMsToAdd)
                    {
                        TitleEntry ROMTitle = new TitleEntry(R.ASCIITitle);
                        if (ROMTitle.ShowDialog() == DialogResult.OK)
                        {
                            R.Title = ROMTitle.Title;
                            R.UseTrueTypeFontForTitleImage = ROMTitle.UseTrueTypeFont;
                        }
                        else return;
                        AddROMToInterface(R);
                    }
                }
            }
        }

        private void updateROMSpace()
        {
            int TakenSpace = 0;
            foreach (ROM R in ROMList)
            {
                TakenSpace += (!R.padded) ? R.ROMSizeKByte : 128;
            }
            FreeROMSpace = ROMSpace - TakenSpace;
            SpaceLabel.Text = String.Format("Free Space: {0}kByte", FreeROMSpace);
            if (0 <= FreeROMSpace) { SpaceLabel.ForeColor = Color.FromArgb(255, 0, 0, 0); }
            else { SpaceLabel.ForeColor = Color.FromArgb(255, 255, 0, 0); }
        }

        private void ROMListPanel_ControlRemoved(Object sender, ControlEventArgs e)
        {
            for (int i = 0; i < ((Panel)sender).Controls.Count; i++)
            {
                ((Panel)sender).Controls[i].Location = new Point(1, i * 91 + (((Panel)sender).AutoScrollPosition.Y + 1));
            }
        }

        private void Main_Load(object sender, EventArgs e)
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

        private void CreateBinariesButtonClick(object sender, EventArgs e)
        {
            if (ROMList.Count < 1) return;

            if (FreeROMSpace < 0)
            {
                MessageBox.Show("Your ROMs exceed the 896kByte of free space!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

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
                if (ToSave.ShowDialog() != DialogResult.OK) return;
                if (!Directory.Exists(Path.GetDirectoryName(ToSave.FileName))) return;
                if (!CheckFolderPermissions(Path.GetDirectoryName(ToSave.FileName))) return;
                SavePath = ToSave.FileName;
            }

            Processing.CreateMenuBinary(ROMList, Ticker, ref MenuBuffer);

            Byte[] MAPBytes = Processing.GenerateMAPForMenuBinary(ROMList);

            String[] FilesToCreate = new String[2];
            FilesToCreate[0] = Path.GetDirectoryName(SavePath) + @"\" + Path.GetFileName(SavePath) + ".gb";
            FilesToCreate[1] = Path.GetDirectoryName(SavePath) + @"\" + Path.GetFileName(SavePath) + ".map";
            File.WriteAllBytes(FilesToCreate[0], MenuBuffer);
            File.WriteAllBytes(FilesToCreate[1], MAPBytes);

            MessageBox.Show("Created:" + Environment.NewLine + FilesToCreate[0] + Environment.NewLine + FilesToCreate[1]);
        }

        private void MAPButtonClick(object sender, EventArgs e)
        {
            using (OpenFileDialog GenerateFor = new OpenFileDialog())
            {
                GenerateFor.Filter = "All GB/C ROMs|*.gb;*.gbc";
                if (GenerateFor.ShowDialog() == DialogResult.OK && File.Exists(GenerateFor.FileName))
                {
                    if ((new FileInfo(GenerateFor.FileName).Length / 1024) > 1024)
                    {
                        MessageBox.Show("ROM size exceeds the maximum of 1024kByte.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    ROM ROMToProcess = new ROM();
                    using (FileStream FileToAdd = new FileStream(GenerateFor.FileName, FileMode.Open, FileAccess.Read))
                    {
                        using (MemoryStream Mem = new MemoryStream())
                        {
                            FileToAdd.CopyTo(Mem);
                            ROMToProcess = Processing.ParseROM(Mem, GenerateFor.FileName);
                        }
                    }

                    Byte[] MAPBytes = Processing.GenerateStandaloneMAPForROM(ROMToProcess);

                    if (!CheckFolderPermissions(Path.GetDirectoryName(ROMToProcess.File))) return;
                    String FileToCreate = Path.GetDirectoryName(ROMToProcess.File) + @"\" + Path.GetFileNameWithoutExtension(ROMToProcess.File) + ".map";
                    File.WriteAllBytes(FileToCreate, MAPBytes);
                    MessageBox.Show("Created:" + Environment.NewLine + FileToCreate);
                }
            }
        }

        private void ImportButtonClick(object sender, EventArgs e)
        {
            using (OpenFileDialog ImportMenu = new OpenFileDialog())
            {
                ImportMenu.Filter = "1024kB GBM binary|*.gb;*.gbc;*.bin";
                ImportMenu.Multiselect = true;
                if (ImportMenu.ShowDialog() == DialogResult.OK)
                {
                    List<ROM> ROMsToAdd = new List<ROM>();
                    foreach (String BinaryFile in ImportMenu.FileNames)
                    {
                        if (!File.Exists(BinaryFile))
                        {
                            continue;
                        }
                        if (new FileInfo(BinaryFile).Length / 1024 != 1024)
                        {
                            MessageBox.Show(String.Format("GBM binary {1}{0}{1} has to be 1024kByte in size!", Path.GetFileName(BinaryFile), '"'), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            continue;
                        }

                        ROMsToAdd.AddRange(Processing.ParseGBMBinary(BinaryFile));
                    }

                    foreach (ROM R in ROMsToAdd)
                    {
                        TitleEntry ROMTitle = new TitleEntry(R.ASCIITitle);
                        if (ROMTitle.ShowDialog() == DialogResult.OK)
                        {
                            R.Title = ROMTitle.Title;
                            R.UseTrueTypeFontForTitleImage = ROMTitle.UseTrueTypeFont;
                        }
                        else return;
                        AddROMToInterface(R);
                    }
                }
            }
        }

        private void FileDrop(object sender, DragEventArgs e)
        {
            String[] FileTypes = new String[] { ".gb", ".gbc", ".bin" };
            String[] Files = (String[])e.Data.GetData(DataFormats.FileDrop);
            List<ROM> ROMsToAdd = new List<ROM>();
            foreach (String FileToProcess in Files)
            {
                if (File.Exists(FileToProcess) && FileTypes.Contains(Path.GetExtension(FileToProcess)))
                {
                    Byte[] temp = new Byte[15], temp1 = new Byte[0x10];
                    using (FileStream Reader = new FileStream(FileToProcess, FileMode.Open, FileAccess.Read))
                    {
                        Reader.Position = 0x134;
                        //ROM ASCII title
                        Reader.Read(temp, 0, 15);
                        //Info entry header for Menu
                        Reader.Position = 0x1C000;
                        Reader.Read(temp1, 0, 0x10);
                        if (Encoding.ASCII.GetString(temp) == "NP M-MENU  MENU" && temp1.SequenceEqual(new Byte[] { 0x0, 0x0, 0x0, 0x1, 0x0, 0x0, 0x0, 0x44, 0x4D, 0x47, 0x20, 0x2D, 0x4D, 0x45, 0x4E, 0x55 }))
                        {
                            ROMsToAdd.AddRange(Processing.ParseGBMBinary(FileToProcess));
                        }
                        else if (Path.GetExtension(FileToProcess) != ".bin")
                        {
                            using (MemoryStream Mem = new MemoryStream())
                            {
                                Reader.Position = 0;
                                Reader.CopyTo(Mem);;
                                ROMsToAdd.Add(Processing.ParseROM(Mem, FileToProcess));
                            }
                        }
                    }
                }
            }

            foreach (ROM R in ROMsToAdd)
            {
                TitleEntry ROMTitle = new TitleEntry(R.ASCIITitle);
                if (ROMTitle.ShowDialog() == DialogResult.OK)
                {
                    R.Title = ROMTitle.Title;
                    R.UseTrueTypeFontForTitleImage = ROMTitle.UseTrueTypeFont;
                }
                else return;
                AddROMToInterface(R);
            }
        }

        private void FileEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                String[] FileTypes = new String[] { ".gb", ".gbc", ".bin" };
                String[] Files = (String[])e.Data.GetData(DataFormats.FileDrop);
                foreach (String F in Files)
                {
                    if (FileTypes.Contains(Path.GetExtension(F)))
                    {
                        e.Effect = DragDropEffects.All;
                        break;
                    }
                }
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void EditTickerButton_Click(object sender, EventArgs e)
        {
            TickerEditor editor = new TickerEditor();
            if (editor.ShowDialog() == DialogResult.OK)
            {
                this.Ticker = editor.TickerImage;
            }
        }
    }
}
