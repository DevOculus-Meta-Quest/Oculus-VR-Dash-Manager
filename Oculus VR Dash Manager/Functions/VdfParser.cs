using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OVR_Dash_Manager.Functions
{
    internal class VdfParser
    {
        public Dictionary<string, object> ParseVdf(string filePath)
        {
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open))
                using (BinaryReader br = new BinaryReader(fs))
                    return ReadNextObject(br);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex, "Error parsing VDF file: " + filePath);
                return null; // or handle the error as appropriate
            }
        }

        private Dictionary<string, object> ReadNextObject(BinaryReader br)
        {
            var result = new Dictionary<string, object>();

            while (true)
            {
                var type = br.ReadByte();

                if (type == 0x00) // Map
                {
                    var key = ReadString(br);
                    var value = ReadNextObject(br);
                    result[key] = value;
                }
                else if (type == 0x01) // String
                {
                    var key = ReadString(br);
                    var value = ReadString(br);
                    result[key] = value;
                }
                else if (type == 0x02) // Integer
                {
                    var key = ReadString(br);
                    var value = br.ReadInt32();
                    result[key] = value;
                }
                else if (type == 0x08) // End of a map
                {
                    break;
                }
                else
                {
                    throw new Exception("Unknown type encountered in VDF file.");
                }
            }

            return result;
        }

        private string ReadString(BinaryReader br)
        {
            var bytes = new List<byte>();

            while (true)
            {
                var b = br.ReadByte();
                if (b == 0) break;
                bytes.Add(b);
            }

            return Encoding.UTF8.GetString(bytes.ToArray());
        }

        public List<ShortcutInfo> ExtractSpecificData(Dictionary<string, object> vdfData)
        {
            var shortcuts = new List<ShortcutInfo>();

            foreach (var entry in vdfData)
            {
                if (entry.Value is Dictionary<string, object> shortcutData)
                {
                    var info = new ShortcutInfo
                    {
                        AppName = shortcutData.ContainsKey("AppName") ? shortcutData["AppName"].ToString() : "Unknown",
                        Exe = shortcutData.ContainsKey("Exe") ? shortcutData["Exe"].ToString() : "Unknown"
                    };

                    shortcuts.Add(info);
                }
            }

            return shortcuts;
        }
    }

    internal class ShortcutInfo
    {
        public string AppName { get; set; }
        public string Exe { get; set; }
    }
}