namespace RemoteSignTool.Server {
    partial class MainForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.label1 = new System.Windows.Forms.Label();
            this.signtoolPath = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.signtoolCmdLine = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.crossCertificate = new System.Windows.Forms.TextBox();
            this.logs = new System.Windows.Forms.TextBox();
            this.pin = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.startBtn = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.browseTool = new System.Windows.Forms.Button();
            this.browseCer = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(135, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Path to original signtool.exe";
            // 
            // signtoolPath
            // 
            this.signtoolPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.signtoolPath.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.signtoolPath.Location = new System.Drawing.Point(12, 24);
            this.signtoolPath.Name = "signtoolPath";
            this.signtoolPath.Size = new System.Drawing.Size(554, 22);
            this.signtoolPath.TabIndex = 1;
            this.signtoolPath.Tag = "signtool_path";
            this.signtoolPath.TextChanged += new System.EventHandler(this.Setting_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(284, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "Default signtool parameters (if not overridden by client call)";
            // 
            // signtoolCmdLine
            // 
            this.signtoolCmdLine.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.signtoolCmdLine.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.signtoolCmdLine.Location = new System.Drawing.Point(12, 74);
            this.signtoolCmdLine.Name = "signtoolCmdLine";
            this.signtoolCmdLine.Size = new System.Drawing.Size(554, 22);
            this.signtoolCmdLine.TabIndex = 5;
            this.signtoolCmdLine.Tag = "signtool_commandline";
            this.signtoolCmdLine.TextChanged += new System.EventHandler(this.Setting_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 109);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(166, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "Corss certificate for Driver signing";
            // 
            // crossCertificate
            // 
            this.crossCertificate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.crossCertificate.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.crossCertificate.Location = new System.Drawing.Point(12, 124);
            this.crossCertificate.Name = "crossCertificate";
            this.crossCertificate.Size = new System.Drawing.Size(554, 22);
            this.crossCertificate.TabIndex = 7;
            this.crossCertificate.Tag = "cross_cert";
            this.crossCertificate.TextChanged += new System.EventHandler(this.Setting_TextChanged);
            // 
            // logs
            // 
            this.logs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.logs.BackColor = System.Drawing.SystemColors.Window;
            this.logs.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.logs.Location = new System.Drawing.Point(12, 270);
            this.logs.Multiline = true;
            this.logs.Name = "logs";
            this.logs.ReadOnly = true;
            this.logs.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.logs.Size = new System.Drawing.Size(600, 166);
            this.logs.TabIndex = 13;
            this.logs.WordWrap = false;
            // 
            // pin
            // 
            this.pin.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pin.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pin.Location = new System.Drawing.Point(12, 174);
            this.pin.Name = "pin";
            this.pin.Size = new System.Drawing.Size(554, 22);
            this.pin.TabIndex = 9;
            this.pin.Tag = "pin";
            this.pin.TextChanged += new System.EventHandler(this.Setting_TextChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 159);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(23, 12);
            this.label6.TabIndex = 0;
            this.label6.Text = "PIN";
            // 
            // startBtn
            // 
            this.startBtn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.startBtn.Location = new System.Drawing.Point(12, 202);
            this.startBtn.Name = "startBtn";
            this.startBtn.Size = new System.Drawing.Size(600, 40);
            this.startBtn.TabIndex = 0;
            this.startBtn.Text = "Test and Start";
            this.startBtn.UseVisualStyleBackColor = true;
            this.startBtn.Click += new System.EventHandler(this.StartBtn_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 255);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(28, 12);
            this.label5.TabIndex = 0;
            this.label5.Text = "Logs";
            // 
            // browseTool
            // 
            this.browseTool.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.browseTool.Location = new System.Drawing.Point(572, 24);
            this.browseTool.Name = "browseTool";
            this.browseTool.Size = new System.Drawing.Size(40, 22);
            this.browseTool.TabIndex = 3;
            this.browseTool.Tag = "";
            this.browseTool.Text = "...";
            this.browseTool.UseVisualStyleBackColor = true;
            this.browseTool.Click += new System.EventHandler(this.BrowseBtn_Click);
            // 
            // browseCer
            // 
            this.browseCer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.browseCer.Location = new System.Drawing.Point(572, 124);
            this.browseCer.Name = "browseCer";
            this.browseCer.Size = new System.Drawing.Size(40, 22);
            this.browseCer.TabIndex = 3;
            this.browseCer.Tag = "";
            this.browseCer.Text = "...";
            this.browseCer.UseVisualStyleBackColor = true;
            this.browseCer.Click += new System.EventHandler(this.BrowseCer_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 441);
            this.Controls.Add(this.browseCer);
            this.Controls.Add(this.browseTool);
            this.Controls.Add(this.startBtn);
            this.Controls.Add(this.logs);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.pin);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.crossCertificate);
            this.Controls.Add(this.signtoolCmdLine);
            this.Controls.Add(this.signtoolPath);
            this.Controls.Add(this.label1);
            this.MinimumSize = new System.Drawing.Size(640, 480);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Remote SignTool Server";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox logs;
        private System.Windows.Forms.Label label6;
        internal System.Windows.Forms.TextBox signtoolPath;
        internal System.Windows.Forms.TextBox signtoolCmdLine;
        internal System.Windows.Forms.TextBox crossCertificate;
        internal System.Windows.Forms.TextBox pin;
        private System.Windows.Forms.Button startBtn;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button browseTool;
        private System.Windows.Forms.Button browseCer;
    }
}