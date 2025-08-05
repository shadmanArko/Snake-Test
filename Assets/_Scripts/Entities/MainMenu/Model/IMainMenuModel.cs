using System.Collections.Generic;
using _Scripts.Entities.MainMenu.DataClasses;
using _Scripts.Entities.MainMenu.Enums;
using UniRx;

namespace _Scripts.Entities.MainMenu.Model
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