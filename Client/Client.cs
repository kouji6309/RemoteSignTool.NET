using System;
using System.Reflection;
using System.IO;

namespace RemoteSignTool.Client {
    internal class Program {
        internal static String PathWithoutExtension { get; } = Path.ChangeExtension(Assembly.GetExecutingAssembly().Location, null);

        internal static String FilenameWithoutExtension { get; } = Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location);

        public static Int32 Main(string[] args) {
            var additional = "";
            var fileToSign = "";

            for (var i = args.Length - 1; i >= 0; i--) {
                var arg = args[i];
                if (fileToSign == "" && arg.Length > 0 && !arg.StartsWith("/") && File.Exists(arg)) {
                    fileToSign = arg;
                } else {
                    if (!(i > 1 && args[i - 1].ToLower() == "/ac")) {
                        if (arg.Contains(" ")) {
                            additional = "\"" + arg + "\" " + additional;
                        } else {
                            additional = arg + " " + additional;
                        }
                    }
                }
            }

            if (fileToSign == "") {
                Logger.Error("File not found or not specified. You must specify at least file to sign");
                return -1;
            }

            ClientCore core = null;
            try {
                core = new ClientCore();
            } catch (Exception ex) {
                Logger.Error("Cannot connect to server");
                Logger.Exception(ex);
                return -1;
            }

            if (!core.SignFile(fileToSign, additional)) {
                return -1;
            } else {
                Logger.Info("File signed succesfully");
                return 0;
            }
        }
    }
}
