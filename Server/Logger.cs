using System;
using System.IO;
using System.Windows.Forms;

namespace RemoteSignTool {
    internal static class Logger {
        public delegate void LoggerEventHandler(String log);
        
        public static event LoggerEventHandler Log;
        
        public static Control UIControl { get; set; } = null;
        
        public static String Path { get; set; } = null;
        
        public static String Format { get; set; } = "HH:mm:ss.fff";
        
        private static void WriteLine(String log) {
            try {
                var space = new String(' ', Format.Length + 1);
                log = DateTime.Now.ToString(Format) + " " + log;
                log = String.Join("\r\n" + space, log.Split(new String[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries));
                Console.WriteLine(log);
                log += "\r\n";
                
                if (UIControl != null && UIControl.Created) {
                    UIControl?.Invoke(new Action(() => { Log?.Invoke(log); }));
                } else {
                    Log?.Invoke(log);
                }

                if (Path != null) {
                    File.AppendAllText(Path, log);
                }
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }
        
        public static void WriteLine() {
            WriteLine("");
        }
        
        public static void WriteLine(Object log) {
            WriteLine(log.ToString());
        }
        
        public static void Info(Object log) {
            WriteLine("Info: " + log.ToString());
        }
        
        public static void Error(Object log) {
            WriteLine("Error: " + log.ToString());
        }
        
        public static void Warning(Object log) {
            WriteLine("Warning: " + log.ToString());
        }
        
        public static void Exception(Exception ex) {
            WriteLine("Exception: " + ex.Message + "\r\n" + ex.StackTrace);
        }
        
        public static void End() {
            if (Path != null) {
                File.AppendAllText(Path, "\r\n");
                Path = null;
            }
        }
    }
}
