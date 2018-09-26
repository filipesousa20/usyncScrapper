namespace uSyncScrapper
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.buttonBrowseFolder = new System.Windows.Forms.Button();
            this.buttonScrap = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // buttonBrowseFolder
            // 
            this.buttonBrowseFolder.Location = new System.Drawing.Point(486, 12);
            this.buttonBrowseFolder.Name = "buttonBrowseFolder";
            this.buttonBrowseFolder.Size = new System.Drawing.Size(113, 37);
            this.buttonBrowseFolder.TabIndex = 0;
            this.buttonBrowseFolder.Text = "Browse folder";
            this.buttonBrowseFolder.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.buttonBrowseFolder.UseVisualStyleBackColor = true;
            this.buttonBrowseFolder.Click += new System.EventHandler(this.buttonBrowseFolder_Click);
            // 
            // buttonScrap
            // 
            this.buttonScrap.Location = new System.Drawing.Point(524, 58);
            this.buttonScrap.Name = "buttonScrap";
            this.buttonScrap.Size = new System.Drawing.Size(75, 38);
            this.buttonScrap.TabIndex = 1;
            this.buttonScrap.Text = "Scrap";
            this.buttonScrap.UseVisualStyleBackColor = true;
            this.buttonScrap.Click += new System.EventHandler(this.buttonScrap_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(12, 12);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(468, 22);
            this.textBox1.TabIndex = 2;
            this.textBox1.Text = "E:\\Projects\\Lowry\\website\\Lowry.Web\\uSync";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(609, 109);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.buttonScrap);
            this.Controls.Add(this.buttonBrowseFolder);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "uSync Scrapper";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button buttonBrowseFolder;
        private System.Windows.Forms.Button buttonScrap;
        private System.Windows.Forms.TextBox textBox1;
    }
}

