using System.Collections.Generic;
using _Scripts.Entities.MainMenu.DataClasses;
using _Scripts.Services.Utils;

namespace _Scripts.Entities.MainMenu.View
{
    public interface IMainMenuView
    {
        Button_UI PlayButton { get; }
        Button_UI HowToPlayButton { get; }
        Button_UI QuitButton { get; }
        Button_UI BackButton { get; }
        List<MainMenuPage> MainMenuPages { get; }
    }
}