using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using SharpTree.Core.Models;

namespace SharpTree.Core.Services
{
    public class NodeToJson
    {
        public static void SaveToJsonFile(INode node, string path)
        {
            using (StreamWriter writer = new StreamWriter(path))
            using (JsonTextWriter jsonWriter = new JsonTextWriter(writer))
            {
                JsonSerializer ser = new JsonSerializer
                {
                    Formatting = Formatting.Indented
                };
                ser.Serialize(jsonWriter, node);
                jsonWriter.Flush();
            }
        }
    }
}