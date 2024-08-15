using Markdig;

namespace HexoArticleEditor.Components
{
    public class Common
    {
        public static event Action<bool>? DarkThemeChanged;

        public static void InvokeDarkModeChanged(bool darkMode)
        {
            DarkThemeChanged?.Invoke(darkMode);
        }

        public static bool IsDarkMode { get; set; } = true;

        private static MarkdownPipeline? MarkdownRender { get; set; }

        public static MarkdownPipeline GetMarkdownRender()
        {
            MarkdownRender ??= new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .UseTaskLists()
                .UseEmphasisExtras()
                .Build();
            return MarkdownRender;
        }
    }
}
