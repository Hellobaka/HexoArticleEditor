namespace HexoArticleEditor.Model
{
    public class Article
    {
        public string Name { get; set; } = "";

        public string FilePath { get; set; } = "";

        public bool Saved { get; set; } = true;

        public bool NewArticle { get; set; }

        public MetaData MetaData { get; set; } = new MetaData();
    }

    public class ArticleComparer : IComparer<string>
    {
        public int Compare(string? x, string? y)
        {
            if (string.IsNullOrEmpty(x) || string.IsNullOrEmpty(y)
                || !File.Exists(x) || !File.Exists(y))
            {
                return 0;
            }

            return new FileInfo(x).LastWriteTime.CompareTo(new FileInfo(y).LastWriteTime);
        }
    }
}
