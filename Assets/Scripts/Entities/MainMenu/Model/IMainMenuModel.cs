using System.Collections.Generic;
using MainMenu.View;
using UniRx;

namespace MainMenu.Model
{
    public interface IMainMenuModel
    {
        IReadOnlyReactiveProperty<MainMenuPageType> CurrentPage { get; }
        void QuitApplication();
        void Initialize();
        void SetActivePage(MainMenuPageType pageType);
        bool IsPageActive(MainMenuPageType pageType);
        void SetPageActive(MainMenuPageType pageType, List<MainMenuPage> mainMenuPages, bool isActive);
        void SetAllPagesInactive(List<MainMenuPage> mainMenuPages);
    }
}