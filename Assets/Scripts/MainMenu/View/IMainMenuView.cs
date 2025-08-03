using CodeMonkey.Utils;

namespace MainMenu.View
{
    public interface IMainMenuView
    {
        Button_UI PlayButton { get; }
        Button_UI HowToPlayButton { get; }
        Button_UI QuitButton { get; }
        Button_UI BackButton { get; }
        void SetPageActive(MainMenuPageType pageType, bool isActive);
        void SetAllPagesInactive();
    }
}