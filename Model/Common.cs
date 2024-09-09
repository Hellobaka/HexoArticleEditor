namespace HexoArticleEditor.Model
{
    public class Common
    {
        public static event Action<bool>? DarkThemeChanged;

        public static event Action? ArticleListClicked;

        public static event Action? TerminalClicked;

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
    }
}
