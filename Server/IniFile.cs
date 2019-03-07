using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace RemoteSignTool {
    internal class IniFile {
        [DllImport("kernel32.dll")]
        private static extern uint GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, uint nSize, string lpFileName);

        [DllImport("kernel32.dll")]
        static extern bool WritePrivateProfileString(string lpAppName, string lpKeyName, string lpString, string lpFileName);

        private String FileName { get; set; }

        private readonly Boolean exists = false;

        public IniFile(String file) {
            if (!(exists = File.Exists(file))) {
                File.WriteAllText(file, "");
            }
            FileName = file;
        }

        private String Read(String app, String key, String def) {
            if (!exists) {
                throw new FileNotFoundException();
            }

            var buffer = new StringBuilder(255);
            var res = GetPrivateProfileString(app, key, def, buffer, (uint)buffer.Capacity, FileName);
            return buffer.ToString();
        }

        public String ReadString(String app, String key) {
            return Read(app, key, "");
        }

        public Int32 ReadInteger(String app, String key) {
            return Int32.Parse(Read(app, key, "0"));
        }

        public Boolean Write(String app, String key, Object value) {
            return WritePrivateProfileString(app, key, value.ToString(), FileName);
        }
    }
}
