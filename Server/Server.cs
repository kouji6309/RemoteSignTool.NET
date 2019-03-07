using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace RemoteSignTool.Server {
    internal class Program {
        public static String PathWithoutExtension { get; } = Path.ChangeExtension(Assembly.GetExecutingAssembly().Location, null);

        public static String FilenameWithoutExtension { get; } = Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location);

        public static String Location { get; } = Assembly.GetExecutingAssembly().Location;

        public static MainForm MainForm { get; }

        static Program() {
            Logger.Path = PathWithoutExtension + ".log";
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            MainForm = new MainForm();
        }

        [STAThread]
        public static void Main(string[] args) {
            Logger.Info("Application starting");
            Application.Run(MainForm);
            Logger.Info("Application exit");
            Logger.End();
        }
    }
}
