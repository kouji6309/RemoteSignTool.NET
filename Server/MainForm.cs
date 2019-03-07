using System;
using System.IO;
using System.Windows.Forms;

namespace RemoteSignTool.Server {
    internal partial class MainForm : Form {
        private Control[] controls = null;

        private ServerCore serverCore = null;

        private IniFile ini = null;

        public MainForm() {
            InitializeComponent();

            controls = new Control[] { signtoolPath, signtoolCmdLine, crossCertificate, pin, browseTool };
        }

        private void MainForm_Load(object sender, EventArgs e) {
            Logger.UIControl = this;
            Logger.Log += Logger_Log;

            ini = new IniFile(Program.PathWithoutExtension + ".ini");
            try {
                foreach (var i in controls) {
                    var tag = i.Tag as String;
                    if (!String.IsNullOrEmpty(tag)) {
                        i.Text = ini.ReadString("server", tag);
                    }
                }
            } catch {
                Logger.Warning("Cannot read '" + Program.FilenameWithoutExtension + ".ini', used default setting");
                signtoolPath.Text = @"C:\Program Files (x86)\Windows Kits\10\bin\x64\signtool.exe";
                signtoolCmdLine.Text = "sign /a";
                crossCertificate.Text = "GlobalSign Root CA.crt";
                pin.Text = "0000";
            }

            Logger.Info("Application started");
        }

        private void Logger_Log(String log) {
            logs.AppendText(log);
            Application.DoEvents();
        }

        private void Setting_TextChanged(object sender, EventArgs e) {
            var s = sender as Control;
            var tag = s?.Tag as String;
            if (!String.IsNullOrEmpty(tag)) {
                ini.Write("server", tag, s.Text);
            }
        }

        private void BrowseBtn_Click(object sender, EventArgs e) {
            var ofd = new OpenFileDialog();
            if (File.Exists(signtoolPath.Text)) {
                ofd.InitialDirectory = Path.GetDirectoryName(signtoolPath.Text);
            } else {
                ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            }
            ofd.FileName = "signtool.exe";
            ofd.Filter = "signtool|signtool.exe";
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == DialogResult.OK) {
                if (signtoolPath.Text != ofd.FileName) {
                    Logger.Info("Change signtool to '" + ofd.FileName + "'");
                }
                signtoolPath.Text = ofd.FileName;
            }
        }

        private void BrowseCer_Click(object sender, EventArgs e) {
            var ofd = new OpenFileDialog();
            if (File.Exists(crossCertificate.Text)) {
                ofd.InitialDirectory = Path.GetDirectoryName(Path.GetFullPath(crossCertificate.Text));
            } else {
                ofd.InitialDirectory = Path.GetDirectoryName(Program.Location);
            }
            ofd.FileName = "";
            ofd.Filter = "Certificate|*.crt";
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == DialogResult.OK) {
                if (crossCertificate.Text != ofd.FileName) {
                    Logger.Info("Certificate to '" + ofd.FileName + "'");
                }
                crossCertificate.Text = ofd.FileName;
            }
        }

        private void StartBtn_Click(object sender, EventArgs e) {
            if (startBtn.Text == "Stop") {
                startBtn.Text = "Test and start";
                foreach (var i in controls) {
                    i.Enabled = true;
                }

                try {
                    serverCore?.Dispose();
                    Logger.Info("Server stopped");
                } catch (Exception ex) {
                    Logger.Error("Unabled to stop server");
                    Logger.Exception(ex);
                }

                serverCore = null;
            } else {
                if (!File.Exists(signtoolPath.Text)) {
                    Logger.Error("Signtool not found");
                    return;
                }

                Logger.Info("Trying to sign test file");
                var testFile = "";
                try {
                    testFile = Path.ChangeExtension(Program.Location, ".bak");
                    File.Copy(Program.Location, testFile, true);
                    Logger.Info("Test file has created");
                } catch (Exception ex) {
                    Logger.Error("Cannot create test file");
                    Logger.Exception(ex);
                    return;
                }

                serverCore = new ServerCore();
                var trySign = false;
                var args = signtoolCmdLine.Text + " \"" + testFile + "\"";
                if (serverCore.SignFile(signtoolPath.Text, args, pin.Text, out Int32 code, out String message)) {
                    if (code == 0) {
                        trySign = true;
                        Logger.Info("Signing test file successfully");
                    } else {
                        Logger.Error("Unable to sign test file");
                    }
                } else {
                    serverCore.Dispose();
                    Logger.Error(message);
                }

                try {
                    File.Delete(testFile);
                    Logger.Info("Test file has deleted");
                } catch {
                    Logger.Warning("Unable to delete test file, please delete it yourself");
                }

                if (!trySign) {
                    return;
                }

                try {
                    serverCore.Listen();
                    Logger.Info("Server started");
                } catch (Exception ex) {
                    Logger.Error("Unable to start server");
                    Logger.Exception(ex);
                    return;
                }

                startBtn.Text = "Stop";
                foreach (var i in controls) {
                    i.Enabled = false;
                }
            }
        }
    }
}
