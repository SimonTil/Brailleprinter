namespace Braille_plotter
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.MS_main = new System.Windows.Forms.MenuStrip();
            this.MI_file = new System.Windows.Forms.ToolStripMenuItem();
            this.MI_new = new System.Windows.Forms.ToolStripMenuItem();
            this.MI_open = new System.Windows.Forms.ToolStripMenuItem();
            this.MI_save = new System.Windows.Forms.ToolStripMenuItem();
            this.MI_saveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.MI_exit = new System.Windows.Forms.ToolStripMenuItem();
            this.MI_edit = new System.Windows.Forms.ToolStripMenuItem();
            this.MI_undo = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.MI_cut = new System.Windows.Forms.ToolStripMenuItem();
            this.MI_copy = new System.Windows.Forms.ToolStripMenuItem();
            this.MI_paste = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.MI_selectAll = new System.Windows.Forms.ToolStripMenuItem();
            this.MI_braille = new System.Windows.Forms.ToolStripMenuItem();
            this.MI_print = new System.Windows.Forms.ToolStripMenuItem();
            this.MI_converteren = new System.Windows.Forms.ToolStripMenuItem();
            this.MI_help = new System.Windows.Forms.ToolStripMenuItem();
            this.MI_onlineHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.MI_shortcuts = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.MI_about = new System.Windows.Forms.ToolStripMenuItem();
            this.MI_helpDevelop = new System.Windows.Forms.ToolStripMenuItem();
            this.TB_input = new System.Windows.Forms.RichTextBox();
            this.TB_preview = new System.Windows.Forms.RichTextBox();
            this.MS_main.SuspendLayout();
            this.SuspendLayout();
            // 
            // MS_main
            // 
            this.MS_main.BackColor = System.Drawing.SystemColors.Window;
            this.MS_main.GripMargin = new System.Windows.Forms.Padding(0);
            this.MS_main.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.MS_main.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MI_file,
            this.MI_edit,
            this.MI_braille,
            this.MI_help});
            this.MS_main.Location = new System.Drawing.Point(0, 0);
            this.MS_main.Name = "MS_main";
            this.MS_main.Padding = new System.Windows.Forms.Padding(0);
            this.MS_main.Size = new System.Drawing.Size(938, 24);
            this.MS_main.TabIndex = 1;
            this.MS_main.TabStop = true;
            this.MS_main.Text = "menuStrip1";
            // 
            // MI_file
            // 
            this.MI_file.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MI_new,
            this.MI_open,
            this.MI_save,
            this.MI_saveAs,
            this.toolStripSeparator1,
            this.MI_exit});
            this.MI_file.Name = "MI_file";
            this.MI_file.Size = new System.Drawing.Size(76, 24);
            this.MI_file.Text = "Bestand";
            // 
            // MI_new
            // 
            this.MI_new.Name = "MI_new";
            this.MI_new.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.MI_new.Size = new System.Drawing.Size(267, 26);
            this.MI_new.Text = "Nieuw";
            this.MI_new.Click += new System.EventHandler(this.MI_new_Click);
            // 
            // MI_open
            // 
            this.MI_open.Name = "MI_open";
            this.MI_open.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.MI_open.Size = new System.Drawing.Size(267, 26);
            this.MI_open.Text = "Openen...";
            this.MI_open.Click += new System.EventHandler(this.MI_open_Click);
            // 
            // MI_save
            // 
            this.MI_save.Name = "MI_save";
            this.MI_save.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.MI_save.Size = new System.Drawing.Size(267, 26);
            this.MI_save.Text = "Opslaan";
            this.MI_save.Click += new System.EventHandler(this.MI_save_Click);
            // 
            // MI_saveAs
            // 
            this.MI_saveAs.Name = "MI_saveAs";
            this.MI_saveAs.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
            this.MI_saveAs.Size = new System.Drawing.Size(267, 26);
            this.MI_saveAs.Text = "Opslaan als...";
            this.MI_saveAs.Click += new System.EventHandler(this.MI_saveAs_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(264, 6);
            // 
            // MI_exit
            // 
            this.MI_exit.Name = "MI_exit";
            this.MI_exit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.MI_exit.Size = new System.Drawing.Size(267, 26);
            this.MI_exit.Text = "Afsluiten";
            this.MI_exit.Click += new System.EventHandler(this.MI_exit_Click);
            // 
            // MI_edit
            // 
            this.MI_edit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MI_undo,
            this.toolStripSeparator2,
            this.MI_cut,
            this.MI_copy,
            this.MI_paste,
            this.toolStripSeparator3,
            this.MI_selectAll});
            this.MI_edit.Name = "MI_edit";
            this.MI_edit.Size = new System.Drawing.Size(87, 24);
            this.MI_edit.Text = "Bewerken";
            this.MI_edit.Click += new System.EventHandler(this.MI_edit_Click);
            // 
            // MI_undo
            // 
            this.MI_undo.Name = "MI_undo";
            this.MI_undo.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.MI_undo.Size = new System.Drawing.Size(260, 26);
            this.MI_undo.Text = "Ongedaan maken";
            this.MI_undo.Click += new System.EventHandler(this.MI_undo_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(257, 6);
            // 
            // MI_cut
            // 
            this.MI_cut.Name = "MI_cut";
            this.MI_cut.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.MI_cut.Size = new System.Drawing.Size(260, 26);
            this.MI_cut.Text = "Knippen";
            this.MI_cut.Click += new System.EventHandler(this.MI_cut_Click);
            // 
            // MI_copy
            // 
            this.MI_copy.Name = "MI_copy";
            this.MI_copy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.MI_copy.Size = new System.Drawing.Size(260, 26);
            this.MI_copy.Text = "Kopiëren";
            this.MI_copy.Click += new System.EventHandler(this.MI_copy_Click);
            // 
            // MI_paste
            // 
            this.MI_paste.Name = "MI_paste";
            this.MI_paste.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.MI_paste.Size = new System.Drawing.Size(260, 26);
            this.MI_paste.Text = "Plakken";
            this.MI_paste.Click += new System.EventHandler(this.MI_paste_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(257, 6);
            // 
            // MI_selectAll
            // 
            this.MI_selectAll.Name = "MI_selectAll";
            this.MI_selectAll.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.MI_selectAll.Size = new System.Drawing.Size(260, 26);
            this.MI_selectAll.Text = "Alles selecteren";
            this.MI_selectAll.Click += new System.EventHandler(this.MI_selectAll_Click);
            // 
            // MI_braille
            // 
            this.MI_braille.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MI_print,
            this.MI_converteren});
            this.MI_braille.Name = "MI_braille";
            this.MI_braille.Size = new System.Drawing.Size(65, 24);
            this.MI_braille.Text = "Braille";
            this.MI_braille.Click += new System.EventHandler(this.MI_braille_Click);
            // 
            // MI_print
            // 
            this.MI_print.Name = "MI_print";
            this.MI_print.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.MI_print.Size = new System.Drawing.Size(224, 26);
            this.MI_print.Text = "Print";
            this.MI_print.Click += new System.EventHandler(this.MI_print_Click);
            // 
            // MI_converteren
            // 
            this.MI_converteren.Name = "MI_converteren";
            this.MI_converteren.Size = new System.Drawing.Size(224, 26);
            this.MI_converteren.Text = "Converteren";
            this.MI_converteren.Click += new System.EventHandler(this.MI_converteren_Click);
            // 
            // MI_help
            // 
            this.MI_help.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MI_onlineHelp,
            this.toolStripSeparator4,
            this.MI_shortcuts,
            this.toolStripSeparator5,
            this.MI_about,
            this.MI_helpDevelop});
            this.MI_help.Name = "MI_help";
            this.MI_help.Size = new System.Drawing.Size(55, 24);
            this.MI_help.Text = "Help";
            // 
            // MI_onlineHelp
            // 
            this.MI_onlineHelp.Name = "MI_onlineHelp";
            this.MI_onlineHelp.ShortcutKeys = System.Windows.Forms.Keys.F1;
            this.MI_onlineHelp.Size = new System.Drawing.Size(250, 26);
            this.MI_onlineHelp.Text = "Online help...";
            this.MI_onlineHelp.Click += new System.EventHandler(this.MI_onlineHelp_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(247, 6);
            // 
            // MI_shortcuts
            // 
            this.MI_shortcuts.Name = "MI_shortcuts";
            this.MI_shortcuts.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F10)));
            this.MI_shortcuts.Size = new System.Drawing.Size(250, 26);
            this.MI_shortcuts.Text = "Sneltoetsen...";
            this.MI_shortcuts.Click += new System.EventHandler(this.MI_shortcuts_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(247, 6);
            // 
            // MI_about
            // 
            this.MI_about.Name = "MI_about";
            this.MI_about.Size = new System.Drawing.Size(250, 26);
            this.MI_about.Text = "Over dit programma...";
            this.MI_about.Click += new System.EventHandler(this.MI_about_Click);
            // 
            // MI_helpDevelop
            // 
            this.MI_helpDevelop.Name = "MI_helpDevelop";
            this.MI_helpDevelop.Size = new System.Drawing.Size(250, 26);
            this.MI_helpDevelop.Text = "Help ontwikkelen...";
            this.MI_helpDevelop.Click += new System.EventHandler(this.MI_helpDevelop_Click);
            // 
            // TB_input
            // 
            this.TB_input.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TB_input.Font = new System.Drawing.Font("Consolas", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TB_input.Location = new System.Drawing.Point(0, 24);
            this.TB_input.Margin = new System.Windows.Forms.Padding(4);
            this.TB_input.Name = "TB_input";
            this.TB_input.Size = new System.Drawing.Size(529, 517);
            this.TB_input.TabIndex = 0;
            this.TB_input.Text = "";
            this.TB_input.TextChanged += new System.EventHandler(this.TB_input_TextChanged);
            // 
            // TB_preview
            // 
            this.TB_preview.Dock = System.Windows.Forms.DockStyle.Right;
            this.TB_preview.Font = new System.Drawing.Font("Consolas", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TB_preview.Location = new System.Drawing.Point(537, 24);
            this.TB_preview.Margin = new System.Windows.Forms.Padding(4);
            this.TB_preview.Name = "TB_preview";
            this.TB_preview.ReadOnly = true;
            this.TB_preview.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.TB_preview.Size = new System.Drawing.Size(401, 520);
            this.TB_preview.TabIndex = 2;
            this.TB_preview.Text = "";
            // 
            // MainForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(938, 544);
            this.Controls.Add(this.TB_preview);
            this.Controls.Add(this.TB_input);
            this.Controls.Add(this.MS_main);
            this.Font = new System.Drawing.Font("Consolas", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.MS_main;
            this.Margin = new System.Windows.Forms.Padding(5);
            this.MinimumSize = new System.Drawing.Size(700, 300);
            this.Name = "MainForm";
            this.Text = "Naamloos - Braille printer";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.MS_main.ResumeLayout(false);
            this.MS_main.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip MS_main;
        private System.Windows.Forms.ToolStripMenuItem MI_file;
        private System.Windows.Forms.ToolStripMenuItem MI_new;
        private System.Windows.Forms.ToolStripMenuItem MI_open;
        private System.Windows.Forms.ToolStripMenuItem MI_save;
        private System.Windows.Forms.ToolStripMenuItem MI_saveAs;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem MI_exit;
        private System.Windows.Forms.ToolStripMenuItem MI_edit;
        private System.Windows.Forms.ToolStripMenuItem MI_undo;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem MI_cut;
        private System.Windows.Forms.ToolStripMenuItem MI_copy;
        private System.Windows.Forms.ToolStripMenuItem MI_paste;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem MI_selectAll;
        private System.Windows.Forms.RichTextBox TB_input;
        private System.Windows.Forms.RichTextBox TB_preview;
        private System.Windows.Forms.ToolStripMenuItem MI_braille;
        private System.Windows.Forms.ToolStripMenuItem MI_print;
        private System.Windows.Forms.ToolStripMenuItem MI_help;
        private System.Windows.Forms.ToolStripMenuItem MI_onlineHelp;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem MI_shortcuts;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem MI_about;
        private System.Windows.Forms.ToolStripMenuItem MI_helpDevelop;
        private System.Windows.Forms.ToolStripMenuItem MI_converteren;
    }
}

