namespace HexoArticleEditor.Model
{
    public class AutoSaveItem
    {
        public string ParentArticleName { get; set; } = "";

        public string FilePath { get; set; } = "";

        public string FileName => Path.GetFileNameWithoutExtension(FilePath);

        public string DisplayName { get; set; } = "";
    }
}
