namespace RapidShareDownloader
{
    partial class Form1
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
            this.textbox_Urls = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.textBox_Protokoll = new System.Windows.Forms.TextBox();
            this.Protokoll = new System.Windows.Forms.Label();
            this.button_Start = new System.Windows.Forms.Button();
            this.button_Stop = new System.Windows.Forms.Button();
            this.button_Abbruch = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.Zielverzeichnis_TextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // textbox_Urls
            // 
            this.textbox_Urls.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textbox_Urls.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::RapidShareDownloader.Properties.Settings.Default, "Urls", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textbox_Urls.Location = new System.Drawing.Point(12, 27);
            this.textbox_Urls.Multiline = true;
            this.textbox_Urls.Name = "textbox_Urls";
            this.textbox_Urls.Size = new System.Drawing.Size(727, 235);
            this.textbox_Urls.TabIndex = 0;
            this.textbox_Urls.Text = global::RapidShareDownloader.Properties.Settings.Default.Urls;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(152, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Download-URLs hier einfügen:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 279);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(134, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Dateien ablegen in Ordner:";
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(709, 293);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(30, 23);
            this.btnBrowse.TabIndex = 4;
            this.btnBrowse.Text = ">>";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // textBox_Protokoll
            // 
            this.textBox_Protokoll.AcceptsReturn = true;
            this.textBox_Protokoll.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_Protokoll.Location = new System.Drawing.Point(12, 370);
            this.textBox_Protokoll.Multiline = true;
            this.textBox_Protokoll.Name = "textBox_Protokoll";
            this.textBox_Protokoll.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox_Protokoll.Size = new System.Drawing.Size(727, 136);
            this.textBox_Protokoll.TabIndex = 5;
            // 
            // Protokoll
            // 
            this.Protokoll.AutoSize = true;
            this.Protokoll.Location = new System.Drawing.Point(9, 353);
            this.Protokoll.Name = "Protokoll";
            this.Protokoll.Size = new System.Drawing.Size(51, 13);
            this.Protokoll.TabIndex = 6;
            this.Protokoll.Text = "Protokoll:";
            // 
            // button_Start
            // 
            this.button_Start.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Start.Location = new System.Drawing.Point(162, 321);
            this.button_Start.Name = "button_Start";
            this.button_Start.Size = new System.Drawing.Size(150, 30);
            this.button_Start.TabIndex = 7;
            this.button_Start.Text = "Start";
            this.button_Start.UseVisualStyleBackColor = true;
            this.button_Start.Click += new System.EventHandler(this.button_Start_Click);
            // 
            // button_Stop
            // 
            this.button_Stop.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Stop.Location = new System.Drawing.Point(431, 321);
            this.button_Stop.Name = "button_Stop";
            this.button_Stop.Size = new System.Drawing.Size(103, 30);
            this.button_Stop.TabIndex = 8;
            this.button_Stop.Text = "Stop";
            this.button_Stop.UseVisualStyleBackColor = true;
            this.button_Stop.Click += new System.EventHandler(this.button_Stop_Click);
            // 
            // button_Abbruch
            // 
            this.button_Abbruch.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Abbruch.Location = new System.Drawing.Point(540, 321);
            this.button_Abbruch.Name = "button_Abbruch";
            this.button_Abbruch.Size = new System.Drawing.Size(103, 30);
            this.button_Abbruch.TabIndex = 9;
            this.button_Abbruch.Text = "Abbruch";
            this.button_Abbruch.UseVisualStyleBackColor = true;
            this.button_Abbruch.Click += new System.EventHandler(this.button_Abbruch_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Checked = global::RapidShareDownloader.Properties.Settings.Default.IESichtbar;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::RapidShareDownloader.Properties.Settings.Default, "IESichtbar", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBox1.Location = new System.Drawing.Point(663, 329);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(76, 17);
            this.checkBox1.TabIndex = 10;
            this.checkBox1.Text = "IE sichtbar";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // Zielverzeichnis_TextBox
            // 
            this.Zielverzeichnis_TextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::RapidShareDownloader.Properties.Settings.Default, "Zielverzeichnis", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.Zielverzeichnis_TextBox.Location = new System.Drawing.Point(12, 295);
            this.Zielverzeichnis_TextBox.Name = "Zielverzeichnis_TextBox";
            this.Zielverzeichnis_TextBox.Size = new System.Drawing.Size(691, 20);
            this.Zielverzeichnis_TextBox.TabIndex = 3;
            this.Zielverzeichnis_TextBox.Text = global::RapidShareDownloader.Properties.Settings.Default.Zielverzeichnis;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(752, 518);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.button_Abbruch);
            this.Controls.Add(this.button_Stop);
            this.Controls.Add(this.button_Start);
            this.Controls.Add(this.Protokoll);
            this.Controls.Add(this.textBox_Protokoll);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.Zielverzeichnis_TextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textbox_Urls);
            this.Name = "Form1";
            this.Text = "Ollis Rapidshare Downloader";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textbox_Urls;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox Zielverzeichnis_TextBox;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.TextBox textBox_Protokoll;
        private System.Windows.Forms.Label Protokoll;
        private System.Windows.Forms.Button button_Start;
        private System.Windows.Forms.Button button_Stop;
        private System.Windows.Forms.Button button_Abbruch;
        private System.Windows.Forms.CheckBox checkBox1;
    }
}

