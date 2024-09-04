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
}
