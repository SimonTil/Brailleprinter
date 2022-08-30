#region Using directives
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Threading;
#endregion

namespace Braille_plotter
{
    public partial class MainForm : Form
    {
        #region Initializing the form
        public MainForm() // DONE
        {
            InitializeComponent();
        }

        #region variables
        private SaveFileDialog saveFileDialog;
        private OpenFileDialog openFileDialog;
        private bool fileChanged = false;
        private string filePath = "";
        private readonly string titleSuffix = " - Braille printer";
        private readonly string headerBarName = "Braille printer";
        private readonly string fileTypes = "Tekstbestanden (*.txt)|*.txt|Alle bestanden (*.*)|*.*";
        private int status = 1; // 0 = cancelled, 1 = available, 2 = waiting
        //private enum Status { cancelled, available, waiting };
        //private Status status = Status.available;
        private bool timeOver = false;

        #endregion
        #endregion

        #region Load and close form
        private void MainForm_Load(object sender, EventArgs e) // DONE
        {
            this.FormClosing += new FormClosingEventHandler(CloseForm);
            this.Name = "Naamloos";
        }
        private void CloseForm(object sender, FormClosingEventArgs e) // DONE
        {
            e.Cancel = true;
            if (!fileChanged)
            {
                e.Cancel = false;
            }
            else if (filePath != "")
            {
                DialogResult result = MessageBox.Show("Wilt u de wijzigingen opslaan in " + this.Name + "?", headerBarName, MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Yes)
                {
                    File.WriteAllText(filePath, this.TB_input.Text);
                    e.Cancel = false;
                }
                else if (result == DialogResult.No)
                {
                    e.Cancel = false;
                }
            }
            else
            {
                DialogResult result = MessageBox.Show("Wilt u de wijzigingen opslaan in " + this.Name + "?", headerBarName, MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Yes)
                {
                    saveFileDialog = new SaveFileDialog { Filter = fileTypes };
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        File.WriteAllText(saveFileDialog.FileName, this.TB_input.Text);
                        e.Cancel = false;
                    }
                }
                else if (result == DialogResult.No)
                {
                    e.Cancel = false;
                }
            }
        }
        #endregion

