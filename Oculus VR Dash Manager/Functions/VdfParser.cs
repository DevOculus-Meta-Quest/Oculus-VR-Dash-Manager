using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OVR_Dash_Manager.Functions
{
    class VdfParser
    {
        public Dictionary<string, object> ParseVdf(string filePath)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            using (BinaryReader br = new BinaryReader(fs))
            {
                return ReadNextObject(br);
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
                    string key = ReadString(br);
                    var value = ReadNextObject(br);
                    result[key] = value;
                }
                else if (type == 0x01) // String
                {
                    string key = ReadString(br);
                    string value = ReadString(br);
                    result[key] = value;
                }
                else if (type == 0x02) // Integer
                {
                    string key = ReadString(br);
                    int value = br.ReadInt32();
                    result[key] = value;
                }
                else if (type == 0x08) // End of a map
                {
                    break;
                }
                else
                {
                    throw new Exception("Unknown type");
                }
            }
            return result;
        }

        private string ReadString(BinaryReader br)
        {
            var bytes = new List<byte>();
            while (true)
            {
                byte b = br.ReadByte();
                if (b == 0) break;
                bytes.Add(b);
            }
            return Encoding.UTF8.GetString(bytes.ToArray());
        }
    }
}
