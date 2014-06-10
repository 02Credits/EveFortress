using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public static class SerializationUtils
    {
        public static T DeserializeFileOrValue<T>(string fileName, T otherwise)
        {
            Console.WriteLine("Attempting deserialization of " + fileName);
            if (File.Exists(fileName))
            {
                using (var stream = File.OpenRead(fileName))
                {
                    var obj = Serializer.Deserialize<T>(stream);
                    Console.WriteLine("Success!");
                    return obj;
                }
            }
            else
            {
                Console.WriteLine("File doesnt exist. Returning default.");
                return otherwise;
            }
        }

        public static void SerializeToFile<T>(string fileName, T obj)
        {
            Console.WriteLine("Saving to " + fileName);
            using (var stream = File.OpenWrite(fileName))
            {
                Serializer.Serialize(stream, obj);
                Console.WriteLine("    Finished");
            }
        }

        public static byte[] Serialize<T>(T itemToSerialize)
        {
            byte[] returnArray;
            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream, itemToSerialize);
                returnArray = stream.ToArray();
            }
            return returnArray;
        }

        public static T Deserialize<T>(byte[] data)
        {
            using (var stream = new MemoryStream(data))
            {
                return Serializer.Deserialize<T>(stream);
            }
        }
    }
}
