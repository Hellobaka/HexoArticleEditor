namespace HexoArticleEditor.Model
{
    public class Common
    {
        public static event Action<bool>? DarkThemeChanged;

        public static event Action? ArticleListClicked;

        public static event Action? TerminalClicked;

        public static event Action<Article>? OnCurrentArticleChanged;

        public static event Action<Article, bool>? OnArticleSaveStateChanged;

        public static void InvokeDarkModeChanged(bool darkMode)
        {
            DarkThemeChanged?.Invoke(darkMode);
        }

        public static void InvokeArticleListClicked()
        {
            ArticleListClicked?.Invoke();
        }

        public static void InvokeTerminalClicked()
        {
            TerminalClicked?.Invoke();
        }

        public static void InvokeCurrentArticleChanged(Article? article)
        {
            if (article != null) 
            {
                OnCurrentArticleChanged?.Invoke(article);
            }
        }

        public static void InvokeArticleSaveStateChanged(Article? article, bool saveState)
        {
            if (article != null) 
            {
                OnArticleSaveStateChanged?.Invoke(article, saveState);
            }
        }

        public static bool IsDarkMode { get; set; } = true;
    }
}
