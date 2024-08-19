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
    }
}