        #region Severe Functions
        /* SEVERE FUNCTIONS */
        private void NewFile() // DONE
        {
            this.Name = "Naamloos";
            this.TB_input.Text = String.Empty;
            this.TB_preview.Text = String.Empty;
            this.Text = this.Name + titleSuffix;
            fileChanged = false;
            filePath = null;
        }
        private void OpenFile() // DONE
        {
            openFileDialog = new OpenFileDialog { Filter = fileTypes };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.TB_input.Text = File.ReadAllText(openFileDialog.FileName);
                filePath = openFileDialog.FileName;
                this.Name = Path.GetFileNameWithoutExtension(filePath);
                fileChanged = false;
                UpdateTitle();
                string input = TB_input.Text;
                TB_preview.Text = PreviewBraille(ConvertToChars(ConvertToBraille(input)));
            }
        }
        private void UpdateTitle() // DONE
        {
            //this.Name = Path.GetFileNameWithoutExtension(filePath);
            this.Text = this.Name + titleSuffix;
            if (fileChanged) this.Text = "*" + this.Text;
        }
        private void TB_input_TextChanged(object sender, EventArgs e) // DONE
        {
            fileChanged = true;
            if (filePath == "" && String.IsNullOrEmpty(TB_input.Text)) fileChanged = false;
            UpdateTitle();
        }
        private void DataReceivedHandler(object sender, EventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            String inData = sp.ReadExisting();
            if (inData == "1")
            {
                status = 1;
            }
            else if (inData == "2")
            {
                status = 0;
            }
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            timeOver = true;
        }
        #endregion

        #region UI handlers
        /* MENU ITEMS */
        private void MI_new_Click(object sender, EventArgs e) // DONE
        {
            if (!fileChanged)
            {
                NewFile();
            }
            else
            {
                var result = MessageBox.Show("Wilt u de wijzingingen opslaan in " + this.Name + "?", headerBarName, MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Yes) // Save file
                {
                    saveFileDialog = new SaveFileDialog { Filter = fileTypes };
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        File.WriteAllText(saveFileDialog.FileName, this.TB_input.Text);
                        NewFile();
                    }
                }
                else if (result == DialogResult.No)
                {
                    NewFile();
                }
            }
        }
        private void MI_open_Click(object sender, EventArgs e) // DONE
        {
            if (!fileChanged) // current existing file not changed
            {
                OpenFile();
            }
            else if (filePath != "")
            { // current existing file changed but not saved. Save? -> close and open
                var result = MessageBox.Show("Wilt u de wijzigingen opslaan in " + this.Name + "?", headerBarName, MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Yes)
                {
                    File.WriteAllText(filePath, this.TB_input.Text);
                    OpenFile();
                }
                else if (result == DialogResult.No)
                {
                    OpenFile();
                }
            }
            else // changed but not saved. Save? -> close and open
            {
                var result = MessageBox.Show("Wilt u de wijzigingen opslaan in " + this.Name + "?", headerBarName, MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Yes)
                {
                    saveFileDialog = new SaveFileDialog { Filter = fileTypes };
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        File.WriteAllText(saveFileDialog.FileName, this.TB_input.Text);
                        OpenFile();
                    }
                }
                else if (result == DialogResult.No)
                {
                    OpenFile();
                }
            }
        }
        private void MI_save_Click(object sender, EventArgs e) // DONE
        {
            if (filePath != "")
            { // save without save-dialog
                File.WriteAllText(filePath, this.TB_input.Text);
                fileChanged = false;
                UpdateTitle();
            }
            else
            {
                saveFileDialog = new SaveFileDialog{ Filter = fileTypes };
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(saveFileDialog.FileName, this.TB_input.Text);
                    fileChanged = false;
                    this.Name = Path.GetFileNameWithoutExtension(saveFileDialog.FileName);
                    UpdateTitle();
                    filePath = saveFileDialog.FileName;
                }
            }
        }
        private void MI_saveAs_Click(object sender, EventArgs e) // DONE
        {
            saveFileDialog = new SaveFileDialog { Filter = fileTypes };
            if (filePath != "")
            {
                saveFileDialog.FileName = this.Name;
            }

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(saveFileDialog.FileName, this.TB_input.Text);
                fileChanged = false;
                this.Name = Path.GetFileNameWithoutExtension(saveFileDialog.FileName);
                UpdateTitle();
                filePath = saveFileDialog.FileName;
            }
        }
        private void MI_exit_Click(object sender, EventArgs e) // DONE
        {
            if (fileChanged)
            {
                var result = MessageBox.Show("Wilt u de wijzingingen opslaan in " + this.Name + "?", headerBarName, MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Yes)
                { // Save file
                    saveFileDialog = new SaveFileDialog { Filter = fileTypes };
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        Application.Exit();
                    }
                }
                else if (result == DialogResult.No)
                {
                    Application.Exit();
                }
            }
            else
            {
                Application.Exit();
            }
        }

        private void MI_edit_Click(object sender, EventArgs e) // DONE
        {
            MI_cut.Enabled = !string.IsNullOrEmpty(TB_input.Text);
            MI_copy.Enabled = !string.IsNullOrWhiteSpace(TB_input.Text);
            MI_paste.Enabled = Clipboard.GetDataObject().GetDataPresent(DataFormats.Text) == true;
        }
        private void MI_undo_Click(object sender, EventArgs e) // DONE
        {
            if (TB_input.CanUndo) TB_input.Undo();
        }
        private void MI_cut_Click(object sender, EventArgs e) // DONE
        {
            TB_input.Cut();
            MI_paste.Enabled = true;
        }
        private void MI_copy_Click(object sender, EventArgs e) // DONE
        {
            TB_input.Copy();
            MI_paste.Enabled = true;
        }
        private void MI_paste_Click(object sender, EventArgs e) // DONE
        {
            TB_input.Paste(DataFormats.GetFormat("Text"));
        }
        private void MI_selectAll_Click(object sender, EventArgs e) // DONE
        {
            if (!string.IsNullOrEmpty(TB_input.Text)) TB_input.SelectAll();
        }
        
        /*
         * How to convert:
         * - Function convertToBraille(string) converts regular text into braille-numbers (type: List<int[]>)
         * - Function previewBraille(string) converts braille-numbers to unicode chars (type: string), and previews in TB_preview
         * 
         * - MI_braille_Click checks whether printer is connected and enables sub-options
         * - MI_converteren_Click runs previewBraille(convertToBraille(string))
         * - MI_print_Click runs previewBraille(convertToBraille(string)) and prints the output from convertToBraille(string) (type: List<int[]>).
         */

        private void MI_braille_Click(object sender, EventArgs e) // DONE
        {
            MI_print.Enabled = String.IsNullOrEmpty(TB_input.Text) != true;
            MI_converteren.Enabled = String.IsNullOrEmpty(TB_input.Text) != true;
        }
        private void MI_print_Click(object sender, EventArgs e)
        {            
            if (String.IsNullOrEmpty(TB_input.Text)) return;

            // Preview text
            String input = TB_input.Text;
            List<byte[]> output = ConvertToChars(ConvertToBraille(input));
            TB_preview.Text = PreviewBraille(output);
            
            // Setup port
            String printerPort = null;
            List<String> listOfPorts = new List<String>(SerialPort.GetPortNames());

            // Scan ports
            for (int i = listOfPorts.Count - 1; i >= 0; i--)
            {
                SerialPort printer = new SerialPort(listOfPorts[i], 256000);
                try
                {
                    printer.Open();
                    printer.Close();
                    printerPort = listOfPorts[i];
                }
                catch (Exception){
                    listOfPorts.RemoveAt(i);
                }
            }

            // Port found, send data
            if (printerPort != null)
            {
                // Run code!
                SerialPort printer = new SerialPort(printerPort, 9600);
                printer.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
                printer.Open();
                MessageBox.Show("Printer verbonden. Print begint nu!", "Braille printer");

                // Send all text
                for (int i = 0; i < output.Count; i++)
                {
                    // Set status to waiting
                    status = 2;

                    // Send 1 line to printer
                    try
                    {
                        printer.Write(output[i], 0, output[i].Length);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Probleem met het sturen van tekst naar de printer", "Braille printer");
                        return;
                    }

                    // Wait for feedback and continue or cancel
                    var timer = new System.Windows.Forms.Timer { Interval = 120_000 };
                    timer.Tick += new EventHandler(Timer_Tick);
                    timer.Start();
                    while (true)
                    {
                        if (status == 1)
                        {
                            timer.Stop();
                            break;
                        }
                        else if (status == 0 || timeOver)
                        {
                            MessageBox.Show("Printer ontkoppeld, printproces afgebroken!", "Braille printer");
                            printer.Close();
                            printerPort = null;
                            timer.Stop();
                            return;
                        }
                        else
                        {
                            // To prevent overheating
                            Thread.Sleep(10);
                        }

                        if (!printer.IsOpen)
                        {
                            printer.Close();
                            MessageBox.Show("Print halverwege afgebroken!", "Braille printer");
                            return;
                        }
                    }
                }

                printer.Close();
                MessageBox.Show("Printen gelukt. Druk op de F-toets op de printer om het papier uit te voeren.", "Braille printer");

                printerPort = null;
            }
            else
            {
                // Should never happen (theoretically)
                MessageBox.Show("Printer niet verbonden!", "Braille printer");
            }
        }
        private void MI_converteren_Click(object sender, EventArgs e)
        {
            string input = TB_input.Text;
            TB_preview.Text = PreviewBraille(ConvertToChars(ConvertToBraille(input)));
        }

        private void MI_onlineHelp_Click(object sender, EventArgs e) // AT FINISH
        {
            Process.Start("https://github.com/SimonTil/Brailleprinter/raw/main/Gebruikershandleiding.docx");
        }
        private void MI_shortcuts_Click(object sender, EventArgs e) // DONE
        {
            Form shortcutForm = new Form { Size = new Size(500, 500) };
            shortcutForm.FormBorderStyle = FormBorderStyle.FixedSingle;
            shortcutForm.MaximizeBox = false; shortcutForm.MinimizeBox = false;
            shortcutForm.StartPosition = FormStartPosition.CenterScreen;
            shortcutForm.ShowIcon = false;
            shortcutForm.Text = "Sneltoetsen";
            shortcutForm.Show();
            shortcutForm.Controls.Add(
                new Label()
                {
                    Text = "Sneltoetsen",
                    Width = 500,
                    Font = new Font("Segoe UI", 12, FontStyle.Bold),
                    Location = new Point(20, 20)
                }); // Title "Sneltoetsen"
            shortcutForm.Controls.Add(
                new Label()
                {
                    Text = "De volgende toetscombinaties kunnen gebruikt worden in dit programma:",
                    Width = 440,
                    Height = 50,
                    Font = new Font("Segoe UI", 10, FontStyle.Regular),
                    Location = new Point(30, 60)
                }); // Subtitle "De volgende toetscombinaties kunnen gebruikt worden in dit programma:"
            int offsetComment = 170, lineOffset = 30;
            shortcutForm.Controls.Add(new Label() { Text = "Ctrl + N", Location = new Point(30, 90 + lineOffset), Font = new Font("Segoe UI", 10), Width = 130 });
            shortcutForm.Controls.Add(new Label() { Text = "Nieuw", Location = new Point(offsetComment, 90 + lineOffset), Font = new Font("Segoe UI", 10) }); lineOffset += 20;
            shortcutForm.Controls.Add(new Label() { Text = "Ctrl + O", Location = new Point(30, 90 + lineOffset), Font = new Font("Segoe UI", 10), Width = 130 });
            shortcutForm.Controls.Add(new Label() { Text = "Openen", Location = new Point(offsetComment, 90 + lineOffset), Font = new Font("Segoe UI", 10) }); lineOffset += 20;
            shortcutForm.Controls.Add(new Label() { Text = "Ctrl + S", Location = new Point(30, 90 + lineOffset), Font = new Font("Segoe UI", 10), Width = 130 });
            shortcutForm.Controls.Add(new Label() { Text = "Opslaan", Location = new Point(offsetComment, 90 + lineOffset), Font = new Font("Segoe UI", 10) }); lineOffset += 20;
            shortcutForm.Controls.Add(new Label() { Text = "Ctrl + Shift + S", Location = new Point(30, 90 + lineOffset), Font = new Font("Segoe UI", 10), Width = 130 });
            shortcutForm.Controls.Add(new Label() { Text = "Opslaan als", Location = new Point(offsetComment, 90 + lineOffset), Font = new Font("Segoe UI", 10) }); lineOffset += 20;
            shortcutForm.Controls.Add(new Label() { Text = "Alt + F4", Location = new Point(30, 90 + lineOffset), Font = new Font("Segoe UI", 10), Width = 130 });
            shortcutForm.Controls.Add(new Label() { Text = "Afsluiten", Location = new Point(offsetComment, 90 + lineOffset), Font = new Font("Segoe UI", 10) }); lineOffset += 20;
            shortcutForm.Controls.Add(new Label() { Text = "Ctrl + Z", Location = new Point(30, 90 + lineOffset), Font = new Font("Segoe UI", 10), Width = 130 });
            shortcutForm.Controls.Add(new Label() { Text = "Ongedaan maken", Location = new Point(offsetComment, 90 + lineOffset), Font = new Font("Segoe UI", 10) }); lineOffset += 20;
            shortcutForm.Controls.Add(new Label() { Text = "Ctrl + X", Location = new Point(30, 90 + lineOffset), Font = new Font("Segoe UI", 10), Width = 130 });
            shortcutForm.Controls.Add(new Label() { Text = "Knippen", Location = new Point(offsetComment, 90 + lineOffset), Font = new Font("Segoe UI", 10) }); lineOffset += 20;
            shortcutForm.Controls.Add(new Label() { Text = "Ctrl + C", Location = new Point(30, 90 + lineOffset), Font = new Font("Segoe UI", 10), Width = 130 });
            shortcutForm.Controls.Add(new Label() { Text = "Kopiëren", Location = new Point(offsetComment, 90 + lineOffset), Font = new Font("Segoe UI", 10) }); lineOffset += 20;
            shortcutForm.Controls.Add(new Label() { Text = "Ctrl + V", Location = new Point(30, 90 + lineOffset), Font = new Font("Segoe UI", 10), Width = 130 });
            shortcutForm.Controls.Add(new Label() { Text = "Plakken", Location = new Point(offsetComment, 90 + lineOffset), Font = new Font("Segoe UI", 10) }); lineOffset += 20;
            shortcutForm.Controls.Add(new Label() { Text = "Ctrl + A", Location = new Point(30, 90 + lineOffset), Font = new Font("Segoe UI", 10), Width = 130 });
            shortcutForm.Controls.Add(new Label() { Text = "Alles selecteren", Location = new Point(offsetComment, 90 + lineOffset), Font = new Font("Segoe UI", 10) }); lineOffset += 20;
            shortcutForm.Controls.Add(new Label() { Text = "Ctrl + P", Location = new Point(30, 90 + lineOffset), Font = new Font("Segoe UI", 10), Width = 130 });
            shortcutForm.Controls.Add(new Label() { Text = "Printen", Location = new Point(offsetComment, 90 + lineOffset), Font = new Font("Segoe UI", 10) }); lineOffset += 20;
            shortcutForm.Controls.Add(new Label() { Text = "F1", Location = new Point(30, 90 + lineOffset), Font = new Font("Segoe UI", 10), Width = 130 });
            shortcutForm.Controls.Add(new Label() { Text = "Online help", Location = new Point(offsetComment, 90 + lineOffset), Font = new Font("Segoe UI", 10) }); lineOffset += 20;
            shortcutForm.Controls.Add(new Label() { Text = "Shift + F10", Location = new Point(30, 90 + lineOffset), Font = new Font("Segoe UI", 10), Width = 130 });
            shortcutForm.Controls.Add(new Label() { Text = "Over", Location = new Point(offsetComment, 90 + lineOffset), Font = new Font("Segoe UI", 10) });
        }
        private void MI_about_Click(object sender, EventArgs e) // DONE
        {
            Form aboutForm = new Form { Size = new Size(300, 400) };
            aboutForm.FormBorderStyle = FormBorderStyle.FixedSingle;
            aboutForm.MaximizeBox = false;
            aboutForm.MinimizeBox = false;
            aboutForm.StartPosition = FormStartPosition.CenterScreen;
            aboutForm.ShowIcon = false;
            aboutForm.Text = "Over dit programma";
            aboutForm.Show();
            aboutForm.Controls.Add(
                new Label()
                {
                    Text = "Braille printer",
                    Width = 280,
                    Font = new Font("Segoe UI", 12, FontStyle.Bold),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Location = new Point(0, 40),

                }); // Title "Braille printer"
            aboutForm.Controls.Add(
                new Label()
                {
                    Text = "Versie 1.0\ndoor Simon Tillema",
                    Width = 280,
                    Height = 40,
                    Font = new Font("Segoe UI", 9, FontStyle.Italic),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Location = new Point(0, 70)

                }); // Version "Versie x door Simon Tillema"
            aboutForm.Controls.Add(
                new PictureBox()
                {
                    ImageLocation = "C:/Users/simon/Documents/GitHub/Brailleprinter/Source code/Program/License.png",
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Size = new Size(100, 50),
                    Location = new Point(100, 150)
                }); // License picture
            aboutForm.Controls.Add(
                new Label()
                {
                    Text = "Dit programma is gelicenseerd\n" +
                    "onder de Creative Commons\n" +
                    "licentie (CC BY-NC-SA).",
                    Width = 280,
                    Height = 60,
                    Font = new Font("Segoe UI", 9, FontStyle.Regular),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Location = new Point(0, 200)
                }); // License text "Dit programma is gelicenseerd onder de Creative Commons licentie (CC BY-NC-SA)."
            aboutForm.Controls.Add(
                new Label()
                {
                    Text = "29-08-2022",
                    Width = aboutForm.Width,
                    Font = new Font("Segoe UI", 9, FontStyle.Italic),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Location = new Point(0, 270)

                }); // date
        }
        private void MI_helpDevelop_Click(object sender, EventArgs e) // DONE
        {
            var result = MessageBox.Show("Wilt u helpen met de ontwikkeling van de Brailleprinter?\nKlik dan op OK om door te gaan naar de Github repository.", "Braille printer", MessageBoxButtons.OKCancel);
            if (result == DialogResult.OK) Process.Start("https://github.com/SimonTil/Brailleprinter");
        }
        #endregion

        #region Functions to convert to braille
        // Split in lines, convert line by line and return as list
        private List<String> ConvertToBraille(String input)
        {
            List<String> output = new List<String>();

            String[] myOutput = ConvertLineToBraille(input).Split('\n');
            for (int i = 0; i < myOutput.Length; i++)
            {
                output.Add(myOutput[i]);
            }

            return output;
        }
        private List<byte[]> ConvertToChars(List<String> input)
        {
            List<byte[]> output = new List<byte[]>();

            for (int i = 0; i < input.Count; i++)
            {
                // We're now looking at 1 line
                List<byte> converted = new List<byte>();

                String myString = "";
                if (input[i] != null) myString = input[i].TrimEnd();

                for (int j = 0; j < myString.Length; j++)
                {
                    // We're now looking at 1 character
                    byte result = 0;
                    switch (myString.Substring(j, 1))
                    {
                        // Standard alphabet
                        case "a": result = 0b00_000001; break;
                        case "b": result = 0b00_000011; break;
                        case "c": result = 0b00_001001; break;
                        case "d": result = 0b00_011001; break;
                        case "e": result = 0b00_010001; break;
                        case "f": result = 0b00_001011; break;
                        case "g": result = 0b00_011011; break;
                        case "h": result = 0b00_010011; break;
                        case "i": result = 0b00_001010; break;
                        case "j": result = 0b00_011010; break;
                        case "k": result = 0b00_000101; break;
                        case "l": result = 0b00_000111; break;
                        case "m": result = 0b00_001101; break;
                        case "n": result = 0b00_011101; break;
                        case "o": result = 0b00_010101; break;
                        case "p": result = 0b00_001111; break;
                        case "q": result = 0b00_011111; break;
                        case "r": result = 0b00_010111; break;
                        case "s": result = 0b00_001110; break;
                        case "t": result = 0b00_011110; break;
                        case "u": result = 0b00_100101; break;
                        case "v": result = 0b00_100111; break;
                        case "w": result = 0b00_111010; break;
                        case "x": result = 0b00_101101; break;
                        case "y": result = 0b00_111101; break;
                        case "z": result = 0b00_110101; break;

                        // Special tokens
                        case "A": result = 0b00_110000; break;
                        case "C": result = 0b00_101000; break;
                        case "K": result = 0b00_010000; break;
                        case "N": result = 0b00_111100; break;
                        case "O": result = 0b00_001000; break;
                        case "P": result = 0b00_011000; break;
                        case "R": result = 0b00_100000; break;

                        // Special characters
                        case " ": result = 0b00_000000; break;
                        case ",": result = 0b00_000010; break;
                        case "\'": result = 0b00_000100; break;
                        case ";": result = 0b00_000110; break;
                        case "/":
                        case "í": result = 0b00_001100; break;
                        case ":": result = 0b00_010010; break;
                        case "*": result = 0b00_010100; break;
                        case "+":
                        case "!": result = 0b00_010110; break;
                        case "@":
                        case "ä": result = 0b00_011100; break;
                        case "â": result = 0b00_100001; break;
                        case "~":
                        case "?": result = 0b00_100010; break;
                        case "ê": result = 0b00_100011; break;
                        case "-": result = 0b00_100100; break;
                        case "(":
                        case "×":
                        case "·": result = 0b00_100110; break;
                        case "î": result = 0b00_101001; break;
                        case "ö": result = 0b00_101010; break;
                        case "ë": result = 0b00_101011; break;
                        case "ó":
                        case "§": result = 0b00_101100; break;
                        case "^": result = 0b00_101100; break;
                        case "è": result = 0b00_101110; break;
                        case "&": result = 0b00_101111; break;
                        case "û": result = 0b00_110001; break;
                        case ".": result = 0b00_110010; break;
                        case "ü": result = 0b00_110011; break;
                        case ")":
                        case "°": result = 0b00_110100; break;
                        case "\"":
                        case "=": result = 0b00_110110; break;
                        case "á":
                        case "à":
                        case "[": result = 0b00_110111; break;
                        case "|": result = 0b00_111001; break;
                        case "ï": result = 0b00_111011; break;
                        case "ú":
                        case "ù":
                        case "]": result = 0b00_111110; break;
                        case "é":
                        case "%": result = 0b00_111111; break;
                        case "\n":result = 0b01_000000; break;
                        //default: result = 0; break;
                    }
                    converted.Add(result);
                }
                if (i == input.Count - 1)
                {
                    converted.Add(0b01_000001);
                }
                else
                {
                    converted.Add(0b01_000000);
                }

                output.Add(converted.ToArray());
                
            }
            return output;
        }
        private String PreviewBraille(List<byte[]> input)
        {
            String output = "";
            for (int i = 0; i < input.Count; i++)
            {
                for (int j = 0; j < input[i].Length; j++)
                {
                    if (input[i][j] == 0b01_000000)
                    {
                        output += Convert.ToChar(10);
                    }
                    else if (input[i][j] < 0b01_000000)
                    {
                        output += Convert.ToChar(input[i][j] + 10240);
                    }
                }
            }
            return output;
        }
        private String ConvertLineToBraille(String input)
        {
            int linewidth = 31;

            // Split on enters, and convert line by line
            String[] output = input.Split('\n');
            for (int i = 0; i < output.Length; i++)
            {
                output[i] = ReplaceCaps(output[i]);

                // Replace number
                output[i] = ReplaceNumbers(output[i]);

                // Replace special characters
                output[i] = ReplaceSpecialCharacters(output[i]);
                output[i] = RemoveUnknownCharacters(output[i]);

                // Cut to max linewidth
                output[i] = ReplaceLongWords(output[i], linewidth);
                output[i] = SplitToLineLength(output[i], linewidth);
            }

            return string.Join("\n", output);
        }
        private String ReplaceCaps(String input) // DONE
        {
            /* Rules regarding cap tokens:
             * - 1 cap: Add a 'C'-token and convert letter to lowercase
             * - > 1 cap: Add a 'P'-token and convert following to lowercase
             *     - permanent modus includes interpunction, like dots and quotation marks
             *     - Recovery token, space and number token cancels permanent modus
             * - More than 3 words of entire caps: Add 2 'P'-tokens before first, and single 'P'-token before last word
             *     - Words can contain numbers and hyphens
             *     - Next words only numbers cancels permanent modus
             *   - Double space and '\n' cancels permanent modus
             */
            bool permanentCapModus(String capsInput) // DONE
            {
                capsInput = Regex.Replace(capsInput, @"[\.,!?;:\\\/*\-\+\'\d]|\x22", "");
                return isUpper(capsInput) && !String.IsNullOrEmpty(capsInput);
            }
            bool singleCapModus(String capsInput) // DONE
            {
                capsInput = Regex.Replace(capsInput, @"[\.,;:\-/*\+\']|\x22", "");
                if (String.IsNullOrEmpty(capsInput)) return false;
                return isUpper(capsInput);
            }
            bool isUpper(String capsInput) // DONE
            {
                if (capsInput == null) return false;

                foreach (Char c in capsInput)
                {
                    if (!Char.IsUpper(c)) return false;
                }
                return true;
            }
            string convertCapsInWord(string capsInput)
            {
                String output = "";
                bool permaCapsFlag = false;

                if (capsInput == null) return "";

                for (int i = 0; i < capsInput.Length; i++)
                {
                    if (permaCapsFlag)
                    {
                        if (isUpper(capsInput.Substring(i, 1)))
                        {
                            output += capsInput.Substring(i, 1).ToLower();
                        }
                        else
                        {
                            permaCapsFlag = false;
                            Regex regex = new Regex(@"[!\?\.,\d]");
                            if (i + 1 < capsInput.Length && regex.IsMatch(capsInput.Substring(i + 1, 1))){
                                output += capsInput.Substring(i, 1);
                            }
                            else
                            {
                                output += "R" + capsInput.Substring(i, 1);
                            }
                        }
                    }
                    else
                    {
                        if (isUpper(capsInput.Substring(i, 1))){
                            if(i + 1 < capsInput.Length && isUpper(capsInput.Substring(i + 1, 1)))
                            {
                                permaCapsFlag = true;
                                output += "P" + capsInput.Substring(i, 1).ToLower();
                            }
                            else
                            {
                                output += "C" + capsInput.Substring(i, 1).ToLower();
                            }
                        }
                        else
                        {
                            output += capsInput.Substring(i, 1);
                        }
                    }
                }

                return output;
            }
            bool isNumber(String numberInput) // DONE
            {
                Regex regex = new Regex(@"^[\d.,]+$");
                return regex.IsMatch(numberInput);
                // Returns true if input is either a number-only
            }
            bool evaluateNextWords(int requestedAmount, String[] nextWords) // DONE
            {
                if (nextWords.Length == 1 || nextWords.Length <= requestedAmount) return false;

                bool result = false;
                for (int i = 1; i <= requestedAmount; i++)
                {
                    if (permanentCapModus(nextWords[i]))
                    {
                        result = true;
                        continue;
                    }
                    for (int j = i; j < nextWords.Length; j++)
                    {
                        result = permanentCapModus(nextWords[j]);
                        if (result) break;
                    }
                }
                return result;
            }

            bool capsFlag = false;
            string[] words = input.Split(' ');
            if (words.Length == 0) return null;
            // Evaluate word by word
            for (int i = 0; i < words.Length; i++)
            {
                if (capsFlag)
                {
                    String[] nextArray = new string[words.Length - i];
                    for (int j = 0; j < nextArray.Length; j++) nextArray[j] = words[j + i]; 
                    if (!evaluateNextWords(1, nextArray))
                    {
                        words[i] = "P" + words[i].ToLower();
                        capsFlag = false;
                    }
                    else
                    {
                        words[i] = words[i].ToLower();
                    }
                }
                else
                {
                    // If word length is 1 or word is number, convert single word
                    if (words[i].Length == 1 || isNumber(words[i])){
                        words[i] = convertCapsInWord(words[i]);
                    }
                    else
                    {
                        if (singleCapModus(words[i]))
                        {
                            words[i] = "P" + words[i].ToLower();

                            String[] followingArray = new string[words.Length - i];
                            for (int j = 0; j < followingArray.Length; j++) followingArray[j] = words[j + i];

                            if (evaluateNextWords(2, followingArray))
                            {
                                words[i] = "P" + words[i];
                                capsFlag = true;
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else
                        {
                            words[i] = convertCapsInWord(words[i]);
                        }
                    }
                }
            }
            return string.Join(" ", words);
        }
        private String ReplaceNumbers(String input) // DONE
        {
            String Callback(Match match)
            {
                string callbackOutput = "N";
                foreach (Char c in match.Groups[1].Value)
                {
                    callbackOutput += (Char)(Char.IsDigit(c) ? (c == '0' ? 'j' : c + '0') : c);
                }
                if (match.Groups[2].Value.Length != 0) callbackOutput += "R" + match.Groups[2].Value;
                return callbackOutput;
            }
            
            string pattern = @"(\d+(?:[.,]*\d+)*)([.,]*[a-j€$©])?"; // (\d[\d.,]*)([a-j€$©])?
            string output = Regex.Replace(input, pattern, Callback);
            return output;
        }
        private String ReplaceSpecialCharacters(String input)
        {
            String output = "";
            for (int i = 0; i < input.Length; i++)
            {
                // Add key "K" before special characters
                Regex rg = new Regex(@"[\\#~©®™<>\{\}]");
                if (rg.IsMatch(input[i].ToString()))
                {
                    output += "K";
                }

                output += input.Substring(i, 1);
            }

            // Add omen "O" before special character
            output = output.Replace("°", "O°");

            // Replace currency scripts
            output = Regex.Replace(output, @"\x20*€\x20*", " e");
            output = Regex.Replace(output, @"\x20*\$\x20*", " d");
            output = Regex.Replace(output, @"\x20*£\x20*", " p");
            output = Regex.Replace(output, @"\x20*¥\x20*", " y");

            // Replace superscript characters
            output = output.Replace("²", "^Nb");
            output = output.Replace("³", "^Nc");

            // Replace special characters
            output = output.Replace("±", "+-");
            output = output.Replace("‰", "%%");
            output = output.Replace("™", "tm");
            output = output.Replace("©", "c");
            output = output.Replace("®", "r");

            // Add alphabet change "A" before non-Dutch character
            output = output.Replace("Cł", "ACl");
            output = output.Replace("ł", "Al");
            output = output.Replace("ñ", "An");
            output = output.Replace("ß", "ss");
            output = output.Replace("α", "Aa");
            output = output.Replace("β", "Ab");
            output = output.Replace("γ", "Ac");
            output = output.Replace("π", "Ap");
            output = output.Replace("Cω", "ACz");

            // Replace smart quotation marks
            output = Regex.Replace(output, @"[`´‘’]", "\'");
            output = Regex.Replace(output, @"[“”]", "\"");

            // Replace non-used diacritic accents
            output = Regex.Replace(output, @"[ò]", "o");
            output = Regex.Replace(output, @"[ýÿ]", "y");
            return output;
        }
        private string RemoveUnknownCharacters(String input)
        {
            string output = "";
            Regex rg = new Regex(@"[\x20-\x7Eäáàâç©°ëéèê€ïíìîöóô§®üúùû×†\n]");
            for (int i = 0; i < input.Length; i++)
            {
                if (rg.IsMatch(input[i].ToString()))
                {
                    output += input[i];
                }
            }
            return output;
        }
        private string ReplaceLongWords(string input, int linewidth) // DONE
        {
            // To do: add "N" before number after split
            string[] words = input.Split(' ');

            for (int i = 0; i < words.Length; i++)
            {
                if (words[i].Length > linewidth)
                {
                    String outputChopped = "";
                    while (words[i].Length > linewidth)
                    {
                        outputChopped += words[i].Substring(0, linewidth - 1) + "K\n";
                        words[i] = words[i].Substring(linewidth - 1);
                    }
                    words[i] = outputChopped + words[i];
                }
            }
            string output = string.Join(" ", words);
            return output;
        }
        private string SplitToLineLength(string input, int linewidth)
        {
            if (String.IsNullOrEmpty(input)) return null;
            int currentLineLength = 0;

            // Split entire line to words, by enter, space, or hyphen
            string[] regelsString = Regex.Split(input, @"(?<=[\n\-\s])");
            string output = "";

            for (int i = 0; i < regelsString.Length; i++)
            {
                int wordLength = Regex.Replace(regelsString[i], @"[\n\s]+","").Length;
                if (currentLineLength + wordLength <= linewidth)
                {
                    output += regelsString[i];
                    currentLineLength += wordLength;
                    if (regelsString[i].Length != wordLength)
                    {
                        currentLineLength++;
                    }
                }
                else
                {
                    output += "\n" + regelsString[i];
                    currentLineLength = regelsString[i].Length;
                }
                if (output[output.Length - 1] == '\n')
                {
                    currentLineLength = 0;
                }
            }
            return output;
        }
        #endregion
    }
}