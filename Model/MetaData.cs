using Newtonsoft.Json.Linq;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using System.Text;

namespace HexoArticleEditor.Model
{
    public class MetaData
    {
        public string Title { get; set; } = "未命名";

        public List<string> Tags { get; set; } = [];

        public List<string> Categories { get; set; } = [];

        public string Cover { get; set; } = "";

        public DateTime? CreateDate { get; set; } = DateTime.Now;

        public TimeSpan? CreateTime { get; set; } = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
       
        public DateTime? UpdateTime { get; set; } = DateTime.Now;

        private bool IsJson { get; set; }

        public string ParseToRaw()
        {
            StringBuilder sb = new();
            DateTime createTime = new(CreateDate.Value.Year, CreateDate.Value.Month, CreateDate.Value.Day, CreateTime.Value.Hours, CreateTime.Value.Minutes, CreateTime.Value.Seconds);
            if (IsJson)
            {
                sb.AppendLine($"\"title\": \"{Title}\",");
                sb.AppendLine($"\"date\": \"{createTime:G}\",");
                sb.AppendLine($"\"updated\": {UpdateTime:G},");
                sb.AppendLine($"\"tags\": {JArray.FromObject(Tags)},");
                sb.AppendLine($"\"categories\": {JArray.FromObject(Categories)},");
                sb.AppendLine($"\"cover\": \"{Cover}\"");
                sb.AppendLine(";;;");
            }
            else
            {
                sb.AppendLine("---");
                sb.AppendLine($"title: {Title}");
                sb.AppendLine($"date: {createTime:G}");
                sb.AppendLine($"updated: {UpdateTime:G}");
                sb.AppendLine("tags:");
                foreach(var tag in Tags)
                {
                    sb.AppendLine($"  - {tag}");
                }
                sb.AppendLine("categories:");
                foreach(var category in Categories)
                {
                    sb.AppendLine($"  - {category}");
                }
                sb.AppendLine($"cover: {Cover}");
                sb.AppendLine("---");
            }
            sb.AppendLine("");
            return sb.ToString();
        }

        public static bool ParseFromRaw(string raw, out MetaData? metaData, out string remain)
        {
            metaData = null;
            remain = raw;

            bool json = false;
            bool success = false;
            bool yamlStart = false;
            using StringReader reader = new(raw);

            string? content = reader.ReadLine()?.Trim();
            string metaDataRaw = "";
            do
            {
                if (content == "---")
                {
                    if (yamlStart)
                    {
                        success = true;
                        break;
                    }
                    else
                    {
                        yamlStart = true;
                        continue;
                    }
                }
                else if (content == ";;;")
                {
                    json = true;
                    success = true;
                    break;
                }
                if (!string.IsNullOrEmpty(content))
                {
                    metaDataRaw += content + Environment.NewLine;
                }
            } while ((content = reader.ReadLine()) != null);

            if (!success)
            {
                return false;
            }

            do
            {
                content = reader.ReadLine();
            } while (string.IsNullOrWhiteSpace(content)) ;

            if (json)
            {
                metaDataRaw = $"{{{metaDataRaw}}}";
                metaData = ParseFromJson(metaDataRaw);
                remain = content + Environment.NewLine + reader.ReadToEnd();
                return true;
            }
            else if (yamlStart)
            {
                metaData = ParseFromYaml(metaDataRaw);
                remain = content + Environment.NewLine + reader.ReadToEnd();
                return true;
            }

            return false;
        }

        private static MetaData? ParseFromJson(string metaDataRaw)
        {
            try
            {
                JObject json = JObject.Parse(metaDataRaw);
                MetaData metaData = new()
                {
                    Title = json["title"]?.ToString() ?? "未命名",
                    CreateDate = ((DateTime?)json["date"]) ?? DateTime.Now,
                    UpdateTime = ((DateTime?)json["updated"]) ?? DateTime.Now,
                    Cover = json["cover"]?.ToString() ?? "",
                    IsJson = true
                };
                metaData.CreateTime = new TimeSpan(metaData.CreateDate.Value.Hour, metaData.CreateDate.Value.Minute, metaData.CreateDate.Value.Second);
                if (json.ContainsKey("tags") && json["tags"] is JArray tags)
                {
                    foreach (var tag in tags) 
                    {
                        metaData.Tags.Add(tag.ToString());
                    }
                }
                if (json.ContainsKey("categories") && json["categories"] is JArray categories)
                {
                    foreach (var category in categories) 
                    {
                        metaData.Categories.Add(category.ToString());
                    }
                }

                return metaData;
            }
            catch
            {
                return null;
            }
        }

        private static MetaData? ParseFromYaml(string metaDataRaw)
        {
            try
            {
                var deserializer = new DeserializerBuilder().WithNamingConvention(UnderscoredNamingConvention.Instance).Build();
                var yaml = deserializer.Deserialize<Dictionary<string, object>>(metaDataRaw);
                MetaData metaData = new();
                if (yaml.TryGetValue("title", out object? value)
                    && value is string title
                    && !string.IsNullOrEmpty(title))
                {
                    metaData.Title = title;
                }
                if (yaml.TryGetValue("tags", out value)
                    && value is List<object> tags)
                {
                    foreach (var tag in tags)
                    {
                        metaData.Tags.Add(tag.ToString());
                    }
                }
                if (yaml.TryGetValue("categories", out value)
                    && value is List<object> categories)
                {
                    foreach (var category in categories)
                    {
                        metaData.Categories.Add(category.ToString());
                    }
                }
                if (yaml.TryGetValue("cover", out value)
                    && value is string cover
                    && !string.IsNullOrEmpty(cover))
                {
                    metaData.Cover = cover;
                }
                if (yaml.TryGetValue("date", out value)
                    && value is string createDateRaw
                    && DateTime.TryParse(createDateRaw, out DateTime createDate))
                {
                    metaData.CreateDate = createDate;
                    metaData.CreateTime = new(createDate.Hour, createDate.Minute, createDate.Second);
                }
                if (yaml.TryGetValue("updated", out value)
                    && value is string updatedDateRaw
                    && DateTime.TryParse(updatedDateRaw, out DateTime updatedDate))
                {
                    metaData.UpdateTime = updatedDate;
                }

                return metaData;
            }
            catch
            {
                return null;
            }
        }
    }
}
