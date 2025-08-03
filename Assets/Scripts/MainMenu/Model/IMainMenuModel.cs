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
    }
}