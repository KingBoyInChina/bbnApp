using Newtonsoft.Json.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace bbnApp
{
    public class ResourcesParse
    {
        /// <summary>
        /// 生成资源文件
        /// </summary>
        /// <param name="filePath"></param>
        public static void ResourcesJsonParse(string filePath)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            string jsoninfo = File.ReadAllText(filePath);
            JObject Obj = JObject.Parse(jsoninfo);
            JArray arrData = JArray.FromObject(Obj["glyphs"]);
            StringBuilder sb = new StringBuilder();
            foreach(JObject item in arrData)
            {
                string key = "bbn-" + item["name"].ToString();
                if (!dic.ContainsKey(key))
                {
                    sb.AppendLine($"<system:String x:Key=\"{key}\">&#x{item["unicode"].ToString()};</system:String>");
                    dic.Add(key, item["unicode"].ToString());
                }
                
            }
            Console.WriteLine( sb.ToString() );
        }
    }
}
