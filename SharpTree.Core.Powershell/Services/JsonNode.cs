using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using SharpTree.Core.Models;

namespace SharpTree.Core.Services
{
    public class JsonNode
    {
        public static void SaveToJson(INode node, string path)
        {
            using (StreamWriter writer = new StreamWriter(path))
            using (JsonTextWriter jsonWriter = new JsonTextWriter(writer))
            {
                JsonSerializer ser = new JsonSerializer
                {
                    TypeNameHandling = TypeNameHandling.Auto
                };
                ser.Serialize(jsonWriter, node);
                jsonWriter.Flush();
            }
        }
        public static RootNode LoadFromJson(string path)
        {
            using (StreamReader reader = new StreamReader(path))
            using (JsonTextReader jsonReader = new JsonTextReader(reader))
            {
                JsonSerializer serializer = new JsonSerializer
                {
                    TypeNameHandling = TypeNameHandling.Auto
                };
                return serializer.Deserialize<RootNode>(jsonReader);
            }
        }

    }
}